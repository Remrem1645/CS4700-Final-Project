using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHole : MonoBehaviour
{

    public string levelName;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ball")
        {
            SceneManager.LoadSceneAsync(levelName);
        }
    }
}
