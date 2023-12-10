using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    public bool IsShown { get; private set; }
    public bool IsInitialized { get; private set; }
    public virtual void OnShow() { }
    public virtual void OnHide() { }

    public virtual void OnInitialized() { }
    public void Initialize() 
    { 
        if(IsInitialized)
        {
            return;
        }
        OnInitialized();

        IsInitialized = true;
    }

    public void Show()
    {
        if(IsShown)
        {
            return;
        }

        OnShow();

        IsShown = true;
    }

    public void Hide()
    {
        if (!IsShown)
        {
            return;
        }

        OnHide();

        IsShown = false;
    }
}
