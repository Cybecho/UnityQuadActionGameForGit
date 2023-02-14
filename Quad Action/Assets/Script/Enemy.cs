using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other) 
    {
        //닿은 콜라이더의 태그가 Melee일 경우 weapon의 컴포넌트를 가져와서 현재 체력에서 weapon의 대미지를 뺀다
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));

            Debug.Log("Melee: " + curHealth);
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject); //총알이 닿으면 총알삭제

            StartCoroutine(OnDamage(reactVec, false)); //OnDamage 메소드 실행
            Debug.Log("Range: " + curHealth);
        }
    }
    
    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos; //현재값에서 폭발범위 위치값을 뺴줌
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        mat.color = Color.red;
        reactVec = reactVec.normalized;
        reactVec += Vector3.up;
        rigid.AddForce(reactVec * 3, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 14;

            //수류탄에 피격되었을때
            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false; //충돌해서 회전하는거 막았던거 해제
                rigid.AddForce(reactVec * 5, ForceMode.Impulse); //위치값 이동
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse); //회전값
            }
            //총알에 피격되었을때
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            
            yield return new WaitForSeconds(1.0f);
            boxCollider.enabled = false;
            Destroy(gameObject, 1);
        }
    }

}
