using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    //Used to determine if the entity is a player, useful when we (maybe) add enemies
    [SerializeField]
    private bool isPlayer;
    //Event for handling when player or ai attempts to move
    public delegate void MovementKey(Vector3 direction);
    public event MovementKey onMovementKeyAttempted;

    //Reference to the outer manager
    public CharacterManager characterManager { get; private set; }

    private void Start()
    {
        //Get the components when this one is added
        characterManager = GetComponent<CharacterManager>();
    }

    void Update()
    {
        ReadMovementKeys();
    }

    private void ReadMovementKeys()
    {
        //If there is any axis movement
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Vector3 direction = Vector3.zero;
            //Input priority: Up -> Down -> Right -> Left, just in case multiple movement inputs
            if (Input.GetAxis("Vertical") > 0) direction = Vector3.up;
            else if (Input.GetAxis("Vertical") < 0) direction = Vector3.down;
            else if (Input.GetAxis("Horizontal") > 0) direction = Vector3.right;
            else if (Input.GetAxis("Horizontal") < 0) direction = Vector3.left;
            //No matter what, attempt movement if there is input
            onMovementKeyAttempted.Invoke(direction);
        }
    }
}
