using UnityEngine;

public class TutorialSpeedTest : MonoBehaviour
{
    [SerializeField] Vector3 position;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = position;
        }
    }
}
