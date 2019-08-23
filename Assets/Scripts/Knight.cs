using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    #region Component References
    protected Rigidbody rb;
    protected Animator animator;
    #endregion
    
    #region Physics
    protected float speed = 10f;
    protected bool isGrounded = true;

    /// <summary>
    /// Damage suffered every second while not grounded
    /// </summary>
    protected float fallDamage = 50f;
    #endregion

    #region Stats
    protected float hp = 100f;
    #endregion

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
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
        
        float velZ;
        velZ = rb.velocity.z / speed;
        //set animator VelZ, but it can't be greater than 1
        animator.SetFloat("VelZ", Mathf.Abs(velZ) > 1 ? 1 : velZ);

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
        rb.constraints = RigidbodyConstraints.FreezeAll;
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
            if (collider.tag == "Terrain")
                return true;
        }
        return false;
    }
}
