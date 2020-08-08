using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using System.Resources;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [Header("GameObject References")]
    [Tooltip("Ragdoll body reference")]
    [SerializeField]
    GameObject Ragdoll;

    enum AI_Type
    {
        Basic,
        Gunner
    }

    [Header("State Machine References")]
    [Tooltip("Sets the AI type")]
    [SerializeField]
    AI_Type ai_Type;

    enum AI_State
    {
        Spawn,
        Moving,
        Attacking,
        Stunned,
        Dead
    }
    [Tooltip("AI state reference")]
    [SerializeField]
    AI_State ai_State;
    AI_State last_State;

    Vector3 bulletforce;

    Vector3 goal;

    NavMeshAgent agent;

    [Tooltip("AI base movement speed")]
    [SerializeField]
    float [] mSpeed = new float [2];
    float curSpeed;

    float deathTime;

    float attackTime;
    float attacking;

    bool slowed;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        setAI_Type();
        agent.speed = curSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        ai_Behavior();
    }

    void ai_Behavior()
    {
        switch (ai_Type)
        {
            case AI_Type.Basic:
                BasicAI_behavior();                
                break;

            case AI_Type.Gunner:
                GunnerAI_behavior();
                break;

            default:
                Debug.Log("Broke ai_Type");
                break;

        }
    }

    void BasicAI_behavior()
    {
        switch (ai_State)
        {
            case AI_State.Moving:
                Debug.Log("ai_State is " + ai_State);
                goal = GameObject.FindGameObjectWithTag("Player").transform.position;
                float distance = Vector3.Distance(goal, gameObject.transform.position);

                if (Mathf.Abs(distance) > 2.5f)
                {
                    agent.destination = goal;
                }
                else if(Mathf.Abs(distance) <= 2.5f)
                {
                    agent.destination = transform.position;
                    last_State = ai_State;
                    ai_State = AI_State.Attacking;
                    Debug.Log("Changing to ai_State " + ai_State);
                }
                break;

            case AI_State.Attacking:
                Debug.Log("ai_State is " + ai_State);
                if(attacking < attackTime)
                {
                    attacking += Time.deltaTime;
                }
                else if (attacking >= attackTime)
                {
                    last_State = ai_State;
                    ai_State = AI_State.Moving;
                    Debug.Log("going back to " + ai_State);
                    attacking = 0;
                }
                break;

            case AI_State.Stunned:
                Debug.Log("ai_State is " + ai_State);
                if (attacking < attackTime)
                {
                    attacking += Time.deltaTime;
                }
                else if (attacking >= attackTime)
                {
                    last_State = ai_State;
                    ai_State = AI_State.Moving;
                    Debug.Log("going back to " + ai_State);
                    attacking = 0;
                }
                break;

            case AI_State.Dead:
                Debug.Log("ai_State is " + ai_State);
                agent.destination = transform.position;
                StartCoroutine(ragDoll());
                break;

            case AI_State.Spawn:
                break;

            default:
                Debug.Log("no assigned AI State for " + gameObject.name);
                break;
        }
    }

    void GunnerAI_behavior()
    {

    }

    void setAI_Type()
    {
        switch (ai_Type)
        {
            case AI_Type.Basic:
                curSpeed = mSpeed[0];
                attackTime = 3f;
                deathTime = 5f;
                break;

            case AI_Type.Gunner:
                curSpeed = mSpeed[1];
                deathTime = 5f;
                break;

            default:
                Debug.Log("Broke ai_Type");
                break;

        }
        Debug.Log(gameObject.name + " type is "+ ai_Type +" AI");
    }

    public void setSpeedAI(bool slowed)
    {
        if(slowed)
        agent.speed = curSpeed / 1.5f;
        else
        agent.speed = curSpeed;
    }

    public void refAIState(string state)
    {
        switch (state)
        {
            case "Spawn":
                ai_State = AI_State.Spawn;
                break;

            case "Moving":
                ai_State = AI_State.Moving;
                break;

            case "Stun":
                ai_State = AI_State.Stunned;
                break;

            case "Dead":
                ai_State = AI_State.Dead;
                break;

            default:
                Debug.Log("Broke ai_State");
                break;

        }
    }

    public void dirForce(Vector3 direction)
    {
        bulletforce = direction;
    }

    IEnumerator ragDoll()
    {
        //gameObject.GetComponent<Rigidbody>().mass = 0;
        Ragdoll.SetActive(true);
        //gameObject.GetComponent<Rigidbody>().AddRelativeForce(bulletforce * 500);
        yield return new WaitForSeconds(deathTime);
        Destroy(this.gameObject);
    }
}
