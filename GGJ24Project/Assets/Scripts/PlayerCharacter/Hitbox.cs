using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(Rigidbody))]
    public class Hitbox<TOwner> : MonoBehaviour where TOwner : Component
    {
        private float _timeCooldownFinished;
        private bool _isOn;
        
        public Collider Collider { get; private set; }
        private bool canHit => 
            _isOn && Time.time >= _timeCooldownFinished;
        public bool IsOn => _isOn;
        
        public UnityEvent<Hitbox<TOwner>, Collider> onHitTrigger;
        public UnityEvent<Hitbox<TOwner>, Collision> onHitCollision;

        private void Start()
        {
            _timeCooldownFinished = -1f;
            Collider = GetComponentInChildren<Collider>();
            ComponentOwnerRegistry<Hitbox<TOwner>, Collider>.Register(this, Collider);
        }

        private void OnEnable()
        {
            InstanceRegistry<Hitbox<TOwner>>.Add(this);
        }
        
        private void OnDisable()
        {
            InstanceRegistry<Hitbox<TOwner>>.Remove(this);
            ComponentOwnerRegistry<Hitbox<TOwner>, Collider>.Remove(Collider);
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
            => RaiseHitIfValid(other);
        private void OnCollisionStay(Collision other)
            => RaiseHitIfValid(other);

        private void OnTriggerEnter(Collider other)
            => RaiseHitIfValid(other);
        private void OnTriggerStay(Collider other)
            => RaiseHitIfValid(other);
        
        private bool IsSelf(Collider other)
        {
            if (ComponentOwnerRegistry<TOwner, Collider>
                .TryGetOwner(other, out var otherOwner)
                && ComponentOwnerRegistry<TOwner, Collider>
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
            
            onHitTrigger.Invoke(this, other);
        }
        
        private void RaiseHitIfValid(Collision collision)
        {
            if (!canHit || IsSelf(collision.collider))
                return;
            
            onHitCollision.Invoke(this, collision);
        }
    }
}
