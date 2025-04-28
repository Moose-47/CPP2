using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform player;
    public int dmg = 2;
    public float speed = 5f;
    public float growthTime = 1f;
    public float shrinkTime = 1f;
    public float maxSize = 2f;

    private float elapsedGrowthTime = 0f;
    private float elapsedShrinkTime = 0f;
    private Vector3 initialScale;

    private bool isFollowingPlayer = true;
    private SphereCollider sphereCollider;

    private void Start()
    {
        initialScale = transform.localScale;

        sphereCollider = GetComponent<SphereCollider>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
    }

    private void Update()
    { 

        if (isFollowingPlayer) FollowPlayer();
        
        if (elapsedGrowthTime < growthTime)
        {
            float growthFactor = Mathf.Lerp(1f, maxSize, elapsedGrowthTime / growthTime);
            transform.localScale = initialScale * growthFactor;
            elapsedGrowthTime += Time.deltaTime;

            //sphereCollider.radius = transform.localScale.x * 0.1f;
        }
        else if (elapsedGrowthTime >= growthTime && elapsedShrinkTime < shrinkTime)
        {
            if (isFollowingPlayer) isFollowingPlayer = false;

            float shrinkFactor = Mathf.Lerp(maxSize, 0f, elapsedShrinkTime / shrinkTime);
            transform.localScale = initialScale * shrinkFactor;
            elapsedShrinkTime += Time.deltaTime;

            //sphereCollider.radius = transform.localScale.x * 0.1f;

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        if (elapsedShrinkTime >= shrinkTime)
        {
            Destroy(gameObject);
        }
    }

    public void FollowPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);
            }

            transform.Translate(Vector3.forward *speed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ground")) Debug.Log("hitground"); Destroy(gameObject);

        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerHealth -= dmg;
            Debug.Log(GameManager.Instance.PlayerHealth);
            Destroy(gameObject);
        }       
    }
}
