using UnityEngine;

public class WindZone : MonoBehaviour
{
    public Vector2 windDirection = Vector2.up;
    public float windForce = 10f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(windDirection.normalized * windForce);
            }
        }
    }
}
