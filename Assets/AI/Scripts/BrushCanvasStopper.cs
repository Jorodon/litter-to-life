using UnityEngine;

public class BrushCanvasStopper : MonoBehaviour
{
    [SerializeField] private Transform brushTip;
    [SerializeField] private Transform canvasSurface;
    [SerializeField] private float stopDistance = 0.005f;

    private float allowedSide;

    private void Awake(){
        // Checks which side is allowed based on brush start (1 for forward side and -1 for opposite)
        float startingSide = Vector3.Dot(brushTip.position - canvasSurface.position, canvasSurface.forward);
        allowedSide = startingSide >= 0f ? 1f : -1f;
    }

    public void ClampToCanvas()
    {
        // Gets canvas direction and how far brush is from canvas
        Vector3 outwardDirection = canvasSurface.forward * allowedSide;
        float distanceFromSurface = Vector3.Dot(brushTip.position - canvasSurface.position, outwardDirection);

        // If it is too close or has gone through the canvas, calcs how far to move back
        if (distanceFromSurface < stopDistance){
            float correction = stopDistance - distanceFromSurface;
            transform.position += outwardDirection * correction;
        }
    }

    // XR Interaction Manager updates at order 100, so 101 runs immediately after
    [BeforeRenderOrder(101)]
    private void ClampAfterXRMovement(){
        ClampToCanvas();
    }

    private void OnEnable(){
        Application.onBeforeRender += ClampAfterXRMovement;
    }

    private void OnDisable(){
        Application.onBeforeRender -= ClampAfterXRMovement;
    }

    // Runs after regualr update calls
    private void LateUpdate(){
        ClampToCanvas();
    }


    

}