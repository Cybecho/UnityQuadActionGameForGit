using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //무기정보 변수설정
    public enum Type { Melee, Range }; //근거리/원거리 열거형으로 무기타입
    public Type type; //실제 무기 타입이 저장될 변수 (enum에서 설정한 타입이 type에 저장)
    public int damage;
    public float rate; //공속
    public int maxAmmo; //최대탄창
    public int curAmmo; //현재탄창

    public BoxCollider meleeArea; //공격범위를 콜라이더로 설정
    public TrailRenderer trailEffect; //무기 휘두르는 효과
    public Transform bulletPos; //총알 프리팹을 생성할 위치
    public GameObject bullet; //총알 프리팹을 저장할 함수
    public Transform bulletCasePos; //총알 프리팹을 생성할 위치
    public GameObject bulletCase; //총알 프리팹을 저장할 함수

    //플레이어가 무기를 사용하고있는지 여부
    public void Use()
    {
        //현재 타입의 무기타입이 근접무기라면
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        //무기타입이 원거리이고 현재 탄창이 0 이상일때 애니메이션 실행
        else if(type == Type.Range && curAmmo > 0)
        {
            curAmmo--; //총알소모
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {
        //yield 결과를 전달하는 함수
        //yield는 순차실행
        yield return new WaitForSeconds(0.3f); // 0.3초 대기
        meleeArea.enabled = true; // false로 해뒀던거 true로 전환
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f); // 0.3초 대기
        meleeArea.enabled = false; //0.3초후 콜라이더 삭제
        trailEffect.enabled = false;

        //break로 코루틴 탈출 가능
        yield break;
    }

    IEnumerator Shot()
    {
        //#1.총알 발사
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50; //bulletPos의 앞쪽방향으로 총알발사
        yield return null;
        //#2.탄피 배출
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantBullet.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2,3);
        caseRigid.AddForce(caseVec,ForceMode.Impulse); //힘을 가해서 총알 튀올라가게
        caseRigid.AddTorque(Vector3.up * -10 , ForceMode.Impulse);
        yield return null;
    }

    //일반함수 : Use() 함수를 메인루트라고할때 Swing()이라는 함수를 호출하면 서브루틴이라고 한다
    //코루틴함수 : Use() 메인함수와 Swing()코루틴이 동시에 실행된다 
}
