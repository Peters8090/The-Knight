using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Knight
{
    protected override void Start()
    {
        base.Start();
    }
    
    protected override void LateUpdate()
    {
        base.LateUpdate();
    }
    
    protected override void Die()
    {
        base.Die();
        Instantiate(Resources.Load("Prefabs/Bomb"), transform.position, Quaternion.identity);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Ally>() && !IsInvoking()) //make sure that I am not already fighting
        {
            if (collision.collider.GetComponent<Ally>().allyRank != Ally.AllyRank.Stranger) //check if other is in the ally group
                Attack(collision.collider.GetComponent<Ally>());
        }
    }
}
