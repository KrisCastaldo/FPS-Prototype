using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMod : MonoBehaviour
{
    enum TriggerType
    {
        Slow,
        Reverse,
        Explosive
    }

    List<GameObject> list = new List<GameObject>();
    [Tooltip("Sets the type of trigger spawned")]
    [SerializeField]
    TriggerType _type;

    //need to create a persistent trigger

    Vector3 size;
    Vector3 offset;

    float timePassed;
    float desTimer;

    int expDmg;


    void Update()
    {
        timePassed += Time.deltaTime;

        if(transform.localScale.z < size.z)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, size, 5 * Time.deltaTime);
        }

        if(timePassed > desTimer)
        {
            Debug.Log("Destroying Trigger");
            if (list != null)
                endTrigger();
            Destroy(gameObject);
        }
    }

    public void setTrigger(string type, int damage, Vector3 big, float upTime)
    {
        if(type == "exp")
        {
            _type = TriggerType.Explosive;
            GetComponent<Renderer>().material.color = new Color(255, 0, 0, 100);
        }
        else if (type == "slow")
        {
            _type = TriggerType.Slow;
            GetComponent<Renderer>().material.color = new Color(255, 255, 255, 100);
        }
        else if (type == "rev")
        {
            _type = TriggerType.Reverse;
            GetComponent<Renderer>().material.color = new Color(255, 209, 0, 100);
        }
        expDmg = damage;
        size = big;
        desTimer = upTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (_type)
        {
            case (TriggerType.Slow):
                if (other.tag == "Enemy")
                {
                    list.Add(other.gameObject);
                    Debug.Log("hit trigger");
                    other.GetComponent<AIController>().setSpeedAI(true);
                }
                else if (other.tag == "Player")
                {
                    list.Add(other.gameObject);
                    Debug.Log("hit trigger");
                    other.gameObject.GetComponent<PlayerController>().changeSpeed(true);
                }
                //else if(other.tag == "Untagged" || other.tag == "Ground")
                //{
                //    GetComponent<Rigidbody>().useGravity = false;
                //}
                break;

            case (TriggerType.Reverse):
                break;

            case (TriggerType.Explosive):
                if (other.gameObject.tag == "Player")
                {
                    if (!list.Contains(other.gameObject))
                    {
                        list.Add(other.gameObject);
                        other.gameObject.GetComponent<Health>().subHP(expDmg);

                        //put stun state in AI and player, add force in opposite direction from explosion
                        //Vector3 direction = transform.position - other.transform.position;
                        //other.gameObject.GetComponent<Rigidbody>().AddRelativeForce(direction.normalized * 500);
                        other.gameObject.GetComponent<PlayerController>().setControlerState("Stunned");
                    }

                }
                else if (other.gameObject.tag == "Enemy")
                {

                    if (!list.Contains(other.gameObject))
                    {
                        list.Add(other.gameObject);
                        other.gameObject.GetComponent<Health>().subHP(expDmg);
                        //put stun state in AI and player, add force in opposite direction from explosion
                        //other.gameObject.GetComponent<AIController>().dirForce();
                    }

                }
                else if (other.gameObject.tag == "DesObj")
                {
                    if (!list.Contains(other.gameObject))
                    {
                        list.Add(other.gameObject);
                        other.gameObject.GetComponent<Health>().subHP(expDmg);
                    }
                }
                break;

            default:
                Debug.Log("No trigger type found");
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (_type)
        {
            case (TriggerType.Slow):
                if (other.tag == "Enemy")
                {
                    list.Remove(other.gameObject);
                    Debug.Log("leaving trigger");
                    other.GetComponent<AIController>().setSpeedAI(false);
                }
                else if (other.tag == "Player")
                {
                    list.Remove(other.gameObject);
                    Debug.Log("hit trigger");
                    other.GetComponent<PlayerController>().changeSpeed(false);
                }
                break;

            case (TriggerType.Reverse):
                break;

            case (TriggerType.Explosive):
                break;

            default:
                Debug.Log("No trigger type found");
                break;
        }
    }

    void endTrigger()
    {
        switch (_type)
        {
            case (TriggerType.Slow):
                for (int i = 0; i < list.Count; i++) {
                    {
                        if (list[i].tag == "Enemy")
                        {
                            list[i].GetComponent<AIController>().setSpeedAI(false);
                        }
                        else if (list[i].tag == "Player")
                        {
                            list[i].GetComponent<PlayerController>().changeSpeed(false);
                        }
                    }
                }
                break;

            case (TriggerType.Reverse):
                break;

            case (TriggerType.Explosive):
                break;

            default:
                Debug.Log("No trigger type found");
                break;
        }
    }

    //add function to check if overlapping trigger of same type
    //combine triggers into new trigger
}
