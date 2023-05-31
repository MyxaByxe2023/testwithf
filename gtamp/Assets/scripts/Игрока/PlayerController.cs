using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _controller;

    [SerializeField]
    private float _playerSpeed = 5f;

    [SerializeField]
    private float _rotationSpeed = 10f;

    [SerializeField]
    private Camera _followCamera;

    private Vector3 _playerVelocity;
    private bool _groundedPlayer;

    [SerializeField]
    private float _jumpHeight = 1.0f;
    [SerializeField]
    private float _gravityValue = -9.81f;
	
	
	[SerializeField]
    private float StandardSpeed;
	
	[SerializeField]
    private float ShiftSpeed;
	
    private float Speed;


    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        _groundedPlayer = _controller.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementInput = Quaternion.Euler(0, _followCamera.transform.eulerAngles.y, 0) * new Vector3(horizontalInput, 0, verticalInput);
        Vector3 movementDirection = movementInput.normalized;

        _controller.Move(movementDirection * _playerSpeed * Time.deltaTime);

        if (movementDirection != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, _rotationSpeed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && _groundedPlayer)
        {
            _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
        }
		
		#region Ускорение
		
		StartCoroutine(SpeedReckoner());
		
		if(_playerSpeed == StandardSpeed && Camera.main.fieldOfView >= 60){
			Camera.main.fieldOfView -= 2f;
		}
		else if(_playerSpeed == ShiftSpeed && Camera.main.fieldOfView <= 90){
			Camera.main.fieldOfView += 2f;
		}
		
		#endregion
		
        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);
    }
	
	IEnumerator SpeedReckoner(){
		Vector3 position_1 = this.transform.position;
		yield return new WaitForSeconds(0.1f);
		Vector3 position_2 = this.transform.position;
		Vector3 speedVector = position_1 - position_2;
		float speed = Mathf.Abs(speedVector.magnitude);
		if(Input.GetKey(KeyCode.LeftShift) && speed > 0.1f){
			_playerSpeed = ShiftSpeed;
		}
		else{
			_playerSpeed = StandardSpeed;
		}
	}
}
