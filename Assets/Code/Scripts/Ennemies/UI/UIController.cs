using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Canvas canvas;

    public List<TargetIndicator> targetIndicators = new List<TargetIndicator>();

    public Camera MainCamera;

    public GameObject TargetIndicatorPrefab;

    void Update()
    {
        if (targetIndicators.Count <= 0) return;
        foreach (var t in targetIndicators)
        {
            t.UpdateTargetIndicator();
        }
    }

    public void AddTargetIndicator(GameObject target)
    {
        var indicator = Instantiate(TargetIndicatorPrefab, canvas.transform).GetComponent<TargetIndicator>();
        indicator.InitialiseTargetIndicator(target, MainCamera, canvas);
        targetIndicators.Add(indicator);
    }

    public void RemoveTargetIndicator(GameObject target)
    {
        var indicator = targetIndicators.Find(t => t.target == target);
        if (indicator == null) return;
        targetIndicators.Remove(indicator);
        Destroy(indicator.gameObject);
    }
}