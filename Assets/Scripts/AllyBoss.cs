using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AllyBoss : Ally
{
    public static AllyBoss Boss
    {
        get
        {
            return FindObjectOfType<AllyBoss>();
        }
    }

    #region Swipes

    Vector3 lastMousePos = Vector3.zero;
    float sensitivity = 0.3f * 1280 / Screen.height;
    float maxMoveLength = 15f * Screen.height / 1280;
    float slide = 0.05f * Screen.height / 1280;
    float speedBoostOnTouch = 0.5f * Screen.height / 1280;

    #endregion
    
    #region Member circle params

    protected float increment = 30;
    internal protected float radius = 3f;

    #endregion

    #region Movement

    float zSpeed = 15f;

    #endregion

    protected override void Start()
    {
        base.Start();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        highlight.enabled = true;
        highlight.color = Color.red;

        animator.SetFloat("VelZ", Mathf.Abs(rb.velocity.z / (zSpeed / 1.3f)));
        if (animator.GetFloat("VelZ") > 1)
            animator.SetFloat("VelZ", 1);

        #region Members' pos

        AllyMember[] allyMembers = AllyMember.Members;
        float angle = 90; // Initial angle
        float factor;

        if (allyMembers.Count() % 2 == 1)
        {
            factor = (allyMembers.Count() - 1) / 2;
        }
        else
        {
            factor = allyMembers.Count() / 2 - 0.5f;
        }

        angle -= factor * increment;

        foreach (AllyMember ally in allyMembers)
        {
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            ally.offsetPos = new Vector3(x, 0, y);

            angle += increment;
        }
        #endregion

        if (!movementLocked)
        {
            //swipes work both on the mobile and pc
            if (Input.GetMouseButtonDown(0))
            {
                lastMousePos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 deltaMousePos = -(lastMousePos - Input.mousePosition);
                lastMousePos = Input.mousePosition;

                //calculate the finger move
                deltaMousePos = Vector3.ClampMagnitude(deltaMousePos, maxMoveLength);

                //if any touch happens, accelerate forward
                rb.velocity += Vector3.forward * speedBoostOnTouch;

                //move with the swipe
                rb.velocity += new Vector3(deltaMousePos.x, 0, deltaMousePos.y) * sensitivity - rb.velocity * slide;
            }

            //player can't move backwards (he can slow down)
            if (rb.velocity.z < 0)
                rb.velocity += Vector3.forward * Mathf.Abs(rb.velocity.z);

            //constant forward force
            if (rb.velocity.z < zSpeed * 0.5f)
                rb.velocity += Vector3.forward * zSpeed * 0.5f * Time.deltaTime;
        }
    }
}
