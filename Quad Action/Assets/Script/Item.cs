using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //enum은 타입일뿐, 해당 인덱스에 값을 담기위해 변수를 지정해줘야한다
    public enum Type {Ammo, Coin, Granade, Heart, Weapon};
    //enum의 Type을 받기위한 type변수 선언
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Update() 
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true; //외부 물리효과에 의해서 움직일 수 없게 변경
            sphereCollider.enabled = false;
        }
    }

}
