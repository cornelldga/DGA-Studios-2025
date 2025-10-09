using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour

{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerManager pm;
    private PlayerInputActions playerControls;
    private Vector2 moveDirection;
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
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * pm.GetSpeed(), moveDirection.y * pm.GetSpeed());
    }
    public void ApplyStatus(float speed_mult, float damage_mult)
    {
        pm.SetSpeedMod(speed_mult);
        pm.SetDamageMod(damage_mult);
    }
}