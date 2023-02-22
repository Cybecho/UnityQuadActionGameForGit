using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet_Enemy
{
    Rigidbody rigid;
    float angularPower = 2;
    float scaleValue = 0.1f;
    bool isShoot;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }
    //GainPower에서 계속 기를 모으고있음
    IEnumerator GainPower()
    {
        //isShoot이 트루가 아닐때까지 반복
        //while 문 안에는 yield return 딜레이를 주지 않으면 오류가 발생한다
        while(!isShoot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            //z축으로 회전하기때문에 transform.right / 값을 지속적으로 올릴거기때문에 Acceleration
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null;
        }
    }

}
