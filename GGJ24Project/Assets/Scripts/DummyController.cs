using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeftOut.GameJam 
{
    [RequireComponent(typeof(CharacterController))]
    public class DummyController : MonoBehaviour
    {
        private CharacterController _cc;
        private ControlState _state;

        [SerializeField, Range(0.01f, 10f)]
        private float movementGain = 1f;
        [SerializeField, Range(60f, 720f)]
        private float rotationSpeed = 1f;
        
        void Start()
        {
            _cc = GetComponent<CharacterController>();
            _state = new ControlState();
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
    }
}
