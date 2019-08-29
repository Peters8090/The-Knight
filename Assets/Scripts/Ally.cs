using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ally : Knight
{
    //TODO: Classes AllyStranger, AllyMember, AllyBoss inheriting after Ally
    //TODO: Swiping variables compatibility with various resolutions (Boss -> Movement)

    #region Enums' Definitions
    /// <summary>
    /// Members follow the Boss, Stranger isn't a member of Allies
    /// </summary>
    public enum AllyRank
    {
        Boss, Member, Stranger, Kicked_Out
    }
    #endregion

    #region Component References
    Light highlight;
    #endregion

    #region Ally Groups

    /// <summary>
    /// If it is not equal to stranger, I am an ally
    /// </summary>
    internal AllyRank allyRank = AllyRank.Stranger;

    /// <summary>
    /// Returns all instances of Ally class with allyRank equal either Member or Boss
    /// </summary>
    public static Ally[] Allies
    {
        get
        {
            return FindObjectsOfType<Ally>().Where(allyClass => allyClass.allyRank != AllyRank.Stranger).ToArray();
        }
    }

    /// <summary>
    /// Static reference to the boss GameObject
    /// </summary>
    /// <returns></returns>
    public static Ally Boss
    {
        get
        {
            Ally[] knights = FindObjectsOfType<Ally>();

            for (int i = 0; i < knights.Length; i++)
            {
                if (knights[i].allyRank == AllyRank.Boss)
                    return knights[i];
            }
            return null;
        }
    }

    #endregion

    #region All Ally

    #region Movement

    float zSpeed = 15f;

    #endregion

    #endregion

    #region Boss

    #region Movement
    Vector3 lastMousePos = Vector3.zero;
    float sensitivity = 0.3f;
    float maxMoveLength = 15f;
    float slide = 0.05f;
    float speedBoostOnTouch = 0.5f;
    #endregion

    #endregion

    #region Member

    #region Movement

    protected Vector3 offsetPos = Vector3.zero;

    #endregion

    #region Fighting

    [HideInInspector]
    public Enemy attackTarget = null;

    float maxDistToEnemy = 0.5f;

    #endregion

    #endregion

    protected override void Start()
    {
        base.Start();

        highlight = transform.Find("Point Light").GetComponent<Light>();

        if (name == "AllyBoss")
        {
            allyRank = AllyRank.Boss;
        }
    }

    protected override void LateUpdate()
    {
        if (GameControlScript.gameOver)
            return;

        base.LateUpdate();

        switch (allyRank)
        {
            case AllyRank.Stranger:
                highlight.enabled = true;
                highlight.color = Color.yellow;

                animator.SetFloat("VelZ", 0);
                break;


            case AllyRank.Member:
                animator.SetFloat("VelZ", Boss.animator.GetFloat("VelZ"));

                if (attackTarget != null)
                {
                    allyRank = AllyRank.Kicked_Out;
                    break;
                }

                highlight.enabled = false;

                if (!movementLocked)
                {
                    if (Boss.isGrounded && isGrounded)
                    {
                        transform.position = Vector3.Lerp(transform.position, Boss.transform.position + offsetPos, 0.2f);
                    }
                }
                break;


            case AllyRank.Boss:
                highlight.enabled = true;
                highlight.color = Color.red;

                animator.SetFloat("VelZ", Mathf.Abs(rb.velocity.z / (zSpeed / 1.3f)));
                if (animator.GetFloat("VelZ") > 1)
                    animator.SetFloat("VelZ", 1);

                #region Members' pos
                Ally[] members = Allies.Where(allyClass => allyClass.allyRank == AllyRank.Member).ToArray();
                float angle = 90; // Initial angle
                float increment = 30;
                float radius = 3f;
                float factor;

                if(members.Count() % 2 == 1)
                {
                    factor = (members.Count() - 1) / 2;
                }
                else
                {
                    factor = members.Count() / 2 - 0.5f;
                }

                angle -= factor * increment;

                foreach (Ally ally in members)
                {
                    float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
                    float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

                    ally.offsetPos = new Vector3(x, 0, y);

                    angle += increment;
                }
                #endregion

                if (!movementLocked)
                {
                    //swipes work both on the mobile and pc
                    if (Input.GetMouseButtonDown(0))
                    {
                        lastMousePos = Input.mousePosition;
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        Vector3 deltaMousePos = -(lastMousePos - Input.mousePosition);
                        lastMousePos = Input.mousePosition;

                        //calculate the finger move
                        deltaMousePos = Vector3.ClampMagnitude(deltaMousePos, maxMoveLength);

                        //if any touch happens, accelerate forward
                        rb.velocity += Vector3.forward * speedBoostOnTouch;

                        //move with the swipe
                        rb.velocity += new Vector3(deltaMousePos.x, 0, deltaMousePos.y) * sensitivity - rb.velocity * slide;
                    }

                    //player can't move backwards (he can slow down)
                    if (rb.velocity.z < 0)
                        rb.velocity += Vector3.forward * Mathf.Abs(rb.velocity.z);

                    //constant forward force
                    if (rb.velocity.z < zSpeed * 0.5f)
                        rb.velocity += Vector3.forward * zSpeed * 0.5f * Time.deltaTime;
                }

                break;


            case AllyRank.Kicked_Out:

                if (attackTarget == null)
                    allyRank = AllyRank.Member;

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

                break;
        }
        

        transform.localEulerAngles = Vector3.zero;
    }
}
