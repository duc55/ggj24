using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LeftOut.GameJam
{
    // TODO: This class should be observing the RagdollCombat classes, not the RagdollHitboxes directly
    public class CombatObserver : MonoBehaviour
    {
        private HashSet<RagdollCombat> _allCombatants;
        public CombatObserver Instance { get; private set; }

        [SerializeField, Range(0.01f, 1f)]
        private float defaultRagdollHitboxCooldown;
        

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            foreach (var baby in InstanceRegistry<RagdollCombat>.All)
            {
                AddCombatant(baby);
            }
        }

        private void OnEnable()
        {
            _allCombatants = new HashSet<RagdollCombat>();
            InstanceRegistry<RagdollCombat>.OnAdd += AddCombatant;
            InstanceRegistry<RagdollCombat>.OnRemove += RemoveCombatant;
        }

        private void OnDisable()
        {
            InstanceRegistry<RagdollCombat>.OnAdd -= AddCombatant;
            InstanceRegistry<RagdollCombat>.OnRemove -= RemoveCombatant;
        }

        private void AddCombatant(RagdollCombat baby)
        {
            if (!Application.isPlaying)
                return;
            
            _allCombatants.Add(baby);
            baby.onHitConnect.AddListener(OnHitConnect);
        }
        
        private void RemoveCombatant(RagdollCombat baby)
        {
            if (!_allCombatants.Contains(baby))
                return;

            baby.onHitConnect.RemoveListener(OnHitConnect);
            _allCombatants.Remove(baby);
        }
        
        private void OnHitConnect(HitConnectEvent hit)
        {
            Debug.Log($"{hit.Instigator.name} just hit {hit.Other.name}!");
            hit.Hitbox.GoOnCooldownFor(defaultRagdollHitboxCooldown);
        }
    }
}