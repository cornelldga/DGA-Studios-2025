using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour

{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;
    [SerializeField] PlayerInputActions playerControls;

    // [SerializeField] InputAction playerControls;

    private Vector2 moveDirection;
    private Vector2 prevMoveDirection;

    private float curSpeed;

    private InputAction move;


    private void OnEnable()
    {
        playerControls = new PlayerInputActions();
        move = playerControls.Player.Move;
        move.Enable(); 
    }
    private void OnDisable()
    {
        move.Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
       
        rb.linearVelocity = new Vector2(moveDirection.x * curSpeed, moveDirection.y * moveSpeed);

    }

    public void setSpeedMod(float mod)
    {
        curSpeed = moveSpeed * mod;
    }
}