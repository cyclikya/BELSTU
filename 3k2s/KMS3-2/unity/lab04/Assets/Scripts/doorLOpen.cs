using UnityEngine;

public class doorLOpen : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q pressed — trying to open door");
            anim.SetBool("turn", true);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed — trying to close door");
            anim.SetBool("turn", false);
        }
    }
}
