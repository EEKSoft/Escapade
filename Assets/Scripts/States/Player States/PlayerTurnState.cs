using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : State
{
    private const float baseDuration = 0.2f;
    private float turnFraction = 0f;
    private float startAngle;
    private float endAngle;

    public override void OnBegin()
    {
        base.OnBegin();
        duration = baseDuration;
        startAngle = self.rotation.eulerAngles.z;
        endAngle = (float)stateParameters[0];
    }

    public override void OnEnd()
    {
        base.OnEnd();
        self.rotation = Quaternion.Euler(0, 0, endAngle);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        turnFraction = age / baseDuration;
        self.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(0, 0, startAngle)), Quaternion.Euler(new Vector3(0, 0, endAngle)), turnFraction);
    }
}
