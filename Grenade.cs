using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    Vector3 trajectory;
    Vector3 size;

    float deltaZ;
    float finalDeltaZ;
    float maxDeltaZ = 8;
    float baseDeltaZ = 10;

    int dmg;
    int pickupCount;
    float upTime;
    [Tooltip("Is this a pickup object?")]
    [SerializeField]
    bool pickup;
    bool thrown;
    [Tooltip("Spawn reference")]
    [SerializeField]
    GameObject triggerMod;

    enum type
    {
        slow,
        rev,
        exp
    }

    [SerializeField]
    type _type;

    // Start is called before the first frame update
    void Start()
    {
        if (pickup)
        {
            thrown = true;
            if(pickupCount == 0)
            {
                pickupCount++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!thrown)
        {
            if (deltaZ < maxDeltaZ)
            {
               deltaZ += (Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (pickup)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<GunScript>().addCount(pickupCount, _type.ToString());
                Debug.Log("Pickup count is " + pickupCount);
                if (pickupCount <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void ThrowGrenade(int _dmg, string grenade)
    {
        switch (grenade)
        {
            case ("exp"):
                _type = type.exp;
                dmg = _dmg;
                size = transform.localScale * 3;
                upTime = 1f;
                break;

            case ("rev"):
                _type = type.rev;
                size = transform.localScale * 3;
                upTime = 10f;
                break;

            case ("slow"):
                _type = type.slow;
                size = transform.localScale * 3;
                upTime = 5f;
                break;

            default:
                Debug.Log("Couldnt read grenade type");
                break;
        }

        thrown = true;

        //set our final distance to the difference between the grenades forward.z vector and our total distance
        //total distance is our change in Z + our base throw distance
        //finalDeltaZ is negative because we want our curve to go upwards
        finalDeltaZ = -(Vector3.forward.z - (Mathf.Abs(Vector3.forward.z + deltaZ)+baseDeltaZ));

        //set our throw angle to radians for easy calculation of rotation
        float angle = Mathf.Deg2Rad * transform.parent.GetComponent<Transform>().eulerAngles.x;
        transform.parent = null;

        //if our radian angle is over 5.5 we subtract 5 so it is equal to our -eulerAngle
        if(angle >= 5.5)
        {
            angle = angle - 5;
        }
        float yTrajectory = Mathf.Sin(finalDeltaZ) * (angle);

        //set our target vector to our forward.x, our calculated peak, and our final distance
        Vector3 trajectory = new Vector3(Vector3.forward.x, yTrajectory, finalDeltaZ);

        //turn off kinematic to allow forces to act on the grenade
        GetComponent<Rigidbody>().isKinematic = false;

        //set the velocity of our grenade = to its current rotation * (trajectory + our player's velocity)
        //allows us to increase or decrease our velocity if we are moving forwards or backwards
        GetComponent<Rigidbody>().velocity = transform.rotation * (trajectory + GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().velocity);

        //set to expload in 10s
        //will change to a multiplyer*our distance so the further you throw, the longer the eplosion timer
        StartCoroutine(expload(3f));
    }

    public void spawnTrigger()
    {
        GameObject expTrigger = Instantiate(triggerMod, transform.position, transform.rotation);
        expTrigger.GetComponent<TriggerMod>().setTrigger(_type.ToString(), dmg, size, upTime);
    }

    public IEnumerator expload(float time)
    {
        yield return new WaitForSeconds(time);
        spawnTrigger();
        Destroy(gameObject);
    }
}
