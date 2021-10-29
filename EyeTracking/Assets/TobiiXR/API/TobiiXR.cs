// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using System.Collections.Generic;
using Tobii.G2OM;
using UnityEngine;

namespace Tobii.XR
{
    /// <summary>
    /// Static access point for Tobii XR eye tracker data.
    /// </summary>
    public static class TobiiXR
    {
        private static readonly TobiiXRInternal _internal = new TobiiXRInternal();
        internal static IEyeTrackingProvider _eyeTrackingProvider;
        private static GameObject _updaterGameObject;
        private static readonly TobiiXR_EyeTrackingData _eyeTrackingData = new TobiiXR_EyeTrackingData();

        /// <summary>
        /// Gets the gaze data. Subsequent calls within the same frame will
        /// return the same value.
        /// </summary>
        /// <returns>The last (newest) <see cref="TobiiXR_EyeTrackingData"/>.</returns>
        public static TobiiXR_EyeTrackingData EyeTrackingData
        {
            get
            {
                VerifyInstanceIntegrity();

                return _eyeTrackingData;
            }
        }

        /// <summary>
        /// Gets all possible <see cref="FocusedCandidate"/> with gaze focus. Only game 
        /// objects with a <see cref="IGazeFocusable"/> component can be focused 
        /// using gaze.
        /// </summary>
        /// <returns>A list of <see cref="FocusedCandidate"/> in descending order of probability.</returns>
        public static List<FocusedCandidate> FocusedObjects
        {
            get
            {
                VerifyInstanceIntegrity();

                return Internal.G2OM.GazeFocusedObjects;
            }
        }

        /// <summary>
        /// Get current <see cref="IEyeTrackingProvider"/> 
        /// </summary>
        public static IEyeTrackingProvider Provider
        {
            get
            {
                VerifyInstanceIntegrity();

                return _eyeTrackingProvider;
            }
        }

        public static bool Start(TobiiXR_Settings settings = null)
        {
            if (!TobiiEulaFile.IsEulaAccepted())
            {
                Debug.LogError("You need to accept Tobii Software Development License Agreement to be able to use Tobii XR Unity SDK.");
                return false;
            }

            if (_eyeTrackingProvider != null)
            {
                Debug.LogWarning(string.Format("TobiiXR already started with provider ({0})", _eyeTrackingProvider));
                VerifyInstanceIntegrity();
                return false;
            }

            if (settings == null)
            {
                settings = TobiiXR_Settings.CreateDefaultSettings();
            }

            if (settings.FieldOfUse == FieldOfUse.NotSelected)
            {
                //For more info, see https://developer.tobii.com/vr/develop/unity/documentation/configure-tobii-xr/
                Debug.LogError("Field of use has not been selected. Please specify intended field of use in TobiiXR_Settings (can also be edited in Window->Tobii->Tobii Settings)");
            }

            _eyeTrackingProvider = settings.EyeTrackingProvider;

            if (_eyeTrackingProvider == null)
            {
                _eyeTrackingProvider = new NoseDirectionProvider();
                Debug.LogWarning(string.Format("Creating ({0}) failed. Using ({1}) as fallback", settings.GetProviderType(), _eyeTrackingProvider.GetType().Name));
            }

            Debug.Log(string.Format("Starting TobiiXR with ({0}) as provider for eye tracking", _eyeTrackingProvider));

            Internal.Settings = settings;
            Internal.G2OM = settings.G2OM;

            VerifyInstanceIntegrity();

            return true;
        }

        public static void Stop()
        {
            if (_eyeTrackingProvider == null) return;

            Internal.G2OM.Destroy();
            _eyeTrackingProvider.Destroy();

            if (_updaterGameObject != null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Object.Destroy(_updaterGameObject.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(_updaterGameObject.gameObject);
                }
#else
            Object.Destroy(_updaterGameObject.gameObject);
#endif
            }


            _updaterGameObject = null;
            Internal.G2OM = null;
            _eyeTrackingProvider = null;
        }

        private static void VerifyInstanceIntegrity()
        {
            if (_updaterGameObject != null) return;

            _updaterGameObject = new GameObject("TobiiXR")
            {
                hideFlags = HideFlags.HideInHierarchy
            };

            if (_eyeTrackingProvider == null)
            {
                Start();
            }

            var updater = _updaterGameObject.AddComponent<TobiiXR_Lifecycle>();
            updater.OnUpdateAction += _eyeTrackingProvider.Tick;
            updater.OnUpdateAction += Tick;
            updater.OnDisableAction += Internal.G2OM.Clear;
            updater.OnApplicationQuitAction += Stop;
        }

        private static void Tick()
        {
            EyeTrackingDataHelper.Copy(_eyeTrackingProvider.EyeTrackingData, _eyeTrackingData);
            if (Internal.Filter != null) Internal.Filter.Filter(_eyeTrackingData);
            var data = CreateDeviceData(_eyeTrackingData);
            Internal.G2OM.Tick(data);
        }

        private static G2OM_DeviceData CreateDeviceData(TobiiXR_EyeTrackingData data)
        {
            var t = _eyeTrackingProvider.LocalToWorldMatrix;
            return new G2OM_DeviceData {
                timestamp = data.Timestamp,
                gaze_ray_world_space = new G2OM_GazeRay{
                    is_valid = data.GazeRay.IsValid.ToByte(),
                    ray = G2OM_UnityExtensionMethods.CreateRay(data.GazeRay.Origin, data.GazeRay.Direction),
                },
                camera_up_direction_world_space = t.MultiplyVector(Vector3.up).AsG2OMVector3(),
                camera_right_direction_world_space = t.MultiplyVector(Vector3.right).AsG2OMVector3()
            };
        }

        /// <summary>
        /// For advanced and internal use only. Do not access this field before TobiiXR.Start has been called.
        /// Do not save a reference to the fields exposed by this class since TobiiXR will recreate them when restarted
        /// </summary>
        public static TobiiXRInternal Internal { get { return _internal; } }

        public class TobiiXRInternal
        {
            public TobiiXR_Settings Settings { get; internal set; }

            public G2OM.G2OM G2OM { get; internal set; }

            /// <summary>
            /// Defaults to no filter. If set, both EyeTrackingData and FocusedObjects will apply this filter to gaze data before using it
            /// </summary>
            public IEyeTrackingFilter Filter { get { return Settings.EyeTrackingFilter; } }
        }
    }

    public enum FieldOfUse
    {
        NotSelected,
        Analytical,
        Interactive
    }
}