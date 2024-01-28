using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeftOut.GameJam.Baby.Expressions
{
    public class ExpressionController : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer headSMR;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                Blink();
            }

            ResetExpression();
        }

        private void Blink()
        {
            headSMR.SetBlendShapeWeight(0, 100);
            headSMR.SetBlendShapeWeight(1, 100);
        }

        private void ResetExpression() 
        {
            if (headSMR.GetBlendShapeWeight(0) > 0) {
                headSMR.SetBlendShapeWeight(0, headSMR.GetBlendShapeWeight(0) - 5f);
            }
            if (headSMR.GetBlendShapeWeight(1) > 0) {
                headSMR.SetBlendShapeWeight(1, headSMR.GetBlendShapeWeight(1) - 5f);
            }
        }
    }
}
