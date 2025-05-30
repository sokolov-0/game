using UnityEngine;
using UnityEngine.UI;
using TMPro; // удалить, если не используете TMP

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;   // заменить на Text, если не TMP

    [SerializeField] private TextMeshProUGUI territoryCountText;
    [SerializeField] private TextMeshProUGUI timerText;

    private float elapsedTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        elapsedTime = 0f;
        // больше не сбрасываем здоровье в 0
        UpdateTerritories(0);
        UpdateTimer(0f);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        UpdateTimer(elapsedTime);
    }

    public void UpdateHealth(int current, int max)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = max;
            healthBar.value = current;
        }
        if (healthText != null)
            healthText.text = $"{current}/{max}";
    }

    public void UpdateTerritories(int captured)
    {
        if (territoryCountText != null)
            territoryCountText.text = $"Territories: {captured}";
    }

    private void UpdateTimer(float t)
    {
        if (timerText == null) return;
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        timerText.text = $"{m:00}:{s:00}";
    }
}
