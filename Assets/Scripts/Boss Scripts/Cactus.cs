using System;
using System.Collections;
using UnityEngine;

public class Cactus : MonoBehaviour, IDamageable
{
    [SerializeField] private float fireRate;
    [SerializeField] private GameObject spine;
    private int spineCount;
    [SerializeField] private int maxHealth;
    [SerializeField] private Animator animator;
    [SerializeField] private float animationTime = 5.0f;
    [SerializeField] private float animOffsetTime = 3.0f;
    private float timer;
    private float health;
    private Vector3[] spinesSpawnPos;
    private Quaternion[] spinesSpawnRot;

    private Coroutine attackCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        health=maxHealth;
        spinesSpawnPos = new Vector3[this.transform.childCount];
        spinesSpawnRot = new Quaternion[this.transform.childCount];
        spineCount = spinesSpawnPos.Length;
        Debug.Log(spinesSpawnPos.Length);

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
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine(SpineAttackCoroutine());
        }
    }

    private IEnumerator SpineAttackCoroutine()
    {
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(animOffsetTime);

        for (int i = 0; i < spineCount; i++)
        {
            GameObject bullet = Instantiate(spine, spinesSpawnPos[i], spinesSpawnRot[i]);
        }

        yield return new WaitForSeconds(animationTime - animOffsetTime);
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
}
