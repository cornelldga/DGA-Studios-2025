using UnityEngine;
/// <summary>
/// A smoke particle that grows and changes size and sprites over time
/// </summary>
public class SmokePellet : MonoBehaviour
{
    [SerializeField] private Sprite[] smokeSprites;

    [Tooltip("Increasing factor on drag")]
    [SerializeField] float dragIncrease;
    [Tooltip("At what velocity should we stop moving completely")]
    [SerializeField] float stopThreshold;
    [Tooltip("Lifetime of a smoke cloud")]
    public float lifetime;
    [Tooltip("Time threshold in seconds for when cloud should begin fading")]
    public float fadeStartTime;
    [Tooltip("Maximum size of a cloud relative to its start size")]
    public float maxScale;
    [Tooltip("How fast should the cloud expand")]
    [SerializeField] float expansionSpeed;
    [Tooltip("How far away before player whip no longer affects this pellet")]
    [SerializeField] public float effectiveDist;

    [SerializeField] public float pushForce;
    [SerializeField] float multiplier = 2.0f;

    private Rigidbody2D rb;
    //A cloud is composed of 3 seperate circle sprites. They are children, and so need to be treated differently.
    private SpriteRenderer[] spriteRenderers;
    //The starting colors of each circle
    private Color[] startColors;
    //How long has this cloud been alive.
    private float aliveTime;
    //The initial scales of each circle
    private Vector2[] childStartScales;
    //If the whip pushed this cloud away
    private bool whipped = false;
    

    /// <summary>
    /// On start, the colors of each cloud piece are stored, as well as their scales. Dambing is set to zero to allow for complete velocity.
    /// A random sprite is selected from the smokeSprites array and applied to all child sprite renderers.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // Select a random sprite from the array
        if (smokeSprites.Length > 0)
        {
            Sprite randomSprite = smokeSprites[Random.Range(0, smokeSprites.Length)];

            // Apply the random sprite to all child sprite renderers
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                if (sr != null)
                {
                    sr.sprite = randomSprite;
                }
            }
        }

        startColors = new Color[spriteRenderers.Length];
        childStartScales = new Vector2[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                startColors[i] = spriteRenderers[i].color;
                childStartScales[i] = spriteRenderers[i].transform.localScale;
            }
        }
        if (rb != null)
        {
            rb.linearDamping = 0f;
        }
    }
    /// <summary>
    /// On update, the damping is increased on the cloud, and if it has been slowed enough it will stop. Each part of the smoke cloud is increased in scale.
    /// Once the cloud has lived long enough, it will begin to fade, and after their lifetime, will be destroyed.
    /// </summary>
    private void Update()
    {
        aliveTime += !whipped ? Time.deltaTime : Time.deltaTime * multiplier;

        if (rb != null)
        {
            rb.linearDamping += dragIncrease * Time.deltaTime;

            if (whipped)
            {
                
            }
            else if (rb.linearVelocity.magnitude <= stopThreshold)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            float scaleMultiplier = 1f + (aliveTime * expansionSpeed);
            scaleMultiplier = Mathf.Min(scaleMultiplier, maxScale);
            spriteRenderers[i].transform.localScale = childStartScales[i] * scaleMultiplier;
        }
        if (aliveTime >= fadeStartTime)
        {
            float fadeProgress = (aliveTime - fadeStartTime) / (lifetime - fadeStartTime);

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] != null && aliveTime > fadeStartTime)
                {
                    Color newColor = startColors[i];
                    newColor.a = Mathf.Lerp(startColors[i].a, 0f, fadeProgress);
                    spriteRenderers[i].color = newColor;
                }
            }
        }
        if (aliveTime >= lifetime)
        {
            SmokePool.Instance.RemoveFromPool(gameObject);
            Destroy(gameObject);
        }
    }

    public void setSmokeLength(float lifetime, float fadeStartTime)
    {
        this.lifetime = lifetime;
        this.fadeStartTime = fadeStartTime;
    }
    public void Push(Vector2 angle)
    {
        Debug.Log("Pushed");
        Debug.Log(rb);
        if (rb != null && !whipped)
        {
            whipped = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(angle * pushForce);
        }
    }
}
