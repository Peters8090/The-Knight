using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    #region Component References
    Rigidbody rb;
    Animator animator;
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

    /// <summary>
    /// Damage suffered every second while not grounded
    /// </summary>
    float fallDamage = 50f;
    #endregion

    #region Stats
    float hp = 100f;
    #endregion
    
    #region Swipes
    Vector2 startPos;
    Vector2 endPos;
    Vector2 deltaPos;
    float minSwipe = 0.025f * Screen.width;
    #endregion
    
    void Start()
    {
        if (name == "Knight")
            knightRank = Enums.KnightRank.AllyBoss;

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        highlight = transform.Find("Point Light").GetComponent<Light>();
    }

    void Update()
    {
        isGrounded = GroundCheck();

        //solution for the gravity problem (friction made it hard/impossible to move horizontally)
        rb.useGravity = !isGrounded;
        if (isGrounded)
        {
            transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
        }
        else
        {
            hp -= fallDamage * Time.deltaTime;
        }
        
        
        float velZ;
        velZ = rb.velocity.z / speed;
        //set animator VelZ, but it can't be greater than 1
        animator.SetFloat("VelZ", Mathf.Abs(velZ) > 1 ? 1 : velZ);
        
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
                    if (Vector3.Distance(transform.position, Boss().transform.position) > 3 && Boss().isGrounded)
                    {
                        transform.LookAt(Boss().transform);
                        transform.Translate(Vector3.forward * Boss().rb.velocity.magnitude * Time.deltaTime);
                        transform.localEulerAngles = Vector3.zero; //reset rotation to make the movement more natural
                    }
                }
                break;


            case Enums.KnightRank.AllyBoss:
                highlight.enabled = true;
                highlight.color = bossHighlightColor;
                

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
