using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyRoomba : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private Vector2 moveDirection = Vector2.down;
    
    public float visionDistance = 4.5f;
    public float visionAngle = 30f;

    
    public LayerMask obstacleMask;
    public Transform player;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate()
    {
        float moveAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, moveAngle + 90f);
        
        rb.velocity = moveDirection * speed;
        
        Vector2 toPlayer = (player.position - transform.position);
        float distance = toPlayer.magnitude;

        if (distance > visionDistance) return;

        float angleToPlayer = Vector2.Angle(moveDirection, toPlayer.normalized);
        if (angleToPlayer > visionAngle) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, distance, obstacleMask);
        if (hit.collider) return;
        
        moveDirection = toPlayer.normalized;

    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ball"))
        {
            LevelManager.main.gameOver();
        }
        else
        {
            moveDirection *= -1;
            transform.Rotate(0f, 0f, 180f);
        }
    }
    
}
