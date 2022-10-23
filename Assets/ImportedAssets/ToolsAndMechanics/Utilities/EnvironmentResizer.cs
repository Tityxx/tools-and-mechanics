using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToolsAndMechanics.Utilities
{
    [Serializable]
    public enum ScreenType
    {
        None,
        iPhone,
        iPhoneX,
        iPad
    }

    [Serializable]
    public class ScreenSize
    {
        public ScreenType ScreenType;
        public GameObject Object;
        public float Size => GetRatioByType(ScreenType);

        private float GetRatioByType(ScreenType type)
        {
            switch (type)
            {
                case ScreenType.iPhone:
                    return 0.5625f;
                case ScreenType.iPhoneX:
                    return 0.45f;
                case ScreenType.iPad:
                    return 0.695f;
                default:
                    return GetRatioByType(ScreenType.iPhoneX);
            }
        }
    }

#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    public class EnvironmentResizer : MonoBehaviour
    {
        [SerializeField]
        private List<ScreenSize> screenSizes = new List<ScreenSize>()
    {
        new ScreenSize() { ScreenType =  ScreenType.iPhone, Object = null},
        new ScreenSize() { ScreenType =  ScreenType.iPhoneX, Object = null},
        new ScreenSize() { ScreenType =  ScreenType.iPad, Object = null}
    };
        [SerializeField]
        private bool useInEditor;

#if UNITY_EDITOR
        [SerializeField, CustomReadOnly]
        private ScreenType currSelected;
#endif

        private void Start()
        {
            DetectScreenSize();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (useInEditor) DetectScreenSize();
        }
#endif

        private void DetectScreenSize()
        {
            ScreenSize currentSize = null;
            var aspectRatio = (float)Screen.width / Screen.height;

            float cachedMin = 100f;
            float newAspect;

            foreach (var screenSize in screenSizes)
            {
                newAspect = Mathf.Abs(screenSize.Size - aspectRatio);
                if (newAspect < cachedMin)
                {
                    currentSize = screenSize;
                    cachedMin = newAspect;
                }
            }
            if (currentSize == null) currentSize = screenSizes[0];

            foreach (var screenSize in screenSizes)
            {
                screenSize.Object.SetActive(screenSize == currentSize);
            }

#if UNITY_EDITOR
            currSelected = currentSize.ScreenType;
#endif
        }
    }
}