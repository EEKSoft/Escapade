using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    //How long the state does it's thing
    protected float? duration;
    //How long it has been active
    protected float age;
    //Used to pass parameters to states (Like direction to turn towards or position to move to)
    //As an object, you can give it anything and convert in the state
    public object[] stateParameters;

    //Machine that controls the state
    public StateMachine machine;

    //Parent transform reference
    protected Transform parent { get { return machine.transform.parent; } }
    //Self transform reference
    protected Transform self { get { return machine.transform; } }

    public virtual void OnBegin()
    {
        //Start at age 0
        age = 0f;
    }

    public virtual void OnEnd() { }

    public virtual void OnUpdate() 
    {
        //Allows for infinite states, like idle, by not assigning duration
        if (duration != null)
        {
            //Tick age up every update
            age += Time.fixedDeltaTime;
            //If age > duration (AKA state has run for 'duration' seconds), exit to main state
            if (age >= duration) machine.ReturnToMain();
        }
    }
}
