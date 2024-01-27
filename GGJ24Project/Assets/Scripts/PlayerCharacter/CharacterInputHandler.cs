using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(RagdollLocomotion))]
    [RequireComponent(typeof(RagdollCombat))]
    public class CharacterInputHandler : MonoBehaviour
    {
        private Camera _cam;
        private RagdollLocomotion _locomotion;
        private RagdollCombat _combat;
        private PlayerInput _input;
        private InputAction _moveInput;
        private InputAction _runInput;
        private Dictionary<InputAction, System.Action<InputAction.CallbackContext>> _actionCallbacks;
        private bool _isInitialized;

        private const string DebugGroup = "DEBUG";
        [SerializeField, BoxGroup(DebugGroup),
         Tooltip("Enable if you are dropping a PlayerCharacter in the scene " +
                 "(instead of spawning from PlayerInputManager)")]
        private bool initializeSelf;

        [Button]
        public void FlushCallbacks()
        {
            foreach (var evt in _input.actionEvents)
            {
                evt.RemoveAllListeners();
            }
            BindInputs(_input);
        }

        private void Awake()
        {
            _cam = Camera.main;
            _isInitialized = false;
            _locomotion = GetComponent<RagdollLocomotion>();
            _combat = GetComponent<RagdollCombat>();
            if (!initializeSelf)
                return;
            
            Debug.Log("Initializing self.", this);
            _input = GetComponent<PlayerInput>();
            if (!_input.enabled)
            {
                Debug.LogWarning($"{nameof(PlayerInput)} was disabled. Enabling it now.", _input);
                _input.enabled = true;
            }
            FlushCallbacks();
        }

        private void Update()
        {
            if (!_isInitialized)
                return;
            OnMove(_moveInput.ReadValue<Vector2>());
            
        }

        public void BindInputs(PlayerInput playerInput)
        {
            _actionCallbacks = new Dictionary<InputAction, System.Action<InputAction.CallbackContext>>();
            //playerInput.actions["Move"].performed += OnMove;
            _input = playerInput;
            _moveInput = playerInput.actions["Move"];
            _runInput = playerInput.actions["Run"];
            // Need to store reference to our callbacks so we can unsubscribe OnDisable
            // (otherwise we get null callbacks when no domain reload)
            AddCallback(playerInput.actions["AttackLeft"], OnAttackLeft);
            AddCallback(playerInput.actions["AttackRight"], OnAttackRight);
            //AddCallback(playerInput.actions["Run"], OnRun);
            _isInitialized = true;
        }

        private void OnDisable()
        {
            foreach (var action in _actionCallbacks.Keys)
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
            _locomotion.SetMove(moveWorld);
        }

        private void OnAttackLeft(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;
            
            _combat.TryPerform(AttackType.SlapLeft);
        }

        private void OnAttackRight(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;
            
            _combat.TryPerform(AttackType.SlapRight);
        }
    }
}
