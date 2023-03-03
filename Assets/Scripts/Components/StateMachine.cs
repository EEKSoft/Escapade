using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    //The main (Idle) state type to return to
    public Type mainStateType { get; private set; }
    //Current running state
    private State currentState;
    //Verification if in a state
    public bool busy { get; private set; }

    //Reference to the outer manager
    public CharacterManager characterManager { get; private set; }

    private void Start()
    {
        //Start not busy
        busy = false;
        //Get manager component
        characterManager = GetComponent<CharacterManager>();
        //Assign the mainState to what the manager says it is
        mainStateType = characterManager.stateReference.GetMainStateType();
        //Set to main state at start
        ReturnToMain();
    }

    private void FixedUpdate()
    {
        //Proc the onupdate of the current state every physics tick
        if (currentState != null) currentState.OnUpdate();
    }

    public void BeginState(Type stateType, object[] parameters = null)
    {
        //Only next state if in main state
        if (currentState.GetType() == mainStateType)
        {
            //Create an instance of the state to be begun
            currentState = Activator.CreateInstance(stateType) as State;
            //Set machine of state to this instance
            currentState.machine = this;
            //Apply parameters to state
            currentState.stateParameters = parameters;
            //Apply begin behavior to current state
            currentState.OnBegin();
            //Set to busy
            busy = true;
        }
    }

    public void ReturnToMain()
    {
        //Apply end behavior to current state
        if(currentState != null) currentState.OnEnd();
        //Set current state to the main state
        currentState = Activator.CreateInstance(mainStateType) as State;
        //Set the machine of the state to this instance
        currentState.machine = this;
        //Set to not busy
        busy = false;
    }
}
