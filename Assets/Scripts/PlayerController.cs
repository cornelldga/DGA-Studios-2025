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
    [SerializeField] PlayerInventory inventory;
    private PlayerInputActions playerControls;
    private Vector2 moveDirection;
    private InputAction move;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(horizontal, vertical);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.SelectBaseSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.SelectBaseSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventory.SelectMixerSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            inventory.SelectMixerSlot(2);
        }
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * pm.GetSpeed(), moveDirection.y * pm.GetSpeed());
    }


}