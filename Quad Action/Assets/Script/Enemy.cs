using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    public enum Type {A,B,C,D}; //몬스터 타입을 결정하기 위한 타입설정
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public int score; //죽었을때 플레이어에게 점수를 주기 위함
    public GameManager manager;
    public Transform target; //목표물을 설정하는 트랜스폼 변수 생성
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject[] coins; //죽었을때 뿌릴 코인
    public bool isChase; //추적을 하냐 안하냐
    public bool isAttack; //공격을 하냐 안하냐
    public bool isDead; //죽었나 안죽었나

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        if(enemyType != Type.D)
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
        if(nav.enabled && enemyType != Type.D) //네비게이션이 활성화 되어있을때만
        {
        nav.SetDestination(target.position);
        nav.isStopped = !isChase;
        }
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
    //스피어 레이케스팅 활용해서 넓은 데미지 범위를 만들것입니다
    void Targeting()
    {
        if(!isDead && enemyType != Type.D)
        {
            float targetRadius = 0;
            float targerRange = 0;

            switch (enemyType) {
                case Type.A:
                    targetRadius = 1.5f;
                    targerRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targerRange = 12f;
                    break;
                case Type.C:
                    targetRadius = 0.1f;
                    targerRange = 25f;
                    break;
            }

            //부피가 있는 레이케스트를 활용하여 피격범위 설정
            //범위내에있는놈들 싹다 죽여야하기때문에 배열로 생성
            //SphereCastAll(시작위치,반지름,레이케스트발사방향,레이케스트길이,레이어마스크) 구체모양의 레이캐스팅
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 
                                                        targetRadius,transform.forward,targerRange,
                                                        LayerMask.GetMask("Player"));
            Debug.DrawRay(transform.position, transform.forward * targerRange,Color.green);
            //rayHits변수에 데이터가 들어오면 공격 코루틴 실행
            //만약 공격 범위 안에 플레이어가 들어왔다면?
            if(rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isChase = false; //쫓아가지 않는 상태 활성화
        isAttack = true; //공격상태 활성화
        anim.SetBool("isAttack",true); //공격 애니메이션 활성화

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.5f);
                meleeArea.enabled = true; //공격범위 활성화

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false; //공격범위 비활성화

                yield return new WaitForSeconds(2f);
                break;
            case Type.B:
                //0.1초 뒤에 앞방향으로 돌격하며 공격범위 활성화
                yield return new WaitForSeconds(0.2f);
                rigid.AddForce(transform.forward * 40, ForceMode.Impulse);
                meleeArea.enabled = true;
                //돌격 끝나고 멈춰 세운 후에 공격범위 비활성화
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                //총알 게임오브젝트 인스턴트 생성 (프리팹 , 생성위치, 생성각도) Enemy기준으로 생성됨
                GameObject instantBullet = Instantiate(bullet, transform.position + new Vector3(0f, 2.5f, 0f), transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 30;

                yield return new WaitForSeconds(2f);
                break;
        }
        
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack",false);
    }
    void FixedUpdate() 
    {
        Targeting();
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
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;
        reactVec = reactVec.normalized;
        reactVec += Vector3.up;
        rigid.AddForce(reactVec * 3, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14;
            isDead = true;
            isChase = false; //추격함수를 종료
            nav.enabled = false; //네비게이션 컴포넌트도 비활성화
            anim.SetTrigger("doDie"); //사망 애니메이션 포함
            
            //죽었을때 플레이어에게 점수를 주는 부분
            Player player = target.GetComponent<Player>();
            player.score += score;
            
            //죽었을때 동전뱉는 부분
            int ranCoin = Random.Range(0, 3);
            Instantiate(coins[ranCoin], transform.position, Quaternion.identity);
            
            //각 몬스터 타입에 맞게 카운트를 감소시키기
            switch (enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    break;
            }
            
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
            
            Destroy(gameObject, 2);
        }
    }

}
