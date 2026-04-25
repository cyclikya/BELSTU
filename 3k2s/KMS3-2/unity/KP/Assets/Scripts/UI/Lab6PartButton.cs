using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Lab6PartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Lab6UIController controller;
    [SerializeField] private Lab6UIController.InstallationSection section = Lab6UIController.InstallationSection.Kuzov;
    [SerializeField] private int setupSceneBuildIndex;

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
        if (controller != null && eventData.button == PointerEventData.InputButton.Left)
        {
            controller.OpenSection(section);
        }
    }

    public void LoadSetupScene()
    {
        SceneManager.LoadScene(setupSceneBuildIndex);
    }
}
