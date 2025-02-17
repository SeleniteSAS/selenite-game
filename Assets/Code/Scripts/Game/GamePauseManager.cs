using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePauseManager : MonoBehaviour
{
    [SerializeField] public bool isPaused;
    [SerializeField] private GameObject pauseMenu;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}