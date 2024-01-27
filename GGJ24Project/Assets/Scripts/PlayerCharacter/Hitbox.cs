using UnityEngine;
using UnityEngine.Events;

namespace LeftOut.GameJam
{
    public class Hitbox : MonoBehaviour
    {
        private float _timeCooldownFinished;
        private bool isOn => 
            enabled && gameObject.activeInHierarchy && Time.time >= _timeCooldownFinished;
        
        public UnityEvent<Hitbox, Collider> OnHit;

        private void Start()
        {
            _timeCooldownFinished = -1f;
        }

        public void SetStatus(bool active)
        {
            gameObject.SetActive(active);
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
        
        private void RaiseHitIfValid(Collider other)
        {
            if (!isOn)
                return;
            
            OnHit.Invoke(this, other);
        }
    }
}
