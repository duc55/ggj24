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
        private bool _wasHoldingRun;
        
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

        private void Start()
        {
            _wasHoldingRun = false;
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
            var runIsPressed = _runInput.IsPressed();
            if (_wasHoldingRun == runIsPressed)
                return;
            
            if (runIsPressed)
            {
                _locomotion.StartRunning();
            }
            else
            {
                _locomotion.StopRunning();
            }

            _wasHoldingRun = runIsPressed;
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
            if (!_isInitialized)
                return;
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


        public void OnMove(InputAction.CallbackContext ctx)
            => OnMove(ctx.ReadValue<Vector2>());
        public void OnMove(InputValue val) => OnMove(val.Get<Vector2>());
        
        private void OnMove(Vector2 moveRelative)
        {
            _locomotion.SetMoveFromCamera(moveRelative);
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
