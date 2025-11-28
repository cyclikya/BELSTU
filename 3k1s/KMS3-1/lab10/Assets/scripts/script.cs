using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class script : MonoBehaviour, IPointerClickHandler
{
    public int forse = 500; 

    public void OnPointerClick(PointerEventData eventData)
    {
        float red = Random.Range(0.0f, 1.0f);
        float green = Random.Range(0.0f, 1.0f);
        float blue = Random.Range(0.0f, 1.0f);

        Color col1 = new Color(red, green, blue);
        gameObject.GetComponent<Renderer>().material.color = col1;

        Vector3 target = eventData.pointerPressRaycast.worldPosition;
        
        Vector3 cameraPos = Camera.main.transform.position;

        Vector3 direction = (target - cameraPos).normalized;

        Vector3 forceVector = direction * forse;

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForceAtPosition(forceVector, target);
        }
    }
}