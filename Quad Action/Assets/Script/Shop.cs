using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //UI를 불러와야 UI의 텍스트가 보인다

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup; //UI를 담을 변수
    public Animator anim;
    public GameObject[] itemObj; //아이템 정보를 불러오기위해 배열로 선언
    public int[] itemPrice;
    public Transform[] itemPos; //아이템이 생성될 위치
    public string[] talkData; //NPC대사를 바꾸기 위한 String 배열
    public Text talkText;
    
    Player enterPlayer; //플레이어 정보를 상점이 받는 변수

    //UI 들어가고
    //enterPlayer를 불러올때 어디서불러오지?
    //불러오기 위해 매개변수를 함수에 넣어준다
    //또 public으로 선언하여 외부 함수를 불러올 수 있게 설정
    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero; //앵커포인트를 0로 고정하여 화면 중앙에
    }


    //UI 나가고
    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000; //다시 화면 아래쪽으로 내려감
    }

    //어떤물건인지 알기 위해서 index를 추가해준다
    public void Buy(int index)
    {
        //우리가 선택한 아이템의 가격은 아이템가격의 인덱스를 참조한다
        int price = itemPrice[index];
        //만약 돈이 부족하다면
        if(price > enterPlayer.coin)
        {
            StopCoroutine(Talk()); //만약 이미 코루틴이 실행중이라면 꼬일수있기때문에 코드를 종료시켜준다
            StartCoroutine(Talk());
            return;
        }
        
        enterPlayer.coin -= price; //물건값 빼기
        //아이템이 생성될 랜덤 위치값 생성
        Vector3 ranVec = Vector3.right * Random.Range(-3,3)
                        + Vector3.forward * Random.Range(-3,3);
        //구입 성공시 아이템생성
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);

    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
