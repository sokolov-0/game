using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    //    [SerializeField] private GameObject trailPrefab;
    //    [SerializeField] private Color playerColor = Color.blue;
    //    [SerializeField] private float spawnInterval = 0.1f;
    //    [SerializeField] private float trailCheckRadius = 0.3f;

    //    private float lastSpawnTime;
    //    public bool isPlayer;
    //    private int trailLayerIndex;

    //    void Start()
    //    {
    //        isPlayer = CompareTag("Player");
    //        trailLayerIndex = LayerMask.NameToLayer("Trail");
    //        // ���� ���� �� ����������, ���������� Default (0)
    //        if (trailLayerIndex == -1) trailLayerIndex = 0;
    //    }

    //    void Update()
    //    {
    //        if (ShouldSpawnTrail() && Time.time - lastSpawnTime > spawnInterval)
    //        {
    //            SpawnTrail();
    //            lastSpawnTime = Time.time;
    //        }
    //    }

    //    private bool ShouldSpawnTrail()
    //    {
    //        if (trailLayerIndex == 0) return true;

    //        // ������� ����� ��� ��������
    //        LayerMask trailLayerMask = 1 << trailLayerIndex;
    //        Collider2D[] nearbyTrails = Physics2D.OverlapCircleAll(transform.position, trailCheckRadius, trailLayerMask);

    //        foreach (Collider2D trailCollider in nearbyTrails)
    //        {
    //            TrailSegment trail = trailCollider.GetComponent<TrailSegment>();
    //            if (trail != null && trail.IsSameColor(isPlayer))
    //            {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }

    //    private void SpawnTrail()
    //    {
    //        GameObject trail = Instantiate(trailPrefab, transform.position, Quaternion.identity);
    //        trail.layer = trailLayerIndex; // ������������� ���� ���� ���

    //        SpriteRenderer renderer = trail.GetComponent<SpriteRenderer>();
    //        renderer.color = isPlayer ? playerColor : Color.red;

    //        TrailSegment trailSegment = trail.AddComponent<TrailSegment>();
    //        trailSegment.Initialize(isPlayer);
    //    }

    //    private void OnDrawGizmosSelected()
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawWireSphere(transform.position, trailCheckRadius);
    //    }
    //}

    //public class TrailSegment : MonoBehaviour
    //{
    //    private bool isPlayerTrail;

    //    public void Initialize(bool isPlayer)
    //    {
    //        isPlayerTrail = isPlayer;
    //    }

    //    public bool IsSameColor(bool isPlayer)
    //    {
    //        return isPlayerTrail == isPlayer;
    //    }

    //    private void OnTriggerEnter2D(Collider2D other)
    //    {
    //        Trail otherTrail = other.GetComponent<Trail>();
    //        if (otherTrail != null && otherTrail.isPlayer != isPlayerTrail)
    //        {
    //            Destroy(gameObject);
    //        }
    //    }
    //}
    [Header("Trail Settings")]
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private Color playerColor = Color.blue;
    [SerializeField] private float spawnInterval = 0.1f;
    [SerializeField] private float trailCheckRadius = 0.3f;
    [SerializeField] private LayerMask obstacleLayers;

    [Header("Visual Settings")]
    [SerializeField] private int maxTrails = 100;
    [SerializeField] private float trailLifetime = 2f;

    private float lastSpawnTime;
    public bool isPlayer;
    private int trailLayerIndex;
    private Transform trailParent;


    void Start()
    {
        isPlayer = CompareTag("Player");
        trailLayerIndex = LayerMask.NameToLayer("Trail");

        // ������� ������������ ������ ��� ������
        trailParent = new GameObject("Trails").transform;

        // ���� ���� �� ����������, ���������� Default (0)
        if (trailLayerIndex == -1) trailLayerIndex = 0;
    }

    void Update()
    {
        if (ShouldSpawnTrail() && Time.time - lastSpawnTime > spawnInterval)
        {
            SpawnTrail();
            lastSpawnTime = Time.time;
        }
    }

    private bool ShouldSpawnTrail()
    {
        // ���������, ���� �� ����������� �� ����� ������
        if (Physics2D.OverlapCircle(transform.position, trailCheckRadius, obstacleLayers))
        {
            return false;
        }

        // ��������� ������ �����
        Collider2D[] nearbyTrails = Physics2D.OverlapCircleAll(transform.position, trailCheckRadius);
        foreach (Collider2D trailCollider in nearbyTrails)
        {
            TrailSegment trail = trailCollider.GetComponent<TrailSegment>();
            if (trail != null)
            {
                return false;
            }
        }
        return true;
    }

    private void SpawnTrail()
    {
        // ������� ������ �����, ���� �� ������� �����
        if (trailParent.childCount >= maxTrails)
        {
            Destroy(trailParent.GetChild(0).gameObject);
        }

        GameObject trail = Instantiate(trailPrefab, transform.position, Quaternion.identity, trailParent);
        trail.layer = trailLayerIndex;

        // ��������� �������
        SpriteRenderer renderer = trail.GetComponent<SpriteRenderer>();
        renderer.color = isPlayer ? playerColor : Color.red;
        renderer.sortingOrder = -1; // ��������, ��� ���� ��� ������� ���������

        // ��������� ����������
        Collider2D collider = trail.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        // ��������� ��������� ��������
        TrailSegment trailSegment = trail.AddComponent<TrailSegment>();
        trailSegment.Initialize(isPlayer, trailLifetime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, trailCheckRadius);
    }
}

public class TrailSegment : MonoBehaviour
{
    private bool isPlayerTrail;
    private float lifetime;

    public void Initialize(bool isPlayer, float lifeTime)
    {
        isPlayerTrail = isPlayer;
        lifetime = lifeTime;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���������� ���� ��� ������������ � ������ ������ ���������������� �����
        TrailSegment otherTrail = other.GetComponent<TrailSegment>();
        if (otherTrail != null && otherTrail.isPlayerTrail != isPlayerTrail)
        {
            Destroy(gameObject);
        }

        // ���������� ���� ��� ������������ � ������� ��� ������
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
