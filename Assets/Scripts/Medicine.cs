using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medicine : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject healthPackPrefab;
    [SerializeField] private float spawnInterval = 30f;
    [SerializeField] private int maxHealthPacks = 5;
    [SerializeField] private Vector2 spawnAreaMin;
    [SerializeField] private Vector2 spawnAreaMax;


    private float nextSpawnTime;
    private int currentHealthPacks = 0;

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;

        // �������������� ��������� �������
        for (int i = 0; i < maxHealthPacks / 2; i++)
        {
            SpawnHealthPack();
        }
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && currentHealthPacks < maxHealthPacks)
        {
            SpawnHealthPack();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnHealthPack()
    {
        // ��������� ��������� ������� � �������� �������
        Vector2 spawnPosition = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        // ������� �������
        GameObject healthPack = Instantiate(healthPackPrefab, spawnPosition, Quaternion.identity);

        currentHealthPacks++;
    }

    public void HealthPackCollected()
    {
        currentHealthPacks--;
    }

    private void OnDrawGizmosSelected()
    {
        // ��������� ���� ������ � ���������
        Gizmos.color = Color.green;
        Vector3 center = (spawnAreaMin + spawnAreaMax) / 2;
        Vector3 size = spawnAreaMax - spawnAreaMin;
        Gizmos.DrawWireCube(center, size);
    }
}