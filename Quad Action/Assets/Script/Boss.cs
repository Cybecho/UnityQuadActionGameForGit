using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    //플레이어의 이동방향을 예측하는 변수
    Vector3 lookVec;
    Vector3 tauntVec;   
    public bool isLook;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true; //보스는 네비게이션을 사용하지 않기때문에 상속받은 nav값을 off시켜둡니다
        StartCoroutine(Think());
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        if(isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            //얼마나 예측할것인가? (5) 만큼 예측하겠다
            lookVec = new Vector3(h,0,v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        //점프공격 할 때 목표지점으로 이동하도록 로직 추가
        else
            //tauntVec을 향해 따라가주십시오
            nav.SetDestination(tauntVec);
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f); //생각하는 시간 길수록 보스가 쉬워진다
        
        //랜덤으로 0~4값이 랜덤액션값에 들어간다
        //보스가 랜덤값에 따라 다른 패턴을 가지기 위함이다
        int ranAction = Random.Range(0,5);
        switch (ranAction)
        {
            case 0:
            case 1:
                //미사일 발사 패턴
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                //돌 굴러가는 패턴
                StartCoroutine(RockShot());
                break;
            case 4:
                //점프 공격 패턴
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        //애니메이션 실행
        anim.SetTrigger("doShot");

        //첫번째 미사일 발사 코드
        yield return new WaitForSeconds(0.2f);
        //Instantiate(인스턴트할 오브젝트변수, 인스턴트 생성 위치, 인스턴트 생성 각도)
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target; //미사일의 타겟에 현재클래스의 타겟을 담는다

        //두번째 미사일 발사 코드
        yield return new WaitForSeconds(0.3f);
        //Instantiate(인스턴트할 오브젝트변수, 인스턴트 생성 위치, 인스턴트 생성 각도)
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target; //미사일의 타겟에 현재클래스의 타겟을 담는다

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }

    IEnumerator RockShot()
    {
        //기를 모을땐 플레이어를 바라보는것을 중지시킨다
        isLook = false;
        anim.SetTrigger("doBigShot");
        //인스턴트를 생서할 오브젝트를 bullet에 저장하였고, 그 bullet의 pos값과 rotate 값을 그대로 받아오겠다는 뜻
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        //내려찍을 위치를 받기 위해 점프공격 위치를 변수에 저장
        //점프상태일때 타겟을 바라보면 어색하니 잠시 isLook을 끔
        //타겟의 위치와 바라보는 위치값을 더함
        tauntVec = target.position + lookVec;
        
        isLook = false;
        nav.isStopped = false; //네비게이션이 다시 작동합니다
        boxCollider.enabled = false; //공중에 있을때 콜라이더가 충돌하여 데미지를 입지 않게
        anim.SetTrigger("doTaunt");

        //1.5초 지나면 공격범위 활성화
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        //공격이 끝났으니 다시 원래대로
        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true; //네비게이션이 종료됩니다
        boxCollider.enabled = true; //공중에 있을때 콜라이더가 충돌하여 데미지를 입지 않게
        StartCoroutine(Think());
    }
}