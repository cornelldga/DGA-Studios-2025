using System.Collections.Generic;
using UnityEngine;

public class SmokePool : MonoBehaviour
{
    [HideInInspector] private List<GameObject> smokePellets = new List<GameObject>();
    

    public static SmokePool Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
        
    public void AddToPool(GameObject pellet)
    {
        smokePellets.Add(pellet);
    }

    public void RemoveFromPool(GameObject pellet)
    {
        smokePellets.Remove(pellet);
    }

    public void OnWhip(Transform playerTransform)
    {
        if (smokePellets.Count == 0) return;
        float effectiveDist = smokePellets[0].GetComponent<SmokePellet>().effectiveDist;
        foreach (GameObject o in smokePellets)
        {
            if ((o.transform.position - playerTransform.position).magnitude < effectiveDist)
            {
                Vector2 dir = o.transform.position - playerTransform.position;
                o.GetComponent<SmokePellet>().Push(dir);
                Debug.Log((o.transform.position - playerTransform.position).magnitude);
            }
        }
    }
}