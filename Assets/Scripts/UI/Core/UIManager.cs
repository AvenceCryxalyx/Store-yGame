using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public struct FeatureUIData
    {
        public Enums.Features Feature;
        public UiController controller;
    }
    public List<FeatureUIData> FeatureControllers;

    [SerializeField]
    private Canvas m_canvas;
}
