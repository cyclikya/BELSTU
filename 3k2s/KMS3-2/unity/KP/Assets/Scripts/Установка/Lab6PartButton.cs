using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Lab6PartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Lab6UIController controller;
    [SerializeField] private Lab6UIController.InstallationSection section = Lab6UIController.InstallationSection.Kuzov;
    [SerializeField] private int setupSceneBuildIndex = 0;

    private void Awake()
    {
        if (controller == null)
        {
            controller = GetComponentInParent<Lab6UIController>();
        }

        if (controller == null)
        {
            Debug.LogError("Lab6PartButton: Lab6UIController не задан и не найден у родителя.");
            enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        controller.HoverSection(section);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        controller.UnhoverSection(section);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            controller.OpenSection(section);
        }
    }

    public void LoadSetupScene()
    {
        SceneManager.LoadScene(setupSceneBuildIndex);
    }
}
