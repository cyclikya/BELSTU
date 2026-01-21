using UnityEngine;

public class Zadanie_1516 : MonoBehaviour
{
    public Light lamp;

    void Start()
    {
        lamp.enabled = false;
    }

    void OnMouseDown()
    {
        Debug.Log("MouseDown");
        
        lamp.enabled = true;

        lamp.GetComponent<Light>().color = Color.red;
    }
}
