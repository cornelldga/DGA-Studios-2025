using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour

{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerManager pm;
    private PlayerInputActions playerControls;
    [SerializeField] float moveSpeed;

    private Vector2 moveDirection;
    private InputAction move;

    void Start()
    {
    }

    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * pm.GetSpeed(), moveDirection.y * pm.GetSpeed());
    }

}