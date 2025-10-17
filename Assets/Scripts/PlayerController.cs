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
    private Vector2 moveDirection;
    private InputAction move;


    private void Awake()
    {
        playerControls = new PlayerInputActions();
        move = playerControls.Player.Move; //Setting the new input actions and enabling the 'move' portion.
        move.Enable();
    }
    private void OnDisable()
    {
        move.Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }
    private void FixedUpdate()
    {
        //Handles movement of character using speed multipliers from the manager.
        rb.linearVelocity = new Vector2(moveDirection.x * pm.GetSpeed(), moveDirection.y * pm.GetSpeed());
    }

}