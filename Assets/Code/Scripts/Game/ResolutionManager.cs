using UnityEngine;
using TMPro; // pour TextMeshPro
using System.Collections.Generic;
using System.Linq;

public class ResolutionManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    private Resolution[] availableResolutions;
    private bool isInitialized = false;

    private void Start()
    {
        availableResolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        var resolutionOptions = new List<string>();

        var currentResolutionIndex = 0;
        for (var i = 0; i < availableResolutions.Length; i++)
        {
            var option = availableResolutions[i].width + " x " + availableResolutions[i].height;
            resolutionOptions.Add(option);

            if (availableResolutions[i].width == Screen.currentResolution.width &&
                availableResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
            else
            {
                currentResolutionIndex = 0;
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        var qualityOptions = QualitySettings.names.ToList();
        qualityDropdown.AddOptions(qualityOptions);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        isInitialized = true;
    }

    public void SetResolution(int resolutionIndex)
    {
        if (!isInitialized) return;

        var selectedResolution = availableResolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }
}
