using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillsPointsBehavior : MonoBehaviour
{
    [Header("=== Skills Points ===")]
    [SerializeField] private int baseSkillsPoints = 0;
    [SerializeField] public int skillPoints;

    [Header("=== UI ===")]
    [SerializeField] private TextMeshProUGUI skillsPointsText; // Affiche constamment le nb de points
    [SerializeField] private Canvas getSkillsPointsCanvas; // Quand tu gagnes des points (pop up du nb)
    [SerializeField] private AudioSource getSkillsPointsSound;

    private void Start()
    {
        skillPoints = baseSkillsPoints;
        UpdateSkillsPointsUI();
        StartNewRound();
    }

    private void UpdateSkillsPointsUI()
    {
        if (skillsPointsText != null)
        {
            skillsPointsText.text = "Points: " + skillPoints;
        }
    }

    public void IncreaseSkillsPoints(int number)
    {
        skillPoints += number;
        UpdateSkillsPointsUI();
        StartCoroutine(DisplayGetSkillsPointsCanvas());
        getSkillsPointsSound.Play();
    }

    public void DecreaseSkillsPoints(int number)
    {
        skillPoints -= number;
        skillPoints = Mathf.Clamp(skillPoints, 0, int.MaxValue);
        UpdateSkillsPointsUI();
    }

    private IEnumerator DisplayGetSkillsPointsCanvas()
    {
        getSkillsPointsCanvas.enabled = true;
        yield return new WaitForSeconds(2f); // Durée de l'affichage
        getSkillsPointsCanvas.enabled = false;
    }

    public void StartNewRound()
    {
        Time.timeScale = 0; // Mettre le jeu en pause
        getSkillsPointsCanvas.enabled = true;
    }

    public void ResumeGame()
    {
        getSkillsPointsCanvas.enabled = false;
        Time.timeScale = 1; // Reprendre le jeu
    }
}
