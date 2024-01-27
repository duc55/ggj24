using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LeftOut.GameJam
{
    // TODO: This class should be observing the RagdollCombat classes, not the Hitboxes directly
    public class CombatObserver : MonoBehaviour
    {
        private HashSet<Hitbox> _allHitboxes;
        public CombatObserver Instance { get; private set; }

        [SerializeField, Range(0.01f, 1f)]
        private float defaultHitboxCooldown;
        

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            foreach (var hitbox in InstanceRegistry<Hitbox>.All)
            {
                AddHitbox(hitbox);
            }
        }

        private void OnEnable()
        {
            _allHitboxes = new HashSet<Hitbox>();
            InstanceRegistry<Hitbox>.OnAdd += AddHitbox;
            InstanceRegistry<Hitbox>.OnRemove += RemoveHitbox;
        }

        private void OnDisable()
        {
            InstanceRegistry<Hitbox>.OnAdd -= AddHitbox;
            InstanceRegistry<Hitbox>.OnRemove -= RemoveHitbox;
        }

        private void AddHitbox(Hitbox hitbox)
        {
            if (!Application.isPlaying)
                return;
            
            _allHitboxes.Add(hitbox);
            hitbox.OnHit.AddListener(OnHitboxCollided);
        }
        
        private void RemoveHitbox(Hitbox hitbox)
        {
            if (!_allHitboxes.Contains(hitbox))
                return;

            hitbox.OnHit.RemoveListener(OnHitboxCollided);
            _allHitboxes.Remove(hitbox);
        }
        
        private void OnHitboxCollided(Hitbox hitbox, Collider other)
        {
            if (!ComponentOwnerRegistry<RagdollCharacter, Collider>.TryGetOwner(
                    hitbox.Collider, out var source))
            {
                Debug.LogError(
                    $"{hitbox.name} just hit {other.name} but it has no registered owner!", hitbox);
                return;
            }

            if (!ComponentOwnerRegistry<RagdollCharacter, Collider>
                    .TryGetOwner(other, out var target))
            {
                Debug.LogError(
                    $"{other.name} just got hit by {hitbox.name} but it has no registered owner!", other);
                return;
            }
            
            Debug.Log($"{source.name} just hit {target.name}!");
            hitbox.GoOnCooldownFor(defaultHitboxCooldown);
        }
    }
}