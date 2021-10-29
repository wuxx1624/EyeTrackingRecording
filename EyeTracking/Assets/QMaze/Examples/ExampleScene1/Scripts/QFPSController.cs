using UnityEngine;
using System.Collections;

namespace qtools.qmaze.example1
{
	public class QFPSController : MonoBehaviour
	{		
		public float mouseSensitivityX = 1.0f;
		public float mouseSensitivityY = 1.0f;
		public float moveScaleX = 1.0f;
		public float moveScaleY = 1.0f;
		public float moveMaxSpeed = 1.0f;
		public float moveLerp = 0.9f;

		private Rigidbody rigidBody;
		private Quaternion rotationTargetHorizontal;
		private Quaternion rotationTargetVertical;
		private Transform cameraTransform;

		void Start () 
		{
			rotationTargetHorizontal = transform.rotation;
			rigidBody = GetComponent<Rigidbody>();
			cameraTransform = transform.GetChild(0);
			rotationTargetVertical = cameraTransform.rotation;
		}

		void Update () 
		{
			Vector3 velocity = (transform.right   * Input.GetAxis("Horizontal") * moveScaleX);
					velocity+= (transform.forward * Input.GetAxis("Vertical")   * moveScaleY);
			velocity = Vector3.ClampMagnitude(velocity, moveMaxSpeed);
			velocity *= moveLerp;
			velocity.y = rigidBody.velocity.y;
			rigidBody.velocity = velocity;

			rotationTargetHorizontal *= Quaternion.Euler (0, Input.GetAxis("Mouse X") * mouseSensitivityX, 0f);
			transform.rotation = rotationTargetHorizontal;	

			Quaternion nextRotationTargetVertical = rotationTargetVertical * Quaternion.Euler(Input.GetAxis("Mouse Y") * mouseSensitivityY, 0, 0);
			if (nextRotationTargetVertical.eulerAngles.x < 90 || nextRotationTargetVertical.eulerAngles.x > 270)
			{
				rotationTargetVertical = nextRotationTargetVertical;
				cameraTransform.localRotation = rotationTargetVertical;
			}
		}

		public void setRotation(Quaternion rotation)
		{
			this.rotationTargetHorizontal = rotation;
			transform.rotation = rotation;

			rotationTargetVertical = Quaternion.identity;
			cameraTransform.rotation = rotationTargetVertical;
		}
	}
}