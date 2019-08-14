using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour
{
    #region Ally Groups
    public enum AllyRank
    {
        Boss, Member
    }
    /// <summary>
    /// If it is not equal to null, I am an ally
    /// </summary>
    public AllyRank? myAllyRank = null;
    
    /// <summary>
    /// Static reference to the boss of allies, but can only be used if myAllyRank != null
    /// </summary>
    public static Ally boss;
    #endregion

    Animator myAnimator;
    Rigidbody myRB;

    #region Swipes
    Vector2 startFingerPos;
    Vector2 endFingerPos;
    Vector2 deltaFingerPos;
    #endregion

    void Start()
    {
        if (name == "Cat")
            myAllyRank = AllyRank.Boss;
        myAnimator = GetComponent<Animator>();
        myRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        try
        {
            if (myAllyRank == AllyRank.Boss)
            {
                boss = this;
                //my speed cannot be lower than 5
                if (myRB.velocity.z < 5)
                    myRB.velocity = new Vector3(myRB.velocity.x, 0, 5);

                foreach (var touch in Input.touches)
                {
                    //on swipe start save touch pos
                    if (touch.phase == TouchPhase.Began)
                    {
                        startFingerPos = touch.position;
                    }

                    if (touch.phase == TouchPhase.Ended)
                    {
                        //when the swipe ends save the touch pos
                        endFingerPos = touch.position;

                        //calculate the swipe length
                        deltaFingerPos = endFingerPos - startFingerPos;

                        //if the swipe isn't enough big, skip to next touch
                        if (deltaFingerPos.magnitude < 50)
                            continue;

                        Vector3 move = Vector3.zero;

                        move.z = deltaFingerPos.y > 0 ? 5f : 0f;
                        move.x = deltaFingerPos.x > 0 ? 10f : -10f;
                        //y is always 0

                        myRB.velocity += move;
                    }
                }
            }
            else if (myAllyRank == AllyRank.Member)
            {
                //check if the boss is alive
                if(boss == null)
                {
                    myAllyRank = AllyRank.Boss;
                    boss = this;
                    transform.localEulerAngles = Vector3.zero; //reset the rotation (because of transform.LookAt)
                }

                if(Vector3.Distance(transform.position, boss.transform.position) > 3)
                {
                    transform.LookAt(boss.transform);
                    Vector3 move = Vector3.forward * boss.GetComponent<Rigidbody>().velocity.magnitude;
                    transform.Translate(move * Time.deltaTime);
                }
            }
            else if (myAllyRank == null)
            {
                //I am not an ally yet
            }
        }
        catch (Exception e) { }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Ally>()) //I am not a knight
            return;
        else
        {
            //I am a knight and a potential ally
            Ally otherAlly = other.GetComponent<Ally>();

            if (otherAlly.myAllyRank != null && //otherAlly is a knight and an ally
                myAllyRank == null) //I cannot be an ally
            {
                //I had been staying on the road and "other" is the player collecting allies
                
                myAllyRank = AllyRank.Member;
            }
        }
    }
}
