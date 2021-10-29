namespace Tobii.XR
{
    public interface ISettingsEditorExtension
    {
        int Priority { get; }
        void Init(TobiiXR_Settings _settings);
        void Render();
    }
}