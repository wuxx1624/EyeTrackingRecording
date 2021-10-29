// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using Tobii.XR.GazeModifier;

namespace Tobii.XR.DevTools
{
    using UnityEngine;

    public class DisableGazeModifierOnFocus : MonoBehaviour, IDisableGazeModifier
    {
        void Start()
        {
            GazeModifierSettings.LoadDefaultSettings().AddDisabler(this);
        }

        public void GazeFocusChanged(bool hasFocus)
        {
            Disable = hasFocus;
        }

        public bool Disable { get; private set; }
    }
}
