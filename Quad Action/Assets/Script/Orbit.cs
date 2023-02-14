using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target; //수류탄이 공전할 중심 설정
    public float orbitSpeed; //수류탄 공전 속도
    Vector3 offset; //플레이어와 수류탄 사이의 거리를 계산할 고정값
    void Start()
    {
        //현재 수류탄의 위치에서 타겟 위치를 뺀다
        //그리고 해당 코드를 Update() 함수 안에 넣어서 사용한다
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        //offset 함수를 더해줘서 수류탄이 중심값을 따라갈 수 있도록 만들어준다
        transform.position = target.position + offset;
        //RotateAround 를 사용하여 특정 개체를 중심으로 물체를돌게할 수 있다
        transform.RotateAround(target.position, //타겟의 포지션을 중심으로 
                                Vector3.up, //z축이 움직이고
                                orbitSpeed * Time.deltaTime); //회전하는 수치는 델타타임 적용
        offset = transform.position - target.position; //offset 함수를 매 코드가 끝날때마다 바뀐값을 넣어준다
    }
}
