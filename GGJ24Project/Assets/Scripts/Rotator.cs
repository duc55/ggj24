using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    float _spinSpeed = 1f;

    void Update()
    {
        transform.Rotate(new Vector3(0, _spinSpeed, 0) * Time.deltaTime);
    }
}
