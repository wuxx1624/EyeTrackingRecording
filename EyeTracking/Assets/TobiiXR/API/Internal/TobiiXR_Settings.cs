// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using UnityEngine;
using Tobii.G2OM;
using System;

namespace Tobii.XR
{

    public class TobiiXR_Settings : ScriptableObject
    {
        public static readonly string TobiiXR_DescriptionPath = typeof(TobiiXR_Settings).Name;
        private static RuntimePlatform _platform = Application.platform;
        private IEyeTrackingProvider _eyeTrackingProvider;
        private IEyeTrackingFilter _eyeTrackingFilter;
        private Tobii.G2OM.G2OM _g2om;

        public static Func<TobiiXR_Settings> LoadDefaultSettings = () => Resources.Load<TobiiXR_Settings>(TobiiXR_DescriptionPath);

        public IEyeTrackingProvider EyeTrackingProvider
        {
            get
            {
                if (_eyeTrackingProvider != null) return _eyeTrackingProvider;
                _eyeTrackingProvider = GetProvider(GetProviderType());
                return _eyeTrackingProvider;
            }
            set
            {
                _eyeTrackingProvider = value;
            }
        }

        public IEyeTrackingFilter EyeTrackingFilter
        {
            get
            {
                if (_eyeTrackingFilter != null)
                {
                    if (string.IsNullOrEmpty(EyeTrackingFilterType) == false)
                    {
                        return _eyeTrackingFilter;
                    }
                    else
                    {
                        _eyeTrackingFilter = null;
                        return null;
                    }
                }
                if (string.IsNullOrEmpty(EyeTrackingFilterType)) return null;

                try
                {
                    _eyeTrackingFilter = (IEyeTrackingFilter)Activator.CreateInstance(Type.GetType(EyeTrackingFilterType));
                }
                catch (Exception e)
                {
                    Debug.Log("Could not instantiate filter " + EyeTrackingFilterType + ": " + e.ToString());
                }

                return _eyeTrackingFilter;
            }
        }

        public Tobii.G2OM.G2OM G2OM
        {
            get
            {
                if (_g2om != null) return _g2om;

                _g2om = Tobii.G2OM.G2OM.Create(new G2OM_Description
                {
                    LayerMask = LayerMask,
                    HowLongToKeepCandidatesInSeconds = HowLongToKeepCandidatesInSeconds
                });

                return _g2om;
            }
            set { _g2om = value; }
        }

        [HideInInspector]
        public string EyeTrackingFilterType = string.Empty;

        [HideInInspector]
        public FieldOfUse FieldOfUse = FieldOfUse.NotSelected;

        [HideInInspector]
        public string EyeTrackingProviderTypeStandAlone = typeof(TobiiProvider).FullName;

        [HideInInspector]
        public string EyeTrackingProviderTypeAndroid = typeof(NoseDirectionProvider).FullName;

        [HideInInspector]
        public LayerMask LayerMask = G2OM_Description.DefaultLayerMask;

        [HideInInspector]
        public float HowLongToKeepCandidatesInSeconds = G2OM_Description.DefaultCandidateMemoryInSeconds;

        [HideInInspector]
        public bool TobiiSDKEulaAccepted = false;


        public static TobiiXR_Settings CreateDefaultSettings()
        {
            bool b;
            return CreateDefaultSettings(out b);
        }

        public static TobiiXR_Settings CreateDefaultSettings(out bool resourceExists)
        {
            var configuration = LoadDefaultSettings != null ? LoadDefaultSettings() : null;
            resourceExists = configuration != null;
            return resourceExists ? configuration : ScriptableObject.CreateInstance<TobiiXR_Settings>();
        }

        public Type GetProviderType()
        {
            string eyeTrackingProviderType = _platform == RuntimePlatform.Android
                ? EyeTrackingProviderTypeAndroid
                : EyeTrackingProviderTypeStandAlone;

            if (string.IsNullOrEmpty(eyeTrackingProviderType)) return null;
            return AssemblyUtils.EyetrackingProviderType(eyeTrackingProviderType);
        }

        public static IEyeTrackingProvider GetProvider(Type type)
        {
            if (type == null) return null;
            try
            {
                return Activator.CreateInstance(type) as IEyeTrackingProvider;
            }
            catch (Exception) { }
            return null;
        }
    }
}