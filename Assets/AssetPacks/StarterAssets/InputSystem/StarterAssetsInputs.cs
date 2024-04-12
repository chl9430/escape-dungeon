using System.Diagnostics;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.Windows;
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
		public bool talk;
		public bool showInventory;
		public bool showQuest;
        public bool interact;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

        // 플레이어
        PlayerManager playerManager;

#if ENABLE_INPUT_SYSTEM
        void Awake()
        {
            // 플레이어 가져오기
            playerManager = GetComponent<PlayerManager>();
        }

        public void OnMove(InputValue value)
		{
            MoveInput(value.Get<Vector2>());
        }

		public void OnLook(InputValue value)
		{
            if (cursorInputForLook)
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

        public void OnReload(InputValue value)
        {
            if (!playerManager.IsTalking
                && !playerManager.IsInventory
                && !playerManager.IsReloading
                && !playerManager.IsInteracting
                && !playerManager.IsDamaged
                && !GameManager.instance.IsWatching)
            {
                ReloadInput(value.isPressed);
            }
        }

        public void OnTalk(InputValue value)
        {
            if (!playerManager.IsInventory
                && !playerManager.IsInteracting
                && !GameManager.instance.IsWatching)
            {
                TalkInput(value.isPressed);
            }
        }

        public void OnShowInventory(InputValue value)
        {
            if (!playerManager.IsTalking && !GameManager.instance.IsWatching)
            {
                ShowInventoryInput(value.isPressed);
            }
        }

        public void OnShowQuest(InputValue value)
        {
            if (!playerManager.IsTalking && !GameManager.instance.IsWatching)
            {
                ShowQuestInput(value.isPressed);
            }
        }

        public void OnInteract(InputValue value)
        {
            InteractInput(value.isPressed);
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

        public void ReloadInput(bool newReloadState)
        {
            reload = newReloadState;
        }

        public void TalkInput(bool newTalkState)
        {
            talk = newTalkState;
        }
		
		public void ShowInventoryInput(bool newShowInventoryState)
		{
			showInventory = newShowInventoryState;
		}

        public void ShowQuestInput(bool newShowQuestState)
        {
            showQuest = newShowQuestState;
        }

        public void InteractInput(bool newInteractState)
        {
            interact = newInteractState;
        }

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
}