using UnityEngine;

public class FloatingArc : MonoBehaviour
{
    // Amplitude du mouvement horizontal.
    public float amplitude = 0.5f;
    // Fréquence d'oscillation (vitesse).
    public float frequency = 1f;

    // Position initiale de l'objet.
    private Vector3 initialPosition;

    void Start()
    {
        // On sauvegarde la position de départ.
        initialPosition = transform.position;
    }

    void Update()
    {
        // t représente le temps, multiplié par la fréquence pour ajuster la vitesse.
        float t = Time.time * frequency;

        // Calcul de l'offset sur l'axe X : mouvement sinusoidal.
        float offsetX = amplitude * Mathf.Sin(t);
        // Calcul de l'offset sur l'axe Y :
        // 1 - cos(t) varie de 0 (à t = 0) à 2 (à t = PI) et revient à 0,
        // on le multiplie par -amplitude pour obtenir un léger déplacement vers le bas au milieu.
        float offsetY = -amplitude * (1 - Mathf.Cos(t));

        // On met à jour la position en ajoutant les offsets à la position initiale.
        transform.position = initialPosition + new Vector3(offsetX, offsetY, 0);
    }
}
