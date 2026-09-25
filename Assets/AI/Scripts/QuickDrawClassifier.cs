using UnityEngine;
using Unity.InferenceEngine;
using System;

public class QuickDrawClassifier : MonoBehaviour
{
    public static event Action<string, float> PredictionMade;

    public string LastPrediction {  get; private set; }
    public float LastConfidence { get; private set; }

    [SerializeField] private ModelAsset modelAsset;
    [SerializeField] private VRDrawingCanvas drawingCanvas;

    private Worker worker;

    private static readonly string[] ClassNames = {
        "bee",
        "butterfly",
        "bird",
        "bridge",
        "campfire",
        "cloud",
        "fish",
        "flower",
        "frog",
        "moon",
        "mushroom",
        "rabbit",
        "rain",
        "rainbow",
        "snail",
        "snake",
        "sun",
        "tree",
        "other"
    };

    private void Awake() {
       if (modelAsset == null || drawingCanvas == null){
            Debug.LogError("QuickDrawClassifier missing model and/or drawing canvas...", this);
            enabled = false;
            return;
        }

        // Initialized model & worker
        Model model = ModelLoader.Load(modelAsset);
        worker = new Worker(model, BackendType.CPU);
    }

    private float[] PrepareInput(Texture2D sourceImg){
        const int modelSize = 28;
        const int drawingSize = 22;
        const float ink = 0.95f;

        int minX = sourceImg.width;
        int minY = sourceImg.height;
        int maxX = -1;
        int maxY = -1;

        // Find area containing drawing by adjusting min and max vals by tracking pixels darker than ink var
        for (int y = 0; y < sourceImg.height; y++){
            for (int x = 0; x < sourceImg.width; x++){
                float grayscale = sourceImg.GetPixel(x, y).grayscale;
                if (grayscale < ink){
                    minX = Mathf.Min(minX, x);
                    minY = Mathf.Min(minY, y);
                    maxX = Mathf.Max(maxX, x);
                    maxY = Mathf.Max(maxY, y);
                }
            }
        }

        float[] inputData = new float[modelSize * modelSize];

        // If canvas is blank
        if (maxX == -1){
            return inputData;
        }

        int cropWidth = maxX - minX + 1;
        int cropHeight = maxY - minY + 1;

        // Finfds a scale that preserves drawing proportions
        float scale = Mathf.Min((float)drawingSize / cropWidth, (float)drawingSize / cropHeight);
        int resizedWidth = Mathf.Max(1, Mathf.RoundToInt(cropWidth * scale));
        int resizedHeight = Mathf.Max(1, Mathf.RoundToInt(cropHeight * scale));

        // Calc for where the resized drawing should begin so it is centered
        int offsetX = (modelSize - resizedWidth) / 2;
        int offsetY = (modelSize - resizedHeight) / 2;

        for (int i = 0; i < resizedHeight; i++) { // Rows
            // maps each resized row back to a row inside cropped original
            float sourceY = Mathf.Lerp(maxY, minY, resizedHeight == 1 ? 0.5f : (float)i / (resizedHeight - 1));
            for (int j = 0; j < resizedWidth; j++) { // Columns
                // maps each resized column back to a column inside cropped original
                float sourceX = Mathf.Lerp(minX, maxX, resizedWidth == 1 ? 0.5f : (float)j / (resizedWidth - 1));

                float u = sourceX / (sourceImg.width - 1f); // Normalized x coord
                float v = sourceY / (sourceImg.height - 1f); // Normalized y coord 

                // Samples cropped drawing by blending nearby pixels to resize to 28x28, and converts color into grayscale
                float grayscale = sourceImg.GetPixelBilinear(u, v).grayscale;

                // FInds model destination pos inside 28x28 input
                int modelX = offsetX + j;
                int modelY = offsetY + i;

                // Converts 2D coords into 1D index, inverts color, and adds to inputData array
                inputData[(modelY * modelSize) + modelX] = 1f - grayscale;
            }
        }
        return inputData;
    }

    // Gets confidence level for prediction
    private float ConfidenceCalc(Tensor<float> logits, int bestIndex) { // Takes 19 output scores tensor and the best index
        float highestLogit = logits[bestIndex];
        float sum = 0f;

        // Softmax denominator
        for (int i = 0; i < ClassNames.Length; i++) {
            sum += Mathf.Exp(logits[i] - highestLogit);
        }

        return 1f / sum;
    }


    // Context menu button
    [ContextMenu("Classify Drawing")]
    public void ClassifyDrawing(){
        // Gets drawing texture from canvas
        Texture2D drawingTexture = drawingCanvas.GetDrawingTexture();
        float[] inputData = PrepareInput(drawingTexture);

        using (Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 1, 28, 28), inputData)){ // Shape: [1, 1, 28, 28]
            // Worker runs the model
            worker.Schedule(inputTensor);

            // Grabs output tensor (tensor contains 19 # scores for each class)
            Tensor<float> deviceOutput = worker.PeekOutput("logits") as Tensor<float>;

            // Copy output to readable memory
            using (Tensor<float> output = deviceOutput.ReadbackAndClone()){
                // Compares all classes to find class with largest output value
                int bestIndex = 0;

                for (int i = 1; i < ClassNames.Length; i++){
                    if (output[i] > output[bestIndex]){
                        bestIndex = i;
                    }
                }
                // finds confidence for prediction
                float confidence = ConfidenceCalc(output, bestIndex);

                // Sets last prediction and last confidence for reciever event
                LastPrediction = ClassNames[bestIndex];
                LastConfidence = confidence;

                // logs result
                Debug.Log($"Prediction: {ClassNames[bestIndex]} | Confidence: {confidence:P1}");

                // Calls prediction made event

                PredictionMade?.Invoke(LastPrediction, LastConfidence);
            }
        }
    }

    // Destroys worker if worker exists on exit
    private void OnDestroy() {
        worker?.Dispose();
    }
}
