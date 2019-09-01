using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyStranger : Ally
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        highlight.enabled = true;
        highlight.color = Color.yellow;

        animator.SetFloat("VelZ", 0);
    }
}
