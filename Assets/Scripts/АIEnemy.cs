using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class АIEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool smoothRotation = true;
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Combat Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 7f;
    [SerializeField] private float bulletOffset = 0.5f;
    [SerializeField] private int maxHealth = 3;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float avoidanceDistance = 1f;
    [SerializeField] private LayerMask obstacleLayer;

    private Transform player;
    private float nextFireTime;
    private Vector2 moveDirection;
    private float targetRotation;
    private float currentRotation;
    private Vector2 lastMoveDirection = Vector2.down;
    private int currentHealth;
    private Rigidbody2D rb;
    private Vector2[] checkDirections = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            CalculateMovementDirection();
            HandleRotation();

            if (distanceToPlayer <= attackRange && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
        else
        {
            moveDirection = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (moveDirection != Vector2.zero)
        {
            rb.velocity = moveDirection * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy hit! Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy defeated!");
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            Destroy(collision.gameObject);
            TakeDamage(1);
        }
        else if (collision.gameObject.CompareTag("Health"))
        {
            if (currentHealth != maxHealth)
            {
                currentHealth++;
                Destroy(collision.gameObject);
            }
        }
    }

    private void CalculateMovementDirection()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector2 avoidanceDirection = GetObstacleAvoidanceDirection();

        // Комбинируем направление к игроку и направление избегания препятствий
        moveDirection = (directionToPlayer + avoidanceDirection * 0.5f).normalized;

        if (moveDirection != Vector2.zero)
        {
            lastMoveDirection = moveDirection;
        }
    }

    private Vector2 GetObstacleAvoidanceDirection()
    {
        Vector2 avoidanceDirection = Vector2.zero;

        foreach (Vector2 dir in checkDirections)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, avoidanceDistance, obstacleLayer);
            if (hit.collider != null)
            {
                // Чем ближе препятствие, тем сильнее уклонение
                float avoidanceForce = 1f - (hit.distance / avoidanceDistance);
                avoidanceDirection -= dir * avoidanceForce;
            }
        }

        return avoidanceDirection.normalized;
    }

    private void HandleRotation()
    {
        if (moveDirection != Vector2.zero)
        {
            targetRotation = Vector2.SignedAngle(Vector2.up, moveDirection);
        }
        else
        {
            targetRotation = Vector2.SignedAngle(Vector2.up, lastMoveDirection);
        }

        if (smoothRotation)
        {
            currentRotation = Mathf.MoveTowardsAngle(
                currentRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, targetRotation);
            currentRotation = targetRotation;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null) return;

        Vector2 spawnPosition = (Vector2)transform.position + lastMoveDirection * bulletOffset;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.tag = "EnemyBullet";

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.velocity = lastMoveDirection * bulletSpeed;
            float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg - 90;
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // Добавляем коллайдер, если его нет
        if (bullet.GetComponent<Collider2D>() == null)
        {
            bullet.AddComponent<BoxCollider2D>();
        }

        Destroy(bullet, 3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Визуализация лучей для обнаружения препятствий
        Gizmos.color = Color.blue;
        foreach (Vector2 dir in checkDirections)
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)dir * avoidanceDistance);
        }
    }
}
