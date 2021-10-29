
using Tobii.XR;

public interface IEyeTrackingFilter
{
    /// <summary>
    /// Applies filter to data parameter
    /// </summary>
    /// <param name="data"></param>
    void Filter(TobiiXR_EyeTrackingData data);
}
