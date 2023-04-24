using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionHandler : MonoBehaviour
{
    const float FADE_TIME = 1f;
    const float SHOW_TIME = 2f;

    float fadeinRemaining = FADE_TIME;
    float fadeoutRemaining = SHOW_TIME;
    float showRemaining = SHOW_TIME;

    public TMP_Text text;
    public TMP_Text lText;

    void Start()
    {
        text.text = Level.depth.ToString();
    }

    void FixedUpdate()
    {
        float percent;
        if (fadeinRemaining > 0)
        {
            percent = fadeinRemaining / FADE_TIME;
            lText.color = text.color = Color.Lerp(Color.white, Color.black, percent);
            fadeinRemaining -= Time.fixedDeltaTime;
        }
        else
        {
            if (showRemaining > 0) showRemaining -= Time.fixedDeltaTime;
            else
            {
                if (fadeoutRemaining > 0)
                {
                    percent = fadeoutRemaining / FADE_TIME;
                    lText.color = text.color = Color.Lerp(Color.black, Color.white, percent);
                    fadeoutRemaining -= Time.fixedDeltaTime;
                }
                else SceneManager.LoadScene("PlayScene");
            }
        }
    }
}
