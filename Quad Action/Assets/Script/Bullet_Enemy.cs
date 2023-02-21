using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Enemy : MonoBehaviour
{
    public int damage;
    public bool isMelee; //근접무기는 비워두세요
    public GameObject effectObj; //폭발이펙트
    public GameObject maeshObj; //미사일오브젝트 등록

    void OnCollisionEnter(Collision collision)
    {
        //몬스터 A,B 전용
        if(isMelee == true)
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
        //몬스터 C 전용 (미사일에 Destroy를 사용하기 위함)
        else
        {
            if(collision.gameObject.tag == "Floor")
            {
                Destroy(gameObject, 2);
            }
            else if(collision.gameObject.tag == "Wall")
            {
                Destroy(gameObject, 2);
            }
        }
        
    }

    void OnTriggerEnter(Collider other) 
    {
        //몬스터 A,B 전용
        if(isMelee == true)
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
        //몬스터 C 전용 (미사일에 Destroy를 사용하기 위함)
        else
        {
            if(other.gameObject.tag == "Floor")
            {
                Destroy(gameObject, 2);
            }
            else if(other.gameObject.tag == "Wall")
            {
                Destroy(gameObject, 2);
            }
            else if(other.gameObject.tag == "Player")
            {
                Debug.Log("몬스터 미사일 플레이어와 충돌");
                maeshObj.SetActive(false);
                effectObj.SetActive(true);
                Destroy(gameObject,5);
            }
        }

    }

}
