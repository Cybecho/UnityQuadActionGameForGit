using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Player player;
    int compareHealth;
    
    void Start()
    {
        compareHealth = player.health;
    }
    void Update()
    {
        //해당 스크립트는 타겟의 포지션에 오프셋을 더한 값이다
        transform.position = target.position + offset;
        if (compareHealth > player.health)
        {
            compareHealth = player.health;
        }
    }

    void CameraShake()
    {
        
    }
}
