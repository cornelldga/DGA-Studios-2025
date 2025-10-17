using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour

{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;

    // [SerializeField] InputAction playerControls;

    private Vector2 moveDirection;
    private Vector2 prevMoveDirection;

    private float curSpeed;

    private InputAction move;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curSpeed = moveSpeed;
    }

    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * curSpeed, moveDirection.y * curSpeed);
    }

    public void setSpeedMod(float mod)
    {
        curSpeed = moveSpeed * mod;
    }
}