using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(RagdollCharacterDriver))]
    public class CharacterInputHandler : MonoBehaviour
    {
        private Camera _cam;
        private RagdollCharacterDriver _characterDriver;
        private PlayerInput _input;
        private InputAction _moveInput;
        private Dictionary<InputAction, System.Action<InputAction.CallbackContext>> _actionCallbacks;
        private bool _isInitialized;

        private const string DebugGroup = "DEBUG";
        [SerializeField, BoxGroup(DebugGroup),
         Tooltip("Enable if you are dropping a PlayerCharacter in the scene " +
                 "(instead of spawning from PlayerInputManager)")]
        private bool initializeSelf;

        private void Awake()
        {
            _cam = Camera.main;
            _isInitialized = false;
            _characterDriver = GetComponent<RagdollCharacterDriver>();
            _actionCallbacks = new Dictionary<InputAction, System.Action<InputAction.CallbackContext>>();
            if (initializeSelf)
            {
                Debug.Log("Initializing self.", this);
                var input = GetComponent<PlayerInput>();
                if (!input.enabled)
                {
                    Debug.LogWarning($"{nameof(PlayerInput)} was disabled. Enabling it now.", input);
                    input.enabled = true;
                }
                BindInputs(GetComponent<PlayerInput>());
            }
        }

        private void Update()
        {
            if (!_isInitialized)
                return;
            OnMove(_moveInput.ReadValue<Vector2>());
        }

        public void BindInputs(PlayerInput playerInput)
        {
            //playerInput.actions["Move"].performed += OnMove;
            _input = playerInput;
            _moveInput = playerInput.actions["Move"];
            // Need to store reference to our callbacks so we can unsubscribe OnDisable
            // (otherwise we get null callbacks when no domain reload)
            AddCallback(playerInput.actions["AttackLeft"], OnAttackLeft);
            AddCallback(playerInput.actions["AttackRight"], OnAttackRight);
            _isInitialized = true;
        }

        private void OnDisable()
        {
            foreach (var action in _input.actions)
            {
                if (_actionCallbacks.TryGetValue(action, out var callback))
                {
                    action.performed -= callback;
                }
            }
        }

        private void AddCallback(InputAction inputAction, System.Action<InputAction.CallbackContext> callback)
        {
            inputAction.performed += callback;
            _actionCallbacks[inputAction] = callback;
        }

        private static Vector3 MoveVectorRelativeToWorld(Transform cameraTf, in Vector2 moveInputRaw)
        {
            var cameraForwardLateral = Vector3.ProjectOnPlane(
                cameraTf.forward, Vector3.up).normalized;
            var cameraPlanarRotation = Quaternion.LookRotation(cameraForwardLateral);
            var moveInputLocal = new Vector3(moveInputRaw.x, 0f, moveInputRaw.y);
            var moveInputWorldPlane = cameraPlanarRotation * moveInputLocal;
            if (moveInputWorldPlane.sqrMagnitude > 1f + float.Epsilon)
            {
                moveInputWorldPlane.Normalize();
            }

            return moveInputWorldPlane;
        }

        public void OnMove(InputAction.CallbackContext ctx)
            => OnMove(ctx.ReadValue<Vector2>());
        public void OnMove(InputValue val) => OnMove(val.Get<Vector2>());
        
        private void OnMove(Vector2 moveRelative)
        {
            var moveWorld = MoveVectorRelativeToWorld(_cam.transform, moveRelative);
            _characterDriver.SetMove(moveWorld);
        }

        private void OnAttackLeft(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;
            
            _characterDriver.AttackLeft();
        }

        private void OnAttackRight(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;
            
            _characterDriver.AttackRight();
        }
    }
}
