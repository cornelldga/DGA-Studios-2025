using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WhipController : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform whipPivot;
    [SerializeField] GameObject whipObject;
    [SerializeField] float cooldownDef = 1; //default for cooldown
    [SerializeField] float existanceDef = 1; //default for existance 
    [SerializeField] TextMeshProUGUI cooldownDisplay;

    private float cooldown = 0;
    private float existance = 0;
    private bool whipping = false;

    //how much we want the cooldown to increase/decrease. Bigger numbers means faster
    public float cooldownMultiplier = 1;
   
    //a magical number that I use to divide the offset of an angle from the angle it should move towards
    //this helps me make that micromovement I need to move the whip in a way that is less warped.
    //the bigger the number, the less we will adjust
    private float MAGIC_ADJUSTMENT_RATIO = 5f;

    private void Update()
    {
        cooldown -= Time.deltaTime * cooldownMultiplier;
        if(cooldownDisplay != null)
        {
            if (cooldown < 0)
            {
                cooldownDisplay.text = "Whip is ready";
            }
            else
            {
                cooldownDisplay.text = "Whip has " + cooldown + " seconds.";
            }
        }
        
           
        if (Input.GetMouseButtonDown(1) && cooldown < 0)
        {
            OnWhip();
            cooldown = cooldownDef;
        }

        if (whipping)
        {
            existance = existance - Time.deltaTime;
            if (existance < 0)
            {
                whipping = false;
                whipObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Returns an adjusted angle that makes the whip's final location more accurate to where the user clicked
    /// </summary>
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

    /// <summary>
    /// Rotates the whip towards the mouse and 
    /// </summary>
    public void OnWhip()
    {
        whipping = true;
        existance = existanceDef;
        whipObject.SetActive(true);
        //find angle between player and mouse
        //whipObject.transform.rotation = Quaternion.identity;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0f;
        Vector3 direction = mousePosition - whipPivot.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = AngleAdjustment(angle);
        whipPivot.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

}
