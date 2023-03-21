using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    //Components
    public CharacterInput characterInput { get; private set; }
    public StateTypeReference stateReference { get; private set; }
    public StateMachine stateMachine { get; private set; }

    //Parameters to be given to the statemachine when needed
    private object[] stateParameters;

    //Where is this character
    public Point location;

    public bool hasKey;

    void Start()
    {
        //Assign the components
        characterInput = GetComponent<CharacterInput>();
        stateReference = GetComponent<StateTypeReference>();
        //Seems unintuitive, but we add this one because it depends on the statetypereference already existing in the start method
        stateMachine = gameObject.AddComponent<StateMachine>();
        //Subscribe to move event
        characterInput.onMovementKeyAttempted += MovementAttempt;
    }

    private void MovementAttempt(Vector3 direction)
    {
        //If statemachine is busy, stop here
        if (stateMachine.busy || stateMachine.buffer > 0) return;
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
            //Update location
            Point newLocation = new Point(Mathf.RoundToInt(location.X + direction.x), Mathf.RoundToInt(location.Y + direction.y));
            //Get tile type at new location
            TileIndex tile = Level.currentLevel.QueryTile(newLocation);
            //Check tile, return if can't move into it
            if ((tile & MapTile.MovementBlocking) != 0) return;
            //Assign the new location * the tile gap to the parameter and set the new player location
            location = newLocation;
            stateParameters[0] = new Vector3(location.X, location.Y) * TerrainMap.TILE_GAP;
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
