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

    Vector3 startFingerPos = Vector3.zero,
            startPlayerPos = Vector3.zero;

    #endregion

    #region Member circle params

    protected float increment = 30;
    internal protected float radius = 3f;

    #endregion

    #region Movement

    float speedBoostOnTouch = 0.08f;

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
                
        animator.SetFloat("VelZ", Mathf.Abs(rb.velocity.z / midSpeed));

        //max value is 1
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
            //on touch start
            if(Input.GetMouseButtonDown(0))
            {
                Plane plane = new Plane(Vector3.up, 0);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (plane.Raycast(ray, out float dist))
                {
                    startFingerPos = ray.GetPoint(dist);
                    startPlayerPos = transform.position;
                }
            }

            //while touching
            if (Input.GetMouseButton(0))
            {
                //if any touch happens, accelerate forward
                rb.velocity += Vector3.forward * speedBoostOnTouch;

                Plane plane = new Plane(Vector3.up, 0);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (plane.Raycast(ray, out float dist))
                {
                    Vector3 curFingerPos = ray.GetPoint(dist);
                    Vector3 touchDelta = curFingerPos - startFingerPos;

                    Vector3 predictedPos = startPlayerPos + touchDelta;
                    predictedPos.z = transform.position.z; //z move is locked

                    transform.position = predictedPos;
                }
            }

            //constant forward force
            if (rb.velocity.z < minSpeed)
                rb.velocity += Vector3.forward * minSpeed * Time.deltaTime;
        }
    }
}
