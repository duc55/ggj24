using System;
using UnityEngine;
using UnityEngine.Events;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(Rigidbody))]
    public class Hitbox : MonoBehaviour
    {
        private float _timeCooldownFinished;
        private bool _isOn;
        
        public Collider Collider { get; private set; }
        private bool canHit => 
            _isOn && Time.time >= _timeCooldownFinished;
        public bool IsOn => _isOn;
        
        public UnityEvent<Hitbox, Collider> OnHit;

        private void Start()
        {
            _timeCooldownFinished = -1f;
            Collider = GetComponentInChildren<Collider>();
        }

        private void OnEnable()
        {
            InstanceRegistry<Hitbox>.Add(this);
        }
        
        private void OnDisable()
        {
            InstanceRegistry<Hitbox>.Remove(this);
        }

        public void TurnOn()
        {
            _isOn = true;
            _timeCooldownFinished = Time.time;
        }

        public void TurnOff()
            => _isOn = false;
                
        public void GoOnCooldownFor(float duration)
        {
            _timeCooldownFinished = Time.time + duration;
        }
        
        private void OnCollisionEnter(Collision other)
            => RaiseHitIfValid(other.collider);
        private void OnCollisionStay(Collision other)
            => RaiseHitIfValid(other.collider);
        private void OnTriggerEnter(Collider other)
            => RaiseHitIfValid(other);
        private void OnTriggerStay(Collider other)
            => RaiseHitIfValid(other);

        private bool IsSelf(Collider other)
        {
            if (ComponentOwnerRegistry<RagdollCharacter, Collider>
                .TryGetOwner(other, out var otherOwner)
                && ComponentOwnerRegistry<RagdollCharacter, Collider>
                    .TryGetOwner(Collider, out var myOwner))
            {
                return ReferenceEquals(otherOwner, myOwner);
            }

            return false;
        }
        
        private void RaiseHitIfValid(Collider other)
        {
            if (!canHit || IsSelf(other))
                return;
            
            OnHit.Invoke(this, other);
        }
    }
}
