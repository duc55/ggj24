using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace LeftOut.GameJam
{
    public class PlayerSpawner : MonoBehaviour
    {
        private int _spawnIdx = -1;

        private const string CinemachineGroup = "Cinemachine";
        [FormerlySerializedAs("cmTargetRadius")]
        [BoxGroup(CinemachineGroup), 
         SerializeField, Range(0.1f, 5f)]
        private float targetRadius = 1f;
        [BoxGroup(CinemachineGroup),
         SerializeField]
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
                StartCoroutine(InitializeCharacter(localPlayer, input, spawnLocations[_spawnIdx]));
            }
        }
        
        private IEnumerator InitializeCharacter(LocalPlayer player, PlayerInput inputs, Transform spawnLocation)
        {
            var playerCharacter = Instantiate(
                player.characterPrefab, spawnLocation.position, spawnLocation.rotation);
            yield return null;
            // var rigidbodies = playerCharacter.GetComponentsInChildren<Rigidbody>();
            // var rigidbodyStates = new Dictionary<Rigidbody, bool>();
            // foreach (var rb in rigidbodies)
            // {
            //     rigidbodyStates[rb] = rb.isKinematic;
            //     rb.isKinematic = true;
            // }
            // yield return null;
            // playerCharacter.transform.SetPositionAndRotation(spawnLocation.position, spawnLocation.rotation);
            // yield return new WaitForFixedUpdate();
            // foreach (var rb in rigidbodies)
            // {
            //     rb.isKinematic = rigidbodyStates[rb];
            // }
            playerCharacter.GetComponent<CharacterInputHandler>().BindInputs(inputs);
            targetGroup.AddMember(
                playerCharacter.GetComponent<RagdollCharacter>().cameraTarget, 1, targetRadius);
        }
    }
}