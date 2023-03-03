using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : State
{
    private const float baseDuration = 0.6f;
    private float movementFraction = 0f;
    private Vector3 startPosition;
    private Vector3 endPosition;

    public override void OnBegin()
    {
        base.OnBegin();
        duration = baseDuration;
        startPosition = parent.position;
        endPosition = (Vector3)stateParameters[0];
    }

    public override void OnEnd()
    {
        base.OnEnd();
        parent.position = endPosition;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        movementFraction = age / baseDuration;
        parent.position = Vector3.Lerp(startPosition, endPosition, movementFraction);
    }
}
