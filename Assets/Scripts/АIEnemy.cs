using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float moveDistance = 1f;

    [Header("Combat Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 7f;
    [SerializeField] private float bulletOffset = 0.5f;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    public int CurrentHealth => currentHealth;
    public int MaxHealth     => maxHealth;

    private Transform player;
    private float nextFireTime;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private Vector2 lastMoveDirection = Vector2.down;
    private float currentRotation;
    private bool hasShotDuringCurrentMove = false;

    private void Awake()
    {
        GameObject ply = GameObject.FindGameObjectWithTag("Player");
        if (ply != null)
            player = ply.transform;

        currentHealth = maxHealth;
        UIManager.Instance?.UpdateEnemyHealth(currentHealth, maxHealth);
   }
    

    void Start()
    {
        
        //UIManager.Instance?.UpdateEnemyHealth(currentHealth, maxHealth);
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            HandleMovement();

            if (distanceToPlayer <= attackRange && Time.time >= nextFireTime && !hasShotDuringCurrentMove)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
                hasShotDuringCurrentMove = true;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        Debug.Log($"Enemy hit! Health: {currentHealth}");
        UIManager.Instance?.UpdateEnemyHealth(currentHealth, maxHealth);
        if (currentHealth == 0) Die();
    }

    private void Die()
    {
        Debug.Log("Enemy defeated!");
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(25);
        }
        else if (other.CompareTag("Health"))
        {
            Destroy(other.gameObject);
            currentHealth = Mathf.Min(currentHealth + 10, maxHealth);
            UIManager.Instance?.UpdateEnemyHealth(currentHealth, maxHealth);
        }
    }

    private void HandleMovement()
    {
        if (!isMoving)
        {
            // Определяем направление к игроку
            Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;

            // Округляем направление до ближайшего из 4-х основных направлений
            Vector2 roundedDirection = RoundDirection(direction);
            lastMoveDirection = roundedDirection;

            // Начинаем движение
            StartMovement(roundedDirection);
            // Сбрасываем флаг выстрела при начале нового движения
            hasShotDuringCurrentMove = false;
        }
        else
        {
            // Продолжаем текущее движение
            PerformMovement();
        }
    }

    private Vector2 RoundDirection(Vector2 direction)
    {
        // Округляем направление до ближайшего из 4-х основных направлений
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return direction.y > 0 ? Vector2.up : Vector2.down;
        }
    }

    private void StartMovement(Vector2 direction)
    {
        targetPosition = (Vector2)transform.position + direction * moveDistance;
        isMoving = true;

        // Устанавливаем поворот
        float targetAngle = Vector2.SignedAngle(Vector2.up, direction);
        currentRotation = targetAngle;
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    private void PerformMovement()
    {
        // Плавное перемещение
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Проверка завершения движения
        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null) return;

        Vector2 spawnPosition = (Vector2)transform.position + lastMoveDirection * bulletOffset;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.tag = "EnemyBullet";
        // Направление пули
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = lastMoveDirection * bulletSpeed;
            float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg - 90;
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        Destroy(bullet, 3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
