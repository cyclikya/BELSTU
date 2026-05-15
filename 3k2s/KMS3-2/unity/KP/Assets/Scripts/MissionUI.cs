using UnityEngine;
using UnityEngine.UI;

// Displays the current mission objective and fills the results table cell by cell.
public class MissionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Text objectiveText;
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private Text[] attemptNumberCells = new Text[5];
    [SerializeField] private Text[] timeCells = new Text[5];
    [SerializeField] private Text[] speedCells = new Text[5];
    [SerializeField] private Text[] gearCells = new Text[5];
    [SerializeField] private Text bestResultText;

    private MissionController controller;

    public void SetController(MissionController missionController)
    {
        controller = missionController;
    }

    public void SetObjective(string text)
    {
        if (objectiveText != null)
        {
            objectiveText.text = text;
        }
    }

    public void ShowResults(MissionResultRecord[] results)
    {
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
        }

        FillAttemptHeaders();
        FillValueRow(timeCells, results, record => FormatTime(record.timeSeconds));
        FillValueRow(speedCells, results, record => $"{Mathf.RoundToInt(record.maxSpeedKmh)} км/ч");
        FillValueRow(gearCells, results, record => record.maxForwardGear > 0 ? record.maxForwardGear.ToString() : "-");

        if (bestResultText != null)
        {
            int bestIndex = MissionResultsStorage.GetBestResultIndex(results);
            bestResultText.text = bestIndex >= 0 ? $"Лучший результат: {bestIndex + 1}" : "Лучший результат: -";
        }
    }

    public void HideResults()
    {
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(false);
        }
    }

    public void OpenResults()
    {
        if (controller != null)
        {
            controller.OpenResultsTable();
        }
    }

    public void CloseResults()
    {
        if (controller != null)
        {
            controller.CloseResultsTable();
        }
        else
        {
            HideResults();
        }
    }

    public void RestartMission()
    {
        if (controller != null)
        {
            controller.RestartMission();
        }
    }

    public void OpenSetupScene()
    {
        if (controller != null)
        {
            controller.LoadSetupScene();
        }
    }

    private void FillAttemptHeaders()
    {
        for (int i = 0; i < attemptNumberCells.Length; i++)
        {
            if (attemptNumberCells[i] != null)
            {
                attemptNumberCells[i].text = (i + 1).ToString();
            }
        }
    }

    private void FillValueRow(Text[] cells, MissionResultRecord[] results, System.Func<MissionResultRecord, string> formatter)
    {
        if (cells == null)
        {
            return;
        }

        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i] == null)
            {
                continue;
            }

            if (results != null && i < results.Length)
            {
                cells[i].text = formatter(results[i]);
            }
            else
            {
                cells[i].text = "-";
            }
        }
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        float secondsPart = seconds - minutes * 60f;
        return $"{minutes:00}:{secondsPart:00.00}";
    }
}
