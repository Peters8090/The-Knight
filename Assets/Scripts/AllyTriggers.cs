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
                if (other.tag == "Player")
                {
                    //other is an ally, which invites knight-ally-stranger to the ally-group
                    if (other.GetComponent<Ally>().allyRank != Ally.AllyRank.Stranger && //other: ally
                        transform.parent.GetComponent<Ally>().allyRank == Ally.AllyRank.Stranger) //stranger: not-an-ally
                    {
                        transform.parent.GetComponent<Ally>().allyRank = Ally.AllyRank.Member;
                    }
                }
                break;

            case "Trigger2":
                if(other.tag == "Enemy")
                {
                    if (Ally.Allies.Where(ally => ally.attackTarget == other.GetComponent<Enemy>()).Count() == 0 //if no one is already attacking this enemy
                        && Ally.Allies.Where(ally => ally.allyRank == Ally.AllyRank.Member && ally.attackTarget == null).Count() > 0) //and if the list of available allies isn't empty
                    {
                        //delegate an available ally to attack the enemy
                        Ally.Allies.Where(ally => ally.allyRank == Ally.AllyRank.Member && ally.attackTarget == null).ToArray()[0].attackTarget = other.GetComponent<Enemy>();
                    }
                }
                break;
        }
    }
}
