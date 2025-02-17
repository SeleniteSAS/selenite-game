using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

[RequireComponent(typeof(Rigidbody))]
public class SpaceShipBehavior : MonoBehaviour
{
    [Header("=== Ship Movement Settings ===")]
    [SerializeField] private float thrustForce = 500f;
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private float verticalThrust = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float rollIntensity = 10f;
    [SerializeField] private float rollResetTime = 3f;

    [Header("=== Camera Settings ===")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float normalFOV = 80f;
    [SerializeField] private float zoomedFOV = 50f;
    [SerializeField] private float zoomSpeed = 20f;

    [Header("=== Boost Settings ===")]
    [SerializeField] private float maxBoostAmount = 100f;
    [SerializeField] private float boostConsumptionRate = 20f;
    [SerializeField] private float boostRechargeRate = 10f;

    [Header("=== Mouse Settings ===")]
    [SerializeField] private float mouseSensitivity = 1f;

    [Header("=== UI Elements ===")]
    [SerializeField] private RectTransform aimZone;
    [SerializeField] private AudioSource spaceshipEngineSound;
    [SerializeField] private RectTransform cursor;
    [SerializeField] private RectTransform fakeCursor;
    [SerializeField] private Image boostBar;

    [Header("=== VFX ===")]
    [SerializeField] private ParticleSystem boostVFX;

    private Rigidbody rb;
    private float thrustInput;
    private bool boosting;
    private Vector2 mouseDelta;
    private float rollInput;
    private float rollResetTimer;
    private float verticalInput;
    private float currentBoostAmount;
    private bool isZooming;
    private float currentFOV;

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
        var currentThrust = thrustForce;
        if (boosting && currentBoostAmount > 0)
        {
            currentThrust *= boostMultiplier;
            currentBoostAmount -= boostConsumptionRate * Time.fixedDeltaTime;
            currentBoostAmount = Mathf.Clamp(currentBoostAmount, 0, maxBoostAmount);
        }

        spaceshipEngineSound.pitch = 0.8f + Mathf.Abs(verticalThrust * 0.01f);

        rb.AddRelativeForce(Vector3.forward * (currentThrust * thrustInput));
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        UpdateBoostUI();
    }

    private void UpdateBoostUI()
    {
        if (boostBar)
        {
            boostBar.fillAmount = currentBoostAmount / maxBoostAmount;
        }
    }

    private void RechargeBoost()
    {
        if (boosting) return;
        currentBoostAmount += boostRechargeRate * Time.fixedDeltaTime;
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

        rollResetTimer -= Time.fixedDeltaTime;
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
