using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionHandler : MonoBehaviour
{
    CircleCollider2D visionArea;
    const float VISION_RANGE = 5f;

    private void Start()
    {
        //Get the collider that we use for vision
        visionArea = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        Collider2D[] nearby = new Collider2D[200];
        LayerMask mask = LayerMask.GetMask("Tile", "VisTile", "Interactive");
        ContactFilter2D filter = new ContactFilter2D
        {
            layerMask = mask
        };
        visionArea.OverlapCollider(filter, nearby);
        mask = LayerMask.GetMask("Tile", "Player");
        foreach (Collider2D collider in nearby)
        {
            if(collider == null || collider.gameObject.tag == "Player") continue;
            //Get the gameobject
            GameObject obj = collider.gameObject;
            //Get spriterenderer off that
            SpriteRenderer renderer = obj.GetComponentInChildren<SpriteRenderer>();
            //Make sure not null
            if (renderer)
            {
                Vector2 center = collider.bounds.center;
                //Maths
                bool visible = false;
                //Raycasting time
                if (LayerMask.LayerToName(obj.layer) != "Interactive")
                {
                    for (int x = -1; x <= 1; x += 2)
                    {
                        for (int y = -1; y <= 1; y += 2)
                        {
                            Vector2 cornerPoint = center + new Vector2(x, y) * (collider.bounds.size / 2) * 0.95f;
                            Vector2 dir = (new Vector2(transform.position.x, transform.position.y) - cornerPoint).normalized;
                            RaycastHit2D hit = Physics2D.Raycast(cornerPoint, dir, VISION_RANGE, mask);
                            if (hit && hit.collider.gameObject.tag == "Player") visible = true;
                        }
                    }
                }
                else
                {
                    Vector2 dir = ((Vector2)transform.position - center).normalized;
                    RaycastHit2D hit = Physics2D.Raycast(center, dir, VISION_RANGE, mask);
                    Debug.DrawRay(center, dir);
                    if (hit && hit.collider.gameObject.tag == "Player") visible = true;
                }
                //Check if it is the same object, render if yes, do not render if no
                renderer.enabled = visible;
            }
        }
    } 
}
