using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Required components, the manager needs components to manage
[RequireComponent(typeof(CharacterInput))]
[RequireComponent(typeof(StateMachine))]
public class CharacterManager : MonoBehaviour
{
    //Components
    public CharacterInput characterInput { get; private set; }
    public StateMachine stateMachine { get; private set; }
    public StateTypeReference stateReference { get; private set; }

    //Parameters to be given to the statemachine when needed
    private object[] stateParameters;


    void Start()
    {
        //Assign the components
        characterInput = GetComponent<CharacterInput>();
        stateMachine = GetComponent<StateMachine>();
        stateReference = GetComponent<StateTypeReference>();
        //Subscribe to move event
        characterInput.onMovementKeyAttempted += MovementAttempt;
    }

    private void MovementAttempt(Vector3 direction)
    {
        //If statemachine is busy, stop here
        if (stateMachine.busy) return;
        //Then, establish parameters variable to a size of 1, since we only need a single input for turn and move
        stateParameters = new object[1];
        //Convert the direction to an angle
        float inputAngle = Mathf.Round(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90);
        //Get the current player angle
        float playerAngle = Mathf.Round(transform.rotation.eulerAngles.z);
        //Compare them
        //If the angles match, we can move
        if(inputAngle == playerAngle)
        {
            //Assign the direction to the parameter
            stateParameters[0] = transform.position + direction;
            //Enter move state with new position parameter
            stateMachine.BeginState(stateReference.GetMoveStateType(), stateParameters);
        }
        //If the angles don't match, we do a turn state
        else
        {
            //Assign the angle to the parameter
            stateParameters[0] = inputAngle;
            //Enter turn state with new angle parameter
            stateMachine.BeginState(stateReference.GetTurnStateType(), stateParameters);
        }
    }
}
