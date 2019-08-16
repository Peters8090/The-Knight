using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Vector3 offset;
    float smoothSpeed = 0.05f;

    void Start()
    {
        offset = transform.position;
    }

    void Update()
    {
        if(Knight.Boss() != null)
        {
            Vector3 targetPos = Knight.Boss().transform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed);
        }
    }
}
