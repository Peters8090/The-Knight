﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Vector3 offset;
    float smoothSpeed = 0.1f;

    void Start()
    {
        offset = transform.position;
    }

    void Update()
    {
        if(Ally.Boss != null)
        {
            Vector3 targetPos = Ally.Boss.transform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed);
        }
    }
}
