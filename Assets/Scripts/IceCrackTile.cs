using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCrackTile : MonoBehaviour
{
    private Animator animator;
    public int crackLevel = 0;
    private bool isCracking = false;

    [SerializeField] private int maxCrackLevel = 4;
    [SerializeField] private float crackCooldown = 1.25f;

    private AudioSource audioSource;
    
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        animator = GetComponent<Animator>();
        
    }

    
    private IEnumerator CrackWithCooldown(Collider2D other)
    {
        isCracking = true;

        if (crackLevel < maxCrackLevel)
        {
            crackLevel++;
            animator.SetInteger("cracked_level", crackLevel);
        }
        else if (crackLevel >= maxCrackLevel)
        {
            HandleBreak(other);
        }

        yield return new WaitForSeconds(crackCooldown);
        isCracking = false;
    }

    
    private void OnTriggerStay2D(Collider2D other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Enemy")) && !isCracking)
        {
            StartCoroutine(CrackWithCooldown(other));
            audioSource.Play();
        }
    }
    
    private void HandleBreak(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.main.gameOver();
            crackLevel = 0;
            isCracking = false;
            animator.SetInteger("cracked_level", crackLevel);
        }
        else if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<PenguinPatrol>();
            if (enemy != null)
            {
                enemy.Fall();
                isCracking = false;
            }
        }

        
    }
    
    
    
}
