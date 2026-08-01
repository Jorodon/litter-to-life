using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class VRDrawingCanvas : MonoBehaviour
{
    [SerializeField] private int textureResolution = 256;
    [SerializeField] private int brushRadius = 3;

    //Model takes input with black background & white strokes (this will be adjusted automatically before model input)
    [SerializeField] private Color backgroundColor = Color.white; 
    [SerializeField] private Color brushColor = Color.black; 

    private Texture2D drawingTexture;
    private Color32[] pixels;
    private Material canvasMaterial;

    private void Awake(){
        Renderer canvasRenderer = GetComponent<Renderer>();

        // Checks for material to ensure there is a place for drawing texture
        if (canvasRenderer.sharedMaterial == null){
            Debug.LogError("Drawing canvas needs a material...", this);
            enabled = false;
            return;
        }

        textureResolution = Mathf.Max(28, textureResolution);
        brushRadius = Mathf.Max(1, brushRadius);

        // Creates a blank texture at runtime
        drawingTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);

        drawingTexture.filterMode = FilterMode.Bilinear; // Smooths texture when larger or smaller than actuall resolution
        drawingTexture.wrapMode = TextureWrapMode.Clamp; // Prevents pixels from wrapping if on edge

        // Array of each pixel on canvas
        pixels = new Color32[textureResolution * textureResolution];

        canvasMaterial = canvasRenderer.material; // Creates runtime material so that the original material is not modified

        // Assigns texture to mat
        if (canvasMaterial.HasProperty("_BaseMap")){
            canvasMaterial.SetTexture("_BaseMap", drawingTexture);
        }
        else{
            canvasMaterial.mainTexture = drawingTexture;
        }

        ClearCanvas();
    }

    //Clears/Resets Canvas
    public void ClearCanvas()
    {
        Color32 clearColor = backgroundColor;

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = clearColor;
        }

        UpdateTexture();
    }
    
    // Changes pixels around pos to create a circle
    private void DrawCircle(int centerX, int centerY){
        int radiusSquared = brushRadius * brushRadius;

        for (int y = -brushRadius; y <= brushRadius; y++){
            for (int x = -brushRadius; x <= brushRadius; x++){

                // Excludes pixels outside circle
                if ((x * x) + (y * y) > radiusSquared){
                    continue;
                }

                int pixelX = centerX + x;
                int pixelY = centerY + y;

                // Prevents drawing outside texture
                if (
                    pixelX < 0 ||
                    pixelX >= textureResolution ||
                    pixelY < 0 ||
                    pixelY >= textureResolution
                ){
                    continue;
                }

                // Convert to array index (Since the array is 1D, y * width + x gives pixel pos in array)
                pixels[pixelY * textureResolution + pixelX] = brushColor;
            }
        }
    }

    // Transfers changed pixel array to Unity texture
    private void UpdateTexture(){
        drawingTexture.SetPixels32(pixels);
        drawingTexture.Apply(false);
    }

    // Function for circle brush mark at specific location
    public void DrawAtUV(Vector2 uv){
        Vector2Int pixel = UVToPixel(uv);

        DrawCircle(pixel.x, pixel.y);
        UpdateTexture();
    }
    
    // Function for drawing a line between two pos
    public void DrawLineUV(Vector2 startUV, Vector2 endUV){
        // Creates dot at start and end location
        Vector2 start = UVToPixel(startUV);
        Vector2 end = UVToPixel(endUV);

        // Finds the distance between positions and calculates how many points needed inbetween
        int steps = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(start, end) / Mathf.Max(1f, brushRadius * 0.5f)));

        // Draws along line
        for (int i = 0; i <= steps; i++){
            Vector2 point = Vector2.Lerp(start, end, (float)i / steps);

            DrawCircle(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
        }

        UpdateTexture();
    }

    // Simple accessor that returns texture (to process & send to classifier onnx model)
    public Texture2D GetDrawingTexture(){
        return drawingTexture;
    }

    // Converts UV coords to actual texture coordinates (e.g. UV 0.0 = pixel 0 | UV 1.0 = pixel 255)
    private Vector2Int UVToPixel(Vector2 uv){
        int x = Mathf.RoundToInt(Mathf.Clamp01(uv.x) * (textureResolution - 1));

        int y = Mathf.RoundToInt(Mathf.Clamp01(uv.y) * (textureResolution - 1));

        return new Vector2Int(x, y);
    }

    // Removess memory instances
    private void OnDestroy(){
        if (drawingTexture != null){
            Destroy(drawingTexture);
        }

        if (canvasMaterial != null){
            Destroy(canvasMaterial);
        }
    }
}