using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class Exit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Get the parent gameobject of the other collider
        GameObject obj = other.gameObject;
        //Get the input of it
        CharacterInput input = obj.GetComponentInChildren<CharacterInput>();
        //First make sure the input exists on the object, then make sure it belongs to a player
        if (input && input.IsPlayer())
        {
            //Check if the player has the 'key'
           if (obj.GetComponentInChildren<CharacterManager>().hasKey)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }
            
            
        }

    }
}
