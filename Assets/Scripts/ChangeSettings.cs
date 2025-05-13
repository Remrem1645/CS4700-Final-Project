using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSettings : MonoBehaviour
{
    private int currentColor = 0;
    public Color ballColor;

    public void OnArrowClick(bool right)
    {
        if(right)
        {
            currentColor++;
            if(currentColor == GameSettings.instance.ballColors.Count)
                currentColor = 0;
            OnSelectionChanged();
        }
        else
        {
            currentColor--;
            if(currentColor < 0)
                currentColor = GameSettings.instance.ballColors.Count - 1;
            OnSelectionChanged();
        }
    }
    private void OnSelectionChanged()
    {
        ballColor = GameSettings.instance.ballColors[currentColor];
        PlayerPrefs.SetInt("currentColor", currentColor);
    }

}
