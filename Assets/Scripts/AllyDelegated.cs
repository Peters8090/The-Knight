using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AllyDelegated : Ally
{
    public static AllyDelegated[] Delegated
    {
        get
        {
            return FindObjectsOfType<AllyDelegated>();
        }
    }

    #region Movement

    float zSpeed = 12f;

    #endregion

    #region Fighting

    [HideInInspector]
    public Enemy attackTarget = null;
    float maxDistToEnemy = 0.5f;

    #endregion
    
    protected override void Start()
    {
        base.Start();
    }
    
    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (attackTarget == null)
            ChangeAllyRankTo<AllyMember>();

        try
        {
            //head asap to the enemy and kill him
            if (Vector3.Distance(transform.position, attackTarget.transform.position) > maxDistToEnemy)
            {
                transform.LookAt(attackTarget.transform);
                transform.Translate(Vector3.forward * zSpeed * Time.deltaTime);
            }
        }
        catch (Exception e) { }
    }
}
