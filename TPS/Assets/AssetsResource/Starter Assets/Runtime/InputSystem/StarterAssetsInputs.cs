using UnityEngine;
using UnityEngine.InputSystem.Interactions;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool aim;
		public bool shoot;
		public bool reload;
		
		[Header("Charge Input")]
		public bool charge;                 // 현재 Q가 눌려있는지
		public float lastChargeTime;        // 직전에 뗐을 때 누르고 있던 시간(초)
		public float chargeMax = 9f;        // 최대 차징 제한 (원하면 변경)
		private float _chargeStartTime = -1f;
		private bool _prevCharge;           // 에지 감지용 (필요하면 외부에서 활용)
		
		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		
		[Header("Aim/Zoom classification")]
		public InputActionReference aimAction;   // Player/Aim 액션 드래그
		[HideInInspector] public bool adsHolding; // Hold로 견착 중인지

		private bool _zoomTapPending;             // Tap 한번 발생 신호 저장

#if ENABLE_INPUT_SYSTEM
		private void OnEnable()
		{
			if (aimAction != null)
			{
				var a = aimAction.action;
				a.Enable();
				a.performed += OnAimPerformed;
				a.canceled  += OnAimCanceled;
			}
		}

		private void OnDisable()
		{
			if (aimAction != null)
			{
				var a = aimAction.action;
				a.performed -= OnAimPerformed;
				a.canceled  -= OnAimCanceled;
			}
		}
		private void OnAimPerformed(InputAction.CallbackContext ctx)
		{
			if (ctx.interaction is HoldInteraction)
			{
				// 길게 눌러 견착 시작
				adsHolding = true;
			}
			else if (ctx.interaction is TapInteraction)
			{
				// 짧게 탭 -> 줌 토글 신호 1회성 저장
				_zoomTapPending = true;
			}
			else
			{
				// 인터랙션 미설정 보호: 탭처럼 취급
				_zoomTapPending = true;
			}
		}

		private void OnAimCanceled(InputAction.CallbackContext ctx)
		{
			// 버튼에서 손을 떼면 견착 해제
			adsHolding = false;
		}

		// PlayerManager가 매 프레임 호출해서 1회성으로 가져감
		public bool ConsumeZoomTap()
		{
			if (_zoomTapPending)
			{
				_zoomTapPending = false;
				return true;
			}
			return false;
		}
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}
		
		
		public void OnShoot(InputValue value)
		{
			ShootInput(value.isPressed);
		}
		public void OnReLoad(InputValue value)
		{
			ReLoadInput(value.isPressed);
		}
		public void OnCharge(InputValue value)
		{
			bool pressed = value.isPressed;

			// 눌리는 순간
			if (pressed && !charge)
			{
				_chargeStartTime = Time.time;
				lastChargeTime = 0f; // 초기화
			}

			// 떼는 순간
			if (!pressed && charge && _chargeStartTime >= 0f)
			{
				float held = Time.time - _chargeStartTime;
				lastChargeTime = Mathf.Clamp(held, 0f, chargeMax);
				_chargeStartTime = -1f;
			}

			_prevCharge = charge;
			charge = pressed;
		}
		
#endif
		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		
		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}
		public void ShootInput(bool newShootState)
		{
			shoot = newShootState;
		}
		public void ReLoadInput(bool newReLoadState)
		{
			reload = newReLoadState;
		}
		
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
		public bool ConsumeChargeReleased(out float chargeSeconds)
		{
			// 지금은 안 눌려 있고, 직전에 눌려있던 경우를 감지하려면
			// PlayerInput의 SendMessages 특성상 OnCharge에서 이미 lastChargeTime을 갱신해둠.
			// 여기서는 lastChargeTime이 유효하면 꺼내주고 0으로 리셋하는 패턴.
			if (!charge && lastChargeTime > 0f)
			{
				chargeSeconds = lastChargeTime;
				lastChargeTime = 0f; // 한 번 소비 후 초기화
				return true;
			}
			chargeSeconds = 0f;
			return false;
		}
	}
	
}