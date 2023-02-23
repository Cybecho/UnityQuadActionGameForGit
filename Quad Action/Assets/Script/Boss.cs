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

    void Start()
    {
        
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
}
