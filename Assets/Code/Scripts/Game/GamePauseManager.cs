using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePauseManager : MonoBehaviour
{
    [SerializeField] private bool isPaused = false;

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
        isPaused = true;
        Time.timeScale = 0;
        SceneManager.LoadScene("MenuPause", LoadSceneMode.Additive);
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("MenuPause");
    }
}
