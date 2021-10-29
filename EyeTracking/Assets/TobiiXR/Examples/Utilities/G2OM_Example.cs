// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using Tobii.XR;
using UnityEngine;

namespace Tobii.G2OM.Examples
{
    /// <summary>
    /// Shows the recommended way to use the C# G2OM abstraction that is created for Unity.
    /// </summary>
    public class G2OM_Example : MonoBehaviour
    {
        [Header("Debug")]
        [Tooltip("Can be null if there is no need for debug visualization")]
        public G2OM_DebugVisualization DebugVisualization;
        public KeyCode DebugVisualizationOnOff = KeyCode.Space;
        public KeyCode DebugVisualizationFreezeOnOff = KeyCode.LeftControl;

        void Start()
        {
            TobiiXR.Start();
        }

        void Update()
        {
            if (DebugVisualization == null) return;

            if (Input.GetKeyUp(DebugVisualizationOnOff))
            {
                DebugVisualization.ToggleVisualization();
            }

            if (Input.GetKeyUp(DebugVisualizationFreezeOnOff))
            {
                DebugVisualization.ToggleFreeze();
            }
        }
    }
}