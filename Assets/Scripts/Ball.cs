using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private GameObject goalFx;

    [Header("Attributes")]
    [SerializeField] private float maxPower = 10f;
    [SerializeField] private float power = 2f;
    [SerializeField] private float maxGoalSpeed = 4f;

    private bool isDragging;
    private bool inHole;

    private float slowTimer = 0f;  
    private float slowThreshold = 0.3f;
    private float slowDuration = 0.5f; 

    private void Update()
    {
        PlayerInput();
        SmoothStop(); 
    }

    private bool IsReady()
    {
        return rb.velocity.magnitude <= slowThreshold;
    }

    private void PlayerInput()
    {
        if (!IsReady()) return;

        Vector2 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(transform.position, inputPos);

        if (Input.GetMouseButtonDown(0) && distance < 0.5f) DragStart();
        if (Input.GetMouseButton(0) && isDragging) DragChange(inputPos);
        if (Input.GetMouseButtonUp(0) && isDragging) DragRelease(inputPos);
    }

    private void DragStart()
    {
        isDragging = true;
        lr.positionCount = 2;
    }

    private void DragChange(Vector2 pos)
    {
        Vector2 direction = (Vector2)transform.position - pos;

        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, (Vector2)transform.position + Vector2.ClampMagnitude((direction * power) / 2, maxPower / 2));
    }

    private void DragRelease(Vector2 pos)
    {
        float distance = Vector2.Distance((Vector2)transform.position, pos);
        isDragging = false;
        lr.positionCount = 0;

        if (distance < 1f) return;

        Vector2 direction = (Vector2)transform.position - pos;
        rb.velocity = Vector2.ClampMagnitude(direction * power, maxPower);
    }

    private void SmoothStop() // <--
    {
        if (rb.velocity.magnitude <= slowThreshold && rb.velocity.magnitude > 0f)
        {
            slowTimer += Time.deltaTime;
            if (slowTimer >= slowDuration)
            {
                rb.velocity = Vector2.zero;
                slowTimer = 0f;
            }
        }
        else
        {
            slowTimer = 0f;
        }
    }

    void CheckWinState()
    {
        if (inHole) return;

        if (rb.velocity.magnitude <= maxGoalSpeed)
        {
            inHole = true;

            rb.velocity = Vector2.zero;
            gameObject.SetActive(false);

            GameObject fx = Instantiate(goalFx, transform.position, Quaternion.identity);
            Destroy(fx, 2f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Goal") CheckWinState();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Goal") CheckWinState();
    }
}
