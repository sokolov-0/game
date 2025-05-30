using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform player;
    public Transform enemy;

    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    void Start()
    {
        if (SideSelector.SelectedSide == "Crips")
        {
            player.position = leftSpawnPoint.position;
            enemy.position = rightSpawnPoint.position;
        }
        else // Bloods
        {
            player.position = rightSpawnPoint.position;
            enemy.position = leftSpawnPoint.position;
        }
    }
}
