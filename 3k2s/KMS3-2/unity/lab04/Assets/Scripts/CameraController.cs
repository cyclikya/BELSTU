using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public float mouseSensitivity = 3f;
    public float scrollSensitivity = 100f;

    public float minDistance = 20f;
    public float maxDistance = 1000f;

    public float minY = -20f;
    public float maxY = 80f;

    private float currentX = 0f;
    private float currentY = 20f;
    private float currentDistance = 100f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Разблокировка курсора
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Вращение камеры
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
            currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Ограничение по вертикали
            currentY = Mathf.Clamp(currentY, minY, maxY);
        }

        // Зум
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance += scroll * scrollSensitivity;

        // Ограничение дистанции
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        Debug.Log(currentDistance);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = rotation * new Vector3(0, 0, -currentDistance);

        transform.position = target.position + direction;
        transform.LookAt(target);
    }
}