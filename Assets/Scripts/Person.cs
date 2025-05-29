using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private bool smoothRotation = true;
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Combat Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletOffset = 0.5f;
    [SerializeField] private LayerMask bulletLayer;
    [SerializeField] private int maxHealth = 3;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private Vector2 targetPosition;
    private bool isMoving = false;
    private Vector2 startPosition;
    private float targetRotation;
    private float currentRotation;
    private Vector2 lastMoveDirection = Vector2.up;
    private bool hasShotDuringCurrentMove = false;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleMovementInput();
        HandleShootingInput();
    }

    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
        Debug.Log($"Player hit! Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player defeated!");
        // «десь может быть перезагрузка уровн€ или другие действи€
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(1);
        }
        else if(other.CompareTag("Health"))
        {
            if(currentHealth != maxHealth)
            {
                currentHealth++;
                Destroy(other.gameObject);
            }
        }
    }

    private void HandleMovementInput()
    {
        if (!isMoving)
        {
            Vector2 newDirection = GetInputDirection();
            if (newDirection != Vector2.zero)
            {
                StartMovement(newDirection);
                hasShotDuringCurrentMove = false;
            }
        }
        else
        {
            PerformMovement();
        }
    }

    private Vector2 GetInputDirection()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) return Vector2.up;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) return Vector2.down;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) return Vector2.left;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) return Vector2.right;
        return Vector2.zero;
    }

    private void HandleShootingInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isMoving && !hasShotDuringCurrentMove)
            {
                ShootFromPlayer();
                hasShotDuringCurrentMove = true;
            }
        }
    }

    private void StartMovement(Vector2 direction)
    {
        startPosition = transform.position;
        targetPosition = startPosition + direction * moveDistance;
        isMoving = true;
        lastMoveDirection = direction;

        targetRotation = Vector2.SignedAngle(Vector2.up, direction);

        if (!smoothRotation)
        {
            transform.rotation = Quaternion.Euler(0, 0, targetRotation);
            currentRotation = targetRotation;
        }
    }

    private void PerformMovement()
    {
        if (smoothRotation && Mathf.Abs(currentRotation - targetRotation) > 0.1f)
        {
            currentRotation = Mathf.MoveTowardsAngle(
                currentRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    private void ShootFromPlayer()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet prefab is not assigned!");
            return;
        }

        Vector2 spawnPosition = (Vector2)transform.position + lastMoveDirection * bulletOffset;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.tag = "PlayerBullet"; // ”станавливаем тег дл€ пули игрока

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.velocity = lastMoveDirection * bulletSpeed;
            float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
        Destroy(bullet, 3f);
        //bullet.AddComponent<BulletLife>().Initialize(2f);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.red;
        Vector2 spawnPoint = (Vector2)transform.position + lastMoveDirection * bulletOffset;
        Gizmos.DrawWireSphere(spawnPoint, 0.1f);
    }
}