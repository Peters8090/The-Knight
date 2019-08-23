using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    internal AllyRank allyRank = AllyRank.Stranger;

    #endregion

    #region Fights

    float dangerZone = 5f;
    public static bool bossInDanger = false;
    public static Ally bossGuard = null;
    public static GameObject enemyDanger = null;

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
                if (GetBoss() == null)
                {
                    //the boss is dead, so I become him (first come, first served)
                    allyRank = AllyRank.Boss;
                }
                else
                {
                    highlight.enabled = false;

                    //TODO: refactor the fighting code

                    if(bossInDanger && enemyDanger != null && (bossGuard == null || bossGuard == this))
                    {
                        bossGuard = this;
                        if (Vector3.Distance(transform.position, enemyDanger.transform.position) > 0.1f)
                        {
                            transform.LookAt(enemyDanger.transform);
                            transform.Translate(Vector3.forward * speed * Time.deltaTime);
                        }
                    }

                    else if (Vector3.Distance(transform.position, GetBoss().transform.position) > 3 && GetBoss().isGrounded && rb.constraints != RigidbodyConstraints.FreezeAll)
                    {
                        //follow the boss
                        transform.LookAt(GetBoss().transform);
                        transform.Translate(Vector3.forward * GetBoss().rb.velocity.magnitude * Time.deltaTime);
                    }
                }
                break;


            case AllyRank.Boss:
                highlight.enabled = true;
                highlight.color = Color.red;

                //velocity.z cannot be lower than 5
                if (rb.velocity.z < speed * 0.5f)
                    rb.velocity += Vector3.forward * speed * 0.5f * Time.deltaTime;

                foreach (var touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        float leftRightfactor = Screen.height * .000004f;
                        float fwdFactor = leftRightfactor * .5f;
                        float backFactor = fwdFactor * .5f;

                        Vector3 rbMove = Vector3.zero;

                        rbMove.x = Mathf.Abs(rb.velocity.x) < speed ? touch.deltaPosition.x * leftRightfactor : 0;

                        rbMove.z = touch.deltaPosition.y * (touch.deltaPosition.y > 0 ?
                            (rb.velocity.z <= speed * 2f ? fwdFactor : 0) : //forward
                            (rb.velocity.z >= speed * .2f ? backFactor : 0)); //back

                        rb.velocity += rbMove * speed;
                    }
                }

                bossInDanger = enemyDanger != null;

                if (bossInDanger)
                {
                    //to prevent alone boss stopping before enemy
                    if(bossGuard != null)
                    {
                        rb.isKinematic = true;
                        GetComponent<BoxCollider>().enabled = false;
                    }
                }
                else
                {
                    bossGuard = null;
                    rb.isKinematic = false;
                    GetComponent<BoxCollider>().enabled = true;
                }
                break;
        }
        transform.localEulerAngles = Vector3.zero;
    }

    /// <summary>
    /// Static function returning the boss GameObject
    /// </summary>
    /// <returns></returns>
    public static Ally GetBoss()
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
