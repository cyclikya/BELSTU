using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Lab6PartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Lab6UIController controller;
    [SerializeField] private Lab6UIController.InstallationSection section = Lab6UIController.InstallationSection.Kuzov;
    [SerializeField] private int setupSceneBuildIndex = 0;
    [SerializeField] private string setupSceneName = "Установка";

    private void Awake()
    {
        if (controller != null)
        {
            return;
        }

        controller = GetComponentInParent<Lab6UIController>();
        if (controller != null)
        {
            return;
        }

#if UNITY_2023_1_OR_NEWER
        controller = FindFirstObjectByType<Lab6UIController>();
#else
        controller = FindObjectOfType<Lab6UIController>();
#endif
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (controller != null)
        {
            controller.HoverSection(section);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (controller != null)
        {
            controller.UnhoverSection(section);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (controller != null)
        {
            controller.OpenSection(section);
        }
    }

    public void LoadSetupScene()
    {
        if (setupSceneBuildIndex >= 0 && setupSceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(setupSceneBuildIndex);
            return;
        }

        if (!string.IsNullOrWhiteSpace(setupSceneName))
        {
            SceneManager.LoadScene(setupSceneName);
        }
    }
}
