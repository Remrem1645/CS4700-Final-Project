using UnityEngine;
using System.Collections;


public class PenguinPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float roamRadius = 5f;
    public float waitTime = 1.5f;

    [Header("Kick Settings")]
    public float kickForce = 5f;
    public float kickCooldown = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;
    private Vector2 originPosition;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private bool canKick = true;
    private AudioSource audioSource;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        originPosition = transform.position;

        StartCoroutine(RoamRoutine());
        StartCoroutine(RandomQuackRoutine());
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (isMoving)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);

            animator.SetBool("isWalking", true);
            sr.flipX = direction.x < 0;

            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                animator.SetBool("isWalking", false);
            }
        }
    }

    IEnumerator RoamRoutine()
    {
        while (true)
        {
            Vector2 offset = Random.insideUnitCircle * roamRadius;
            targetPosition = originPosition + offset;
            isMoving = true;

            float moveDuration = Random.Range(1.0f, 2.5f);
            yield return new WaitForSeconds(moveDuration);

            isMoving = false;
            animator.SetBool("isWalking", false);
            
            yield return new WaitForSeconds(waitTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canKick)
        {
            Rigidbody2D ballRb = other.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                Vector2 kickDir = (other.transform.position - transform.position).normalized;
                ballRb.velocity = kickDir * kickForce;

                StartCoroutine(KickCooldown());
            }
        }
    }

    IEnumerator KickCooldown()
    {
        canKick = false;
        yield return new WaitForSeconds(kickCooldown);
        canKick = true;
    }

    public void Fall()
    {
        Destroy(gameObject); // or play animation/sound
    }
    
    IEnumerator RandomQuackRoutine()
    {
        while (true)
        {
            float delay = Random.Range(4f, 16f);
            yield return new WaitForSeconds(delay);
            audioSource.Play();
        }
    }
}
