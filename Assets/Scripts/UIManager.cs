using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Player UI")]
    [SerializeField] private Slider playerHealthBar;
    [SerializeField] private TextMeshProUGUI playerHealthText;

    [Header("Enemy UI")]
    [SerializeField] private Slider enemyHealthBar;
    [SerializeField] private TextMeshProUGUI enemyHealthText;

    [Header("Territories UI")]
    [SerializeField] private TextMeshProUGUI territoryCountText;

    [Header("Timer UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Match Settings")]
    [SerializeField] private float matchDuration = 120f;

    private float elapsedTime;
    private bool matchEnded = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        // Заменяем устаревшие FindObjectOfType на новую форму
        var player = Object.FindFirstObjectByType<Person>();
        if (player != null)
            UpdatePlayerHealth(player.CurrentHealth, player.MaxHealth);

        var enemy = Object.FindFirstObjectByType<AIEnemy>();
        if (enemy != null)
            UpdateEnemyHealth(enemy.CurrentHealth, enemy.MaxHealth);

        UpdateTerritories(0);
        UpdateTimer(0f);
    }

    private void Update()
    {
        if (matchEnded) return;
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= matchDuration)
        {
            elapsedTime = matchDuration;
            UpdateTimer(elapsedTime);
            EndMatch("Time is up!");
            return;
        }
        UpdateTimer(elapsedTime);
    }

    public void UpdatePlayerHealth(int current, int max)
    {
        current = Mathf.Max(0, current);
        playerHealthBar.maxValue = max;
        playerHealthBar.value    = current;
        playerHealthText.text    = $"{current}/{max}";
        if (current == 0) EndMatch("Player died!");
    }

    public void UpdateEnemyHealth(int current, int max)
    {
        current = Mathf.Max(0, current);
        enemyHealthBar.maxValue = max;
        enemyHealthBar.value    = current;
        enemyHealthText.text    = $"{current}/{max}";
        if (current == 0) EndMatch("Enemy died!");
    }

    public void UpdateTerritories(int captured)
    {
        territoryCountText.text = $"Territories: {captured}";
    }

    private void UpdateTimer(float t)
    {
        int m = Mathf.FloorToInt(t / 60f), s = Mathf.FloorToInt(t % 60f);
        timerText.text = $"{m:00}:{s:00}";
    }

    private void EndMatch(string reason)
    {
        matchEnded = true;
        Time.timeScale = 0f;
        Debug.Log($"Match ended: {reason}");
    }
}
