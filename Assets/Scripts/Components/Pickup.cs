using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class Pickup : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        //Get the parent gameobject of the other collider
        GameObject obj = other.gameObject;
        //Get the input of it
        CharacterInput input = obj.GetComponentInChildren<CharacterInput>();
        //First make sure the input exists on the object, then make sure it belongs to a player
        if (input && input.IsPlayer())
        {
            //Give the player the 'key'
            obj.GetComponentInChildren<CharacterManager>().hasKey = true;
            //Destroy self in this case
            Destroy(gameObject);
            //Then trip flag
            GameObject events = GameObject.Find("EventSystem");
            events.GetComponent<KeyHolder>().TripKeystate();
        }
    }
}
