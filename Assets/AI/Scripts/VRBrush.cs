using UnityEngine;

public class VRBrush : MonoBehaviour
{
    [SerializeField] private Transform brushTip;
    [SerializeField] private float drawDistance = 0.05f; // How far away from tip paintbrush draws

    private VRDrawingCanvas previousCanvas;
    private Vector2 previousUV;
    private bool hasPreviousPoint;
    private BrushCanvasStopper canvasStopper;

    private void Awake(){
        if (brushTip == null){
            Debug.LogError("VRBrush needs a brush tip assigned...", this);
            enabled = false;
            return;
        }

        canvasStopper = GetComponent<BrushCanvasStopper>();
    }

    // Ends current brush stroke
    private void ResetStroke(){
        previousCanvas = null;
        hasPreviousPoint = false;
    }

    // Constantly checks whether brush is touching canvas after other updates
    private void LateUpdate(){
        // Clamps ray to canvas stopper
        canvasStopper.ClampToCanvas();

        // Checks if brush is in range of object
        if (!Physics.Raycast(brushTip.position, brushTip.forward, out RaycastHit hit, drawDistance)){
            ResetStroke();
            return;
        }

        // Ensures object hit is the canvas
        VRDrawingCanvas canvas = hit.collider.GetComponent<VRDrawingCanvas>();
        if (canvas == null){
            ResetStroke();
            return;
        }

        Vector2 currentUV = hit.textureCoord;
        
        // Continue stroke
        if (hasPreviousPoint && canvas == previousCanvas){
            canvas.DrawLineUV(previousUV, currentUV);
        }

        // New stroke
        else{
            canvas.DrawAtUV(currentUV);
        }

        // Saves info to class object
        previousCanvas = canvas;
        previousUV = currentUV;
        hasPreviousPoint = true;
    }
}