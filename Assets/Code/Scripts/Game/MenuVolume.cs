using UnityEngine;
using UnityEngine.UI;

public class MenuVolume : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private static void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
