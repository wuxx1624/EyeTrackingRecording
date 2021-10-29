// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using Tobii.XR.GazeModifier;
using Tobii.XR.GazeVisualizer;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Tobii.XR.DevTools
{
    public class DevToolsMenuController : MonoBehaviour
    {
        private G2OMVisualisation _debugVisualization;

#pragma warning disable 649
        [SerializeField] private GameObject _gazeModifierViz;

        [SerializeField] private GameObject _unGazeModifierViz;

        [SerializeField] private GameObject _openMenuButton;
        [SerializeField] private GameObject _toolkitMenu;

        [SerializeField] private GameObject _gazeModifierTools;

        [SerializeField] private DevToolsUITriggerGazeToggleButton _enableButton;
        [SerializeField] private DevToolsUITriggerGazeToggleButton _visualizeButton;
        [SerializeField] private DevToolsUITriggerGazeSlider _percentileSlider;
#pragma warning restore 649

        private GazeModifierSettings _settings;
        private bool _visualizersActive = true;
        private bool runonce = false;
        private bool started = false;

        public bool _startWithVisualizers = true;

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();   // Wait a sec so the default G2OM instance can be instantiated in the 

            bool settingsExists;
            _settings = GazeModifierSettings.CreateDefaultSettings(out settingsExists);
            string providerString = TobiiXR.Provider.ToString();

            _unGazeModifierViz.GetComponentInChildren<CustomProviderVisualizer>().ChangeProvider(providerString);
            _unGazeModifierViz.GetComponentInChildren<GazeVisualizer.GazeVisualizer>().ScaleAffectedByPrecision = false;

            var cameraTransform = CameraHelper.GetCameraTransform();
            _debugVisualization = cameraTransform.gameObject.AddComponent<G2OMVisualisation>();

            _visualizersActive = _startWithVisualizers;
            SetGazeVisualizer(_visualizersActive);
            started = true;
            EnsureCorrectVisualizer();
        }

        void LateUpdate()
        {
            if (started)
            {
                EnsureCorrectVisualizer();

                if (_toolkitMenu.activeInHierarchy)
                {
                    CheckMenuSettings();
                }
            }
        }

        public void ShowMenu(bool set)
        {
            _toolkitMenu.SetActive(set);
            _openMenuButton.SetActive(!set);
        }

        public void SetPercentile(int percentile)
        {
            if (_settings == null)
            {
                return;
            }

            //Mathf.RoundToInt(value * 100f);
            _settings.SelectedPercentileIndex = percentile;
            if (_toolkitMenu.activeInHierarchy &&
                _percentileSlider.Value != _settings.SelectedPercentileIndex)
            {
                _percentileSlider.SetSliderTo(percentile);
            }
        }

        public void SetMasterGazeModifier(bool set)
        {
            if (TobiiXR.Internal.Settings == null)
            {
                return;
            }

            TobiiXR.Internal.Settings.EyeTrackingFilterType = set ? typeof(GazeModifierFilter).AssemblyQualifiedName : null;
        }

        public void SetGazeVisualizer(bool set)
        {
            _visualizersActive = set;
        }

        private void EnsureCorrectVisualizer()
        {
            if (_visualizersActive && !_settings.Active)
            {
                _gazeModifierViz.SetActive(false);
                _unGazeModifierViz.SetActive(true);
            }
            else if (_visualizersActive && _settings.Active)
            {
                _gazeModifierViz.SetActive(true);
                _unGazeModifierViz.SetActive(false);
            }
            else if (!_visualizersActive)
            {
                _gazeModifierViz.SetActive(false);
                _unGazeModifierViz.SetActive(false);
            }
        }

        public void SetG2OMDebugView(bool set)
        {
            _debugVisualization.SetVisualization(set);
        }

        // makes sure that the settings from the Tobii Settings Unity window matches the debug window's settings
        private void CheckMenuSettings()
        {
            if (!runonce)
            {
                // make menu match our tobii xr settings when first opened
                runonce = true;
                _percentileSlider._sliderGraphics.UpdateValueText(
                    _settings.SelectedPercentileIndex); //force color change
            }

            if (string.IsNullOrEmpty(TobiiXR.Internal.Settings.EyeTrackingFilterType) == false)
            {
                _enableButton.ToggleOn();
            }
            else
            {
                _enableButton.ToggleOff();
            }

            if (_visualizersActive)
            {
                _visualizeButton.ToggleOn();
            }
            else
            {
                _visualizeButton.ToggleOff();
            }

            if (_percentileSlider.Value != _settings.SelectedPercentileIndex)
            {
                _percentileSlider.SetSliderTo(_settings.SelectedPercentileIndex);
            }
        }
    }
}
