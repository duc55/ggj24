using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LeftOut.GameJam
{
    public class PlayerSpawner : MonoBehaviour
    {
        private int _spawnIdx = -1;

        [SerializeField, Range(0.1f, 5f)]
        private float cmTargetRadius = 1f;
        
        [SerializeField]
        private CinemachineTargetGroup targetGroup;
        [SerializeField]
        private List<Transform> spawnLocations;
        
        public void OnPlayerJoined(PlayerInput input)
        {
            _spawnIdx++;
            if (_spawnIdx >= spawnLocations.Count)
                _spawnIdx = 0;
            Debug.Log("Player Joined!", input);
            if (input.TryGetComponent<LocalPlayer>(out var localPlayer))
            {
                var character = localPlayer.InitializeCharacter(input, spawnLocations[_spawnIdx]);
                targetGroup.AddMember(
                    character.GetComponent<RagdollCharacterDriver>().cameraTarget, 1, cmTargetRadius);
            }
        }
    }
}