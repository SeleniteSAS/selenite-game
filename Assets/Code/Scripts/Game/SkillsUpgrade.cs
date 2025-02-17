using UnityEngine;

public class SkillsUpgrade : MonoBehaviour
{
    [Header("=== Base Values ===")]
    [SerializeField] public float playerSpeed = 0; // 1 à 5
    [SerializeField] public float bulletSpeed = 0; // 1 à 5
    [SerializeField] public float bulletDamage = 0; // 1 à 5
    [SerializeField] public float playerMaxHealth = 0; // 1 à 5
    [SerializeField] public float boostMaxCharge = 0; // 1 à 5
    [SerializeField] public float boostChargeSpeed = 0; // 1 à 5
    [SerializeField] public float laserChargeSpeed = 0; // 1 à 5
    [SerializeField] public float laserMaxCharge = 0; // 1 à 5

    private SkillsPointsBehavior skillsPointsBehavior;

    private void Start()
    {
        skillsPointsBehavior = GetComponent<SkillsPointsBehavior>();
    }

    public void UpgradeSkill(string skillName)
    {
        if (skillsPointsBehavior.skillPoints <= 0) return;

        switch (skillName)
        {
            case "playerSpeed":
                if (playerSpeed < 5) playerSpeed++;
                break;
            case "bulletSpeed":
                if (bulletSpeed < 5) bulletSpeed++;
                break;
            case "bulletDamage":
                if (bulletDamage < 5) bulletDamage++;
                break;
            case "playerMaxHealth":
                if (playerMaxHealth < 5) playerMaxHealth++;
                break;
            case "boostMaxCharge":
                if (boostMaxCharge < 5) boostMaxCharge++;
                break;
            case "boostChargeSpeed":
                if (boostChargeSpeed < 5) boostChargeSpeed++;
                break;
            case "laserChargeSpeed":
                if (laserChargeSpeed < 5) laserChargeSpeed++;
                break;
            case "laserMaxCharge":
                if (laserMaxCharge < 5) laserMaxCharge++;
                break;
            default:
                Debug.LogWarning("Skill name not recognized.");
                return;
        }

        skillsPointsBehavior.DecreaseSkillsPoints(1);
        UpdateShipStats();
    }

    private void UpdateShipStats()
    {
        // Vous pouvez ajouter ici le code pour mettre à jour les statistiques du vaisseau en fonction des nouvelles valeurs.
        Debug.Log("Ship stats updated.");
    }
}
