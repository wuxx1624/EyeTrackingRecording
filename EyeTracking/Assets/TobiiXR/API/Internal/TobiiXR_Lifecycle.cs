// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using System;
using UnityEngine;

namespace Tobii.XR
{
    public class TobiiXR_Lifecycle : MonoBehaviour
    {
        public Action OnApplicationQuitAction = delegate { };
        public Action OnUpdateAction = delegate { };
        public Action OnDisableAction = delegate { };

        private void Awake()
        {
            Application.quitting += OnQuitting;
        }

        private void OnDisable()
        {
            OnDisableAction.Invoke();
        }

        private void Update()
        {
            OnUpdateAction.Invoke();
        }

        private void FixedUpdate()
        {
            OnUpdateAction.Invoke();
        }

        private void OnQuitting()
        {
            OnApplicationQuitAction.Invoke();
        }
    }
}