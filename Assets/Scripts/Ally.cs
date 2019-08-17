using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Knight
{
    #region Enums
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
    [HideInInspector] public AllyRank allyRank = AllyRank.Stranger;
    
    #endregion
    
    #region Swipes
    Vector2 startPos;
    Vector2 endPos;
    Vector2 deltaPos;
    float minSwipe = 0.025f * Screen.width;
    #endregion
    
    protected override void Start()
    {
        base.Start();

        highlight = transform.Find("Point Light").GetComponent<Light>();

        if (name == "Ally (1)")
            allyRank = AllyRank.Boss;
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
                if (Boss() == null)
                {
                    //the boss is dead, so I become him (first come, first served)
                    allyRank = AllyRank.Boss;
                }
                else
                {
                    highlight.enabled = false;

                    //follow the boss
                    if (Vector3.Distance(transform.position, Boss().transform.position) > 3 && Boss().isGrounded)
                    {
                        transform.LookAt(Boss().transform);
                        transform.Translate(Vector3.forward * Boss().rb.velocity.magnitude * Time.deltaTime);
                        transform.localEulerAngles = Vector3.zero; //reset rotation to make the movement more natural
                    }
                }
                break;


            case AllyRank.Boss:
                highlight.enabled = true;
                highlight.color = Color.red;
                

                Vector2 swipe = DetectSwipes();
                
                Vector3 bossMove = Vector3.zero;

                if (swipe.x != 0)
                    bossMove.x = swipe.x > 0 ? 1f : -1f;
                if (swipe.y != 0)
                    bossMove.z = swipe.y > 0 ? 0.5f : -0.2f;
                
                //velocity.z cannot be lower than 5
                if (rb.velocity.z < speed * 0.5f)
                    bossMove.z += speed * 0.5f * Time.deltaTime;

                rb.velocity += bossMove * speed;
                break;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Ally>())
        {
            //other is an ally, which invites knight-ally-stranger to the ally-group
            if (other.GetComponent<Ally>().allyRank != AllyRank.Stranger && //other: ally
                allyRank == AllyRank.Stranger) //stranger: not-an-ally
            {
                allyRank = AllyRank.Member;
            }
        }
        else
        {
            //other is not an ally
            return;
        }
    }
    
    Vector2 DetectSwipes()
    {
        Vector2 swipeDir = Vector2.zero;

        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                startPos = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                endPos = touch.position;
                deltaPos = endPos - startPos;
                
                //if the swipe's direction was up, then the swipeDir.x = 1f, if it was down, then swipeDir.y = -1f; Same thing with swipeDir.y
                
                if(Mathf.Abs(deltaPos.x) >= minSwipe)
                    swipeDir.x = deltaPos.x > 0 ? 1f : -1f;
                if (Mathf.Abs(deltaPos.y) >= minSwipe)
                    swipeDir.y = deltaPos.y > 0 ? 1f : -1f;
            }
        }

        return swipeDir;
    }

    /// <summary>
    /// Static function returning the boss GameObject
    /// </summary>
    /// <returns></returns>
    public static Ally Boss()
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
