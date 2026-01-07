using System.Collections;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

[System.Serializable]
public class CarInteractables : MonoBehaviour
{
    #region [Properties] CarInteractables
    [Header("System")]
    public PlayerInteraction playerInteraction;
    public CarAssembly carAssembly;

    [Header("DoorCar")]
    public Animator doorAnimator;
    string openbool = "isOpen";
    [HideInInspector]public bool isOpen;
    public AudioSource doorAudio;
    public AudioClip doorAudioOpen;
    public AudioClip doorAudioClose;
    public MeshCollider doorCollider;

    [Header("HoodCar")]
    bool isHoodOpen;
    bool isHoodUnLocked;
    public Animator hoodAnimator;
    public AudioSource hoodAudio;
    public AudioClip hoodAudioOpen;
    public AudioClip hoodAudioClose;
    public AudioClip hoodAudioUnlocked;

    [Header("TrunkCar")]
    bool isTrunkOpen;
    public Animator trunkAnimator;
    public AudioSource trunkAudio;
    public AudioClip trunkAudioOpen;
    public AudioClip trunkAudioClose;
    public StorageObject trunkStorageObject;

    [Header("Sit")]
    [SerializeField] private Transform carCamera;    
    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private GameObject PlayerTransform;
    [SerializeField] private GameObject Car;
    [SerializeField] private Transform PlayerCameraPosition;
    [SerializeField] private Transform exitPoint;


    public bool isSeated = false;


    [SerializeField] private float transitionTime = 1.5f;


    #region [Properties]Ignition
    [Header("Ignition System")]
    public Interactable ignitionInteractable;
    public Transform ignitionKeyObject; // objekt kľúča
    public Vector3 keyDefaultRotation;  // uložená rotácia v inspector
    public Vector3 keyStartRotation;    // rotácia pri štartovaní (napr. o +75° na osi Y)
    public AudioSource ignitionAudioSource;
    public AudioClip keyInsertClip;
    public AudioClip keyTurnClip;
    public AudioClip starterLoopClip;
    public AudioClip engineStartClip;
    public AudioClip engineShutdownClip;
    public AudioSource engineIdleSound; 
    public AudioSource engineReverseSound;

    private bool keyVisible = false;
    private bool isStarting = false;
    private Coroutine startingRoutine;
    public bool engineRunning = false;
    public ParticleSystem exhaustParticle;

    #endregion

    [Header("ExteriorLights")]
    public Material lightMaterial;
    public Light firstLight;
    public Light secondLight;
    private bool isLight;
    [Header("InteriorLights")]
    public Material interiorLightMaterial;
    public Light interiorLight;
    private bool isInteriorLight;

    [Header("Fuel")]
    public Interactable fuelInteractable;
    public float fuelLevel;
    [Range(0.05f, 0.1f)]
    public float fillingSpeed;
    public float defillingSpeed;
    public Image progressBar;
    public Image progressBarBackground;
    public AudioSource fuelFilling;
    public FuelPumpHandler fuelPumpHandler;
    public FuelPumpHandler fuelPumpHandler2;

    [Header("Radio")]
    public RadioManager radioManager;
    private bool isBroadcasting;
    private bool isPlayingRadio;
    public AudioSource radioAudioSource;
    #endregion
    #region [Properties] PlayerStats
    public PlayerStatsSystem playerStatsSystem;
    #endregion
    #region Start/Update
    public void Start()
    {
        progressBarBackground.gameObject.SetActive(false);
    }
    public void Update()
    {
        if (!fuelInteractable.isHolding)
        {
            progressBarBackground.gameObject.SetActive(false);
            fuelFilling.Stop();
        }
        if (!isSeated && isBroadcasting)
        {

            radioManager.StopRadio();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Ignition();
        }
        if (!isSeated)
        {
            HideKey();
        }
    }
    #endregion
    #region DoorInteract
    public void DoorInteract()
    {
        if(!isOpen && !isHoodUnLocked)
        {
            doorAnimator.SetBool(openbool, true);
            isOpen = true;
            doorAudio.clip = doorAudioOpen;
            doorAudio.Play();
        }
        else
        {
            doorAnimator.SetBool(openbool, false);
            isOpen = false;
            doorAudio.clip = doorAudioClose;
            doorAudio.Play();
        }
    }
    #endregion
    #region HoodInteract

    public void HoodUnlock()
    {
        if (!isHoodOpen && !isHoodUnLocked)
        {
            hoodAnimator.SetBool("isUnlocked", true);
            isHoodUnLocked = true;
            hoodAudio.PlayOneShot(hoodAudioUnlocked);
        }
    }
    public void HoodInteract()
    {
        if (!isHoodOpen && isHoodUnLocked)
        {
            hoodAnimator.SetBool("isOpen", true);
            isHoodOpen = true;
            hoodAudio.PlayOneShot(hoodAudioOpen);
        }
        else if (isHoodOpen && isHoodUnLocked)
        {
            hoodAnimator.SetBool("isUnlocked", false);
            isHoodUnLocked = false; 
            hoodAnimator.SetBool("isOpen", false);
            isHoodOpen = false;
            hoodAudio.PlayOneShot(hoodAudioClose);
        }
        else if (!isHoodUnLocked && !isHoodOpen)
        {
            playerInteraction.errorMessage.text = "CAN´T OPEN HOOD";
        }
    }
    #endregion
    #region TrunkInteract
    public void TrunkInteract()
    {
        if (!isTrunkOpen)
        {
            trunkAnimator.SetBool(openbool, true);
            trunkStorageObject.OpenStorage();
            isTrunkOpen = true;
            trunkAudio.clip = trunkAudioOpen;
            trunkAudio.Play();
        }
        else
        {
            trunkAnimator.SetBool(openbool, false);
            isTrunkOpen = false;
            trunkAudio.clip = trunkAudioClose;
            trunkAudio.Play();
        }
    }
    #endregion
    #region SitToCar
    public void SitToCar()
    {
        if (!isSeated)
        {
            // ======================
            // SADNUTIE DO AUTA
            // ======================

            // skry hráča
            PlayerTransform.SetActive(false);

            // camera prepnutie
            PlayerCamera.gameObject.SetActive(false);
            carCamera.gameObject.SetActive(true);

            // zarovnanie car kamery (žiadny tilt z minulosti)
            carCamera.rotation = GetUprightRotation(carCamera.rotation);

            // parent na auto (až PO rotácii)
            carCamera.SetParent(this.transform, true);

            isSeated = true;
        }
        else if (isSeated && isOpen)
        {
            // camera prepnutie
            carCamera.gameObject.SetActive(false);
            PlayerCamera.gameObject.SetActive(true);

            // reset parentingu
            PlayerTransform.transform.SetParent(null);

            // bezpečná pozícia a rotácia
            Vector3 safeExitPos = exitPoint.position + Vector3.up;
            Quaternion safeExitRot = GetUprightRotation(exitPoint.rotation);

            PlayerTransform.transform.position = safeExitPos;
            PlayerTransform.transform.rotation = safeExitRot;

            PlayerCamera.SetParent(PlayerCameraPosition, false);
            PlayerCamera.localPosition = Vector3.zero;
            PlayerCamera.localRotation = Quaternion.identity;

            PlayerTransform.SetActive(true);

            isSeated = false;
        }
    }
    private Quaternion GetUprightRotation(Quaternion source)
    {
        Vector3 euler = source.eulerAngles;
        return Quaternion.Euler(0f, euler.y, 0f);
    }
    #endregion
    #region Ignition
    public void Ignition()
    {
        if (carAssembly.IsPartInstalled(ItemType.Battery))
        {
            if (!isSeated)
            {
                HideKey();
                return;
            }

        if (engineRunning)
        {
            EngineTurnOff();
            return;
        }

        // ak už štartuje → nerob nič
        if (isStarting)
            return;

        // ak kľúč ešte nebol vložený → vložiť
        if (!keyVisible)
        {
            ShowKey();
            return;
        }

        // ak drží interactable → začať štartovanie
        if (ignitionInteractable.isHolding && !isStarting)
        {
            startingRoutine = StartCoroutine(StartEngineHold());
        }
       else if (!ignitionInteractable.isHolding && isStarting)
        {
            // pustil predčasne → stop
            StopCoroutine(startingRoutine);
            ResetKey();
        }
        }

    }

    private void ShowKey()
    {
        keyVisible = true;
        ignitionKeyObject.gameObject.SetActive(true);
        ignitionKeyObject.localEulerAngles = keyDefaultRotation;
        ignitionInteractable.canHold = true;

        if (keyInsertClip) ignitionAudioSource.PlayOneShot(keyInsertClip);
    }

    private void HideKey()
    {
        keyVisible = false;
        ignitionKeyObject.gameObject.SetActive(false);
        ignitionInteractable.canHold = false;
    }

    private IEnumerator StartEngineHold()
    {
        isStarting = true;

        if (keyTurnClip) ignitionAudioSource.PlayOneShot(keyTurnClip);

        // otočenie kľúča
        Quaternion startRot = Quaternion.Euler(keyDefaultRotation);
        Quaternion endRot = Quaternion.Euler(keyStartRotation);
        float rotTime = 0.2f;
        float t = 0f;
        while (t < rotTime)
        {
            t += Time.deltaTime;
            ignitionKeyObject.localRotation = Quaternion.Slerp(startRot, endRot, t / rotTime);
            yield return null;
        }

        // prehrávanie loop zvuku
        ignitionAudioSource.loop = true;
        ignitionAudioSource.clip = starterLoopClip;
        ignitionAudioSource.Play();

        // náhodná doba držania
        float holdTime = Random.Range(1.5f, 3f);
        float elapsed = 0f;

        while (ignitionInteractable.isHolding && elapsed < holdTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        ignitionAudioSource.Stop();
        ignitionAudioSource.loop = false;

        if (elapsed >= holdTime)
        {
            // motor naštartoval
            ignitionInteractable.canHold = false;
            engineRunning = true;
            if (engineStartClip) ignitionAudioSource.PlayOneShot(engineStartClip);

            exhaustParticle.Play();
            engineIdleSound.Play();
        }

        // kľúč späť
        ResetKey();
        isStarting = false;
    }

    public void EngineTurnOff()
    {
        if (!engineRunning) return;

        exhaustParticle.Stop();
        engineIdleSound.Stop();
        if (engineReverseSound.isPlaying) engineReverseSound.Stop();

        if (engineShutdownClip) ignitionAudioSource.PlayOneShot(engineShutdownClip);

        engineRunning = false;
        ResetKey();
        HideKey();


    }

    private void ResetKey()
    {
        ignitionKeyObject.localEulerAngles = keyDefaultRotation;
        ignitionAudioSource.Stop();
        ignitionAudioSource.loop = false;
        isStarting = false;
    }
    #endregion
    #region Lights
    public void Lights ()
    {
        if(!isLight)
        {
            lightMaterial.EnableKeyword("_EMISSION");
            firstLight.enabled = true;
            secondLight.enabled = true;
            isLight = true;
        }
        else
        {
            lightMaterial.DisableKeyword("_EMISSION");
            firstLight.enabled = false;
            secondLight.enabled = false;
            isLight = false;
        }

    }
    #endregion
    #region InteriorLights
    public void InteriorLights()
    {
        if (!isInteriorLight)
        {
            interiorLightMaterial.EnableKeyword("_EMISSION");
            interiorLight.enabled = true;
            isInteriorLight = true;
        }
        else
        {
            interiorLightMaterial.DisableKeyword("_EMISSION");
            interiorLight.enabled = false;
            isInteriorLight = false;
        }

    }
    #endregion
    #region Fuel
    public void Filling()
    {
        if (fuelPumpHandler.isHoldingNozzle || fuelPumpHandler2.isHoldingNozzle && playerStatsSystem.currentMoney > 0)
        {
            if (fuelLevel < 100)
            {
                if (!fuelFilling.isPlaying)
                {
                    fuelFilling.Play();
                }
                fuelLevel += fillingSpeed;
                playerStatsSystem.SubtractMoney(0.5f);
                progressBarBackground.gameObject.SetActive(true);
                progressBar.fillAmount = fuelLevel / 100;
            }
            else { fuelLevel = 100; }
        }
        else
        {
            playerInteraction.errorMessage.text = "CAN´T REFUEL";
        }


    }
    public void DeFuelling()
    {
     if (fuelLevel > 0.000001f)
     {
      fuelLevel -= defillingSpeed;
     }
     else
     {
      EngineTurnOff();
      fuelLevel = 0;
     }
    }
    #endregion
    #region LiveRadio
    public void LiveRadioTurn()
    {
        if (!isBroadcasting && isSeated)
        {
            StartCoroutine(radioManager.PlayRadio(radioManager.icecastUrls[0]));
            isBroadcasting = true;
        }
        else
        {
            radioManager.StopRadio();
            isBroadcasting = false; ;
        }
    }
    #endregion
    public void RadioTurn()
    {
        if(!isPlayingRadio)
        {
            isPlayingRadio = true;
            radioAudioSource.Play();
        }
        else
        {
            radioAudioSource.Pause();
            isPlayingRadio = false;
        }
    }
}
