using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //UI를 다루기 위해선 해당 라이브러리 활성화

public class GameManager : MonoBehaviour
{
    //게임매니저가 필요한 변수들부터 실행
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    public int stage;
    public float playTime; //플레이시간
    public bool isBattle; //현재 전투 스테이지인지?
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    //몬스터 리스폰에 필요한 변수들 선언
    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList; //몬스터가 얼마나 소환될것인지 리스트 작성

    //UI를 위한 변수설정
    public GameObject menuPanel;
    public GameObject gamePanel;
    public Text maxScoreTxt;
    public Text scoreTxt; //gamePanel 상단부
    public Text stageTxt;
    public Text playTimeTxt;
    public Text PlayerHealthTxt; //gamePanel 하단부
    public Text PlayerAmmoTxt;
    public Text PlayerCoinTxt;
    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;
    public Image weapon1Img; //gamPanel 무기부
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    public RectTransform bossHealthGroup; //보스 HP를 활성화시키는 스위치
    public RectTransform bossHealthBar; //보스 피통

    void Awake() 
    {
        //몬스터 생성 정보를 불러옴
        enemyList = new List<int>();
        //최고점수 불러오기
        //그냥 GetInt하면 기존 maxScore가 String으로 선언되어있기때문에
        //string.Format() 함수로 문자열 양식을 적용할것이다
        //string.Format("어떤양식을원하는가?",실제 값)
        maxScoreTxt.text = string.Format(("{0:n0}"),PlayerPrefs.GetInt("MaxScore"));
    }

    void Update()
    {
        //싸울때만 플레이타임을 더할것임
        if(isBattle)
            playTime  += Time.deltaTime;
        
    }

    public void GameStart()
    {
        //메뉴 관련 오브젝트 비활성화
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true); //비활성화해뒀던 플레이어 오브젝트 활성화
    }

    public void GameOver()
    {
        
    }

    //스테이지 시작, 종료 함수 생성
    public void StageStart()
    {
        //전투가 시작되면 상점 및 게임스타트 영역을 비활성화시킵니다
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);
        
        //몬스터 소환존 스테이지 시작할때 활성화
        foreach(Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true; //전투상태 ON
        StartCoroutine(InBattle());
    }

    //코루틴으로 전투 상태 구현
    IEnumerator InBattle()
    {
        //스테이지가5단위일때라면 보스를 소환한다
        if(stage % 5 == 0)
        {
            enemyCntD++;
            //보스 하나만 소환할거기 떄문에 enemies[3] 고정
            //보스가 소환될 위치도 0번째로 고정
            GameObject instantEnemy = Instantiate(enemies[3]
                                                    ,enemyZones[0].position
                                                    ,enemyZones[0].rotation);
            //인스턴트된 enemy에서 컴포넌트값을 가져온다
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform; //enemy의 타겟은 player의 위치값이된다
            enemy.manager = this; //몬스터의 매니저 변수는 스스로를 넣습니다
            boss = instantEnemy.GetComponent<Boss>(); //비어있는 보스 변수에 Boss의 컴포넌트를 채워넣는다
        }
        //일반 스테이지라면
        else
        {
            //소환 리스트를 for문을 사용하여 데이터 채우기
            for(int index=0; index < stage; index++)
            {
                //0~3의 랜덤한 값을 enemyList에 넣는다
                int ran = Random.Range(0,3);
                enemyList.Add(ran);

                switch(ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }
            //랜덤값을 담은 enemyList는 while문을 통해 지속적으로 몬스터를 소환한다
            //enemyList에 데이터가 없을때까지
            while(enemyList.Count > 0)
            {
                //소환하는곳이 4곳이니 랜덤값 0~4 설정
                //게임오브젝트 변수를 따로 만들어서 인스턴트를 생성함
                //인스턴트 내용 : 인스턴트한다(몬스터프리팹[몹종류들[n]],소환될장소[].위치,소환될장소[].회전값)
                int ranZone = Random.Range(0,4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]]
                                                    ,enemyZones[ranZone].position
                                                    ,enemyZones[ranZone].rotation);
                //인스턴트된 enemy에서 컴포넌트값을 가져온다
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform; //enemy의 타겟은 player의 위치값이된다
                enemy.manager = this; //몬스터의 매니저 변수는 스스로를 넣습니다
                //다 쓴 데이터는 첫번째 배열부터 지워줍니다
                enemyList.RemoveAt(0); //소환하고 목록지우고 반복.. 하다보면 배열이 전부 삭제되어 while이 종료됩니다
                yield return new WaitForSeconds(4f); //안전하게 while문을 돌리기 위해서 yield return을 함
            }
        }
        //남은 몬스터 숫자를 검사하는 while문
        //이렇게 반복문을 돌리면 마치 Update처럼 매시간 현상태를 확인할 수 있다
        //소환된 몬스터의 수를 합쳤을때 0보다 크다면 몬스터가 존재하는것이니 몬스터가 없을때까지 계속 반복한다
        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }
        //몬스터를 모두 처리해 위에 while 장벽을 뚫었다면, 4초 뒤 스테이지는 종료됩니다
        yield return new WaitForSeconds(4f);

        boss = null; //보스를 처리했다면 보스는 null이 됩니다
        StageEnd();
    }

    public void StageEnd()
    {
        //플레이어를 시작 위치로 초기화
        player.transform.position = Vector3.up * 0.8f;
        //전투가 종료되면 상점 및 게임스타트 영역을 활성화시킵니다
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        //몬스터 소환존 스테이지 끝날때 비활성화
        foreach(Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++; //스테이지를 한단계 올림
    }

    //LateUpdate : Update()가 끝난 후 호출되는 생명주기
    void LateUpdate() 
    {
        //상단 UI
        //플레이어 스크립트에서 점수 체력 코인 정보 변수에 저장
        scoreTxt.text = string.Format(("{0:n0}"),player.score);
        stageTxt.text = "STAGE " + stage;
        
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60); //이미 시간단위로 나눴기 때문에 그 나눈값을 60으로 나눔
        int second = (int)(playTime % 60); //분을 나눈 나머지값이 초다
        playTimeTxt.text = string.Format("{0:00}",hour) + ":" + string.Format("{0:00}",min) + ":" + string.Format("{0:00}",second);
        
        PlayerHealthTxt.text = player.health + " / " + player.maxHealth;
        PlayerCoinTxt.text = string.Format(("{0:n0}"),player.coin);
        
        //플레이어 UI
        //만약 플레이어의 장착템이 null이거나 melee라면 표시를 '-' 로 표기
        if(player.equipweapon == null)
            PlayerAmmoTxt.text = "- / " + player.ammo;
        else if(player.equipweapon.type == Weapon.Type.Melee)
            PlayerAmmoTxt.text = "- / " + player.ammo;
        else
            PlayerAmmoTxt.text = player.equipweapon.curAmmo + " / " + player.ammo;

        //무기 UI
        //무기가 00입니까? [n] 맞다면? 1 아니라면: 0
        weapon1Img.color = new Color(1,1,1, player.hasWeapons[0] ? 1 : 0); //망치
        weapon2Img.color = new Color(1,1,1, player.hasWeapons[1] ? 1 : 0); //권총
        weapon3Img.color = new Color(1,1,1, player.hasWeapons[2] ? 1 : 0); //서브머신건
        weaponRImg.color = new Color(1,1,1, player.hasGrenades > 0 ? 1 : 0); //수류탄이 0개보단 많다

        //몬스터 숫자 UI
        //적의 수를 설정하여 문자열로 저장 후 UI에 저장
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();
        
        //보스 체력 UI
        //보스의 현재체력에서 최대체력을 나눈값을 X축에 넣어서 크기를 줄여준다
        if(boss != null) //보스가 활성화 되어있을때만
        {
            //보스 체력바가 보이도록 앵커포인트위치를 변경한다
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth,1,1);
        }
        else
        {
            //보스 체력바가 안보이도록 앵커포인트위치를 변경한다
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
    }
}
