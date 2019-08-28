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
        Boss, Member, Stranger
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
    float minDistFromBoss = 6f;
    #endregion

    #region Fighting

    public Enemy attackTarget = null;
    float maxDistToEnemy = 0.5f;

    #endregion

    #endregion

    protected override void Start()
    {        
        base.Start();

        highlight = transform.Find("Point Light").GetComponent<Light>();

        if (name == "Ally (1)")
        {
            allyRank = AllyRank.Boss;
        }
    }

    protected override void Update()
    {
        base.Update();
        switch (allyRank)
        {
            case AllyRank.Stranger:
                highlight.enabled = true;
                highlight.color = Color.yellow;
                break;


            case AllyRank.Member:
                //check if the boss is alive
                if (Boss == null)
                {
                    //the boss is dead, so I become him (first come, first served)
                    allyRank = AllyRank.Boss;
                }
                else
                {
                    highlight.enabled = false;

                    if(!movementLocked)
                    {
                        //if this ally doesn't have to attack any enemies, do stuff right below
                        if (attackTarget == null)
                        {
                            //ally won't follow the boss, when he is about to die with no chance to rescue him (boss is falling)
                            if (Boss.isGrounded)
                            {
                                /*
                                if (Vector3.Distance(transform.position, GetBoss().transform.position) > 3)
                                {
                                    transform.LookAt(GetBoss().transform);
                                    transform.Translate(Vector3.forward * GetBoss().rb.velocity.magnitude * Time.deltaTime);
                                }*/

                                //boss follow
                                transform.LookAt(Boss.transform);
                                transform.Translate(Vector3.forward * Boss.rb.velocity.magnitude * Time.deltaTime);

                                //members have to be in front of the boss to protect him from enemies
                                if (transform.position.z < Boss.transform.position.z + minDistFromBoss) //true if this ally is behind the boss
                                {
                                    //reset the rotation because of the transform.Translate
                                    transform.localEulerAngles = Vector3.zero;

                                    //minimum making up the Z position speed
                                    float speed = zSpeed;

                                    //if the boss is moving faster
                                    if (Boss.rb.velocity.z > speed)
                                        speed = Boss.rb.velocity.z * 2; //let's be two times faster then he is
                                    
                                    //finally move the ally and make up the Z pos
                                    transform.Translate(Vector3.forward * speed * Time.deltaTime);

                                }
                            }
                        }
                        else
                        {
                            //head as fast to the enemy and kill him
                            if(Vector3.Distance(transform.position, attackTarget.transform.position) > maxDistToEnemy)
                            {
                                transform.LookAt(attackTarget.transform);
                                transform.Translate(Vector3.forward * zSpeed * Time.deltaTime);
                            }
                        }
                    }
                }
                break;


            case AllyRank.Boss:
                highlight.enabled = true;
                highlight.color = Color.red;

                if(!movementLocked)
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
        }

        float velZ = rb.velocity.z / (zSpeed / 1.3f);
        //set animator VelZ value, but it cannot be greater than 1 (animation would be too fast)
        animator.SetFloat("VelZ", Mathf.Abs(velZ) > 1 ? 1 : velZ);
        transform.localEulerAngles = Vector3.zero;
    }
}
