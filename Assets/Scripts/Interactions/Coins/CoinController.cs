using UnityEngine;

public class CoinController : MonoBehaviour
{
    [Header("Animation Settings")]
    public float rotationSpeed = 100f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.2f; 

    private float startY;

    void Start()
    {
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        SphereCollider col = GetComponent<SphereCollider>();
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
        }
        col.isTrigger = true;

        col.radius = 0.025f;

        startY = transform.position.y;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            PlayerData.coinsCollected++;

            Destroy(gameObject);
        }
    }
}