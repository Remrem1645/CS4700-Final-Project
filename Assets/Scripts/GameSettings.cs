using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{

    public static GameSettings instance;
    private void Awake()
    {
        if (GameSettings.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public List<Color> ballColors;


}
