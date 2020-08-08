using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GunScript : MonoBehaviour
{
    GameStates GameStates;

    [Header("Text Output References")]
    [Tooltip("Ammo output reference")]
    [SerializeField]
    Text Ammo;
    [Tooltip("Gun output reference")]
    [SerializeField]
    Text GunType;

    [Header("Camera Settings")]
    public GameObject MainCam;
    public GameObject ADSCam;
    
    [Header("Grenade Settings")]
    [Tooltip("Grenade prefab")]
    public GameObject grenade;
    private GameObject _grenade;
    enum GrenadeType
    {
        exp,
        rev,
        slow
    }

    GrenadeType grenadeType;

    int max_grenade = 2;
    int [] grenade_count = new int[] {100, 0, 0};
    bool gSpawned;

    bool reloading;

    int cur_weaponType;

    int burstCount;
    float burstRate;
    //float burstTimer;

    //string pState;

    bool startTimer;

    enum GunState
    {
        Pistol,
        SMG,
        Rifle,
        Sniper,
        Shotty,
        Grenade
    }
    [Header("Gun Settings")]
    [Tooltip("Current gun equipped")]
    [SerializeField]
    GunState gunState;
    GunState lastGunState;
    [Tooltip("Bullet prefab")]
    public GameObject[] Bullet;
    [Tooltip("Spawn position for bullets & grenades")]
    public GameObject[] barrel;

    [Tooltip("Fire rate for each gun type")]
    public float[] fireRate;
    float fireTimer = 0f;
    [Tooltip("Maximum ammo count for each gun type")]
    public int[] max_ammo = new int[5];
    int[] cur_ammo = new int[5];

    enum FireType
    {
        SemiAuto,
        FullAuto,
        Burst,
        BoltAction,
        ShotGun
    }

    FireType fireType;

    enum CamState
    {
        ADS,
        Main
    }

    CamState camstate;

    private void Awake()
    {
        GameStates = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameStates>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //pState = GetComponent<PlayerController>().playerState();
        camstate = CamState.Main;
        for (int i = 0; i < cur_ammo.Length; i++)
        {
            cur_ammo[i] = max_ammo[i];
            Debug.Log("Cur_ammo is " + cur_ammo[i] + " / " + max_ammo[i]);
        }
        grenadeType = GrenadeType.exp;
        setWeapon();
        fireTimer = fireRate[cur_weaponType];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void shoot()
    {
        if (!reloading)
        {
            switch (fireType)
            {
                case (FireType.SemiAuto):
                    fireTimer += Time.deltaTime;
                    if (Input.GetButtonDown("Fire1"))
                    {
                        if (fireTimer >= fireRate[cur_weaponType] && cur_ammo[cur_weaponType] >= 1)
                        {
                            Instantiate(Bullet[cur_weaponType], barrel[cur_weaponType].transform.position, barrel[cur_weaponType].transform.rotation);
                            cur_ammo[cur_weaponType]--;

                            Ammo.text = cur_ammo[cur_weaponType].ToString() + " / " + max_ammo[cur_weaponType].ToString();
                            //Debug.Log(cur_ammo[cur_weaponType] + " / " + max_ammo[cur_weaponType]);
                            fireTimer = 0;
                        }
                    }
                    break;

                case (FireType.FullAuto):
                    fireTimer += Time.deltaTime;
                    if (Input.GetButton("Fire1"))
                    {
                        if (fireTimer >= fireRate[cur_weaponType] && cur_ammo[cur_weaponType] >= 1)
                        {
                            Instantiate(Bullet[cur_weaponType], barrel[cur_weaponType].transform.position, barrel[cur_weaponType].transform.rotation);
                            cur_ammo[cur_weaponType]--;

                            Ammo.text = cur_ammo[cur_weaponType].ToString() + " / " + max_ammo[cur_weaponType].ToString();
                            //Debug.Log(cur_ammo[cur_weaponType] + " / " + max_ammo[cur_weaponType]);
                            fireTimer = 0;
                        }
                    }
                    break;

                case (FireType.Burst):
                    fireTimer += Time.deltaTime;

                    if (Input.GetButtonDown("Fire1"))
                    {
                        if (fireTimer >= fireRate[cur_weaponType] && cur_ammo[cur_weaponType] >= 3)
                        {
                            StartCoroutine(BurstFire());
                            fireTimer = 0;
                        }
                    }
                    break;

                case (FireType.BoltAction):

                    if (cur_ammo[cur_weaponType] > max_ammo[cur_weaponType] - 1)
                    {
                        startTimer = true;
                    }
                    if (startTimer == true)
                    {
                        fireTimer += Time.deltaTime;
                    }

                    if (Input.GetButtonDown("Fire1"))
                    {
                        if (fireTimer >= fireRate[cur_weaponType] && cur_ammo[cur_weaponType] >= 1)
                        {
                            startTimer = false;
                            Instantiate(Bullet[cur_weaponType], barrel[cur_weaponType].transform.position, barrel[cur_weaponType].transform.rotation);
                            cur_ammo[cur_weaponType]--;

                            Ammo.text = cur_ammo[cur_weaponType].ToString() + " / " + max_ammo[cur_weaponType].ToString();
                            //Debug.Log(cur_ammo[cur_weaponType] + " / " + max_ammo[cur_weaponType]);
                            fireTimer = 0;
                        }
                    }
                    else if (camstate == CamState.Main)
                    {
                        startTimer = true;
                    }
                    break;

                case (FireType.ShotGun):
                    fireTimer += Time.deltaTime;

                    if (Input.GetButtonDown("Fire1"))
                    {
                        if (fireTimer >= fireRate[cur_weaponType] && cur_ammo[cur_weaponType] >= 1)
                        {
                            for (int bulletsFired = 0; bulletsFired < 20; bulletsFired++)
                            {
                                float spreadAngle;

                                if (camstate == CamState.ADS)
                                    spreadAngle = 7.5f;
                                else
                                    spreadAngle = 15f;

                                /* use for instatiating gameobj
                                GameObject Bullets = Instantiate(Bullet[cur_weaponType], barrel[cur_weaponType].transform.position, barrel[cur_weaponType].transform.rotation);
                                Bullets.transform.rotation = Quaternion.RotateTowards(Bullets.transform.rotation, Random.rotation, spreadAngle);
                                */


                                RaycastHit spray;
                                //rotate the raycast towards the target location, then cast, and draw
                                Quaternion roatationAngle = Quaternion.RotateTowards(barrel[cur_weaponType].transform.rotation, Random.rotation, spreadAngle);

                                if (Physics.Raycast(barrel[cur_weaponType].transform.position, roatationAngle * Vector3.forward, out spray, 12f))
                                {
                                    Debug.Log(spray);
                                    if (spray.collider.gameObject.tag == "Enemy")
                                    {
                                        spray.collider.gameObject.GetComponent<Health>().subHP(2);
                                        spray.collider.gameObject.GetComponent<AIController>().dirForce(-gameObject.transform.forward);
                                    }
                                    else if (spray.collider.gameObject.gameObject.tag == "DesObj")
                                    {
                                        spray.collider.gameObject.gameObject.GetComponent<Health>().subHP(2);
                                    }
                                }
                                Debug.DrawRay(barrel[cur_weaponType].transform.position, roatationAngle * Vector3.forward * 12f, Color.cyan, 1f);

                            }

                            cur_ammo[cur_weaponType]--;

                            Ammo.text = cur_ammo[cur_weaponType].ToString() + " / " + max_ammo[cur_weaponType].ToString();
                            //Debug.Log(cur_ammo[cur_weaponType] + " / " + max_ammo[cur_weaponType]);

                            fireTimer = 0;
                        }
                    }
                    break;

                default:
                    Debug.Log("Unknown fireType");
                    break;
            }
        }

    }

    void grenadeCheck()
    {
        if (!gSpawned)
        {
            switch (grenadeType)
            {
                case (GrenadeType.exp):
                    if (grenade_count[0] > 0)
                    {
                        grenade_count[0]--;
                        _grenade = Instantiate(grenade, barrel[5].transform);
                        gSpawned = true;
                    }
                    break;
                case (GrenadeType.rev):
                    if (grenade_count[1] > 0)
                    {
                        grenade_count[1]--;
                        _grenade = Instantiate(grenade, barrel[5].transform);
                        gSpawned = true;
                    }
                    break;
                case (GrenadeType.slow):
                    if (grenade_count[2] > 0)
                    {
                        grenade_count[2]--;
                        _grenade = Instantiate(grenade, barrel[5].transform);
                        gSpawned = true;
                    }
                    break;
                default:
                    Debug.Log("grenadeType out of enum range, GunScript: ln 126");
                    break;
            }
        }
    }

    public void doThrow()
    {
        if (_grenade != null)
        {
            _grenade.GetComponent<Grenade>().ThrowGrenade(30, grenadeType.ToString());
            _grenade = null;
            gSpawned = false;
        }

    }

   public void dropGrenade()
   {
       if(gunState == GunState.Grenade)
        {
            gunState = lastGunState;
            Destroy(_grenade);
            _grenade = null;
            gSpawned = false;
            setWeapon();
        }
   }

    public void switchGrenade()
    {
        if (!gSpawned)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (grenadeType == GrenadeType.exp)
                {
                    grenadeType = GrenadeType.rev;
                }
                else if (grenadeType == GrenadeType.rev)
                {
                    grenadeType = GrenadeType.slow;
                }
                else if (grenadeType == GrenadeType.slow)
                {
                    grenadeType = GrenadeType.exp;
                }
            }
        }
    }

    IEnumerator BurstFire()
    {
        for (int bulletsFired = 0; bulletsFired < burstCount; bulletsFired++)
        {
            Instantiate(Bullet[cur_weaponType], barrel[cur_weaponType].transform.position, barrel[cur_weaponType].transform.rotation);
            cur_ammo[cur_weaponType]--;

            Ammo.text = cur_ammo[cur_weaponType].ToString() + " / " + max_ammo[cur_weaponType].ToString();
            //Debug.Log(cur_ammo[cur_weaponType] + " / " + max_ammo[cur_weaponType]);
            yield return new WaitForSeconds(burstRate);
        }
    }

    public void Reload()
    {
        if (Input.GetButtonDown("Reload") && !reloading)
        {
            if (cur_ammo[cur_weaponType] != max_ammo[cur_weaponType])
            {
                reloading = true;
                GameObject.FindGameObjectWithTag("GameController").GetComponent<UIController>().reloading(reloading);
                Debug.Log("Reloading");
                StartCoroutine(_reload());
            }
        }
    }

    IEnumerator _reload()
    {
        yield return new WaitForSeconds(2f);
        cur_ammo[cur_weaponType] = max_ammo[cur_weaponType];
        Ammo.text = cur_ammo[cur_weaponType].ToString() + " / " + max_ammo[cur_weaponType].ToString();
        reloading = false;
        GameObject.FindGameObjectWithTag("GameController").GetComponent<UIController>().reloading(reloading);
    }

    void setWeapon()
    {
        switch (gunState)
        {
            case (GunState.Pistol):
                Debug.Log("EQUIPING PISTOL");
                GunType.text = "Pistol";
                cur_weaponType = 0;
                fireType = FireType.SemiAuto;
                break;

            case (GunState.SMG):
                Debug.Log("EQUIPING SMG");
                GunType.text = "SMG";
                cur_weaponType = 1;
                fireType = FireType.FullAuto;
                break;

            case (GunState.Rifle):
                Debug.Log("EQUIPING Rifle");
                GunType.text = "Rifle";
                cur_weaponType = 2;
                fireType = FireType.Burst;
                burstRate = 0.02f;
                burstCount = 3;
                break;

            case (GunState.Sniper):
                Debug.Log("EQUIPING Sniper");
                GunType.text = "Sniper";
                cur_weaponType = 3;
                fireType = FireType.BoltAction;
                break;

            case (GunState.Shotty):
                Debug.Log("EQUIPING Shotgun");
                GunType.text = "Shotgun";
                cur_weaponType = 4;
                fireType = FireType.ShotGun;
                break;

            case (GunState.Grenade):
                Debug.Log("EQUIPING Grenade");
                grenadeCheck();
                break;

            default:
                Debug.Log("Unknown GunState");
                GunType.text = "Unknown Gun";
                break;

        }

        if (lastGunState == GunState.Grenade)
        {
            if (gSpawned)
            {
                switch (grenadeType)
                {
                    case (GrenadeType.exp):
                        grenade_count[0]++;
                        break;
                    case (GrenadeType.rev):
                        grenade_count[1]++;
                        break;
                    case (GrenadeType.slow):
                        grenade_count[2]++;
                        break;
                    default:
                        Debug.Log("grenadeType out of enum range, GunScript: ln 126");
                        break;
                }

                Destroy(_grenade);
                _grenade = null;
                gSpawned = false;
            }
        }
       
        Ammo.text = cur_ammo[cur_weaponType].ToString() + " / " + max_ammo[cur_weaponType].ToString();
    }

    public void weaponSwap()
    {
        if (!reloading)
        {
            if (camstate == CamState.Main && fireTimer > 0)
            {
                if (gunState != GunState.Pistol && Input.GetKeyDown(KeyCode.Alpha1))
                {
                    lastGunState = gunState;
                    gunState = GunState.Pistol;

                    setWeapon();
                }
                else if (gunState != GunState.SMG && Input.GetKeyDown(KeyCode.Alpha2))
                {
                    lastGunState = gunState;
                    gunState = GunState.SMG;

                    setWeapon();
                }
                else if (gunState != GunState.Rifle && Input.GetKeyDown(KeyCode.Alpha3))
                {
                    lastGunState = gunState;
                    gunState = GunState.Rifle;

                    setWeapon();
                }
                else if (gunState != GunState.Sniper && Input.GetKeyDown(KeyCode.Alpha4))
                {
                    lastGunState = gunState;
                    gunState = GunState.Sniper;

                    setWeapon();
                }
                else if (gunState != GunState.Shotty && Input.GetKeyDown(KeyCode.Alpha5))
                {
                    lastGunState = gunState;
                    gunState = GunState.Shotty;

                    setWeapon();
                }
                else if (gunState != GunState.Grenade && Input.GetButtonDown("Fire3"))
                {
                    lastGunState = gunState;
                    gunState = GunState.Grenade;

                    setWeapon();
                }
                else if (gunState == GunState.Grenade && Input.GetButtonUp("Fire3"))
                {
                    doThrow();
                    gunState = lastGunState;
                    setWeapon();
                }
            }
        }
    }

    public void ADS(string state)
    {
        switch (camstate)
        {
            case (CamState.Main):
                if (GameStates.holdADS == false)
                {
                    if (Input.GetButtonDown("Fire2"))
                    {
                        MainCam.SetActive(false);
                        ADSCam.SetActive(true);
                        camstate = CamState.ADS;
                    }
                }
                else if (GameStates.holdADS == true)
                {
                    if (Input.GetButtonDown("Fire2"))
                    {
                        MainCam.SetActive(false);
                        ADSCam.SetActive(true);
                        camstate = CamState.ADS;
                    }
                }

                GameStates.SlowTime(false);

                break;

            case (CamState.ADS):
                if (Input.GetButtonDown("Fire2"))
                {
                    MainCam.SetActive(true);
                    ADSCam.SetActive(false);
                    camstate = CamState.Main;
                }
                else if (GameStates.holdADS == true && Input.GetButtonUp("Fire2"))
                {
                    MainCam.SetActive(true);
                    ADSCam.SetActive(false);
                    camstate = CamState.Main;

                }

                if (state == "Air")
                {
                    GameStates.SlowTime(true);
                }
                else
                {
                    GameStates.SlowTime(false);
                }
                break;

            default:
                Debug.Log("Unknown CamState");
                break;
        }
        
    }

    public int addCount(int count, string type)
    {
        int i;
        switch (type)
        {
            case ("exp"):
                i = 0;
                break;

            case ("slow"):
                i = 1;
                break;

            case ("rev"):
                i = 2;
                break;

            default:
                Debug.Log("No grenade type specified");
                return count;
        }

        if(grenade_count[i] < max_grenade)
        {
            int difference = max_grenade - grenade_count[i];
            if(count >= difference)
            {
                count = count - difference;
                grenade_count[i] = max_grenade;
            }
            else if(count < difference)
            {
                grenade_count[i] = grenade_count[i] + count;
                count = 0;
            }
        }

        Debug.Log("Grenade count is " + grenade_count[i]);
        return count;
    }
}


