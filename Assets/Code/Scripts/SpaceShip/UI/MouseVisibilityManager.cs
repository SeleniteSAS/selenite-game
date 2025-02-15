using UnityEngine;

public class MouseVisibilityManager : MonoBehaviour
{
    private void Start()
    {
        HideMouseInGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMouse();
        }
    }

    private static void HideMouseInGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private static void ShowMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
