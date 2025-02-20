using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsResolution : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private float currentRefreshRate;
    private int currentResolutionIndex = 0;

    private void Start()
    {
        InitializeResolutionSettings();
        InitializeQualitySettings();
    }

    private void InitializeResolutionSettings()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        foreach (var t in resolutions)
        {
            if (Mathf.Approximately((float)t.refreshRateRatio.value, currentRefreshRate))
            {
                filteredResolutions.Add(t);
            }
        }

        filteredResolutions.Sort((a, b) => a.width != b.width ? b.width.CompareTo(a.width) : b.height.CompareTo(a.height));

        var options = new List<string>();
        for (var i = 0; i < filteredResolutions.Count; i++)
        {
            var resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRateRatio.value.ToString("0.##") + " Hz";
            options.Add(resolutionOption);
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height && Mathf.Approximately((float)filteredResolutions[i].refreshRateRatio.value, currentRefreshRate))
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        SetResolution(currentResolutionIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        var resolution = filteredResolutions[resolutionIndex];
        Debug.Log("Resolution: " + resolution.width + "x" + resolution.height + " " + resolution.refreshRateRatio.value + " Hz");
        Screen.SetResolution(resolution.width, resolution.height, true);
    }

    private void InitializeQualitySettings()
    {
        qualityDropdown.ClearOptions();
        var qualityLevels = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(qualityLevels);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
        Debug.Log("Quality Level Set To: " + QualitySettings.names[qualityIndex]);
    }
}
