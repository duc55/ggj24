using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace LeftOut.GameJam
{
    public class LocalPlayer : MonoBehaviour
    {
        private PlayerInput _input;
        public GameObject characterPrefab;
        
        public GameObject InitializeCharacter(PlayerInput inputs, Transform spawnLocation)
        {
            var character = SpawnCharacter(characterPrefab, spawnLocation);
            SetUpCharacter(character, inputs);
            return character;
        }

        private GameObject SpawnCharacter(GameObject characterPrefab, Transform spawn)
        {
            var playerCharacter = Instantiate(characterPrefab);
            playerCharacter.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            return playerCharacter;
        }
        
        private void SetUpCharacter(GameObject character, PlayerInput inputs)
        { 
            character.GetComponent<CharacterInputHandler>().BindInputs(inputs);
        }
    }
}