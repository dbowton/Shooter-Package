using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using UnityEngine;
using HoaxGames;

[RequireComponent(typeof(CharacterController))]
public class PlayerManager : Character
{
    [SerializeField] ModelController gameController;
    [SerializeField] ModelController viewController;

    [SerializeField] FirstPersonCamera firstPersonCamera;
    [SerializeField] ThirdPersonCamera thirdPersonCamera;

    Timer coyoteTimer;

    [Serializable]
    public class PlayerData
    {
        public float enemyDetecRange;
        public FootIK footMagic;

        public float idleTime;
        public float coyoteTime;

        public Inventory inventory;
        [HideInInspector] public bool lockNeckRotation;
        [HideInInspector] public bool hitIdleMax;

        [HideInInspector] public bool ads;
        [HideInInspector] public bool fire;
        [HideInInspector] public bool crouching;
        [HideInInspector] public bool sprinting;

        [HideInInspector] public Vector2 look;
        [HideInInspector] public Vector3 rotation;
        [HideInInspector] public Vector2 neckRotation;
        [HideInInspector] public Vector3 movement;
        [HideInInspector] public Vector2 dir;

        public float jumpHeight = 10f;
        [HideInInspector] public float jumpForce;

        [Tooltip("3 - Spine, Spine1, Spine2")] public List<Transform> gameModelSpines;
        [Tooltip("3 - Spine, Spine1, Spine2")] public List<Transform> viewModelSpines;

        [Tooltip("x-forward | y-back | z-side")] public Vector3 speed;
        public float maxSpeed;

        [HideInInspector] public CharacterController characterController;
        [SerializeField] public Transform playerEye;

        public Vector2 lookSensitivity;
        [Tooltip("x-back | y-forward")] public Vector2 lookXLimits;
        [Tooltip("x-Left | y-right")] public float lookYLimit;
        [Tooltip("x-back | y-forward")] public Vector2 neckDeadZones;
        public Vector3 spineWeights;
    }

    public PlayerData playerData;

    public Vector3 Rotation { get { return playerData.rotation; } }
    
    Timer idleTimer;

    private void Awake()
    {
        playerData.spineWeights.Normalize();
    }

    private void Start() 
    {
        coyoteTimer = new Timer(playerData.coyoteTime);
        idleTimer = new Timer(playerData.idleTime);
        playerData.characterController = GetComponent<CharacterController>();
    }

    public void LateUpdate()
    {
        gameController?.SetOverrides();
        viewController?.SetOverrides();

        if (!active) return;

        #region rotation
        //  keeps head rotated straight when moving
        if (playerData.lockNeckRotation || firstPersonCamera.isActive)
        {
            playerData.rotation.y += playerData.neckRotation.y;
            playerData.neckRotation.y = 0;
        }

        Vector3 workingRotation;

        //  adjusts neck-rotation based on active weapon type
        if (playerData.inventory.activeWeapon && ((int)playerData.inventory.activeWeapon.weaponType != -1 || playerData.ads || playerData.fire))
        {
            playerData.rotation.y += playerData.neckRotation.y;
            playerData.neckRotation.y = 0;

            playerData.neckRotation.x = 0;

            //  clamps new x look limits
            if (playerData.rotation.x < -30)
                playerData.rotation.x = -30;
            else if (playerData.rotation.x > 65)
                playerData.rotation.x = 65;

            workingRotation = playerData.rotation;
        }
        else
        {
            workingRotation = playerData.rotation;
            workingRotation.x -= playerData.neckRotation.x;
        }

        //  x
        if (workingRotation.x < 0)
            for (int i = 0; i < 3; i++)
            {
                playerData.gameModelSpines[i].localEulerAngles = new Vector3(360 - Mathf.Abs(workingRotation.x * playerData.spineWeights[i]), 0, 0);
                playerData.viewModelSpines[i].localEulerAngles = new Vector3(360 - Mathf.Abs(workingRotation.x * playerData.spineWeights[i]), 0, 0);
            }
        else
            for (int i = 0; i < 3; i++)
            {
                playerData.gameModelSpines[i].localEulerAngles = new Vector3(workingRotation.x * playerData.spineWeights[i], 0, 0);
                playerData.viewModelSpines[i].localEulerAngles = new Vector3(workingRotation.x * playerData.spineWeights[i], 0, 0);
            }

        //  y
        gameController.animator.GetBoneTransform(HumanBodyBones.Head).localEulerAngles = playerData.neckRotation;
        viewController.animator.GetBoneTransform(HumanBodyBones.Head).localEulerAngles = playerData.neckRotation;
        transform.localEulerAngles = new Vector3(0, workingRotation.y, 0);
        #endregion

        #region movement
        playerData.movement *= ((playerData.sprinting) ? 2 : 1);

        if (playerData.movement.sqrMagnitude > playerData.maxSpeed * playerData.maxSpeed * ((playerData.sprinting) ? 4 : 1))
            playerData.movement = ((playerData.sprinting) ? 2 : 1) * playerData.maxSpeed * playerData.movement.normalized;

        playerData.movement.y = playerData.jumpForce + gravity;

        playerData.characterController.Move(playerData.movement * Time.deltaTime);

        #endregion

        #region animator

        if(playerData.inventory.activeWeapon)
        {
            gameController.animator.SetInteger("armed", (int)playerData.inventory.activeWeapon.weaponType);
            viewController.animator.SetInteger("armed", (int)playerData.inventory.activeWeapon.weaponType);
        }

        float speed = (new Vector2(playerData.movement.x, playerData.movement.z)).magnitude;
        viewController.animator.SetFloat("speed", speed);

        viewController.animator.SetBool("aiming", playerData.ads);
        viewController.animator.SetBool("fire", playerData.fire);
        viewController.animator.SetBool("idle", playerData.hitIdleMax);
        viewController.animator.SetBool("sprinting", playerData.sprinting);

        Vector2 tempDir = new Vector2(playerData.dir.x, playerData.dir.y);
        tempDir.Normalize();

        viewController.animator.SetFloat("dirx", tempDir.x);
        viewController.animator.SetFloat("diry", tempDir.y);

        gameController.animator.SetFloat("dirx", tempDir.x);
        gameController.animator.SetFloat("diry", tempDir.y);
        gameController.animator.SetFloat("speed", speed);

        gameController.animator.SetBool("aiming", playerData.ads);
        gameController.animator.SetBool("crouching", playerData.crouching);
        gameController.animator.SetBool("fire", playerData.fire);
        gameController.animator.SetBool("idle", playerData.hitIdleMax);
        gameController.animator.SetBool("sprinting", playerData.sprinting);

        float strafeOffset;
        if (playerData.dir.y > 0)
            strafeOffset = 50 * playerData.dir.x;
        else if (playerData.dir.y < 0)
            strafeOffset = -15 * playerData.dir.x;
        else
            strafeOffset = -50 * playerData.dir.x;

        if (!playerData.ads)
        {
            gameController.animator.GetBoneTransform(HumanBodyBones.Hips).localEulerAngles += Vector3.up * strafeOffset;
            gameController.animator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles -= Vector3.up * strafeOffset;

            viewController.animator.GetBoneTransform(HumanBodyBones.Hips).localEulerAngles += Vector3.up * strafeOffset;
            viewController.animator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles -= Vector3.up * strafeOffset;
        }

        if(playerData.fire && 
            (playerData.inventory.activeWeapon == null || playerData.inventory.activeWeapon.weaponType.Equals(Weapon.WeaponType.Unarmed)))
            playerData.fire = false;

        #endregion
    }

    public const float Gravity = -9.8f;
    float gravity = 0f;

    public void Update()
    {
        health.active = active;

        if (!active) return;

        float dt = Time.deltaTime;
        base.update(dt);

        if (IsGrounded())
        {
            gravity = 0f;
            playerData.jumpForce = 0;
        }
        else
            gravity += Gravity * dt;

        idleTimer.Update(dt);

        if(IsGrounded())
        {
            coyoteTimer.Reset();
        }
        else
            coyoteTimer.Update();

        playerData.hitIdleMax = idleTimer.IsOver;

        foreach(var c in Physics.OverlapSphere(transform.position, playerData.enemyDetecRange))
        {
            if(c.gameObject.TryGetComponent<Enemy>(out _))
            {
                playerData.hitIdleMax = false;
                break;
            }
        }

        #region rotation
        playerData.rotation.x += -playerData.look.y * playerData.lookSensitivity.y;
        playerData.neckRotation.y += playerData.look.x * playerData.lookSensitivity.x;

        //  clamps inputted rotations
        while (playerData.rotation.x >= 360) playerData.rotation.x -= 360;
        while (playerData.rotation.x < 0) playerData.rotation.x += 360;
        while (playerData.neckRotation.y >= 360) playerData.neckRotation.y -= 360;
        while (playerData.neckRotation.y <= -360) playerData.neckRotation.y += 360;

        //  clamps total x-rotation
        if (playerData.rotation.x < playerData.lookXLimits.x && playerData.rotation.x > playerData.lookXLimits.y)
            if (Mathf.Abs(playerData.rotation.x - playerData.lookXLimits.x) < Mathf.Abs(playerData.rotation.x - playerData.lookXLimits.y))
                playerData.rotation.x = playerData.lookXLimits.x;
            else
                playerData.rotation.x = playerData.lookXLimits.y;

        //  required for some reason
        if (playerData.rotation.x > 180)
            playerData.rotation.x -= 360;

        //  clamps neck-x rotation
        playerData.neckRotation.x = playerData.rotation.x;
        playerData.neckRotation.x = Mathf.Max(playerData.neckDeadZones.x, playerData.neckRotation.x);
        playerData.neckRotation.x = Mathf.Min(playerData.neckDeadZones.y, playerData.neckRotation.x);

        //  clamps then transfers neck-y rotation to spine rotation
        if (playerData.neckRotation.y > playerData.lookYLimit)
        {
            playerData.rotation.y += playerData.neckRotation.y - playerData.lookYLimit;
            playerData.neckRotation.y = playerData.lookYLimit;
        }
        else if (playerData.neckRotation.y < -playerData.lookYLimit)
        {
            playerData.rotation.y += playerData.neckRotation.y + playerData.lookYLimit;
            playerData.neckRotation.y -= playerData.neckRotation.y + playerData.lookYLimit;
        }
        #endregion
        #region movement
        playerData.movement = Vector3.zero;

        Vector3 forward = Vector3.zero;
        if (playerData.dir.y > 0)
            forward = playerData.dir.y * playerData.speed.x * ((playerData.lockNeckRotation) ? transform.forward : playerData.playerEye.forward);
        else if (playerData.dir.y < 0)
            forward = playerData.dir.y * playerData.speed.y * ((playerData.lockNeckRotation) ? transform.forward : playerData.playerEye.forward);

        Vector3 right = -playerData.dir.x * playerData.speed.z * ((playerData.lockNeckRotation) ? transform.right : playerData.playerEye.right);
        playerData.lockNeckRotation = false;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        forward *= Mathf.Abs(playerData.dir.y) * ((playerData.dir.y > 0) ? playerData.speed.x : playerData.speed.y);
        right *= Mathf.Abs(playerData.dir.x) * playerData.speed.z;

        playerData.movement += forward - right;

        if (playerData.dir.y < 0.7f)
            playerData.sprinting = false;

        //  keeps head rotated straight when moving
        if (playerData.movement.sqrMagnitude > 0)
            playerData.lockNeckRotation = true;

        if (playerData.ads) playerData.sprinting = false;
        if (playerData.crouching) playerData.sprinting = false;
        if (playerData.ads) playerData.lockNeckRotation = true;
        #endregion

        //  Manage Active Weapon
        playerData.inventory.activeWeapon?.update(playerData.fire);
    }

    public override void Die()
    {
        base.Die();
        print("Ha Loser");
    }

    #region Player Controls
    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        idleTimer.Reset();
        playerData.dir = callbackContext.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext callbackContext)
    {
        idleTimer.Reset();
        playerData.look = callbackContext.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext callbackContext)
    {
        idleTimer.Reset();
        if (callbackContext.performed)
            playerData.ads = true;
        else if (callbackContext.canceled)
            playerData.ads = false;
    }

    public void OnFire(InputAction.CallbackContext callbackContext)
    {
        idleTimer.Reset();

        if (callbackContext.performed)
        {
            playerData.fire = true;
            playerData.sprinting = false;
        }
        else if (callbackContext.canceled)
            playerData.fire = false;
    }

    public void OnReload(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.performed)
            if (playerData.inventory.activeWeapon is Gun) 
                (playerData.inventory.activeWeapon as Gun).Reload();
    }

    public void OnEquip(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.performed)
        {
            if(playerData.inventory.activeWeapon)
            {
                playerData.inventory.weapons.Add(playerData.inventory.activeWeapon);

                playerData.inventory.activeWeapon.gameObject.SetActive(false);
                
                playerData.inventory.activeWeapon.transform.parent = playerData.inventory.gameObject.transform;
            }

            if (playerData.inventory.weapons.Count == 0) return;

            playerData.inventory.activeWeapon = playerData.inventory.weapons[0];
            playerData.inventory.weapons.RemoveAt(0);

            playerData.inventory.activeWeapon.gameObject.SetActive(true);


            if(SmartCamera.activeCamera is FirstPersonCamera)
                playerData.inventory.activeWeapon.transform.parent = viewController.animator.GetBoneTransform(HumanBodyBones.RightHand);
            else
                playerData.inventory.activeWeapon.transform.parent = gameController.animator.GetBoneTransform(HumanBodyBones.RightHand);

            playerData.inventory.activeWeapon.transform.localPosition = playerData.inventory.activeWeapon.posOffset;
            playerData.inventory.activeWeapon.transform.localEulerAngles = playerData.inventory.activeWeapon.rotOffset;
            playerData.inventory.activeWeapon.owner = this;
        }
    }

    public void OnSprint(InputAction.CallbackContext callbackContext)
    {
        if (GameManager.Instance.input.currentControlScheme.Equals("MouseKeyboard"))
        {
            if (callbackContext.performed && playerData.dir.y > 0)
                playerData.sprinting = true;
            else if (callbackContext.canceled)
                playerData.sprinting = false;
        }
        else if (callbackContext.performed)
            playerData.sprinting = !playerData.sprinting;

        if (playerData.sprinting)
            playerData.crouching = false;
    }

    public void OnCrouch(InputAction.CallbackContext callbackContext)
    {
        if (GameManager.Instance.input.currentControlScheme.Equals("MouseKeyboard"))
        {
            if (callbackContext.performed)
                playerData.crouching = true;
            else if (callbackContext.canceled)
                playerData.crouching = false;
        }
        else if (callbackContext.performed)
            playerData.crouching = !playerData.crouching;

        if (playerData.crouching)
            playerData.sprinting = false;
    }


    [SerializeField] float footCastHeight = 0.5f;
    [SerializeField] float footHeight = 0.25f;

    public bool IsGrounded()
    {
        #region Left Foot

        Transform leftFoot = gameController.animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        
        if(Physics.Raycast(leftFoot.position, Vector3.down, out RaycastHit leftHitInfo, footCastHeight))
        {
            if(leftHitInfo.distance < footHeight)
            {
                print("grounded");
                return true;
            }
        }

        #endregion

        #region Right Foot

        Transform rightFoot = gameController.animator.GetBoneTransform(HumanBodyBones.RightFoot);

        if (Physics.Raycast(rightFoot.position, Vector3.down, out RaycastHit rightHitInfo, footCastHeight))
        {
            if (rightHitInfo.distance < footHeight)
            {
                print("grounded");
                return true;
            }
        }

        #endregion

        print("not grounded");
        return false;
    }

    public void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            if (!coyoteTimer.IsOver)
            {
                coyoteTimer.End();
                playerData.jumpForce = playerData.jumpHeight;
            }
        }
    }

    public void OnMenu(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            if (GameManager.Instance.GameState == GameManager.State.PLAYER)
                GameManager.Instance.GameState = GameManager.State.INVENTORY;
            else
                GameManager.Instance.GameState = GameManager.State.PLAYER;
        }
    }

    public void OnGamepadZoom(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed) return;

        if(SmartCamera.activeCamera is FirstPersonCamera)
        {
            thirdPersonCamera.SetActive();
            thirdPersonCamera.outerOffsetWeight = 0;

            if (playerData.inventory.activeWeapon)
            {
                playerData.inventory.activeWeapon.transform.parent = gameController.animator.GetBoneTransform(HumanBodyBones.RightHand);
                playerData.inventory.activeWeapon.transform.localPosition = playerData.inventory.activeWeapon.posOffset;
                playerData.inventory.activeWeapon.transform.localEulerAngles = playerData.inventory.activeWeapon.rotOffset;
            }

            viewController.SetActive(true, UnityEngine.Rendering.ShadowCastingMode.Off);
            gameController.SetActive(true, UnityEngine.Rendering.ShadowCastingMode.On);
        }
        else
        {
            float weight = thirdPersonCamera.outerOffsetWeight;

            if (weight == 0) thirdPersonCamera.outerOffsetWeight = 0.5f;
            else if (weight < 1f) thirdPersonCamera.outerOffsetWeight = 1f;
            else    //  become first person
            {
                thirdPersonCamera.outerOffsetWeight = 0;
                firstPersonCamera.SetActive();

                if (playerData.inventory.activeWeapon)
                {
                    playerData.inventory.activeWeapon.transform.parent = viewController.animator.GetBoneTransform(HumanBodyBones.RightHand);
                    playerData.inventory.activeWeapon.transform.localPosition = playerData.inventory.activeWeapon.posOffset;
                    playerData.inventory.activeWeapon.transform.localEulerAngles = playerData.inventory.activeWeapon.rotOffset;
                }

                viewController.SetActive(true);
                gameController.SetActive(true, UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly);
            }
        }
    }

    public void OnMouseZoom(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed) return;

        bool increase = callbackContext.ReadValue<Vector2>().y > 0;

        float cameraChange = 1 / thirdPersonCamera.steps * ((increase) ? 1 : -1);

        if(cameraChange > 0)
        {
            if(SmartCamera.activeCamera is FirstPersonCamera){} else
            {

                if(thirdPersonCamera.outerOffsetWeight - cameraChange < 0)
                {
                    thirdPersonCamera.outerOffsetWeight = 0;

                    firstPersonCamera.SetActive();

                    if(playerData.inventory.activeWeapon)
                    {
                        playerData.inventory.activeWeapon.transform.parent = viewController.animator.GetBoneTransform(HumanBodyBones.RightHand);
                        playerData.inventory.activeWeapon.transform.localPosition = playerData.inventory.activeWeapon.posOffset;
                        playerData.inventory.activeWeapon.transform.localEulerAngles = playerData.inventory.activeWeapon.rotOffset;
                    }

                    viewController.SetActive(true);
                    gameController.SetActive(true, UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly);              
                }
                else
                {
                    thirdPersonCamera.outerOffsetWeight -= cameraChange;
                }
            }
        }
        else if(cameraChange < 0)
        {
            if(SmartCamera.activeCamera is FirstPersonCamera)
            {
                thirdPersonCamera.SetActive();

                if (playerData.inventory.activeWeapon)
                {
                    playerData.inventory.activeWeapon.transform.parent = gameController.animator.GetBoneTransform(HumanBodyBones.RightHand);
                    playerData.inventory.activeWeapon.transform.localPosition = playerData.inventory.activeWeapon.posOffset;
                    playerData.inventory.activeWeapon.transform.localEulerAngles = playerData.inventory.activeWeapon.rotOffset;
                }

                viewController.SetActive(false);
                gameController.SetActive(true, UnityEngine.Rendering.ShadowCastingMode.On);
            }
            else
            {
                if(SmartCamera.activeCamera is ThirdPersonCamera)
                {
                    (SmartCamera.activeCamera as ThirdPersonCamera).outerOffsetWeight -= cameraChange;
                }
            }
        }
    }

    #endregion
}
