using UnityEngine;
using UnityEngine.InputSystem;

public class WhipController : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform whipPivot;
    [SerializeField] GameObject whipObject;
    PlayerInputActions playerControls;

    private InputAction whip;
    private bool whipping;
    private float timer;

    private void OnEnable()
    {
        playerControls = new PlayerInputActions();
        whip = playerControls.Player.Whip;
        whip.Enable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        whipObject.SetActive(false);
    }

    private void Update()
    {
        if (whipping)
        {
            timer = timer - Time.deltaTime;
            if (timer < 0)
            {
                whipping = false;
                whipObject.SetActive(false);
            }
        }
    }

    public void OnWhip()
    {
        whipping = true;
        timer = 1;
        whipObject.SetActive(true);
        //find angle between player and mouse
        //whipObject.transform.rotation = Quaternion.identity;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0f;
        Vector3 direction = mousePosition - whipPivot.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        whipPivot.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

}
