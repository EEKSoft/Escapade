using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Pickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Get the parent gameobject of the other collider
        GameObject obj = other.gameObject;
        //Get the input of it
        CharacterInput input = obj.GetComponentInChildren<CharacterInput>();
        if (input && input.IsPlayer())
        {
            //Give the player the 'key'
            obj.GetComponentInChildren<CharacterManager>().hasKey = true;
            //Destroy self in this case
            Destroy(gameObject);
        }

    }
}
