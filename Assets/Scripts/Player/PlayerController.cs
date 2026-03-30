using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("PlayerController")]
        [SerializeField] public Transform Camera;
        [SerializeField, Range(1, 10)] float walkingSpeed = 3.0f;
        [Range(0.1f, 5)] public float CroughSpeed = 1.0f;
        [SerializeField, Range(2, 20)] float RuningSpeed = 4.0f;
        [SerializeField, Range(0, 20)] float jumpSpeed = 6.0f;
        [SerializeField, Range(0.5f, 10)] float lookSpeed = 2.0f;
        [SerializeField, Range(10, 120)] float lookXLimit = 80.0f;
        [Space(20)]
        [Header("Advance")]
        [SerializeField] float RunningFOV = 65.0f;
        [SerializeField] float SpeedToFOV = 4.0f;
        [SerializeField] float CroughHeight = 1.0f;
        [SerializeField] float gravity = 20.0f;
        [SerializeField] float timeToRunning = 2.0f;
        [HideInInspector] public static bool canMove = true;
        [HideInInspector] public bool CanRunning = true;
        [HideInInspector] public bool CanCrough = true;


    [Space(20)]
        [Header("Audio")]
        [SerializeField] float TransitionVolume = 4f;
        [SerializeField] public AudioSource Audio;
        [SerializeField] AudioClip WalkingAudio;
        [SerializeField] AudioClip RunningAudio;
        [SerializeField] AudioClip CroughWalkAudio;

        [Space(20)]
        [Header("Input")]
        [SerializeField] KeyCode CroughKey = KeyCode.LeftControl;


        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Vector3 moveDirection = Vector3.zero;
        [HideInInspector] public bool isCrough = false;
        float InstallCroughHeight;
        float rotationX = 0;
        [HideInInspector] public bool isRunning = false;
        float InstallFOV;
        bool UltraJump = false;
        bool TrampolineJump = false;
        Vector3 TrampolineVector;
        float UltraJumpValue;
        float TrampolineJumpValue;
        Camera cam;
        [HideInInspector] public bool Moving;
        [HideInInspector] static public bool canMoveCamera = true;
        [HideInInspector] public float vertical;
        [HideInInspector] public float horizontal;
        [HideInInspector] public float Lookvertical;
        [HideInInspector] public float Lookhorizontal;
        [HideInInspector] public int MovementState = 0;
        float RunningValue;
        [HideInInspector] public float WalkingValue;


     [Header("Zoom Settings")]
     public float zoomFOV = 30f;
     public float defaultFOV = 60f;
     public float zoomSpeed = 10f;
     public float smoothZoomTime = 0.2f;
     private float targetZoom;
     private float zoomVelocity = 0f;
    void Start()
        {
            characterController = GetComponent<CharacterController>();
            cam = GetComponentInChildren<Camera>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            InstallCroughHeight = characterController.height;
            InstallFOV = cam.fieldOfView;
            RunningValue = RuningSpeed;
            WalkingValue = walkingSpeed;
        characterController.detectCollisions = false;
        canMoveCamera = true;
        canMove = true;
    }

        void Update()
        {
            HandleZoom();
            RaycastHit CroughCheck;

            if (!characterController.isGrounded && !UltraJump)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            isRunning = !isCrough ? CanRunning ? Input.GetKey(KeyCode.LeftShift) : false : false;
            vertical = canMove ? (isRunning ? RunningValue : WalkingValue) * Input.GetAxis("Vertical") : 0;
            horizontal = canMove ? (isRunning ? RunningValue : WalkingValue) * Input.GetAxis("Horizontal") : 0;
            if (isRunning) RunningValue = Mathf.Lerp(RunningValue, RuningSpeed, timeToRunning * Time.deltaTime);
            else RunningValue = WalkingValue;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * vertical) + (right * horizontal);
            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }
            if (UltraJump)
            {
                UltraJump = false;
                moveDirection.y = UltraJumpValue;
            }
            if (TrampolineJump)
            {
                TrampolineJump = false;
                moveDirection = TrampolineVector * TrampolineJumpValue;
            }
            if (TrampolineJump)
            {
                TrampolineJump = false;
                moveDirection = TrampolineVector * TrampolineJumpValue;
            }
            characterController.Move(moveDirection * Time.deltaTime);
            Moving = horizontal < 0 || vertical < 0 || horizontal > 0 || vertical > 0 ? true : false;
            if (Cursor.lockState == CursorLockMode.Locked && canMoveCamera)
            {
                Lookvertical = -Input.GetAxis("Mouse Y");
                Lookhorizontal = Input.GetAxis("Mouse X");
            }
            else
            {
                Lookvertical = Mathf.Lerp(Lookvertical, 0, 7 * Time.deltaTime);
                Lookhorizontal = Mathf.Lerp(Lookhorizontal, 0, 7 * Time.deltaTime);
            }
            rotationX += Lookvertical * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            Camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Lookhorizontal * lookSpeed, 0);

            if (isRunning && Moving) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, RunningFOV, SpeedToFOV * Time.deltaTime);
            else cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, InstallFOV, SpeedToFOV * Time.deltaTime);

            if (Input.GetKey(CroughKey) && CanCrough)
            {
                isCrough = true;
                float Height = Mathf.Lerp(characterController.height, CroughHeight, 5 * Time.deltaTime);
                characterController.height = Height;
                WalkingValue = Mathf.Lerp(WalkingValue, CroughSpeed, 6 * Time.deltaTime);

            }
            else if (!Physics.Raycast(GetComponentInChildren<Camera>().transform.position, transform.TransformDirection(Vector3.up), out CroughCheck, 0.8f, 1))
            {
                if (characterController.height != InstallCroughHeight)
                {
                    isCrough = false;
                    float Height = Mathf.Lerp(characterController.height, InstallCroughHeight, 6 * Time.deltaTime);
                    characterController.height = Height;
                    WalkingValue = Mathf.Lerp(WalkingValue, walkingSpeed, 4 * Time.deltaTime);
                }
            }
        }
        private void FixedUpdate()
        {
            if (Moving && characterController.isGrounded)
            {
                MovementState = 1;
                if (isCrough) MovementState = -1;
                if (isRunning) MovementState = 2;
            }
            else MovementState = 0;
            MovementAudio();
        }
    private void HandleZoom()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            targetZoom = zoomFOV;
        }
        else
        {
            targetZoom = defaultFOV;
        }

        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetZoom, ref zoomVelocity, smoothZoomTime);
    }

    private void MovementAudio()
    {
            if (MovementState == -1)
            {
                if (Audio.clip != CroughWalkAudio || !Audio.isPlaying)
                {
                    Audio.clip = CroughWalkAudio;
                    Audio.Play();
                    Audio.volume = 0;
                }
                Audio.volume = Mathf.Lerp(Audio.volume, 1, TransitionVolume * Time.deltaTime);
            }
            if (MovementState == 0)
            {
                Audio.Stop();
            }
            if (MovementState == 1)
            {
                if (Audio.clip != WalkingAudio || !Audio.isPlaying)
                {
                    Audio.clip = WalkingAudio;
                    Audio.Play();
                    Audio.volume = 0;
                }
                Audio.volume = Mathf.Lerp(Audio.volume, 1, TransitionVolume * Time.deltaTime);
            }
            if (MovementState == 2)
            {
                if (Audio.clip != RunningAudio || !Audio.isPlaying)
                {
                    Audio.clip = RunningAudio;
                    Audio.Play();
                    Audio.volume = 0;
                }
                Audio.volume = Mathf.Lerp(Audio.volume, 1, TransitionVolume * Time.deltaTime);
            }
     }
    public static void SwitchingCameraMovement()
    {
        if(!canMoveCamera)
        {
            canMoveCamera = true;
            canMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (canMoveCamera)
        {
            canMoveCamera = false;
            canMove = false;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true; 
        }
    }
}
