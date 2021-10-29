using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.Extras;

public class LaserPointerInput : BaseInputModule
{
    public SteamVR_LaserPointer[] laserPointers;

    private GameObject currentObject;
    private PointerEventData eventData;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        eventData = new PointerEventData(eventSystem);
    }

    public override void Process()
    {
        for (int i = 0; i < laserPointers.Length; i++)
        {
            if (laserPointers[i].pose.GetDeviceIndex() >= 0)
            {
                Camera rayCastCamera = laserPointers[i].GetComponent<Camera>();
                SteamVR_Action_Boolean clickAction = laserPointers[i].interactWithUI;

                eventData.Reset();
                eventData.position = new Vector2(rayCastCamera.pixelWidth / 2, rayCastCamera.pixelHeight / 2);

                eventSystem.RaycastAll(eventData, m_RaycastResultCache);
                eventData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
                currentObject = eventData.pointerCurrentRaycast.gameObject;

                m_RaycastResultCache.Clear();

                HandlePointerExitAndEnter(eventData, currentObject);

                if (clickAction.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    ProcessPress(eventData);
                }
                else if (clickAction.GetStateUp(SteamVR_Input_Sources.Any))
                {
                    ProcessRelease(eventData);
                }
                else if (clickAction.GetState(SteamVR_Input_Sources.Any))
                {
                    ProcessDrag(eventData);
                }
            }
        }
    }

    public PointerEventData GetData()
    {
        return eventData;
    }

    private void ProcessPress(PointerEventData data)
    {
        data.pointerPressRaycast = data.pointerCurrentRaycast;
        ExecuteEvents.ExecuteHierarchy(currentObject, data, ExecuteEvents.pointerClickHandler);

        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(currentObject, data, ExecuteEvents.pointerDownHandler);

        data.pressPosition = data.position;
        data.pointerPress = newPointerPress;
        data.rawPointerPress = currentObject;
    }

    private void ProcessRelease(PointerEventData data)
    {
        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);
        GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);

        if (data.pointerPress == pointerUpHandler)
        {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);
        }

        eventSystem.SetSelectedGameObject(null);
        data.pressPosition = Vector2.zero;
    }

    private void ProcessDrag(PointerEventData data)
    {
        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.dragHandler);
        GameObject pointerDragHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);

        if (data.pointerPress == pointerDragHandler)
        {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.dragHandler);
        }

        data.pressPosition = data.position;
    }
}
