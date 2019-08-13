using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;

    Vector3 terrainSize;

    Vector2 startFingerPos;
    Vector2 endFingerPos;
    Vector2 deltaFingerPos;

    GameObject observationObj;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        terrainSize = FindObjectOfType<Terrain>().terrainData.size;
    }

    void Update()
    {
        if (observationObj != null && Vector3.Distance(transform.position, observationObj.transform.position) > 3)
        {
            transform.LookAt(observationObj.transform);
            transform.Translate(Vector3.forward * observationObj.GetComponent<Rigidbody>().velocity.magnitude * Time.deltaTime);
        }

        if(name == "Cat")
        {
            if (rb.velocity.y < 5)
                rb.velocity = new Vector3(rb.velocity.x, 0, 5);
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    startFingerPos = touch.position;
                    CancelInvoke();
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    endFingerPos = touch.position;

                    deltaFingerPos = endFingerPos - startFingerPos;

                    Vector3 playerDir = Vector3.zero;

                    playerDir.z = deltaFingerPos.y > 0 ? 0.5f : 0f;
                    playerDir.x = deltaFingerPos.x > 0 ? 0.5f : -0.5f;

                    rb.velocity += playerDir * 10;
                }
            }
        }
    }
        
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && name != "Cat")
        {
            tag = "Player";
            //I have been staying on the road and "other" is the player collecting allies
            /*transform.SetParent(other.transform.parent);
            transform.localEulerAngles = Vector3.zero;*/
            observationObj = other.gameObject;
        }
    }
}
