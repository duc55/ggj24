using UnityEngine;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(ParticleSystem))]
    public class VomitParticles : MonoBehaviour
    {
        private ParticleSystem _ps;

        public void StartVomit()
        {
            _ps.Play();
        }

        public void StopVomit()
        {
            _ps.Stop();
        }

        private void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.name.StartsWith("foot"))
            {
                // TODO: Call a slip function on the player that stepped in the vom 
                Debug.Log("Stepped in vomit!");
            }
        }
    }
}
