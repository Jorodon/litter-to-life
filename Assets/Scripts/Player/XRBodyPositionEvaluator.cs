using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

public class XRBodyPositionEvaluator : MonoBehaviour, IXRBodyPositionEvaluator
{
    public Vector3 GetBodyGroundLocalPosition(XROrigin xrOrigin)
    {
        if (xrOrigin != null && xrOrigin.Camera != null)
        {
            Vector3 cameraLocalPos = xrOrigin.Camera.transform.localPosition;
            return new Vector3(cameraLocalPos.x, 0f, cameraLocalPos.z);
        }

        return Vector3.zero;
    }

    public float GetBodyHeight(XROrigin xrOrigin)
    {
        if (xrOrigin != null && xrOrigin.Camera != null)
        {
            return xrOrigin.Camera.transform.localPosition.y;
        }

        return 0f;
    }
}