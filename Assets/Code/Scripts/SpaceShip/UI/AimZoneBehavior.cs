using UnityEngine;

public class AimZoneBehavior : MonoBehaviour
{
    [Header("=== UI Elements ===")]
    [SerializeField] private RectTransform aimZone;
    [SerializeField] private RectTransform aimZoneDecorator;


    [Header("=== Aim Zone Settings ===")]
    [Range(0.1f, 1f)]
    [SerializeField] private float widthPercentage = 0.2f;
    [Range(0.1f, 1f)]
    [SerializeField] private float heightPercentage = 0.2f;

    private void Start()
    {
        UpdateAimZoneSize();
    }

    private void UpdateAimZoneSize()
    {
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;

        var zoneWidth = screenWidth * widthPercentage;
        var zoneHeight = screenHeight * heightPercentage;

        aimZone.sizeDelta = new Vector2(zoneWidth, zoneHeight);
        aimZoneDecorator.sizeDelta = new Vector2(zoneWidth, zoneHeight);
    }
}