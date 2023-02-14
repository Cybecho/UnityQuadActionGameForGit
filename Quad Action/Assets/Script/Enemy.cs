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
            StartCoroutine(OnDamage(reactVec));

            Debug.Log("Melee: " + curHealth);
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject); //총알이 닿으면 총알삭제

            StartCoroutine(OnDamage(reactVec)); //OnDamage 메소드 실행
            Debug.Log("Range: " + curHealth);
        }
    }
    
    IEnumerator OnDamage(Vector3 reactVec)
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

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            yield return new WaitForSeconds(1.0f);
            boxCollider.enabled = false;
            Destroy(gameObject, 1);
        }
    }
}
