using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

[RequireComponent(typeof(Rigidbody))]
public class SpaceShipBehavior : MonoBehaviour
{
    [Header("=== Ship Movement Settings ===")]
    [SerializeField] public float thrustForce = 500f;
    [SerializeField] public float boostMultiplier = 2f;
    [SerializeField] public float verticalThrust = 10f;
    [SerializeField] public float rotationSpeed = 2f;
    [SerializeField] public float rollIntensity = 10f;
    [SerializeField] public float rollResetTime = 3f;

    [Header("=== Camera Settings ===")]
    [SerializeField] public float normalFOV = 80f;
    [SerializeField] public float zoomedFOV = 50f;
    [SerializeField] public float zoomSpeed = 20f;

    [Header("=== Boost Settings ===")]
    [SerializeField] public float maxBoostAmount = 100f;
    [SerializeField] public float boostConsumptionRate = 20f;
    [SerializeField] public float boostRechargeRate = 10f;

    [Header("=== Mouse Settings ===")]
    [SerializeField] public float mouseSensitivity = 1f;

    [Header("=== UI Elements ===")]
    [SerializeField] public RectTransform aimZone;
    [SerializeField] public AudioSource spaceshipEngineSound;
    [SerializeField] public RectTransform cursor;
    [SerializeField] public RectTransform fakeCursor;
    [SerializeField] public Image boostBar;

    [Header("=== VFX ===")]
    [SerializeField] public TrailRenderer boostTrailRight;
    [SerializeField] public TrailRenderer boostTrailLeft;

    private Rigidbody rb;
    private float thrustInput;
    private bool boosting;
    private Vector2 mouseDelta;
    private float rollInput;
    private float rollResetTimer;
    private float verticalInput;
    private float currentBoostAmount;
    private bool isZooming;
    private CinemachineVirtualCamera virtualCamera;
    private float currentFOV;

    public float PlayerSpeed { get; set; } = 0;
    public float PlayerMaxHealth { get; set; } = 0;
    public float BoostMaxCharge { get; set; } = 0;
    public float BoostChargeSpeed { get; set; } = 0;
    public float LaserChargeSpeed { get; set; } = 0;
    public float LaserMaxCharge { get; set; } = 0;

    private void Start()
    {
        Time.timeScale = 1;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = 0.3f;
        rb.angularDrag = 2f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentBoostAmount = maxBoostAmount;

        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            if (virtualCamera == null)
            {
                Debug.Assert(false, "Virtual Camera not found in the scene!");
                return;
            }
        }
        currentFOV = normalFOV;
        virtualCamera.m_Lens.FieldOfView = currentFOV;

        UpdateBoostUI();
    }

    private void FixedUpdate()
    {
        HandleMouseControl();
        HandleThrust();
        HandleVerticalMovement();
        HandleRoll();
        HandleRollReset();
        HandleZoom();
        RechargeBoost();
    }

    private void HandleMouseControl()
    {
        var currentSensitivity = isZooming ? mouseSensitivity * 0.5f : mouseSensitivity;

        cursor.anchoredPosition = new Vector2(
            Mathf.Clamp(cursor.anchoredPosition.x + mouseDelta.x * currentSensitivity, -aimZone.sizeDelta.x / 2, aimZone.sizeDelta.x / 2),
            Mathf.Clamp(cursor.anchoredPosition.y + mouseDelta.y * currentSensitivity, -aimZone.sizeDelta.y / 2, aimZone.sizeDelta.y / 2)
        );
        fakeCursor.anchoredPosition = cursor.anchoredPosition;

        rb.AddRelativeTorque(new Vector3(
            -mouseDelta.y * rotationSpeed * currentSensitivity,
            mouseDelta.x * rotationSpeed * currentSensitivity,
            0));

        mouseDelta = Vector2.zero;
    }

    private void HandleZoom()
    {
        if (!virtualCamera) return;

        var targetFOV = isZooming ? zoomedFOV : normalFOV;
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.fixedDeltaTime * zoomSpeed);
        virtualCamera.m_Lens.FieldOfView = currentFOV;
    }

    private void HandleThrust()
    {
        var targetVelocity = transform.forward * (thrustInput * GetCurrentThrust());

        rb.velocity = Vector3.Lerp(
            rb.velocity,
            targetVelocity,
            1 * Time.deltaTime
        );

        HandleBoostEffects();
        UpdateEngineSound();
        if (!boosting || !(currentBoostAmount > 0)) return;
        currentBoostAmount -= boostConsumptionRate * Time.deltaTime;
        currentBoostAmount = Mathf.Clamp(currentBoostAmount, 0, maxBoostAmount);
        UpdateBoostUI();
    }

    private float GetCurrentThrust()
    {
        var thrust = thrustForce;
        if (boosting && currentBoostAmount > 0)
        {
            thrust *= boostMultiplier;
        }
        return thrust;
    }

    private void UpdateEngineSound()
    {
        var targetPitch = 0.8f + Mathf.Abs(rb.velocity.magnitude / (thrustForce * boostMultiplier));
        spaceshipEngineSound.pitch = Mathf.Lerp(
            spaceshipEngineSound.pitch,
            targetPitch,
            1 * Time.deltaTime
        );
    }

    private void UpdateBoostUI()
    {
        if (boostBar)
        {
            boostBar.fillAmount = currentBoostAmount / maxBoostAmount;
        }
    }

    private void HandleBoostEffects()
    {

        if (boosting && currentBoostAmount > 0)
        {
            if (!boostTrailLeft.emitting) boostTrailLeft.emitting = true;
            if (!boostTrailRight.emitting) boostTrailRight.emitting = true;
        }
        else
        {
            if (boostTrailLeft.emitting) boostTrailLeft.emitting = false;
            if (boostTrailRight.emitting) boostTrailRight.emitting = false;
        }
    }


    private void RechargeBoost()
    {
        if (boosting) return;
        currentBoostAmount += boostRechargeRate * Time.deltaTime;
        currentBoostAmount = Mathf.Clamp(currentBoostAmount, 0, maxBoostAmount);
        UpdateBoostUI();
    }

    private void HandleRoll()
    {
        rb.AddRelativeTorque(Vector3.forward * (rollInput != 0 ? -rollInput * rollIntensity : -mouseDelta.x * rollIntensity));
    }

    private void HandleRollReset()
    {
        if (rollInput != 0)
        {
            rollResetTimer = rollResetTime;
            return;
        }

        rollResetTimer -= Time.deltaTime;
        if (rollResetTimer > 0) return;

        var currentRoll = transform.rotation.eulerAngles.z;
        if (currentRoll > 180) currentRoll -= 360;
        rb.AddRelativeTorque(Vector3.forward * (-currentRoll * 0.5f));
    }

    private void HandleVerticalMovement()
    {
        rb.AddForce(Vector3.up * (verticalInput * verticalThrust));
    }

    #region Input Methods
    public void OnThrust(InputAction.CallbackContext context) => thrustInput = context.ReadValue<float>();
    public void OnBoost(InputAction.CallbackContext context) => boosting = context.performed;
    public void OnLook(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>() * mouseSensitivity;
        mouseDelta = Vector2.Lerp(mouseDelta, input, 0.1f);
    }
    public void OnRoll(InputAction.CallbackContext context) => rollInput = context.ReadValue<float>();
    public void OnVerticalMove(InputAction.CallbackContext context) => verticalInput = context.ReadValue<float>();
    public void OnZoom(InputAction.CallbackContext context) => isZooming = context.performed;
    #endregion
}
