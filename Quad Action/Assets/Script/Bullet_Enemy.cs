using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Enemy : MonoBehaviour
{
    public enum Type {Melee, Range, Boss};
    public Type type;
    public GameObject effectObj; //폭발이펙트
    public GameObject maeshObj; //미사일오브젝트 등록
    public bool isRock;
    public int damage;
    void OnCollisionEnter(Collision collision)
    {
        //몬스터 A,B 전용
        if(type == Type.Melee)
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
        else if (type == Type.Range )
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
        else if (type == Type.Boss )
        {
            if(!isRock && collision.gameObject.tag == "Floor")
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
        if(type == Type.Melee)
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
        else if (type == Type.Range)
        {
            if(other.gameObject.tag == "Floor")
            {
                Destroy(gameObject);
            }
            else if(other.gameObject.tag == "Wall")
            {
                Destroy(gameObject);
            }
            else if(other.gameObject.tag == "Player")
            {
                Debug.Log("몬스터 미사일 플레이어와 충돌");
                maeshObj.SetActive(false);
                effectObj.SetActive(true);
                Destroy(gameObject,2f);
            }
        }
        else if (type == Type.Boss )
        {
            if(!isRock && other.gameObject.tag == "Floor")
            {
                Destroy(gameObject);
            }
            else if(other.gameObject.tag == "Wall")
            {
                Destroy(gameObject);
            }
            else if(other.gameObject.tag == "Player")
            {
                Debug.Log("보스 공격 플레이어와 충돌");
                Destroy(gameObject);
            }
        }

    }

}
