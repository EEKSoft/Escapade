using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    void Start()
    {
        Level.InstantiateNewLevel(0, gameObject);
    }

}
