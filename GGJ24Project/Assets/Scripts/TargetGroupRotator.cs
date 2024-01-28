using System;
using Cinemachine;
using UnityEngine;

namespace LeftOut.GameJam
{
    [RequireComponent(typeof(CinemachineTargetGroup))]
    public class TargetGroupRotator : MonoBehaviour
    {
        private CinemachineTargetGroup _group;
        private readonly Quaternion _flipHeading = Quaternion.Euler(0, 180, 0);
        
        [SerializeField, Min(0), Tooltip(
             "Number of transforms in the target group to ignore.")]
        private int numTransformsIgnored = 1;

        [SerializeField, Range(90f, 180f)]
        private float flipThreshold = 120f;

        private void Start()
        {
            _group = GetComponent<CinemachineTargetGroup>();
        }

        private void Update()
        {
            var newRotation = CalculateAverageOrientation();
            // This check stops the camera from constantly flipping directions
            // whenever a baby crosses the line of action
            if (Mathf.Abs(Mathf.DeltaAngle(
                    newRotation.eulerAngles.y, transform.rotation.eulerAngles.y))
                > flipThreshold)
            {
                newRotation *= _flipHeading;
            }
            transform.rotation = newRotation;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }

        Quaternion CalculateAverageOrientation()
        {
            var count = _group.m_Targets.Length;
            if (count - numTransformsIgnored - 1 <= 0)
            {
                return _group.m_Targets[0].target.rotation;
            }

            var numCompared = count - numTransformsIgnored;
            var tfWeight = 1f / (numCompared * (numCompared - 1));
            var avgRotation = Quaternion.identity;
            for (var i = numTransformsIgnored; i < count; ++i)
            {
                var fromPosition = _group.m_Targets[i].target.position;
                for (var j = numTransformsIgnored; j < count; ++j)
                {
                    if (i == j)
                        continue;
                    var toPosition = _group.m_Targets[j].target.position;
                    var rot = Quaternion.LookRotation(toPosition - fromPosition);
                    avgRotation *= Quaternion.Slerp(Quaternion.identity, rot, tfWeight);
                }
            }

            return avgRotation;
        }
    }
}