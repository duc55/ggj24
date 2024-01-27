using UnityEngine;

namespace LeftOut.GameJam
{
    public class VomitParticles : MonoBehaviour
    {
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
