using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    Ally.enemyDanger = other.gameObject;
                }
                break;
        }
    }
}
