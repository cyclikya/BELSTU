using System.Collections;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public float rotspeedbash = 3f;  // Скорость поворота башни
    
    public Transform bash;           // Башня (bashnya)
    public Transform stvol;          // Ствол (dylo)
    public GameObject core;          // Снаряд
    private float spawnDistance = 3f;
    private float coreLifetime = 2f;
    private float shootCooldown = 3f;
    
    private int life = 3;
    private bool canshoot = true;
    private Transform playerTarget;
    
    public float moveSpeed = 4f;
    public float rotSeedTank = 1f;
    public Transform body;  // Корпус танка (если отдельный объект)

    void Start()
    {
        // Если body не назначен, используем сам объект
        if (body == null) body = transform;
        
        // Добавляем триггер если нет
        if (GetComponent<SphereCollider>() == null)
        {
            SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
            trigger.radius = 30f;
            trigger.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTarget = other.transform;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.transform == playerTarget)
        {
            playerTarget = null;
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        Vector3 toPlayer = playerTarget.position - body.position;
        toPlayer.y = 0f;
        float dist = toPlayer.magnitude;

        if (dist > 100f)
        {
            Quaternion bodyRot = Quaternion.LookRotation(toPlayer);
            body.rotation = Quaternion.Slerp(body.rotation, bodyRot, Time.deltaTime * rotSeedTank);

            body.position += body.forward * moveSpeed * Time.deltaTime;
        }

        Vector3 dirToPlayer = playerTarget.position - bash.position;
        dirToPlayer.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(dirToPlayer);
        bash.rotation = Quaternion.Slerp(bash.rotation, lookRotation, Time.deltaTime * rotspeedbash);

        float aimDot = Vector3.Dot(bash.forward, dirToPlayer.normalized);
        RaycastHit hit;
        if (canshoot && aimDot > 0.98f && 
            Physics.Raycast(stvol.position, stvol.forward, out hit, 100f))
        {
            if (hit.transform.CompareTag("Player"))
                StartCoroutine(botshoot());
        }
    }

    IEnumerator botshoot()
    {
        canshoot = false;

        Vector3 spawnPos = stvol.position + stvol.forward * spawnDistance;
        GameObject newcore = Instantiate(core, spawnPos, stvol.rotation);

        Destroy(newcore, coreLifetime);

        yield return new WaitForSeconds(shootCooldown);
        canshoot = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("core"))
        {
            life--;
            if (life <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}