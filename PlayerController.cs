using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    GameStates GameStates;
    GunScript gunScript;
    UIController uIController;

    //for time manipulation
    enum controlStates
    {
        Norm,
        Stunned,
        Dead,
        Air,
        DeusVault
    }

    controlStates controlState;
    //controlStates lastState;
    //controlStates nextState;

    [Header("Object References")]
    [Tooltip("Arms: used in look rotation")]
    [SerializeField]
    GameObject arm;

    GameObject vaultObj;

    [Header("Variables")]
    //public float jump;
    [Tooltip("Base movement speed")]
    public float baseSpeed;
    float moveSpeed;
    float modSpeed = 1f;
    [Tooltip("Rate of rotation")]
    public float rotSpeed;
    [Tooltip("Force added when you jump")]
    public float jumpForce = 350f;
    [Tooltip("How long player sprints")]
    public float sprintTimer;
    [Tooltip("Time before next sprint")]
    public float sprintCDTimer;

    bool sprintCD;
    [SerializeField]
    Animation animator;

    private void Awake()
    {
        GameStates = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameStates>();
        uIController = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIController>();
        gunScript = GetComponent<GunScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        baseSpeed = 2f;
        moveSpeed = baseSpeed;
        rotSpeed = 1f;
        jumpForce = 250f;

        //Debug.Log("up orientation is " + arm.transform.up.x);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Player transform.forward is " + transform.forward);
        switch (controlState) {
            case (controlStates.Norm):
                mouseRotation();
                doJump();
                sprint();
                gunScript.switchGrenade();
                gunScript.ADS(controlState.ToString());
                gunScript.shoot();
                gunScript.Reload();
                gunScript.weaponSwap();
                //Debug.Log(transform.forward.z);
                break;

            case (controlStates.Air):
                mouseRotation();
                gunScript.switchGrenade();
                gunScript.ADS(controlState.ToString());
                gunScript.shoot();
                gunScript.Reload();
                gunScript.weaponSwap();
                break;

            case (controlStates.Stunned):
                gunScript.dropGrenade();
                break;

            case (controlStates.Dead):
                break;

            case (controlStates.DeusVault):
                break;

            default:
                Debug.Log("control state not found");
                break;
        }
    }
    void FixedUpdate()
    {
        switch (controlState)
        {
            case (controlStates.Norm):
                playerMovement();
                break;

            case (controlStates.Air):
                playerMovement();
                break;

            case (controlStates.Dead):
                break;

            case (controlStates.Stunned):
                break;

            default:
                Debug.Log("control state not found");
                break;
        }
        
    }

    void playerMovement()
    {
        //Movement
        if (Input.GetButton("Vertical") && Input.GetAxis("Vertical") > 0)
        {
            this.gameObject.transform.position += moveSpeed * modSpeed * Time.deltaTime* transform.forward;
        }
        else if (Input.GetButton("Vertical") && Input.GetAxis("Vertical") < 0)
        {
            this.gameObject.transform.position += moveSpeed * modSpeed * Time.deltaTime * -transform.forward;
        }
        if (Input.GetButton("Horizontal") && Input.GetAxis("Horizontal") < 0)
        {
            this.gameObject.transform.position += moveSpeed * modSpeed * Time.deltaTime * -transform.right;
        }
        else if (Input.GetButton("Horizontal") && Input.GetAxis("Horizontal") > 0)
        {
            this.gameObject.transform.position += moveSpeed * modSpeed * Time.deltaTime * transform.right;
        }
    }

    void sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !sprintCD)
        {
            Debug.Log("Start Sprint");
            moveSpeed = baseSpeed * 2.5f * modSpeed;
            StartCoroutine(_sprint());
        }
    }

    IEnumerator _sprint()
    {

        uIController.resetSprint();
        yield return new WaitForSeconds(sprintTimer);

        sprintCD = true;
        uIController.sprintCD = sprintCD;
        moveSpeed = baseSpeed * modSpeed;
        Debug.Log("End Sprint, Sprint CD Start");
        

        yield return new WaitForSeconds(sprintCDTimer);
        Debug.Log("End Sprint CD");
        sprintCD = false;
        uIController.sprintCD = sprintCD;
        uIController.setSliders();
    }

    IEnumerator _stunned()
    {

        yield return new WaitForSeconds(5f);
        controlState = controlStates.Norm;
    }

    void mouseRotation()
    {
        float mouseX = rotSpeed * Input.GetAxis("Mouse X");
        float mouseY = rotSpeed * Input.GetAxis("Mouse Y");

        transform.Rotate(new Vector3(0, mouseX));

        //Debug.Log(arm.transform.localEulerAngles.x);

        //check the upwards roation of camera
        //using localEulerAngles to radians
        if (Mathf.Deg2Rad * arm.transform.localEulerAngles.x < 1.134f || Mathf.Deg2Rad * arm.transform.localEulerAngles.x > 5.6f)
        {
                arm.transform.Rotate(new Vector3(-mouseY, 0, 0));
        }
        else if (5.5f < Mathf.Deg2Rad * arm.transform.localEulerAngles.x && Mathf.Deg2Rad * arm.transform.localEulerAngles.x < 5.6f)
        {
            if (mouseY < 0)
                arm.transform.Rotate(new Vector3(-mouseY, 0, 0));
        }
        else if (1.2 > Mathf.Deg2Rad * arm.transform.localEulerAngles.x && Mathf.Deg2Rad * arm.transform.localEulerAngles.x > 1.134f)
        {
            if (mouseY > 0)
                arm.transform.Rotate(new Vector3(-mouseY, 0, 0));
        }
    }

    void doJump()
    {
        //Debug.Log("Checking vars for jump");
        if (Input.GetButtonDown("Jump") && vaultObj != null)
        {
           Debug.Log(Mathf.Abs(vaultObj.transform.forward.z - transform.forward.z));
           float lookDir = Mathf.Abs(vaultObj.transform.forward.z) - Mathf.Abs(transform.forward.z);
           if (0 <= lookDir && lookDir <= 0.2f)
           {
               StartCoroutine(_vault());
               controlState = controlStates.DeusVault;
           }
           else
           {
               GetComponent<Rigidbody>().velocity += (Vector3.up * jumpForce * Time.fixedDeltaTime);
               moveSpeed = baseSpeed / 2f * modSpeed;
               controlState = controlStates.Air;
           }
        }
        else if (Input.GetButtonDown("Jump"))
        {
            //Debug.Log("Vars okay, Doing Jump");
            //GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
            GetComponent<Rigidbody>().velocity += (Vector3.up * jumpForce * Time.fixedDeltaTime);
            moveSpeed = baseSpeed / 2f * modSpeed;
            controlState = controlStates.Air;
        }

    }

    IEnumerator _vault()
    {
        animator.Play("Vault");
        //needs to use probuilder object size z
        float vaultDistance = vaultObj.transform.localScale.z + 1;
        //nneds to use probuilder object size y
        float height;

        //set the velocity of our grenade = to its current rotation * (trajectory + our player's velocity)
        //allows us to increase or decrease our velocity if we are moving forwards or backwards
        GetComponent<Rigidbody>().velocity = transform.forward * 1;  //Mathf.Tan(height/(vaultDistance/2));

        yield return new WaitForSeconds(1);
        controlState = controlStates.Norm;
    }

    //void setState()
    //{
    //    switch (controlState)
    //    {
    //        case controlStates.DeusVault:
    //            controlState = controlStates.Norm;
    //            break;
    //
    //        default:
    //            Debug.Log("no animation state found");
    //            break;
    //    }
    //}

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            if(controlState == controlStates.Stunned)
                StopCoroutine(_stunned());
            moveSpeed = baseSpeed;
            controlState = controlStates.Norm;
            
        }
        else if (collision.gameObject.CompareTag("Climbable"))
        {

        }
        else if (collision.gameObject.CompareTag("Vault"))
        {
            vaultObj = collision.gameObject;
            //Debug.Log(vaultObj);
            //Debug.Log("Vault Obj forward: " + vaultObj.transform.forward.z);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Climbable"))
        {

        }
        else if (collision.gameObject.CompareTag("Vault"))
        {
            vaultObj = null;
            Debug.Log(vaultObj);
        }
    
    }

    //state ref for other classes
    public string playerState()
    {
        return controlState.ToString();
    }
    public void setControlerState(string state)
    {
        switch (state)
        {
            case ("Norm"):
                controlState = controlStates.Norm;
                break;

            case ("Stunned"):
                controlState = controlStates.Stunned;
                gunScript.MainCam.SetActive(true);
                gunScript.ADSCam.SetActive(false);
                StartCoroutine(_stunned());
                break;

            case ("Dead"):
                controlState = controlStates.Dead;
                break;

            default:
                break;
        }
    }

    public void changeSpeed(bool slowed)
    {
        Debug.Log("Doing the thing");
        if (slowed)
        {
            modSpeed = 0.5f;
        }
        else
            modSpeed = 1;
    }
}
