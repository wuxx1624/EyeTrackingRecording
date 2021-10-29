// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using UnityEngine;
using System.Collections.Generic;

namespace Tobii.XR.GazeModifier
{
    public class GazeModifierFilter : IEyeTrackingFilter
    {
        private readonly IEnumerable<IGazeModifier> _modifiers;
        private readonly IGazeModifierSettings _settings;
        private readonly ICameraHelper _camera;
        private AccuracyModifier _accuracyModifier;
        private PrecisionModifier _precisionModifier;

        public GazeModifierFilter() : this(GazeModifierSettings.CreateDefaultSettings(), new CameraHelper())
        {

        }

        public GazeModifierFilter(IGazeModifierSettings settings, ICameraHelper camera, IEnumerable<IGazeModifier> modifiers = null)
        {
            var repo = new PercentileRepository();
            _settings = settings;
            _camera = camera ?? new CameraHelper();
            _accuracyModifier = new AccuracyModifier(repo, _settings);
            _precisionModifier = new PrecisionModifier(repo, _settings);
            _modifiers = modifiers ?? new List<IGazeModifier>() { _accuracyModifier, _precisionModifier, new TrackabilityModifier(repo, _settings) };
        }

        public void Filter(TobiiXR_EyeTrackingData data)
        {
            if (_settings.Active)
            {
                foreach (var gazeModifier in _modifiers)
                {
                    gazeModifier.Modify(data, _camera.Forward);
                }
            }
        }

        public void FilterAccuracyOnly(TobiiXR_EyeTrackingData data)
        {
            _accuracyModifier.Modify(data, _camera.Forward);
        }

        public float GetMaxPrecisionAngleDegrees(Vector3 gazeDirection)
        {
            return _precisionModifier.GetMaxAngle(gazeDirection, _camera.Forward);
        }
    }
}
