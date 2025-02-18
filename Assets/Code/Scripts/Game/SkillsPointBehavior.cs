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
    [SerializeField] private TextMeshProUGUI upgradePointsText; // Texte dans le Canvas des améliorations
    [SerializeField] private Canvas skillsPointsCanvas; // Canvas d'amélioration des compétences
    [SerializeField] private AudioSource getSkillsPointsSound;

    private void Start()
    {
        skillPoints = baseSkillsPoints;
        UpdateSkillsPointsUI();
        skillsPointsCanvas.enabled = false; // Désactiver le Canvas au début
    }

    private void UpdateSkillsPointsUI()
    {
        if (skillsPointsText != null)
        {
            skillsPointsText.text = "Points: " + skillPoints;
        }
        if (upgradePointsText != null)
        {
            upgradePointsText.text = "Points: " + skillPoints;
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
        skillsPointsCanvas.enabled = true;
        yield return new WaitForSeconds(2f); // Durée de l'affichage
        skillsPointsCanvas.enabled = false;
    }

    public void EndRound()
    {
        Time.timeScale = 0; // Mettre le jeu en pause
        skillsPointsCanvas.enabled = true; // Activer le Canvas d'amélioration des compétences
    }

    public void ResumeGame()
    {
        skillsPointsCanvas.enabled = false;
        Time.timeScale = 1; // Reprendre le jeu
    }
}
