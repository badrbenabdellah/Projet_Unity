using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public Transform shootingPoint;
    public GameObject bulletPrefab;

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("I'm here....");
            Instantiate(bulletPrefab, shootingPoint.position, transform.rotation);
        }
    }
}
