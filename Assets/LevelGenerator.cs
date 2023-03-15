using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    void Start()
    {
        Level.InstantiateNewLevel(new System.Random().Next(), gameObject);
    }

}
