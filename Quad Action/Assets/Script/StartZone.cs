using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    //게임 매니저를 변수화하여 플레이어 접촉시 스테이지 시작
    public GameManager manager;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
            manager.StageStart();
    }
}
