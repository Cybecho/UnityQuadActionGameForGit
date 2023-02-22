using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Player player;
    public int compareHealth;
    public int shakeIntencity = 3;
    public bool isDamage;
    
    void Start()
    {
        compareHealth = player.health;
    }
    void Update()
    {
        decreaseHealth();
        switch (isDamage)
        {
            case false:
                transform.position = target.position + offset;
                break;
            case true:
                CameraShake();
                Invoke("isDamageOff",0.3f);
                break;
        }
    }
    void decreaseHealth()
    {
        if (compareHealth > player.health)
        {
            compareHealth = player.health;
            isDamage = true;
        }
    }

    void isDamageOff()
    {
        isDamage = false;
    }
    void CameraShake()
    {
    transform.position = new Vector3((target.position.x + Random.Range(-shakeIntencity, shakeIntencity))
                                            , (target.position.y + Random.Range(-shakeIntencity, shakeIntencity))
                                            , (target.position.z + Random.Range(-shakeIntencity, shakeIntencity))) + offset;
    }
}
