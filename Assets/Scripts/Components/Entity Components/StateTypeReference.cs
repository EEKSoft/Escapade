using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateTypeReference : MonoBehaviour
{
    [SerializeField]
    private string mainStateName;
    [SerializeField]
    private string moveStateName;
    [SerializeField]
    private string turnStateName;

    public Type GetMainStateType()
    {
        return Type.GetType(mainStateName);
    }

    public Type GetMoveStateType()
    {
        return Type.GetType(moveStateName);
    }

    public Type GetTurnStateType() 
    {
        return Type.GetType(turnStateName);
    }
}
