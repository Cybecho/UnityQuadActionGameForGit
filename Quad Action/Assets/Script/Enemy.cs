using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public Transform target; //목표물을 설정하는 트랜스폼 변수 생성
    public bool isChase = false; //추적을 하냐 안하냐

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart",2); //추격하는 함수를 2초뒤에 실행한다
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk",true);
    }
    void Update()
    {
        //도착할 목표 위치를 지정하는 함수
        //isChase상태일때만 추적을 시작한다
        if(isChase)
            nav.SetDestination(target.position);
    }
    void FreezeVelocity()
    {
        if(isChase) //추격상태일때만
        {
             //플레이어와 닿았을 때 플레이어의 리지드바디에의해 물리값을 가지지 않도록
            rigid.velocity = Vector3.zero; //물리적인속도
            rigid.angularVelocity = Vector3.zero; //회전력
        }
    }
    void FixedUpdate() 
    {
        FreezeVelocity();
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
            isChase = false; //추격함수를 종료
            nav.enabled = false; //네비게이션 컴포넌트도 비활성화
            anim.SetTrigger("doDie"); //사망 애니메이션 포함
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
