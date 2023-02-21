using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Enemy : MonoBehaviour
{
    public int damage;
    void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.tag == "Floor")
        {
            Debug.Log("몬스터 Floor와 충돌");
        }
        else if(collision.gameObject.tag == "Wall")
        {
            Debug.Log("몬스터 Wall와 충돌");
        }
    }

    void OnTriggerEnter(Collider other) 
    {

        if(other.gameObject.tag == "Floor")
        {
            Debug.Log("몬스터 Floor와 충돌");
        }
        else if(other.gameObject.tag == "Wall")
        {
            Debug.Log("몬스터 Wall와 충돌");
        }
        else if(other.gameObject.tag == "Player")
        {
            Debug.Log("몬스터 플레이어와 충돌");
        }
    }

}
