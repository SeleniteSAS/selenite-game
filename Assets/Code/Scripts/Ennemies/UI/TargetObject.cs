using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TargetObject : MonoBehaviour
{
    private enum TargetType
    {
        Enemy,
        Outpost
    }

    [SerializeField] private TargetType type;

    private void Awake()
    {
        var ui = GetComponentInParent<UIController>();
        if(ui == null)
        {
            ui = GameObject.Find("WAVE MANAGER").GetComponent<UIController>();
        }
        if (ui == null) Debug.LogError("No UIController component found");
        switch (type)
        {
            case TargetType.Enemy:
                ui.AddTargetIndicator(gameObject);
                break;
            case TargetType.Outpost:
                ui.AddOutpostIndicator(gameObject);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnDestroy()
    {
        var ui = GetComponentInParent<UIController>();
        if(ui == null)
        {
            ui = GameObject.Find("WAVE MANAGER").GetComponent<UIController>();
        }
        if (ui == null) Destroy(gameObject);
        switch (type)
        {
            case TargetType.Enemy:
                ui.RemoveTargetIndicator(gameObject);
                break;
            case TargetType.Outpost:
                ui.RemoveOutpostIndicator(gameObject);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}