﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    #region References
    internal protected Rigidbody rb;
    internal protected Animator animator;
    #endregion
    
    #region Physics
    internal protected bool isGrounded = true;

    internal protected bool movementLocked = false;

    /// <summary>
    /// Damage suffered every second while not grounded
    /// </summary>
    protected float fallDamage = 80f;

    protected float minSpeed = 8f, midSpeed = 12f, maxSpeed = 16f;
    #endregion

    #region Stats
    [HideInInspector]
    public float hp = 100f;
    #endregion
    
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    protected virtual void LateUpdate()
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

        rb.isKinematic = movementLocked;

        if (hp <= 0f)
            Destroy(gameObject);

        //health regeneration
        hp += Time.deltaTime;
    }

    /// <summary>
    /// Called when attacking a knight
    /// </summary>
    /// <param name="knight"></param>
    protected void Attack(Knight knight)
    {
        knight.BeingHit();
        this.BeingHit();
    }

    /// <summary>
    /// Called during the fight after being hit
    /// </summary>
    protected void BeingHit()
    {
        movementLocked = true;
        animator.Play("Hit");
        Invoke("Die", 1f);
    }
    
    protected virtual void Die()
    {
        hp = 0;
    }

    protected bool GroundCheck()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);

        foreach (var collider in hitColliders)
        {
            if (collider.tag == "Scenery")
                return true;
        }
        return false;
    }
}
