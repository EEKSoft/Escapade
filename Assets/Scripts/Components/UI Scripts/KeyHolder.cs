using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KeyHolder : MonoBehaviour
{
    public Image keyImage;
    private static KeyHolder _instance;

    private void Start()
    {
        keyImage.enabled = false;
    }

    public void TripKeystate()
    {
        keyImage.enabled = true; 
    }
}
