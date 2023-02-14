using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject maeshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        //3초 뒤 코드 실행
        yield return new WaitForSeconds(3f);
        
        //기존에 가지고있던 postions값과 rotation값을 초기화시켜준다
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        //메쉬오브젝트는 안보이게설정하고 이펙트오브젝트는 보이게 설정한다
        maeshObj.SetActive(false);
        effectObj.SetActive(true);

        //부피가 있는 레이케스트를 활용하여 피격범위 설정
        //범위내에있는놈들 싹다 죽여야하기때문에 배열로 생성
        //SphereCastAll(시작위치,반지름,레이케스트발사방향,레이케스트길이,레이어마스크) 구체모양의 레이캐스팅
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15,Vector3.up,0,LayerMask.GetMask("Enemy"));

        //rayHits[] 배열 안에있는 Enemy태그가 붙어있는 넘들
        //foreach 문으로 수류탄 범위 적들의 피격함수 호출
        foreach (RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }
        Destroy(gameObject,5);
    }
}
