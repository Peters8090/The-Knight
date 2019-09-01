using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AllyTriggers : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        switch(name)
        {
            case "Trigger1":
                if (transform.parent.GetComponent<AllyStranger>())
                {
                    Debug.Log(other.name);
                    //other is an ally member/boss, which invites (me) Ally Stranger to the ally group
                    if (other.GetComponent<AllyMember>() || other.GetComponent<AllyBoss>())
                    {
                        transform.parent.GetComponent<Ally>().ChangeAllyRankTo<AllyMember>();
                    }
                }
                break;

            case "Trigger2":
                if (other.GetComponent<Enemy>())
                {
                    //check if other isn't already attacked by an ally
                    if (AllyDelegated.Delegated.Where(ally => ally.attackTarget == other.GetComponent<Enemy>()).Count() == 0)
                    {
                        //delegate the parent ally (if available) to attack the enemy
                        if (!transform.parent.GetComponent<AllyDelegated>())
                        {
                            transform.parent.GetComponent<Ally>().ChangeAllyRankTo<AllyDelegated>();
                            transform.parent.GetComponent<AllyDelegated>().attackTarget = other.GetComponent<Enemy>();
                        }
                    }
                }
                break;
        }
    }
}
