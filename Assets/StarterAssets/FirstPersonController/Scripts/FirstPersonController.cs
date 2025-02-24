using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

        [Header("Stamina System")]
        public float MaxStamina = 5.0f;
        public float StaminaDrainRate = 1.0f;
        public float StaminaRecoveryRate = 0.5f;
        public float TiredSpeedMultiplier = 0.75f;
        private float _currentStamina;
        private bool _isTired;
		public StaminaUI StaminaUI;

        [Header("Dizziness Effect")]
        public float DizzinessIntensity = 5.0f;
        public float DizzinessDuration = 5.0f;
        public float CameraResetSpeed = 2.0f;
        private float _dizzinessTimer;
        private bool _isDizzy;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		[Header("Head Bobbing")]
		public float WalkBobFrequency = 6.0f;   // Frequência do balanço ao andar
		public float WalkBobAmplitude = 0.02f;  // Intensidade do balanço ao andar
		public float SprintBobFrequency = 10.0f; // Frequência do balanço ao correr
		public float SprintBobAmplitude = 0.05f; // Intensidade do balanço ao correr
		

		[Header("Idle Breathing")]
		public float BreathingFrequency = 1.5f; // Frequência do movimento de respiração
		public float BreathingAmplitude = 0.02f; // Intensidade da respiração

		

		// cinemachine
		private float _cinemachineTargetPitch;
		private Vector3 _cameraInitialLocalPosition;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		// head bobbing
		private float _headBobbingTimer = 0.0f;

		// idle breathing
		private float _breathingTimer = 0.0f;

	
#if ENABLE_INPUT_SYSTEM
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif
            _cameraInitialLocalPosition = CinemachineCameraTarget.transform.localPosition;
            _currentStamina = MaxStamina;
        }

		private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			HandleStamina();
			Move();

		}

		private void LateUpdate()
		{
			CameraRotation();
			ApplyCameraMotion();
            ApplyDizzinessEffect();
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// Se houver entrada do jogador
			if (_input.look.sqrMagnitude >= _threshold)
			{
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// Mantém a rotação vertical dentro dos limites
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Aplica a rotação na câmera sem alterar sua posição
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// Garante que a posição da câmera não seja alterada
				CinemachineCameraTarget.transform.localPosition = _cameraInitialLocalPosition;

				// Rotaciona o jogador na horizontal
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void ApplyCameraMotion()
		{
			if (_controller.velocity.magnitude > 0.1f && Grounded)
			{
				// Diferenciar entre andar e correr
				float bobFrequency = _input.sprint ? SprintBobFrequency : WalkBobFrequency;
				float bobAmplitude = _input.sprint ? SprintBobAmplitude : WalkBobAmplitude;

				// Head bobbing (movimento ao andar/correr)
				_headBobbingTimer += Time.deltaTime * bobFrequency;
				float bobOffset = Mathf.Sin(_headBobbingTimer) * bobAmplitude;
				CinemachineCameraTarget.transform.localPosition = _cameraInitialLocalPosition + new Vector3(0, bobOffset, 0);
			}
			else
			{
				// Movimento de respiração (quando parado)
				_breathingTimer += Time.deltaTime * BreathingFrequency;
				float breathOffsetY = Mathf.Sin(_breathingTimer) * BreathingAmplitude; // Leve sobe e desce
				float breathOffsetX = Mathf.Cos(_breathingTimer * 0.5f) * (BreathingAmplitude / 2); // Pequeno balanço lateral
				CinemachineCameraTarget.transform.localPosition = _cameraInitialLocalPosition + new Vector3(breathOffsetX, breathOffsetY, 0);
			}
		}

		private void HandleStamina()
        {
			if (_input.sprint && _speed == SprintSpeed)
			{
				_currentStamina -= StaminaDrainRate * Time.deltaTime;
				if (_currentStamina <= 0)
				{
					_currentStamina = 0;
					_isTired = true;
					_isDizzy = true;
					_dizzinessTimer = DizzinessDuration;
				}
			}
			else
			{
				_currentStamina += StaminaRecoveryRate * Time.deltaTime;
				if (_currentStamina >= MaxStamina)
				{
					_currentStamina = MaxStamina;
					_isTired = false;
				}
			}

			if (StaminaUI != null)
			{
				StaminaUI.UpdateStamina(_currentStamina, MaxStamina, _isTired); // Atualiza a barra no UI
			}
		}

        private void ApplyDizzinessEffect()
        {
		if (_isDizzy)
		{
			_dizzinessTimer -= Time.deltaTime;
			float shakeAmount = Mathf.Sin(Time.time * DizzinessIntensity) * 2.0f; // Ajusta a intensidade do balanço
			CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, shakeAmount, shakeAmount * 0.5f);
			
			if (_dizzinessTimer <= 0)
			{
				_isDizzy = false;
			}
		}
		else if (CinemachineCameraTarget.transform.localRotation != Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f))
		{
			CinemachineCameraTarget.transform.localRotation = Quaternion.Lerp(
				CinemachineCameraTarget.transform.localRotation,
				Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f), 
				CameraResetSpeed * Time.deltaTime
			);
		}
	}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
				float targetSpeed = _input.sprint && !_isTired ? SprintSpeed : MoveSpeed;

				if (_isDizzy)
				{
					targetSpeed *= TiredSpeedMultiplier; // Reduz a velocidade durante a tontura
				}

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}