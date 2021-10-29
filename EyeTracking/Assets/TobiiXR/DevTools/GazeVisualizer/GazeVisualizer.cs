// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using Tobii.XR.GazeModifier;
using UnityEngine;

namespace Tobii.XR.GazeVisualizer
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GazeVisualizer : MonoBehaviour
    {
        public enum GazeVisualizerType
        {
            Default,
            Bubble,
        }

        public IEyeTrackingProvider EyetrackingProvider { get; set; }

        public bool ScaleAffectedByPrecision;

#pragma warning disable 649
        [SerializeField] private GazeVisualizerType _visualizerType;

        [SerializeField] private bool _smoothMove = true;

        [SerializeField] [Range(1, 30)] private int _smoothMoveSpeed = 7;
#pragma warning restore 649

        private float ScaleFactor
        {
            get { return _visualizerType == GazeVisualizerType.Bubble ? 0.03f : 0.003f; }
        }

        private float _defaultDistance;

        private Camera _mainCamera;

        private SpriteRenderer _spriteRenderer;
        private Vector3 _lastGazeDirection;

        private const float OffsetFromFarClipPlane = 10f;
        private const float PrecisionAngleScaleFactor = 5f;
        
        private void Start()
        {
            _mainCamera = CameraHelper.GetMainCamera();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _defaultDistance = _mainCamera.farClipPlane - OffsetFromFarClipPlane;
        }

        void Update()
        {
            var provider = EyetrackingProvider ?? TobiiXR.Provider;
            var gazeModifierFilter = TobiiXR.Internal.Filter as GazeModifierFilter;

            var eyeTrackingData = EyeTrackingDataHelper.Clone(provider.EyeTrackingData);
            if (ScaleAffectedByPrecision && gazeModifierFilter != null) gazeModifierFilter.FilterAccuracyOnly(eyeTrackingData);
            var gazeRay = eyeTrackingData.GazeRay;

            _spriteRenderer.enabled = gazeRay.IsValid;

            if (_spriteRenderer.enabled == false) return;

            SetPositionAndScale(gazeRay);

            if (ScaleAffectedByPrecision && gazeModifierFilter != null)
            {
                UpdatePrecisionScale(gazeModifierFilter.GetMaxPrecisionAngleDegrees(eyeTrackingData.GazeRay.Direction));
            }
        }

        public void SetPositionAndScale(TobiiXR_GazeRay gazeRay)
        {
            RaycastHit hit;
            var distance = _defaultDistance;
            if (Physics.Raycast(gazeRay.Origin, gazeRay.Direction, out hit))
            {
                distance = hit.distance;
            }

            var interpolatedGazeDirection = Vector3.Lerp(_lastGazeDirection, gazeRay.Direction,
                _smoothMoveSpeed * Time.unscaledDeltaTime);

            var usedDirection = _smoothMove ? interpolatedGazeDirection.normalized : gazeRay.Direction.normalized;
            transform.position = gazeRay.Origin + usedDirection * distance;

            transform.localScale = Vector3.one * distance * ScaleFactor;

            transform.forward = usedDirection.normalized;

            _lastGazeDirection = usedDirection;
        }

        private void UpdatePrecisionScale(float maxPrecisionAngleDegrees)
        {
            transform.localScale *= (1f + GetScaleAffectedByPrecisionAngle(maxPrecisionAngleDegrees));
        }

        private static float GetScaleAffectedByPrecisionAngle(float maxPrecisionAngleDegrees)
        {
            return maxPrecisionAngleDegrees * Mathf.Sin(maxPrecisionAngleDegrees * Mathf.Deg2Rad) * PrecisionAngleScaleFactor;
        }
    }
}