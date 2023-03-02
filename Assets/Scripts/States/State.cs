using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    //How long the state does it's thing
    protected float duration;
    //How long it has been active
    protected float age;

    public StateMachine machine;

    protected Rigidbody2D rigidBody
    {
        get
        {
            return machine.rigidBody;
        }
    }

    public virtual void OnBegin() { }

    public virtual void OnEnd() { }

    public virtual void OnUpdate() 
    {
        //Tick age up every update
        age += Time.fixedDeltaTime;
        //If age > duration (AKA state has run for 'duration' seconds), exit to main state
        if (age >= duration) machine.ReturnToMain();
    }
}
