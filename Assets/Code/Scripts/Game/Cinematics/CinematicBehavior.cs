using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CinematicBehavior : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string sceneName;
    public GameObject skipCanvas;
    public Image fillCircle;
    public float holdDuration = 2f;
    private float holdTime = 0f;

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

        skipCanvas.SetActive(false);
        fillCircle.fillAmount = 0f;
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            skipCanvas.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            holdTime += Time.deltaTime;
            fillCircle.fillAmount = holdTime / holdDuration;

            if (holdTime >= holdDuration)
            {
                SkipCinematic();
            }
        }
        else
        {
            holdTime = 0f;
            fillCircle.fillAmount = 0f;
        }
    }

    private void SkipCinematic()
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(sceneName);
    }
}