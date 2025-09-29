using UnityEditor.Build.Content;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {

    [SerializeField] private GameObject bullet;

    public static BulletManager Instance;

    void Awake()
    {
        Instance = this;
    }

    /*To be added:
     Switch cases for bullet patterns
     
     
     */

    void Update()
    {
        Instantiate(bullet, transform.position, Quaternion.identity);
    }
    
}
