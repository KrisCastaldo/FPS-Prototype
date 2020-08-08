using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Text Output References")]
    [Tooltip("Enemy health reference")]
    [SerializeField]
    TextMesh healthOutput;
    [Tooltip("Player health reference")]
    [SerializeField]
    Text playerHeath;
    [Tooltip("Health Bar Ref")]
    [SerializeField]
    Slider healthBar;
    [Tooltip("Fill image for Health Bar")]
    [SerializeField]
    Image fill;

    [Header("Heath Component")]
    [Tooltip("Maximum Hp")]
    public float max_hp;
    float cur_hp;

    float healthPercent;

    [SerializeField]
    GameObject triggerMod;
    
    enum ObjectType
    {
        Barrel,
        Breakable,
        Rubble,
        Player,
        Enemy
    }
    [Header("Object Type Declairation")]
    [SerializeField]
    ObjectType _objectType;

    public Gradient hpGradient;

    private void Start()
    {
        cur_hp = max_hp;
        if (_objectType == ObjectType.Enemy)
        {
            playerHeath = null;
            healthOutput.color = Color.green;
            healthOutput.text = (cur_hp / max_hp * 100).ToString();
        }
        else if (_objectType == ObjectType.Player)
        {
            healthOutput = null;
            healthBar.SetValueWithoutNotify(cur_hp / max_hp * 100);
            fill.color = hpGradient.Evaluate(cur_hp / max_hp * 100);
            playerHeath.text = (cur_hp / max_hp * 100).ToString();
        }
    }

    private void Update()
    {
        if(_objectType == ObjectType.Enemy)
        {
            healthOutput.transform.rotation = Camera.main.transform.rotation;
        }
    }


    public void subHP(int damage)
    {
        cur_hp -= damage;

        if (_objectType == ObjectType.Enemy)
        {
            healthOutput.text = (cur_hp / max_hp * 100).ToString();
        }
        else if (_objectType == ObjectType.Player)
        {
            playerHeath.text = (cur_hp / max_hp * 100).ToString();
            healthBar.value = cur_hp / max_hp;
            fill.color = hpGradient.Evaluate(cur_hp / max_hp);
        }

        if((cur_hp / max_hp) * 100 <= 60 && (cur_hp / max_hp) * 100 > 30)
        {
            //healthOutput.color = Color.yellow;
        }
        else if((cur_hp / max_hp) * 100 <= 30 && (cur_hp / max_hp) > 0)
        {
           // healthOutput.color = Color.red;
        }
        else if (cur_hp <= 0)
        {
            //if(healthOutput != null)
               // healthOutput.color = Color.red;

            switch (_objectType)
            {
                case (ObjectType.Barrel):
                    GameObject expTrigger = Instantiate(triggerMod, transform.position, transform.rotation);
                    expTrigger.GetComponent<TriggerMod>().setTrigger("exp", 45, new Vector3(5, 5, 5), 1f);
                    Destroy(gameObject);
                    break;

                case (ObjectType.Breakable):
                    //breakable object function
                    break;

                case (ObjectType.Rubble):
                    //rubble function
                    break;

                case (ObjectType.Player):
                    (cur_hp / max_hp * 100).ToString();
                    GetComponent<PlayerController>().setControlerState("Dead");
                    break;

                case (ObjectType.Enemy):
                    healthOutput.text = "Dead";
                    this.gameObject.GetComponent<AIController>().refAIState("Dead");
                    break;

                default:
                    break;
            }
        }
    }
}
