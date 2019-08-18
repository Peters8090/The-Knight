using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Knight
{
    protected override void Start()
    {
        base.Start();
    }
    
    protected override void Update()
    {
        base.Update();
    }
    
    protected override void Die()
    {
        base.Die();
        Instantiate(Resources.Load("Bomb"), transform.position, Quaternion.identity);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ally>() && !IsInvoking()) //make sure that I am not already fighting
        {
            if(other.GetComponent<Ally>().allyRank != Ally.AllyRank.Stranger) //check if other is in the ally group
                Attack(other.GetComponent<Ally>());
        }
    }
}
