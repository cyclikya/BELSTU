using UnityEngine;

public class Zadanie_11 : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log("MouseDown");
        GetComponent<Renderer>().material.color = Color.red;
    }
}
