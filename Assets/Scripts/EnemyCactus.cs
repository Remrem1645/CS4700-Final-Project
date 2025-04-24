using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCactus : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private Vector2 moveDirection = Vector2.down;
    float zRotation;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        zRotation = transform.eulerAngles.z;
        
        float radians = zRotation * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }
    
    void FixedUpdate()
    {
        
        float moveAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, moveAngle);
        
        rb.velocity = moveDirection.normalized * speed;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.contacts[0];
        Vector2 bounceDirection = contact.normal;
        
        moveDirection = contact.normal;
        rb.velocity = moveDirection.normalized * speed;
        
        float angle = Mathf.Atan2(bounceDirection.y, bounceDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        
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
