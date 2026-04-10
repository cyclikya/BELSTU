using UnityEngine;

public class WheelController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float speed = 0f;
    
    // Публичное свойство для доступа извне
    public float Speed 
    { 
        get => speed;
        set 
        {
            speed = value;
            if (animator != null)
                animator.SetFloat("speed", speed);
        }
    }
    
    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
            
        // Инициализация
        Speed = speed;
    }
    
    // Пример для теста (меняйте скорость в рантайме)
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            Speed += 1f;
        if (Input.GetKeyDown(KeyCode.K))
            Speed -= 1f;
    }
}