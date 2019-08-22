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

    /// <summary>
    /// List of all allies
    /// </summary>
    public static List<Ally> allies = new List<Ally>();

    #endregion

    #region Swipes
    Vector3 startPos;
    Vector3 myStartPos;
    Vector3 deltaPos;
    float minSwipe = 0.025f * Screen.width;
    #endregion

    protected override void Start()
    {
        base.Start();

        highlight = transform.Find("Point Light").GetComponent<Light>();

        if (name == "Ally (1)")
        {
            allyRank = AllyRank.Boss;
            allies.Add(this);
        }

        myStartPos = transform.position;
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

                    if (GetFollowTarget() == GetBoss().transform)
                    {
                        if (Vector3.Distance(transform.position, GetFollowTarget().transform.position) > 3 && GetBoss().isGrounded && rb.constraints != RigidbodyConstraints.FreezeAll)
                        {
                            //follow the boss
                            transform.LookAt(GetFollowTarget().transform);
                            transform.Translate(Vector3.forward * GetBoss().rb.velocity.magnitude * Time.deltaTime);
                            transform.localEulerAngles = Vector3.zero; //reset rotation to make the movement more natural
                        }
                    }
                    else
                    {
                        transform.LookAt(GetFollowTarget().transform);
                        transform.Translate(Vector3.forward * speed * Time.deltaTime);
                    }
                }
                break;


            case AllyRank.Boss:
                highlight.enabled = true;
                highlight.color = Color.red;

                //the bigger group is, the speed is smaller
                speed = maxSpeed - (allies.Count - 1) * 0.5f;

                //velocity.z cannot be lower than 5
                if (rb.velocity.z < speed * 0.5f)
                    rb.velocity += Vector3.forward * speed * 0.5f * Time.deltaTime;

                if (GetFollowTarget() == transform)
                {
                    foreach (var touch in Input.touches)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            startPos = Camera.main.ScreenToWorldPoint(touch.position);
                            myStartPos = transform.position;
                        }

                        if (touch.phase == TouchPhase.Moved)
                        {
                            deltaPos = Camera.main.ScreenToWorldPoint(touch.position) - startPos;
                            Debug.Log(startPos);
                        }
                    }
                    transform.position = myStartPos + deltaPos;

                    rb.isKinematic = false;
                    GetComponent<BoxCollider>().enabled = true;
                }
                else if (allies.Count > 1)
                {
                    rb.isKinematic = true;
                    GetComponent<BoxCollider>().enabled = false;
                }
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ally>())
        {
            //other is an ally, which invites knight-ally-stranger to the ally-group
            if (other.GetComponent<Ally>().allyRank != AllyRank.Stranger && //other: ally
                allyRank == AllyRank.Stranger) //stranger: not-an-ally
            {
                allyRank = AllyRank.Member;
                allies.Add(this);
            }
        }
    }

    void OnDestroy()
    {
        allies.Remove(this);
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

    public Transform GetFollowTarget()
    {
        if (GetNearestEnemyAndDist().Item1 > 5)
            return GetBoss().transform;
        else
        {
            return GetNearestEnemyAndDist().Item2.transform;
        }
    }

    //TODO: code refactor (fights with enemy)

    public (float, GameObject) GetNearestEnemyAndDist()
    {
        Dictionary<float, GameObject> enemyAndDistPairs = new Dictionary<float, GameObject>();
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemyAndDistPairs.Add(Vector3.Distance(GetBoss().transform.position, enemy.transform.position), enemy.gameObject);
        }

        enemyAndDistPairs.OrderBy(key => key.Key);

        if (enemyAndDistPairs.Count > 0)
            return (enemyAndDistPairs.First().Key, enemyAndDistPairs.First().Value);
        else
            return (10, null);
    }
}
