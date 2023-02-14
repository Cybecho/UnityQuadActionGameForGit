using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 2);
        }
        else if(collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 2);
        }
        else if(other.gameObject.tag == "Wall" || other.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
