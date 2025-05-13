using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI strokeUI;
    [Space(10)]
    [SerializeField] private GameObject levelCompleteUI;
    [SerializeField] private TextMeshProUGUI levelCompletedStrokeUI;
    [Space(10)]
    [SerializeField] private GameObject gameOverUI;

    [Header("Attributes")]
    [SerializeField] private int maxStrokes;

    private int strokes;
    [HideInInspector] public bool outOfStrokes;
    [HideInInspector] public bool levelCompleted;
    [HideInInspector] public bool isGameOver;
    private void Awake()
    {
        main = this;
    }

    private AudioSource audioSource;
    private void Start()
    {
        updateStrokeUI();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void IncreaseStroke()
    {
        strokes++;
        updateStrokeUI();

        if (strokes >= maxStrokes)
        {
            outOfStrokes = true;
        }
    }

    public void levelComplete()
    {
        levelCompleted = true;
        audioSource.Play();
        StartCoroutine(ShowLevelCompleteAfterDelay());
        
    }

    public void gameOver()
    {
        isGameOver = true;
        gameOverUI.SetActive(true);
    }

    private void updateStrokeUI()
    {
        strokeUI.text = strokes + "/" + maxStrokes;
    }

    private IEnumerator ShowLevelCompleteAfterDelay()
{
    yield return new WaitForSeconds(1.0f); 

    levelCompletedStrokeUI.text = strokes > 1 
        ? "You completed the hole in " + strokes + " strokes" 
        : "Hole in one!";

    levelCompleteUI.SetActive(true);
}

}
