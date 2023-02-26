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

    public void GameStart()
    {
        //메뉴 관련 오브젝트 비활성화
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true); //비활성화해뒀던 플레이어 오브젝트 활성화
    }
}
