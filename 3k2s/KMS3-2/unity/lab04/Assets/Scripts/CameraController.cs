using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // Камаз

    public float mouseSensitivity = 3f;
    public float scrollSensitivity = 100f;

    public float minDistance = 20f;
    public float maxDistance = 1000f;

    public float minY = -20f;
    public float maxY = 80f;

    // Параметры для свободного движения камеры
    public float moveSpeed = 10f;
    public float boostSpeed = 20f; // Скорость при зажатой клавише Shift

    private float currentX = 0f;
    private float currentY = 20f;
    private float currentDistance = 100f;
    
    private bool isAttachedToTarget = true; // Прикреплена ли камера к камазу
    private Vector3 freeMovePosition; // Позиция камеры в свободном режиме

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (target != null)
        {
            freeMovePosition = target.position + transform.rotation * new Vector3(0, 0, -currentDistance);
        }
        else
        {
            freeMovePosition = transform.position;
        }
    }

    void Update()
    {
        // Управление курсором
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Зажата правая кнопка мыши - прикрепляем камеру к камазу и вращаем
        if (Input.GetMouseButton(1))
        {
            if (!isAttachedToTarget)
            {
                // Переключаемся в режим прикрепления к камазу
                AttachToTarget();
            }
            
            // Вращаем камеру вокруг камаза
            currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
            currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            currentY = Mathf.Clamp(currentY, minY, maxY);
        }
        else
        {
            // Отпустили правую кнопку мыши - открепляем камеру для свободного движения
            if (isAttachedToTarget)
            {
                DetachFromTarget();
            }
            
            // Свободное движение камеры на WASD
            MoveCameraWithWASD();
        }

        // Зум колесиком мыши (работает в обоих режимах)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance += -scroll * scrollSensitivity;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }

    void AttachToTarget()
    {
        if (target == null) return;
        
        isAttachedToTarget = true;
        
        // Сохраняем текущую позицию камеры относительно камаза
        Vector3 offset = transform.position - target.position;
        
        // Восстанавливаем углы вращения из текущей позиции
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.position);
        
        // Вычисляем углы
        float angleX = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        float angleY = Mathf.Asin(directionToTarget.y) * Mathf.Rad2Deg;
        
        currentX = angleX;
        currentY = -angleY;
        currentDistance = distance;
        
        // Ограничиваем углы
        currentY = Mathf.Clamp(currentY, minY, maxY);
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }

    void DetachFromTarget()
    {
        isAttachedToTarget = false;
        
        // Сохраняем текущую позицию для свободного движения
        freeMovePosition = transform.position;
        
        // Сохраняем текущее вращение
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    void MoveCameraWithWASD()
    {
        // Получаем ввод с клавиатуры
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Если есть движение
        if (horizontal != 0 || vertical != 0)
        {
            // Определяем текущую скорость
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? boostSpeed : moveSpeed;
            
            // Получаем направления относительно текущего поворота камеры
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            
            // Убираем наклон по вертикали для движения по горизонтали
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            
            // Вычисляем направление движения
            Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;
            
            // Перемещаем камеру
            freeMovePosition += moveDirection * currentSpeed * Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        if (isAttachedToTarget && target != null)
        {
            // Режим прикрепления к камазу - вращаемся вокруг него
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            Vector3 direction = rotation * new Vector3(0, 0, -currentDistance);
            transform.position = target.position + direction;
            transform.LookAt(target);
        }
        else
        {
            // Свободный режим - камера просто ходит по уровню
            transform.position = freeMovePosition;
            
            // Сохраняем вращение камеры в свободном режиме (если нужно)
            // Если хотите, чтобы камера всегда смотрела в направлении движения, раскомментируйте:
            // if (moveDirection != Vector3.zero)
            // {
            //     Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            //     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            // }
        }
    }
}