using UnityEngine;

public class ModeSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject baseModeCanvas;
    [SerializeField] private GameObject vrModeCanvas;
    [SerializeField] private GameObject optionsMenuCanvas;

    private bool isOptionsMenuActive = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionsMenu();
        }
    }

    public void ShowBaseMode()
    {
        baseModeCanvas.SetActive(true);
        vrModeCanvas.SetActive(false);
        optionsMenuCanvas.SetActive(false);
        isOptionsMenuActive = false;
    }

    public void ShowVRMode()
    {
        baseModeCanvas.SetActive(false);
        vrModeCanvas.SetActive(true);
        optionsMenuCanvas.SetActive(false);
        isOptionsMenuActive = false;
    }

    public void ShowOptions()
    {
        baseModeCanvas.SetActive(false);
        vrModeCanvas.SetActive(false);
        optionsMenuCanvas.SetActive(true);
        isOptionsMenuActive = true;
    }

    public void CloseOptions()
    {
        optionsMenuCanvas.SetActive(false);
        baseModeCanvas.SetActive(true);
        isOptionsMenuActive = false;
    }

    private void ToggleOptionsMenu()
    {
        if (isOptionsMenuActive)
        {
            CloseOptions();
        }
       
    }
}
