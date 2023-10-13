using System;
using UnityEngine;
using TMPro;
using deVoid.UIFramework;

[Serializable]
public class AlertPanelProperties : PanelProperties
{
    public readonly string alertText;
    public readonly float  hideDelay;

    public AlertPanelProperties(string alertText, float hideDelay = 1.5f)
    {
        this.alertText = alertText;
        this.hideDelay = hideDelay;
    }
}

public class AlertPanelController : APanelController<AlertPanelProperties>
{
    [SerializeField] private TextMeshProUGUI messageText;

    private bool  checkTimer;
    private float timer;
    
    protected override void AddListeners()
    {
        InTransitionFinished += DoInTransitionFinished;
    }

    protected override void RemoveListeners()
    {
        InTransitionFinished -= DoInTransitionFinished;
    }

    protected override void OnPropertiesSet()
    {
        checkTimer = false;
        timer = 0.0f;
        messageText.text = Properties.alertText;
    }

    private void DoInTransitionFinished(IUIScreenController screen)
    {
        checkTimer = true;
    }

    private void Update()
    {
        if (checkTimer)
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= Properties.hideDelay)
            {
                checkTimer = false;
                UI_Close();
            }
        }
    }
}