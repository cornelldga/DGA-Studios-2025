using System;
using UnityEngine;

/// <summary>
/// This script can be added to any moving object to make them home in on the player's position.
/// They will maintain their current speed and rotate at `turnSpeed`.
/// </summary>
public class Homing : MonoBehaviour
{
    [Tooltip("The maximum speed that this object will turn in while homing, in deg/s")]
    [SerializeField] private float turnSpeed;
    private Rigidbody2D rb;
    private Rigidbody2D playerRB;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRB = GameManager.Instance.player.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {

        Vector2 targetDirection = (playerRB.position - rb.position).normalized;

        float currentAngle = rb.rotation;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, turnSpeed * Time.fixedDeltaTime);

        rb.rotation = newAngle;
        float newAngleRad = newAngle * Mathf.Deg2Rad;
        rb.linearVelocity = new Vector2(Mathf.Cos(newAngleRad), Mathf.Sin(newAngleRad)) * rb.linearVelocity.magnitude;
    }
}