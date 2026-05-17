using UnityEngine;
using UnityEngine.UI;

// Показывает текущее задание миссии и заполняет таблицу результатов по ячейкам.
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

    // Связывает UI с контроллером миссии, чтобы кнопки вызывали его методы.
    public void SetController(MissionController missionController)
    {
        controller = missionController;
    }

    // Выводит текущее задание под миникамерой.
    public void SetObjective(string text)
    {
        if (objectiveText != null)
        {
            objectiveText.text = text;
        }
    }

    // Заполняет таблицу последними сохраненными попытками миссии.
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

    // Скрывает панель результатов, пока игрок не откроет ее снова.
    public void HideResults()
    {
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(false);
        }
    }

    // Открывает таблицу результатов через контроллер миссии.
    public void OpenResults()
    {
        if (controller != null)
        {
            controller.OpenResultsTable();
        }
    }

    // Закрывает таблицу результатов, не удаляя сохраненные данные.
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

    // Перезапускает сцену практики по постоянной кнопке в интерфейсе.
    public void RestartMission()
    {
        if (controller != null)
        {
            controller.RestartMission();
        }
    }

    // Загружает сцену установки по постоянной кнопке в интерфейсе.
    public void OpenSetupScene()
    {
        if (controller != null)
        {
            controller.LoadSetupScene();
        }
    }

    // Записывает в шапку таблицы номера столбцов от 1 до 5.
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

    // Заполняет одну строку таблицы, например время, скорость или передачу.
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

    // Форматирует время миссии в минуты и секунды для вывода в таблицу.
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        float secondsPart = seconds - minutes * 60f;
        return $"{minutes:00}:{secondsPart:00.00}";
    }
}
