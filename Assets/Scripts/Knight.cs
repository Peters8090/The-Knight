using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    #region Component References
    Rigidbody rb;
    Light highlight;
    #endregion

    #region Ally Groups

    /// <summary>
    /// If it is not equal to stranger, I am an ally
    /// </summary>
    [HideInInspector] public Enums.KnightRank knightRank = Enums.KnightRank.Stranger;

    /// <summary>
    /// Highlight color of the allies-boss
    /// </summary>
    Color bossHighlightColor = Color.red;

    /// <summary>
    /// Highlight color of stranger knights
    /// </summary>
    Color strangerKnightHighlightColor = Color.yellow;

    #endregion

    #region Physics
    float speed = 10f;
    bool isGrounded = true;
    #endregion

    #region Stats
    float hp = 100f;
    #endregion

    void Start()
    {
        if (name == "Knight")
            knightRank = Enums.KnightRank.AllyBoss;

        rb = GetComponent<Rigidbody>();
        highlight = transform.Find("Point Light").GetComponent<Light>();
    }

    void Update()
    {
        //solution for the gravity problem (friction made it hard/impossible to move horizontally)
        transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);

        isGrounded = GroundCheck();
        
        if (!isGrounded)
            hp = 0f;

        switch (knightRank)
        {
            case Enums.KnightRank.Stranger:
                highlight.enabled = true;
                highlight.color = strangerKnightHighlightColor;
                break;


            case Enums.KnightRank.AllyMember:
                //check if the boss is alive
                if (Boss() == null)
                {
                    //the boss is dead, so I become him (first come, first served)
                    knightRank = Enums.KnightRank.AllyBoss;
                }
                else
                {
                    highlight.enabled = false;

                    //follow the boss
                    if (Vector3.Distance(transform.position, Boss().transform.position) > 3)
                    {
                        transform.LookAt(Boss().transform);
                        Vector3 move = Vector3.forward * Boss().rb.velocity.magnitude;
                        transform.Translate(move * Time.deltaTime);
                        transform.localEulerAngles = Vector3.zero; //reset rotation to make the movement more natural
                    }
                }
                break;


            case Enums.KnightRank.AllyBoss:
                highlight.enabled = true;
                highlight.color = bossHighlightColor;

                //velocity.z cannot be lower than 5
                if (rb.velocity.z < speed * 0.5f)
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, speed * 0.5f);

                Vector2 swipe = DetectSwipes();

                if (swipe != Vector2.zero)
                {
                    Vector3 move;
                    move.x = swipe.x > 0 ? 1f : -1f;
                    move.y = 0;
                    move.z = swipe.y > 0 ? 0.5f : 0f;

                    rb.velocity += move * speed;
                }
                break;
        }

        if (hp <= 0f)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Knight>())
        {
            //other is an ally, which invites knight-stranger to the ally-group
            if (other.GetComponent<Knight>().knightRank != Enums.KnightRank.Stranger && //other: ally
                knightRank == Enums.KnightRank.Stranger) //stranger: not-an-ally
            {
                knightRank = Enums.KnightRank.AllyMember;
            }
        }
        else
        {
            //other is not a knight
            return;
        }
    }

    #region Swipes
    Vector2 startPos;
    Vector2 endPos;
    Vector2 deltaPos;

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

                //if the swipe isn't enough big, skip to next touch
                if (deltaPos.magnitude < 10)
                    continue;

                //if the swipe's direction was up, then the swipeDir.x = 1f, if it was down, then swipeDir.y = -1f; Same thing with swipeDir.y
                swipeDir.x = deltaPos.x > 0 ? 1f : -1f;
                swipeDir.y = deltaPos.y > 0 ? 1f : -1f;
            }
        }

        return swipeDir;
    }
    #endregion

    bool GroundCheck()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);

        foreach (var collider in hitColliders)
        {
            if (collider.tag == "Terrain")
                return true;
        }
        return false;
    }

    /// <summary>
    /// Static function returning the boss GameObject
    /// </summary>
    /// <returns></returns>
    public static Knight Boss()
    {
        Knight[] knights = FindObjectsOfType<Knight>();

        for (int i = 0; i < knights.Length; i++)
        {
            if (knights[i].knightRank == Enums.KnightRank.AllyBoss)
                return knights[i];
        }
        return null;
    }
}
