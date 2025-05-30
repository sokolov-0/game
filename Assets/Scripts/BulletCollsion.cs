using UnityEngine;

public class BulletCollsion:MonoBehaviour
{
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        Debug.Log(collision.gameObject.tag);
    }
}
