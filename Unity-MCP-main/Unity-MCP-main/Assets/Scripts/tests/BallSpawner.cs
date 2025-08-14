using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject ballPrefab; // Assign a ball prefab with a Rigidbody in the Inspector
    public BeatTracker beatTracker;
    [Range(1, 100)]
    public int maxActiveBalls = 10;
    [Range(0f, 1f)]
    public float spawnOffset = 0f; // Offset from beat (0 = on beat, 0.5 = between beats)

    [Header("Ball Physics")]
    public float minVelocity = 2f;
    public float maxVelocity = 8f;
    public float despawnTime = 5f;  // Add this line

    [Header("Spread Settings")]
    [Range(0f, 180f)]
    public float spreadAngleX = 10f; // Maximum spread angle in X axis
    [Range(0f, 180f)]
    public float spreadAngleY = 10f; // Maximum spread angle in Y axis

    private List<GameObject> activeBalls = new List<GameObject>();
    private float spawnTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (beatTracker == null)
        {
            beatTracker = FindObjectOfType<BeatTracker>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (beatTracker != null)
        {
            float spawnInterval = 60f / beatTracker.bpm;
            spawnTimer += Time.deltaTime;
            
            if (spawnTimer >= spawnInterval)
            {
                SpawnBall();
                spawnTimer = 0f;
            }
        }
    }

    void SpawnBall()
    {
        // Check if we need to remove the oldest ball
        if (activeBalls.Count >= maxActiveBalls)
        {
            if (activeBalls[0] != null)
            {
                Destroy(activeBalls[0]);
            }
            activeBalls.RemoveAt(0);
        }

        // Spawn at spawner's position
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        activeBalls.Add(ball);

        // Add despawn component with callback
        DestroyAfterDelay destroyer = ball.AddComponent<DestroyAfterDelay>();
        destroyer.OnDestroyCallback += () => activeBalls.Remove(ball);
        destroyer.delay = despawnTime;

        // Ensure the ball has a Rigidbody
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = ball.AddComponent<Rigidbody>();
        }

        // Give random velocity
        Vector3 randomDir = GetRandomDirection();
        float randomSpeed = Random.Range(minVelocity, maxVelocity);
        rb.velocity = randomDir * randomSpeed;
    }

    private Vector3 GetRandomDirection()
    {
        // Calculate random angles within the specified ranges
        float randomX = Random.Range(-spreadAngleX, spreadAngleX);
        float randomY = Random.Range(-spreadAngleY, spreadAngleY);

        // Apply rotation to the forward direction
        Vector3 direction = transform.forward;
        direction = Quaternion.Euler(randomX, randomY, 0) * direction;

        return direction.normalized;
    }

    private void OnValidate()
    {
        // Clamp the spread angles to valid ranges
        spreadAngleX = Mathf.Clamp(spreadAngleX, 0f, 180f);
        spreadAngleY = Mathf.Clamp(spreadAngleY, 0f, 180f);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw forward direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        // Draw X spread (horizontal)
        Gizmos.color = Color.red;
        Vector3 rightSpread = Quaternion.Euler(0, spreadAngleX, 0) * transform.forward;
        Vector3 leftSpread = Quaternion.Euler(0, -spreadAngleX, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightSpread * 2f);
        Gizmos.DrawRay(transform.position, leftSpread * 2f);

        // Draw Y spread (vertical)
        Gizmos.color = Color.green;
        Vector3 upSpread = Quaternion.Euler(-spreadAngleY, 0, 0) * transform.forward;
        Vector3 downSpread = Quaternion.Euler(spreadAngleY, 0, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, upSpread * 2f);
        Gizmos.DrawRay(transform.position, downSpread * 2f);
    }
}
