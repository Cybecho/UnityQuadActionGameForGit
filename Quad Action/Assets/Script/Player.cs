using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] Weapons; //게임오브젝트를 받는 배열 게임 모델을 배열안에 넣을 수 있음
    public bool[] hasWeapons; //무기를 가지고있는지 아닌지 확인하는 배열
    public GameObject[] grenades; //플레이어 주위를 공전하는 수류탄을 만들기 위해서
    public int hasGrenades;
    public GameObject grenadeObjects; //던져질 수류탄 오브젝트
    public Camera followCamera;
    public GameManager manager;
    //플레이어에게 탄약 동전 체력 수류탄 변수를 생성
    public int ammo;
    public int coin;
    public int health;
    //아이템의 최대 보유수를 한정시킬 변수 생성
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;
    public int score;

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown; //점프가j 눌렸나? space
    bool iDown; //아이템이i 눌렸나? e
    bool fDown; //공격이f 눌렸나?
    bool gDown; //수류탄이g 눌렸나? 마우스우클릭
    bool rDown; //장전 r 이 눌렸나? r
    bool sDown1; //아이템변경(swuap) 이 눌렸냐? 1
    bool sDown2; //아이템변경(swuap) 이 눌렸냐? 2
    bool sDown3; //아이템변경(swuap) 이 눌렸냐? 3

    bool isJump;
    bool isDodge; //점프키를 그대로 이용하지만 구분자만 다르게 설정한다
    bool isSwap;
    bool isFireReady = true; //지금은 공격상태다를 알려주기 위함
    bool isReroad = false;
    bool isBorder; //벽과 닿았나 안닿았나?
    bool isDamage; //무적타임부여
    bool isShop;
    bool isDead;
    Vector3 moveVec;
    Vector3 dodgeVec;
    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs; //팔,다리 몸통별로 따로따로 메쉬렌더러를 가지고 있기 때문에 배열로 선언
    GameObject nearObject;
    public Weapon equipweapon; //장착하고있는 무기의 메쉬 //Weapon.cs의 클래스를 사용해야하기 때문
    int equipweaponIndex = -1;
    float fireDelay; //공격캔슬을 막기 위한 딜레이
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        //플레이어 프리팹의 자식에서 컴포넌트를 받아오겠다
        //플레이어 프리팹의 Animator를 anim에 담음

        meshs = GetComponentsInChildren<MeshRenderer>();
        //요소를 여러개 가져올땐 Compent's' 복수형이다
        Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        PlayerPrefs.SetInt("MaxScore",112500);
        //유니티에서 기본 제공하는 저장기능
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Dodge();
        Swap();
        Reload();
        Interaction();
    }

    void GetInput() //프로젝트 세팅에서 설정한 input Key를 받는 함수
    {
        //GetAxisRaw = 방향표키를 받아오겠다
        hAxis = Input.GetAxisRaw("Horizontal"); //x축
        vAxis = Input.GetAxisRaw("Vertical"); //z축
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump"); //GetButtonDown은 버튼이 눌린 순간에만 함수호출
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reroad");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis,0,vAxis).normalized;

        //혹시 지금 닷지중입니까?
        //닷지를 했을때 dodgeVec에 moveVec이 담겨서 방향값이 고정된 다
        if(isDodge)
            moveVec = dodgeVec;
        
        if(isSwap || !isFireReady || isReroad || isDead)
            moveVec = Vector3.zero;
        
        if(!isBorder)
        //걸을땐 느리게
        //벽에 닿는다면 이동 자체를 막는다(회전은 가능)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        anim.SetBool("IsRun", moveVec != Vector3.zero); // 0,0,0만 아니면 isRun을 받는다
        anim.SetBool("IsWalk", wDown);
    }

    void Turn()
    {
        //#1 키보드에 의해 결정되는 시점
        transform.LookAt(transform.position + moveVec);
        //#2 마우스에 의해 결정되는 시점
        if (fDown && !isDead && !isFireReady && (equipweapon.type == Weapon.Type.Melee || equipweapon.type == Weapon.Type.Range))
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); //스크린에서 월드로 Ray를 쏘는 함수
            RaycastHit rayHit;
            
            //레이케스트 함수에서 ray가 어딘가에 닿았다면 rayHit에 저장해준다. 이때 저장할때 쓰는 함수가 바로 out
            if(Physics.Raycast(ray, out rayHit, 100)) //out : return 처럼 반환값을 주어진 변수에 저장하는 키워드
            {
                //RaytHit의 마우스 클릭 위치를 활용하여 회전을 구현
                //히트의 포인트가 있는데 ray가 닿았던 지점이다. 그곳에서 플레이어의 위치를 빼면 상대 위치가 나온다
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0; //RayCastHit의 높이는 무시하도록 Y축 값을 0으로 초기화
                //그 위치로 플레이어가 돌아보면 된다
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if(jDown && moveVec == Vector3.zero && !isJump && !isSwap && !isDead)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("IsJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Grenade()
    {
        if(hasGrenades == 0)
            return;
        if(gDown && !isReroad && !isSwap && !isDead)
        {
/*수류탄 투척 위치 코드*/
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); //스크린에서 월드로 Ray를 쏘는 함수
            RaycastHit rayHit;
            
            //레이케스트 함수에서 ray가 어딘가에 닿았다면 rayHit에 저장해준다. 이때 저장할때 쓰는 함수가 바로 out
            if(Physics.Raycast(ray, out rayHit, 100)) //out : return 처럼 반환값을 주어진 변수에 저장하는 키워드
            {
                //RaytHit의 마우스 클릭 위치를 활용하여 회전을 구현
                //히트의 포인트가 있는데 ray가 닿았던 지점이다. 그곳에서 플레이어의 위치를 빼면 상대 위치가 나온다
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10; //던지는 효과가 나도록 높이값인 y를 높게 고정한다
                //그 위치로 플레이어가 돌아보면 된다

/*수류탄 날라댕기는 부분 코드*/
                GameObject instantGrenade = Instantiate(grenadeObjects, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

/*수류탄 사용 완료 코드*/
                hasGrenades--; //보유 수류탄 -1
                grenades[hasGrenades].SetActive(false); //공전 수류탄 수도 hasGrenades값 참조하여 비활성화
            }
        }
    }

    void Attack()
    {
        //공격하기위해선 일단 손에 무기가 있는지부터 확인 무기 없으면(null) 나가~
        if(equipweapon == null)
            return;
        
        fireDelay += Time.deltaTime; //공격 딜레이에 시간을 더해주고 공격가능 여부 확인
        isFireReady = equipweapon.rate < fireDelay; //공격속도보다 파이어딜레이가 크면 true가 저장된다
        
        if(fDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead)
        {
            equipweapon.Use(); //Weapon.cs 내부에 Use() 함수 실행
            anim.SetTrigger(equipweapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
        else if (jDown)
        {
            Dodge();
            return;
        }
    }

    void Reload()
    {
        if (equipweapon == null)
            return;
        if (equipweapon.type == Weapon.Type.Melee)
            return;
        if (ammo == 0)
            return;
        
        if(rDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop && !isDead)
        {
            anim.SetTrigger("doReload");
            isReroad = true;
            Invoke("ReloadOut",0.8f);
        }
    }
    
    void ReloadOut()
    {
        //보유총알이 총의 최대탄창보다 적으면 장전해도 그대로 보유총알 아니라면 보유총알의 최대탄차안큼 저장된다
        int reAmmo = ammo < equipweapon.maxAmmo ? ammo : equipweapon.maxAmmo;
        equipweapon.curAmmo = reAmmo;
        ammo -= reAmmo; //보유총알에서 장전하는 값을 계속해서 뺀다
        isReroad = false;
    }

    void Dodge()
    {
        if(jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isDead)
        {
            //방향전환을 막기위해서
            //현재 방향값을 받아온다
            dodgeVec = moveVec;
            
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut",0.4f); //함수이름을 문자열로 저장해줘야한다
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }
    void OnCollisionEnter(Collision collision) 
    {
        if(collision.gameObject.tag == "Floor") 
        {
            anim.SetBool("IsJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            //Item 태그에 있는 다른 컴포넌트를 받아온다
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value; //아이템의 밸류값을 ammo에 넣는다
                    if (ammo > maxAmmo) //만약 최대치를 넘는다면 최대치로 고정
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Granade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet")
        {   if(!isDamage)
            {
                Bullet_Enemy enemyBullet = other.GetComponent<Bullet_Enemy>();
                health -= enemyBullet.damage;
                
                bool isBossAtk = other.name == "BossMelleArea";
                StartCoroutine(OnDamage(isBossAtk));
            }

            /*
            //Bullet_Enemy 에서 디스트로이는 이미 구현함
            if(other.GetComponent<Rigidbody>() != null)
            Destroy(other.gameObject);
            */
        }
    }
    IEnumerator OnDamage(bool isBossAtk) //플레이어가 적의 총알의 데미지를 입었을 때
    {
        isDamage = true; //데미지를 입는 상태다(무적타임 ture)
        foreach(MeshRenderer mesh in meshs) //메쉬렌더러 mesh에서 mehs까지
        {
            
            mesh.material.color = Color.yellow; //isDamage가 true라면 한대 맞은 효과로 노란색이 되겠슴다
        }
        //보스공격을 맞았을땐 그냥 뒤로 넉백을 줘버리자
        if(isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        yield return new WaitForSeconds(1f); //1초동안 무적

        isDamage = false; //데미지를 입지 않는 상태
        foreach(MeshRenderer mesh in meshs) //메쉬렌더러 mesh에서 mehs까지
        {
            mesh.material.color = Color.white; //isDamage가 false라면 화이트로 되돌리겠슴다
        }

        if(isBossAtk)
            rigid.velocity = Vector3.zero;
        
        //플레이어의 체력이 0 이하면 죽음 함수 호출
        if(health <= 0)
            OnDie();
    }
    //플레이어 죽음 함수
    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        //Player.cs는 위 작업만 하도록 하고 나머지는 매니저에 모두 넘긴다
        manager.GameOver();
    }
    void OnTriggerStay(Collider other) 
    {
        if(other.tag == "Weapon" || other.tag == "Shop")
            nearObject = other.gameObject;
        //콜라디어 내에 있는 값들 출력
        //Debug.Log(nearObject.name);
    }

    void OnTriggerExit(Collider other) 
    {
        if(other.tag == "Weapon")
            nearObject = null;
        else if(other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }

    void Swap()
    {
        if(sDown1 && (!hasWeapons[0] || equipweaponIndex == 0)) return;
        if(sDown2 && (!hasWeapons[1] || equipweaponIndex == 1)) return;
        if(sDown3 && (!hasWeapons[2] || equipweaponIndex == 2)) return;
        //스왑버튼이 눌려있고 무기를 가지고있지 않거나 현재 무기 인덱스가 해당무기를 가지고 있을때 리턴시켜서 함수를 종료시킨다

        int weaponsIndex = -1; //weaponsIndex 기본값은 -1 즉 없는값 입니다
        if (sDown1) weaponsIndex = 0;
        if (sDown2) weaponsIndex = 1;
        if (sDown3) weaponsIndex = 2;

        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge) //1 2 3 키 중 하나만 눌린 상태이고 점프와 회피상태가 아닐떄 실행됩니다
        {
            //처음시작하면 손에 아무것도 없는 Null상태기 때문에 false를 하면 에러가뜬다
            //고로 비어있는상태가 아닐때만 현재 쥐고있는 무기를 off하는 코드작성
            if(equipweapon != null) equipweapon.gameObject.SetActive(false);
            
            equipweaponIndex = weaponsIndex;
            equipweapon = Weapons[weaponsIndex].GetComponent<Weapon>();
            //게임오브젝트[] Weapons 값은 위에서 if로 weaponsIndex을 받아오고 해당 오브젝트를 활성화시켜 보이게합니다
            equipweapon.gameObject.SetActive(true);
            isSwap = true;

            anim.SetTrigger("doSwap");
            Invoke("SwapOut",0.4f); //0.4초뒤에 isSwap을 다시 false로 되돌린다
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
    if(iDown && nearObject != null && !isJump && !isDodge && !isDead) //만약 아이템이 눌린상태라면 (e가눌린상태라면)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponsIndex = item.value;
                hasWeapons[weaponsIndex] = true;

                Destroy(nearObject);
            }
            else if(nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this); //자기 자신(현재는 Player.cs)을 넣어준다
                isShop = true;
            }
        }
    }

    void StopToWal()
    {
        Debug.DrawRay(transform.position, transform.forward * 3/*Ray의 길이*/,Color.green);
        //첫번째는 포지션 두번째는 방향 세번째는 길이 네번째는 레이어마스크
        isBorder = Physics.Raycast(transform.position, transform.forward, 3, LayerMask.GetMask("Wall"));
    }
    
    void FreezeRotation()
    {
        //angularVelocity = 물리 회전 속도
        //Update() 안에 넣어 매프레임 FreezeRotations()을 호출하여 물리회전속도를 0으로 초기화시켜 회전을 막는다
        rigid.angularVelocity = Vector3.zero;
    }

    void FixedUpdate() 
    {
        FreezeRotation();
        StopToWal();
    }
}
