using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    //The main (Idle) state type to return to
    public Type mainStateType = typeof(PlayerIdleState);
    //Current running state
    public State currentState;
    //Gets rigidbody from the state machine owner
    public Rigidbody2D rigidBody
    {
        get
        {
            return gameObject.GetComponentInParent<Rigidbody2D>();
        }
    }

    private void Start()
    {
        //Set to main state at start
        ReturnToMain();
    }

    private void FixedUpdate()
    {
        if (currentState != null) currentState.OnUpdate();
    }

    public void BeginState(Type stateType)
    {
        //Only next state if in main state
        if (currentState.GetType() == mainStateType)
        {
            //Create an instance of the state to be begun
            currentState = Activator.CreateInstance(stateType) as State;
            currentState.machine = this;
        }
    }

    public void ReturnToMain()
    {
        //Set current state to the main state
        currentState = Activator.CreateInstance(mainStateType) as State;
        currentState.machine = this;
    }
}
