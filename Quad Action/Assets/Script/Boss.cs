using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    //플레이어의 이동방향을 예측하는 변수
    Vector3 lookVec;
    Vector3 tauntVec;
    bool isLook;

    void Awake()
    {
        StartCoroutine(Think());
    }

    void Update()
    {
        if(isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            //얼마나 예측할것인가? (5) 만큼 예측하겠다
            lookVec = new Vector3(h,0,v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
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
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }
}
