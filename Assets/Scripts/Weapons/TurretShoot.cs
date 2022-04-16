using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShoot : MonoBehaviour
{
    public GameObject bullet;
    public Rigidbody bulletRB;

    public Transform firingPoint;

    // Start is called before the first frame update
    void Start()
    {
        bulletRB = bullet.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            var bulletFire = Instantiate(bullet, firingPoint.position, firingPoint.rotation);
            bulletFire.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 1000);  
        }
       
    }
}
