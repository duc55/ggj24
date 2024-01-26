using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LeftOut.GameJam
{
    public class LocalPlayer : MonoBehaviour
    {
        private PlayerInput _input;
        public GameObject characterPrefab;

        private void Awake()
        {
            _input = GetComponent<PlayerInput>();
        }
    }
}