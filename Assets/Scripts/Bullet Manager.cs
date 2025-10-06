using UnityEditor.Build.Content;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletManager : MonoBehaviour {

    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletCooldown;
    private float timer;

    public static BulletManager Instance;

    void Awake()
    {
        Instance = this;
        timer = 0f;
    }

    /*To be added:
     Switch cases for bullet patterns
     Bullet patterns may include homing attacks
     
     */

    void Update()
    {
        transform.position = new Vector2(Random.Range(-5,9), 0);
        timer += Time.deltaTime;
        if (timer > bulletCooldown)
        {
            Instantiate(bullet, transform.position, Quaternion.identity);
            timer = 0f;
        }
    }
    
}
