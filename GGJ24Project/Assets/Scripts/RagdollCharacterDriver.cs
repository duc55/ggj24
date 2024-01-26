using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LeftOut.GameJam 
{
    [RequireComponent(typeof(CharacterController))]
    public class RagdollCharacterDriver : MonoBehaviour
    {
        private CharacterController _cc;
        private PlayerCombat _combat;
        private ControlState _state;
        
        [SerializeField]
        private Transform ragDollRoot;
        public Transform cameraTarget;

        [SerializeField, Range(0.01f, 10f)]
        private float movementGain = 1f;
        [SerializeField, Range(60f, 1080f)]
        private float rotationSpeed = 720f;
        
        void Start()
        {
            _cc = GetComponent<CharacterController>();
            _state = new ControlState();
            _combat = ragDollRoot.GetComponentInChildren<PlayerCombat>();
            ragDollRoot.parent = null;
        }

        private void Update()
        {
            _cc.Move(movementGain * Time.deltaTime * _state.MoveVector);
            if (!_state.IsMoving)
                return;
            
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(_state.MoveVector), rotationSpeed * Time.deltaTime);
        }

        public void SetMove(Vector3 moveVector)
            => SetMove(moveVector.x, moveVector.z);
        public void SetMove(Vector2 moveVector)
            => SetMove(moveVector.x, moveVector.y);
        
        private void SetMove(float x, float z)
        {
            _state.MoveVector.x = x;
            _state.MoveVector.z = z;
        }

        private Coroutine _attackRoutine;

        private void CancelAttack()
        {
            if (_attackRoutine != null)
                StopCoroutine(_attackRoutine);
        }

        public void AttackLeft()
        {
            CancelAttack();
            _attackRoutine = StartCoroutine(_combat.HitL());
        }
        public void AttackRight()
        {
            CancelAttack();
            _attackRoutine = StartCoroutine(_combat.HitR());
        }
    }
}
