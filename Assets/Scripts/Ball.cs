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
    [SerializeField] private float maxGoalSpeed = 5f;
    [SerializeField] private float sandDragMultiplier = 0.05f;
    [SerializeField] private float iceSlideMultiplier = 0.01f;
    [SerializeField] private float snowDragMultiplier = 0.085f;

    private bool isDragging;
    private bool inHole;

    private float slowTimer = 0f;
    private float slowThreshold = 0.3f;
    private float slowDuration = 0.5f;

    private enum TerrainType { Normal, Sand, Snow, Ice }
    private TerrainType currentTerrain = TerrainType.Normal;

    // Track terrain contacts
    private HashSet<string> terrainContacts = new HashSet<string>();

    private void Update()
    {
        if (LevelManager.main.isGameOver)
        {
            rb.simulated = false;
            return;
        }

        PlayerInput();
        SmoothStop();

        if (LevelManager.main.outOfStrokes && rb.velocity.magnitude <= 0.2f && !LevelManager.main.levelCompleted)
        {
            LevelManager.main.gameOver();
        }
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
        if (distance < 1f) return;

        isDragging = false;
        lr.positionCount = 0;

        LevelManager.main.IncreaseStroke();

        Vector2 direction = (Vector2)transform.position - pos;
        float powerModifier = GetPowerModifier();

        rb.velocity = Vector2.ClampMagnitude(direction * power * powerModifier, maxPower);
    }

    private float GetPowerModifier()
    {
        return currentTerrain switch
        {
            TerrainType.Sand => 0.5f,
            TerrainType.Snow => 0.75f,
            TerrainType.Ice => 1.1f,
            _ => 1f,
        };
    }

    private void ApplyDrag()
    {
        float v = rb.velocity.magnitude;

        switch (currentTerrain)
        {
            case TerrainType.Sand when v > 0.2f:
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, sandDragMultiplier);
                break;

            case TerrainType.Snow when v > 0.1f:
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, snowDragMultiplier);
                break;

            case TerrainType.Ice when v > 0.05f:
                rb.velocity *= (1f - iceSlideMultiplier);
                break;
        }
    }

    private void HandleWaterCollision()
    {
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        gameObject.SetActive(false);
        LevelManager.main.gameOver();
    }

    private void SmoothStop()
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

    private void CheckWinState()
    {
        if (inHole) return;

        if (rb.velocity.magnitude <= maxGoalSpeed)
        {
            inHole = true;

            rb.velocity = Vector2.zero;
            gameObject.SetActive(false);

            GameObject fx = Instantiate(goalFx, transform.position, Quaternion.identity);
            Destroy(fx, 2f);

            LevelManager.main.levelComplete();
        }
    }

    private void FixedUpdate()
    {
        ApplyDrag();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal")) CheckWinState();
        if (collision.CompareTag("Water")) HandleWaterCollision();

        if (IsTerrainTag(collision.tag))
        {
            terrainContacts.Add(collision.tag);
            UpdateCurrentTerrain();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal")) CheckWinState();
        if (collision.CompareTag("Water")) HandleWaterCollision();

        if (IsTerrainTag(collision.tag))
        {
            terrainContacts.Add(collision.tag);
            UpdateCurrentTerrain();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsTerrainTag(collision.tag))
        {
            terrainContacts.Remove(collision.tag);
            UpdateCurrentTerrain();
        }
    }

    private bool IsTerrainTag(string tag)
    {
        return tag == "Sand" || tag == "Snow" || tag == "Ice";
    }

    private void UpdateCurrentTerrain()
    {
        if (terrainContacts.Contains("Sand")) currentTerrain = TerrainType.Sand;
        else if (terrainContacts.Contains("Snow")) currentTerrain = TerrainType.Snow;
        else if (terrainContacts.Contains("Ice")) currentTerrain = TerrainType.Ice;
        else currentTerrain = TerrainType.Normal;
    }
}
