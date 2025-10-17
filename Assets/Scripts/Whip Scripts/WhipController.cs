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

    //a magical number that I use to divide the offset of an angle from the angle it should move towards
    //this helps me make that micromovement I need to move the whip in a way that is less warped.
    //the bigger the number, the less we will adjust
    private float MAGIC_ADJUSTMENT_RATIO = 3f;

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

    public float AngleAdjustment(float originalAngle)
    {
        float finalAngle = originalAngle;
        float sign = originalAngle / Mathf.Abs(originalAngle);
        float adjustAmountAbs = 0;
        if (originalAngle >= -45 && originalAngle <= 45)
        {
            //adjust against 0
            adjustAmountAbs = Mathf.Abs(originalAngle);
        } else if (originalAngle <= -45  && originalAngle >= -135)
        {
            adjustAmountAbs = Mathf.Abs(90 - Mathf.Abs(originalAngle));
            //adjust against -90
        } else if (originalAngle <= 135 && originalAngle >= 45)
        {
            adjustAmountAbs = Mathf.Abs(90 - Mathf.Abs(originalAngle));
            //adjust against 90
            
        } else if (originalAngle >= 135 || originalAngle <= -135)
        {
            //adjust against 180
            adjustAmountAbs = 180 - Mathf.Abs(originalAngle);
        }




        if (Mathf.Abs(originalAngle) < 90)
        {
            //adjust towards 0
            finalAngle = originalAngle - (sign * adjustAmountAbs / MAGIC_ADJUSTMENT_RATIO);
        }
        else if (Mathf.Abs(originalAngle) > 90)
        {
            //adjust towards 180
            finalAngle = originalAngle + (sign * adjustAmountAbs / MAGIC_ADJUSTMENT_RATIO);
        } 
        return finalAngle;
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
        Debug.Log(angle);
        angle = AngleAdjustment(angle);
        whipPivot.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

}
