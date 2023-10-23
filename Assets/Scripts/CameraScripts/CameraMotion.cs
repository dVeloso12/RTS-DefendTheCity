using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMotion : MonoBehaviour{		
		public bool cantMoveScreen;
		[SerializeField] private float _speed = 1f;
		[SerializeField] private float _smoothing = 5f;
		[SerializeField] private Vector2 _range = new (100, 100);
		
		private Vector3 _targetPosition;
		private Vector3 _input;
    public Vector2 screenPos;


		private void Awake() {
			_targetPosition = transform.position;
		}
			
		private void HandleInput() {

		
      		Vector2 X_Z_Values = checkPositionInScreen();
			Vector3 right = transform.right * X_Z_Values.x;
			Vector3 forward = transform.forward * X_Z_Values.y;

			_input = (forward + right).normalized;
			
		}

    Vector2 checkPositionInScreen(){
      int screenDivider = 20;
      Vector2 XZ = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
		float HSection = Screen.height / screenDivider;
		float wSection = Screen.width / screenDivider;

		screenPos = Input.mousePosition;

		if (screenPos.x >= 0 || screenPos.y >= 0 ||
			screenPos.x <= Screen.width || Input.mousePosition.y <= Screen.height)
		{

			if (screenPos.y < HSection)
			{
				XZ.y = -1;
			}
			else if (screenPos.y > (Screen.height - HSection))
			{
				XZ.y = 1;
			}
			if (screenPos.x < wSection)
			{
				XZ.x = -1;
			}
			else if (screenPos.x > (Screen.width - wSection))
			{
				XZ.x = 1;

			}
		}

		return XZ;
    }

		private void Move() {
			Vector3 nextTargetPosition = _targetPosition + _input * _speed;
			if (IsInBounds(nextTargetPosition)) _targetPosition = nextTargetPosition;
			transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _smoothing);
		}

		private bool IsInBounds(Vector3 position) {
			return position.x > -_range.x &&
				   position.x < _range.x &&
				   position.z > -_range.y &&
				   position.z < _range.y;
		}
		
		private void Update() {

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (cantMoveScreen) cantMoveScreen = false;
			else if (!cantMoveScreen) cantMoveScreen = true;
		}
	
			if(!cantMoveScreen)
			{
			HandleInput();
			Move();
			}
		}

}
