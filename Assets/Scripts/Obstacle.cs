using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    float damage = 500f;

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ally")
            collision.gameObject.GetComponent<Ally>().hp -= damage * Time.deltaTime;
    }
}
