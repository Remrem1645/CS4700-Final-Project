using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindVisualMover : MonoBehaviour
{
    public Vector2 windDirection = Vector2.up;
    public float speed = 2f;
    public float loopDistance = 20f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position += (Vector3)(windDirection.normalized * speed * Time.deltaTime);

        // Reset position to loop
        if (Vector3.Distance(startPos, transform.position) > loopDistance)
        {
            transform.position = startPos;
        }
    }
}
