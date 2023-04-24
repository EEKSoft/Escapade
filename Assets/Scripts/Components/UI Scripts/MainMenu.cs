using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField input;

    public void PlayGame()
    {
        SceneManager.LoadScene("PregameScene");
    }

    public void RerollSeed()
    {
        input.text = new System.Random().Next().ToString();
    }

    public void LaunchGame()
    {
        int seed = 0;
        string seedString = input.text.Trim();
        if (seedString == string.Empty) RerollSeed();
        seedString = input.text.Trim();
        if (!int.TryParse(seedString, out seed))
        {
            List<byte> byteList = Encoding.ASCII.GetBytes(seedString).ToList();
            while(byteList.Count < 4) byteList.Add(0);
            seed = BitConverter.ToInt32(byteList.ToArray());
        }
        Level.depth = 1;
        LevelGenerator.seed = seed;
        SceneManager.LoadScene("TransitionScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToCredits()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Credits");
    }

}
