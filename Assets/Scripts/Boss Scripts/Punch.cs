using UnityEngine;

public class Punch : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(1);
    }
}
