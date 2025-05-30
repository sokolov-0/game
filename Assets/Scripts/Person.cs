using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool smoothRotation = true;
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Combat Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletOffset = 0.5f;
    [SerializeField] private LayerMask bulletLayer;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject cripsPrefab;
    [SerializeField] private GameObject bloodsPrefab;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private Vector2 moveDirection;
    private float targetRotation;
    private float currentRotation;
    private Vector2 lastMoveDirection = Vector2.up;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool canShoot = true;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        GameObject enemyObj = GameObject.FindWithTag("Enemy");

        if (playerObj == null || enemyObj == null)
        {
            Debug.LogError("Player or Enemy not found in scene!");
            return;
        }

        if (SideSelector.SelectedSide == "Crips")
        {
            playerObj.transform.position = new Vector2(-12.5f, 0.5f);
            enemyObj.transform.position = new Vector2(12.5f, 0.5f);
        }
        else
        {
            playerObj.transform.position = new Vector2(12.5f, 0.5f);
            enemyObj.transform.position = new Vector2(-12.5f, 0.5f);
        }
    }

    void Update()
    {
        HandleMovementInput();
        HandleShootingInput();
        HandleRotation();
    }

    void FixedUpdate()
    {
        if (moveDirection != Vector2.zero)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
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
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(1);
        }
        else if (other.gameObject.CompareTag("Health"))
        {
            if (currentHealth != maxHealth)
            {
                currentHealth++;
                Destroy(other.gameObject);
            }
        }
    }

    private void HandleMovementInput()
    {
        Vector2 inputDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) inputDirection += Vector2.up;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) inputDirection += Vector2.down;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) inputDirection += Vector2.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) inputDirection += Vector2.right;

        moveDirection = inputDirection.normalized;

        if (moveDirection != Vector2.zero)
        {
            lastMoveDirection = moveDirection;
            canShoot = true;
        }
    }

    private void HandleShootingInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canShoot)
        {
            ShootFromPlayer();
            canShoot = false;
            StartCoroutine(ResetShootCooldown());
        }
    }

    private IEnumerator ResetShootCooldown()
    {
        yield return new WaitForSeconds(0.1f); // Небольшая задержка перед следующим выстрелом
        canShoot = true;
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

    private void ShootFromPlayer()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet prefab is not assigned!");
            return;
        }

        Vector2 spawnPosition = (Vector2)transform.position + lastMoveDirection * bulletOffset;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.tag = "PlayerBullet";

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = lastMoveDirection * bulletSpeed;
            float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }

        Destroy(bullet, 3f);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.red;
        Vector2 spawnPoint = (Vector2)transform.position + lastMoveDirection * bulletOffset;
        Gizmos.DrawWireSphere(spawnPoint, 0.1f);
    }
}