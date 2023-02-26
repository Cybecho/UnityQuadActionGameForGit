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
    public int stage;
    public float playTime; //플레이시간
    public bool isBattle; //현재 전투 스테이지인지?
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;

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
        bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth,1,1);
    }
}
