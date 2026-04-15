using System;
using System.Collections;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class Cactus : MonoBehaviour, IDamageable
{
    [SerializeField] private float fireRate;
    [SerializeField] private GameObject spine;
    private int spineCount;
    [SerializeField] private int maxHealth;
    private Animator animator;
    private float timer;
    private float health;
    [HideInInspector]
    public int locationID = -1;
    private Ash ash;

    private Vector3[] spinesSpawnPos;
    private Quaternion[] spinesSpawnRot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        health=maxHealth;
        ash = FindAnyObjectByType<Ash>();
        animator = GetComponent<Animator>();

        spinesSpawnPos = new Vector3[this.transform.childCount];
        spinesSpawnRot = new Quaternion[this.transform.childCount];
        spineCount = spinesSpawnPos.Length;

        for (int i = 0; i < this.transform.childCount; i++)
        {
            spinesSpawnPos[i] = new Vector3(this.transform.GetChild(i).position.x, this.transform.GetChild(i).position.y, this.transform.GetChild(i).position.z);
            spinesSpawnRot[i] = new Quaternion(this.transform.GetChild(i).rotation.x, this.transform.GetChild(i).rotation.y, this.transform.GetChild(i).rotation.z, this.transform.GetChild(i).rotation.w);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > fireRate)
        {
            timer = 0;
            EnterAttackingAnimation();
        }
    }

    public void SpawnSpines()
    {
        for (int i = 0; i < spineCount; i++)
            Instantiate(spine, spinesSpawnPos[i], spinesSpawnRot[i]);
    }
     void EnterAttackingAnimation()
    {
        animator.SetBool("isAttacking", true);
    }

    public void ExitAttackingAnimation()
    {
        animator.SetBool("isAttacking", false);
    }

    public void TakeDamage(float damage)
    {
        health --;
        if (health == 0)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        ash.deployedSeeds[locationID] = false;
    }
}
