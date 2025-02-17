using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Canvas canvas;

    public List<TargetIndicator> targetIndicators = new List<TargetIndicator>();
    public List<OutpostIndicator> outpostIndicators = new List<OutpostIndicator>();

    public Camera MainCamera;

    public GameObject TargetIndicatorPrefab;
    public GameObject OutpostIndicatorPrefab;

    void Update()
    {
        if (targetIndicators.Count <= 0) return;
        foreach (var t in targetIndicators)
        {
            t.UpdateTargetIndicator();
        }

        if (outpostIndicators.Count <= 0) return;
        foreach (var o in outpostIndicators)
        {
            o.UpdateOutpostIndicator();
        }
    }

    public void AddTargetIndicator(GameObject target)
    {
        var indicator = Instantiate(TargetIndicatorPrefab, canvas.transform).GetComponent<TargetIndicator>();
        indicator.InitialiseTargetIndicator(target, MainCamera, canvas);
        targetIndicators.Add(indicator);
    }

    public void AddOutpostIndicator(GameObject target)
    {
        var indicator = Instantiate(OutpostIndicatorPrefab, canvas.transform).GetComponent<OutpostIndicator>();
        indicator.InitialiseOutpostIndicator(target, MainCamera, canvas);
        outpostIndicators.Add(indicator);
    }

    public void RemoveTargetIndicator(GameObject target)
    {
        var indicator = targetIndicators.Find(t => t.target == target);
        if (indicator == null) return;
        targetIndicators.Remove(indicator);
        Destroy(indicator.gameObject);
    }

    public void RemoveOutpostIndicator(GameObject target)
    {
        var indicator = outpostIndicators.Find(t => t.target == target);
        if (indicator == null) return;
        outpostIndicators.Remove(indicator);
        Destroy(indicator.gameObject);
    }
}