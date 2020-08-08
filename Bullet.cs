using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Variables")]
    [Tooltip("Force added to rigidbody")]
    [SerializeField]
    float bulletForce;
    [Tooltip("Damage dealt")]
    [SerializeField]
    int damage;

    GameStates gState;

    // Start is called before the first frame update
    void Start()
    {
        gState = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameStates>();

        if (gState.gameState.ToString() == "Slow")
        {
            bulletForce = bulletForce * 2;
        }

        this.gameObject.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * bulletForce);
        this.gameObject.transform.parent = null;
    }

    void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Health>().subHP(damage);

        }
        else if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<Health>().subHP(damage);
            other.gameObject.GetComponent<AIController>().dirForce(-gameObject.transform.forward);
        }
        else if (other.gameObject.tag == "DesObj")
        {
            other.gameObject.GetComponent<Health>().subHP(damage);
        }

        Debug.Log("Hit");
        Destroy(this.gameObject);
    }
}
