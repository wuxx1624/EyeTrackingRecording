using UnityEngine;
using Valve.VR;

public class ControllerLocomotion : MonoBehaviour
{
    public enum Direction { Viewpoint, Pointing };
    public Direction direction = Direction.Viewpoint;

    public float maxSpeed = 2.0f;
    public float moveDeadzone = 0.2f;
    public float pointingPitchAdjustment = 0.0f;
    public bool verticalFlying = false;
    public bool keyboardControl = false;

    public enum TurnMode { Continuous, Discrete, None };
    public TurnMode turnMode = TurnMode.Continuous;

    public float turnAngle = 45.0f;
    public float turnDuration = 0.3f;
    public float turnDeadzone = 0.75f;

    public SteamVR_Action_Vector2 moveAction;
    public SteamVR_Action_Pose poseAction;
    public SteamVR_Action_Boolean moveForwardAction;
    public SteamVR_Action_Boolean moveBackAction;
    public SteamVR_Action_Boolean turnLeftAction;
    public SteamVR_Action_Boolean turnRightAction;

    public float velocity;

    private Transform cameraTransform = null;
    private int turnDirection = 0;
    private float turnTotal = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = transform.Find("Camera");
    }

    // Update is called once per frame
    void Update()
    {
        velocity = 0.0f;
        Quaternion travelDirection = new Quaternion();

        if (moveAction != null && (moveAction.GetAxis(SteamVR_Input_Sources.Any).y >= moveDeadzone || moveAction.GetAxis(SteamVR_Input_Sources.Any).y <= -moveDeadzone))
        {
            velocity = moveAction.GetAxis(SteamVR_Input_Sources.Any).y * maxSpeed;
        }
        else if ((moveForwardAction != null && moveForwardAction.GetState(SteamVR_Input_Sources.Any)) || (keyboardControl && Input.GetKey(KeyCode.UpArrow)))
        {
            velocity = maxSpeed;
        }
        else if ((moveBackAction != null && moveBackAction.GetState(SteamVR_Input_Sources.Any)) || (keyboardControl && Input.GetKey(KeyCode.DownArrow)))
        {
            velocity = -maxSpeed;
        }


        if (velocity != 0.0f)
        {
            if (direction == Direction.Viewpoint)
            {
                travelDirection = cameraTransform.rotation;
            }
            else
            {
                travelDirection = transform.localRotation * poseAction.GetLocalRotation(SteamVR_Input_Sources.Any) * Quaternion.Euler(pointingPitchAdjustment, 0, 0);
            }
            

            if (!verticalFlying)
            {
                Vector3 restrictedTravelVector = travelDirection * new Vector3(0, 0, 1.0f);
                restrictedTravelVector[1] = 0.0f;
                restrictedTravelVector.Normalize();
                travelDirection = Quaternion.LookRotation(restrictedTravelVector);
            }

            Vector3 travelVector = travelDirection * new Vector3(0, 0, velocity * Time.deltaTime);
            

            transform.localPosition = transform.localPosition + travelVector;
        }


        if (turnMode != TurnMode.None && turnTotal == 0)
        {
            if ((moveAction != null && moveAction.GetAxis(SteamVR_Input_Sources.Any).x >= turnDeadzone) || (turnRightAction != null && turnRightAction.GetState(SteamVR_Input_Sources.Any)) || (keyboardControl && Input.GetKey(KeyCode.RightArrow)))
            {
                turnDirection = 1;
            }
            else if ((moveAction != null && moveAction.GetAxis(SteamVR_Input_Sources.Any).x <= -turnDeadzone) || (turnLeftAction != null && turnLeftAction.GetState(SteamVR_Input_Sources.Any)) || (keyboardControl && Input.GetKey(KeyCode.LeftArrow)))
            {
                turnDirection = -1;
            }
        }

        if (turnDirection != 0)
        {
            if (turnMode == TurnMode.Discrete)
            {
                float thisTurn = Time.deltaTime * turnAngle / turnDuration;

                if (turnTotal + thisTurn > turnAngle)
                    thisTurn = (turnTotal + thisTurn) - turnAngle;


                transform.Translate(cameraTransform.localPosition);
                transform.Rotate(0.0f, thisTurn * turnDirection, 0.0f);
                transform.Translate(-cameraTransform.localPosition);

                turnTotal += thisTurn;

                if (turnTotal >= turnAngle)
                {
                    turnDirection = 0;
                    turnTotal = 0;
                }
            }
            else if (turnMode == TurnMode.Continuous)
            {
                transform.Translate(cameraTransform.localPosition);
                transform.Rotate(0.0f, turnDirection * turnAngle / turnDuration * Time.deltaTime, 0.0f);
                transform.Translate(-cameraTransform.localPosition);
                turnDirection = 0;
                turnTotal = 0;
            }
        }
    }
}
