using UnityEngine;
using System.Collections;

public class LandingIndicator : MonoBehaviour
{
    [SerializeField] float spawnScale;
    [SerializeField] float spinMaxDegrees;
    [SerializeField] float duration;

    void Start()
    {
        StartCoroutine(SpawnAnim());
        spinMaxDegrees *= Random.Range(0.9f, 1.1f);
    }

    IEnumerator SpawnAnim()
    {
        float elapsed = 0f;
        float startZ = transform.eulerAngles.z;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float eased = Mathf.Log(1f + t * (Mathf.Exp(1) - 1f));

            transform.localScale = Vector3.one * Mathf.Lerp(spawnScale, 1.5f, eased);
            transform.eulerAngles = new Vector3(0, 0, startZ + spinMaxDegrees * (1f - eased));

            yield return null;
        }

        transform.localScale = Vector3.one;
    }
}