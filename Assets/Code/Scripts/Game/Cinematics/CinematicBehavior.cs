using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CinematicBehavior : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string sceneName;

    private void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogError("Aucun VideoPlayer trouvé sur l'objet.");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(sceneName);
    }
}