using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class FixSpeedLocomotion : MonoBehaviour
{
    [System.Serializable]
    public class Velocity
    {
        public float speed;
        public float maxSpeed = 2.0f;
        public float moveDeadzone = 0.5f;
        public float _slew = 0.0f;
        public float _smoothing = 0.3f;
        public float _smoothed = 0.0f;
    }
    public SteamVR_Action_Vector2 moveAction;
    public SteamVR_Action_Boolean clickTrackpad;
    //private bool padClicked = false;

    public enum ControllerType { ViveCosmos, Vive };
    public ControllerType controllerType;

    public Velocity velocity;
    public Velocity angularVelocity;
    private Transform cameraTransform = null;
    private Quaternion travelDirection;


    // Start is called before the first frame update
    void Start()
    {

        cameraTransform = transform.Find("Camera");
        //controllerType = (GlobalControl.Instance.savedData.joystick) ? ControllerType.ViveCosmos : ControllerType.Vive;
        travelDirection = new Quaternion();
    }

    // Update is called once per frame
    void Update()
    {

        if (controllerType == ControllerType.ViveCosmos)
            UpdateForViveCosmos();
        else
            UpdateForVive();
    }

    void UpdateForViveCosmos()
    {
        // velocity
        if (moveAction != null && (moveAction.GetAxis(SteamVR_Input_Sources.Any).y >= velocity.moveDeadzone || moveAction.GetAxis(SteamVR_Input_Sources.Any).y <= -velocity.moveDeadzone))
        {
            velocity.speed = Mathf.Sign(moveAction.GetAxis(SteamVR_Input_Sources.Any).y) * velocity.maxSpeed;
        }
        else
            velocity.speed = 0.0f;
        velocity._smoothed = Mathf.SmoothDamp(velocity._smoothed, velocity.speed, ref velocity._slew, velocity._smoothing);
        if (velocity._smoothed != 0.0f)
        {
            travelDirection = cameraTransform.rotation;
            Vector3 restrictedTravelVector = travelDirection * new Vector3(0, 0, 1.0f);
            restrictedTravelVector[1] = 0.0f;
            restrictedTravelVector.Normalize();
            travelDirection = Quaternion.LookRotation(restrictedTravelVector);
            Vector3 travelVector = travelDirection * new Vector3(0, 0, velocity._smoothed * Time.deltaTime);
            transform.position = transform.position + travelVector;
        }


        // angular velocity
        angularVelocity.speed = 0.0f;
        if (moveAction != null && (moveAction.GetAxis(SteamVR_Input_Sources.Any).x >= angularVelocity.moveDeadzone || moveAction.GetAxis(SteamVR_Input_Sources.Any).x <= -angularVelocity.moveDeadzone))
        {
            angularVelocity.speed = Mathf.Sign(moveAction.GetAxis(SteamVR_Input_Sources.Any).x) * angularVelocity.maxSpeed;
        }
        angularVelocity._smoothed = Mathf.SmoothDamp(angularVelocity._smoothed, angularVelocity.speed, ref angularVelocity._slew, angularVelocity._smoothing);
        if (angularVelocity._smoothed != 0.0f)
        {
            transform.RotateAround(cameraTransform.position, Vector3.up, angularVelocity._smoothed * Time.deltaTime);
        }
    }

    void UpdateForVive()
    {
        // velocity
        if (clickTrackpad != null && clickTrackpad.GetState(SteamVR_Input_Sources.Any) && moveAction != null && (moveAction.GetAxis(SteamVR_Input_Sources.Any).y >= velocity.moveDeadzone || moveAction.GetAxis(SteamVR_Input_Sources.Any).y <= -velocity.moveDeadzone))
        {
            velocity.speed = Mathf.Sign(moveAction.GetAxis(SteamVR_Input_Sources.Any).y) * velocity.maxSpeed;
        }
        else
            velocity.speed = 0.0f;
        velocity._smoothed = Mathf.SmoothDamp(velocity._smoothed, velocity.speed, ref velocity._slew, velocity._smoothing);
        if (velocity._smoothed != 0.0f)
        {
            travelDirection = cameraTransform.rotation;
            Vector3 restrictedTravelVector = travelDirection * new Vector3(0, 0, 1.0f);
            restrictedTravelVector[1] = 0.0f;
            restrictedTravelVector.Normalize();
            travelDirection = Quaternion.LookRotation(restrictedTravelVector);
            Vector3 travelVector = travelDirection * new Vector3(0, 0, velocity._smoothed * Time.deltaTime);
            transform.position = transform.position + travelVector;
        }


        // angular velocity
        angularVelocity.speed = 0.0f;
        if (clickTrackpad != null && clickTrackpad.GetState(SteamVR_Input_Sources.Any))
        {
            if (moveAction != null && (moveAction.GetAxis(SteamVR_Input_Sources.Any).x >= angularVelocity.moveDeadzone || moveAction.GetAxis(SteamVR_Input_Sources.Any).x <= -angularVelocity.moveDeadzone))
            {
                angularVelocity.speed = Mathf.Sign(moveAction.GetAxis(SteamVR_Input_Sources.Any).x) * angularVelocity.maxSpeed;
            }
        }
        angularVelocity._smoothed = Mathf.SmoothDamp(angularVelocity._smoothed, angularVelocity.speed, ref angularVelocity._slew, angularVelocity._smoothing);
        if (angularVelocity._smoothed != 0.0f)
        {
            transform.RotateAround(cameraTransform.position, Vector3.up, angularVelocity._smoothed * Time.deltaTime);
        }
    }
}
