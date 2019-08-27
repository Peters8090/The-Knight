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

    #region Physics

    Vector3 lastMousePos;
    
    float minZSpeed = 7f;
    
    float sensitivity = 0.3f;
    float clampDelta = 15f;
    float velDivider = 20f;
    float speedBoostOnTouch = 0.5f;

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
                            //transform.position = Vector3.Lerp(transform.position, enemyDanger.transform.position - Vector3.forward, 0.05f);
                            transform.Translate(Vector3.forward * minZSpeed * 3 * Time.deltaTime);
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

                bossInDanger = enemyDanger != null;

                GetComponent<BoxCollider>().enabled = !bossInDanger || bossGuard == null;

                if (Input.GetMouseButtonDown(0))
                {
                    lastMousePos = Input.mousePosition;
                }
                else if (Input.GetMouseButton(0))
                {
                    Vector3 deltaMousePos = -(lastMousePos - Input.mousePosition);
                    lastMousePos = Input.mousePosition;
                    deltaMousePos = Vector3.ClampMagnitude(deltaMousePos, clampDelta);

                    rb.velocity += Vector3.forward * speedBoostOnTouch;
                    rb.velocity += new Vector3(deltaMousePos.x, 0, deltaMousePos.y) * sensitivity - rb.velocity / velDivider;
                }

                //player can't move backwards (he can slow down)
                if (rb.velocity.z < 0)
                    rb.velocity += Vector3.forward * Mathf.Abs(rb.velocity.z);

                //constant forward force
                if (rb.velocity.z < minZSpeed)
                    rb.velocity += Vector3.forward * minZSpeed * Time.deltaTime;

                break;
        }
        float velZ;
        velZ = rb.velocity.z / (minZSpeed * 1.5f);
        //set animator VelZ, but it can't be greater than 1
        animator.SetFloat("VelZ", Mathf.Abs(velZ) > 1 ? 1 : velZ);
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
