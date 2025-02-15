using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    private void Awake()
    {
        var ui = GetComponentInParent<UIController>();
        if(ui == null)
        {
            ui = GameObject.Find("WAVE MANAGER").GetComponent<UIController>();
        }

        if (ui == null) Debug.LogError("No UIController component found");

        ui.AddTargetIndicator(gameObject);
    }

    private void OnDestroy()
    {
        var ui = GetComponentInParent<UIController>();
        if(ui == null)
        {
            ui = GameObject.Find("WAVE MANAGER").GetComponent<UIController>();
        }

        if (ui == null) Debug.LogError("No UIController component found");

        ui.RemoveTargetIndicator(gameObject);
    }
}