using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WhipController : MonoBehaviour
{

    [SerializeField] Rigidbody2D rb;
    PlayerInputActions playerControls;

    private InputAction whip;

    private void OnEnable()
    {
        playerControls = new PlayerInputActions();
        whip = playerControls.Player.Whip;
        whip.Enable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnWhip()
    {
        Debug.Log("YO");
    }
}
