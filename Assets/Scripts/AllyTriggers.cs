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
                if (other.tag == "Enemy")
                {
                    //check if other isn't already attacked by an ally
                    if (Ally.Allies.Where(ally => ally.attackTarget == other.GetComponent<Enemy>()).Count() == 0)
                    {
                        //delegate the parent ally (if available) to attack the enemy
                        if (transform.parent.GetComponent<Ally>().allyRank == Ally.AllyRank.Member && transform.parent.GetComponent<Ally>().attackTarget == null)
                        {
                            transform.parent.GetComponent<Ally>().attackTarget = other.GetComponent<Enemy>();
                        }
                    }
                }
                break;
        }
    }
}
