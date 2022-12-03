using UnityEngine;
using TMPro;

public abstract class UIIncrementText<T> : UIIncrementText
{
    [SerializeField] protected float incrementDuration = 1.0f;

    protected bool            isInitialized;
    protected T               curValue;
    protected T               targetValue;
    protected TextMeshProUGUI valueText;

    public void Initialize(T initVal)
    {
        curValue       = initVal;
        targetValue    = initVal;
        valueText.text = initVal.ToString();
        isInitialized  = true;
    }

    public abstract void Increment(T targetVal);

    protected virtual void UpdateValue(T val)
    {
        curValue       = val;
        valueText.text = val.ToString();
    }

    private void Awake()
    {
        valueText = GetComponent<TextMeshProUGUI>();
    }
}

public abstract class UIIncrementText : MonoBehaviour
{
    
}