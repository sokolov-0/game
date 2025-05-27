using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Tooltip("Скорость передвижения врага")]
    public float moveSpeed = 3f;

    private Transform playerTransform;
    private Rigidbody2D rb;

    void Start()
    {
        // Найти объект игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Сохраняем ссылку на Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Если игрок не найден — ничего не делаем
        if (playerTransform == null) return;

        // Вычисляем направление к игроку
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Перемещаем Rigidbody2D в сторону игрока
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }
}
