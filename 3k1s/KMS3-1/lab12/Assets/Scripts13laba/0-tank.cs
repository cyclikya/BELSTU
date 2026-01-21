using UnityEngine;

public class Tank13 : MonoBehaviour
{
    Transform bash;
    Transform stv;
    public float TankMoveSpeed = 0.1f;
    public float RotateSpeed = 1f;

    public AudioSource tankMoveAudio;

    float h = 0;
    
    void Start()
    {
        bash = gameObject.transform.Find("bashnya");
        stv = bash.transform.Find("dylo");
    }

    void Update()
    {
        float z = Input.GetAxis("Vertical") * TankMoveSpeed;
        transform.Translate(0, 0, z);

        float x = Input.GetAxis("Horizontal") * RotateSpeed;
        transform.Rotate(0f, x, 0f);

        float h = Input.GetAxis("Mouse X") * RotateSpeed;
        bash.Rotate(0f, h, 0f);

        float v = Input.GetAxis("Mouse Y");
        stv.transform.Rotate(0, 0, v);

        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            if (!tankMoveAudio.isPlaying)
            {
                tankMoveAudio.Play();
            }
        }
        else
        {
            tankMoveAudio.Pause();
        }
    }

    void OnGUI()
    {
        GUI.BeginGroup(new Rect(10, h, 250, 300)); 

        GUI.Box(new Rect(10, 0, 200, 200), "SPEED CONTROL");

        GUI.Label(new Rect(15, 30, 200, 30), "Tank Speed " + TankMoveSpeed + " ");

        TankMoveSpeed = GUI.HorizontalSlider(new Rect(15, 50, 170, 30), TankMoveSpeed, 0.0f, 10.0f);

        if (GUI.Button(new Rect(10, 170, 90, 20), "Hide UI"))
        {
            Hide();
        }

        if (GUI.Button(new Rect(100, 170, 90, 20), "Show UI"))
        {
            Show();
        }


        GUI.EndGroup();
    }

    public void Hide() { h = -170; }
    public void Show() { h = 0; }
}