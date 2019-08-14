using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Vector3 offset = Vector3.zero;

    void Update()
    {
        if(Ally.boss != null)
        {
            Vector3 targetPos = Ally.boss.transform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.05f);
        }
    }
}
