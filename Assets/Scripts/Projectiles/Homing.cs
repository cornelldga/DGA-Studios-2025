using System;
using UnityEngine;

// TODO add header
public class Homing : MonoBehaviour
{
    [Tooltip("The maximum speed that this object will turn in while homing, in deg/s")]
    [SerializeField] private float turnSpeed;
    private Rigidbody2D player;
    private Rigidbody2D rb;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            player = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        }
        else
        {
            enabled = false;
        }

        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 targetDirection = (player.position - rb.position).normalized;
        
        float currentAngle = rb.rotation;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, turnSpeed * Time.fixedDeltaTime);

        rb.rotation = newAngle;
        float newAngleRad = newAngle * Mathf.Deg2Rad;
        rb.linearVelocity = new Vector2(Mathf.Cos(newAngleRad), Mathf.Sin(newAngleRad)) * rb.linearVelocity.magnitude;
    }
}