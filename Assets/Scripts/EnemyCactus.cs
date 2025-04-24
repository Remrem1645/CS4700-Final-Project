using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCactus : MonoBehaviour
{
    public float speed = 2f;
    private Rigidbody2D rb;
    private Vector2 moveDirection = Vector2.down;

    public float spinSpeed = 120f; // degrees per second

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float zRotation = transform.eulerAngles.z;
        float radians = zRotation * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }

    void FixedUpdate()
    {
        // Move the cactus
        rb.velocity = moveDirection.normalized * speed;

        // Spin the cactus as it moves
        float rotationAmount = spinSpeed * Time.fixedDeltaTime;
        transform.Rotate(0f, 0f, rotationAmount);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Bounce off walls
        ContactPoint2D contact = collision.contacts[0];
        moveDirection = contact.normal;
        rb.velocity = moveDirection.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            Debug.Log("Entered Water Trigger: " + collision.name);
            Destroy(gameObject);
        }
    }
}
