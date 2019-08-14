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
    
    /// <summary>
    /// Highlight color of the boss
    /// </summary>
    Color bossHighlightColor = Color.red;

    /// <summary>
    /// Highlight color of knights, possible allies
    /// </summary>
    Color possibleAllyHighlightColor = Color.yellow;

    #endregion

    Animator myAnimator;
    Rigidbody myRB;
    Light myLight;

    float speed = 10f;
    
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
        myLight = transform.Find("Point Light").GetComponent<Light>();
    }

    void Update()
    {
        //temporary solution to the gravity problem
        transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
        try
        {
            //the boss is highlighted
            if (myAllyRank == AllyRank.Boss)
            {
                myLight.enabled = true;
                myLight.color = bossHighlightColor;
            }
            else if (myAllyRank == null)
            {
                myLight.enabled = true;
                myLight.color = possibleAllyHighlightColor;
            }
            else
                myLight.enabled = false;


            if (myAllyRank == AllyRank.Boss)
            {
                boss = this;

                //my z speed cannot be lower than 5
                if (myRB.velocity.z < speed * 0.5f)
                    myRB.velocity = new Vector3(myRB.velocity.x, myRB.velocity.y, speed * 0.5f);

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

                        Vector3 move;

                        //if the swipe's direction was up, then the move.z = 5f, but if the swipe dir was down move.z 0f. Same thing in move.z

                        move.x = deltaFingerPos.x > 0 ? 1f : -1f;
                        move.y = 0;
                        move.z = deltaFingerPos.y > 0 ? 0.5f : 0f;
                        
                        myRB.velocity += move * speed;
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
                    transform.localEulerAngles = Vector3.zero;
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
