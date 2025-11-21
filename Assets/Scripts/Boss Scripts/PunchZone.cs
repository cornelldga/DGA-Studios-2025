using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PunchZone : MonoBehaviour
{
    private bool punching;
    private float timer;
    [Tooltip("How long it takes to punch")]
    [SerializeField] float punchTime =.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (punching)
        {
            timer += Time.deltaTime;
            if (timer > punchTime)
            {

            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            punching = true;
        }
    }
}
