using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static int seed = 0;
    void Start()
    {
        Level.InstantiateNewLevel(seed, gameObject);
    }

}
