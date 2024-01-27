using UnityEngine;
using UnityEngine.Events;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(Rigidbody))]
    public class Hitbox : MonoBehaviour
    {
        private Collider _collider;
        private float _timeCooldownFinished;
        private bool IsTrigger => _collider.isTrigger;
        private bool isOn => 
            enabled && gameObject.activeInHierarchy && Time.time >= _timeCooldownFinished;
        
        public UnityEvent<Hitbox, Collider> OnHit;

        private void Start()
        {
            _timeCooldownFinished = -1f;
            _collider = GetComponentInChildren<Collider>();
        }
        
        public void SetStatus(bool active)
        {
            if (IsTrigger)
                gameObject.SetActive(active);
            else
                enabled = active;
        }

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
            if (InstanceRegistry<RagdollCharacter, Collider>
                .TryGetOwner(other, out var otherOwner)
                && InstanceRegistry<RagdollCharacter, Collider>
                    .TryGetOwner(_collider, out var myOwner))
            {
                return ReferenceEquals(otherOwner, myOwner);
            }

            return false;
        }
        
        private void RaiseHitIfValid(Collider other)
        {
            if (!isOn || IsSelf(other))
                return;
            
            OnHit.Invoke(this, other);
        }
    }
}
