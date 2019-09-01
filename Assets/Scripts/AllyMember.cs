using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyMember : Ally
{
    public static AllyMember[] Members
    {
        get
        {
            return FindObjectsOfType<AllyMember>();
        }
    }

    #region Movement

    internal protected Vector3 offsetPos = Vector3.zero;

    #endregion

    protected override void Start()
    {
        base.Start();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        animator.SetFloat("VelZ", AllyBoss.Boss.animator.GetFloat("VelZ"));

        if (Vector3.Distance(transform.position, AllyBoss.Boss.transform.position) > AllyBoss.Boss.radius * 2)
        {
            ChangeAllyRankTo<AllyStranger>();
        }

        highlight.enabled = false;

        if (!movementLocked)
        {
            if (AllyBoss.Boss.isGrounded && isGrounded)
            {
                Vector3 desiredPos = Vector3.Lerp(transform.position, AllyBoss.Boss.transform.position + offsetPos, 0.2f);

                //to prevent falling out of the scene
                bool collidesWithScenery = false;
                foreach (var collider in Physics.OverlapSphere(desiredPos, 0.01f))
                {
                    if (collider.tag == "Scenery")
                    {
                        collidesWithScenery = true;
                        break;
                    }
                }

                if (!collidesWithScenery)
                    transform.position = desiredPos;
            }
        }
    }
}
