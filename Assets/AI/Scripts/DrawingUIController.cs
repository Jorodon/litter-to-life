using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DrawingUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField, Range(0f, 1f)] private float minConfidence = 0.55f;
    [SerializeField] private GameObject instructionsPanel;

    private readonly HashSet<string> unavailable = new(){
        "bridge",
        "campfire",
        "cloud",
        "fish",
        "frog",
        "moon",
        "mushroom",
        "rabbit",
        "rain",
        "rainbow",
        "snail",
        "snake",
        "sun",
        "other" 
    };

    private readonly HashSet<string> alreadySpawned = new();

    private void ShowPrediction(string prediction, float confidence){
        // Failed to predict message
        if (unavailable.Contains(prediction) || confidence < minConfidence){
            resultText.text = "Hmm... Try drawing it more clearly and keep it as simple as possible.\n\nPress the help button to see available objects to draw!";
        }
        // Already spawned message
        else if (alreadySpawned.Contains(prediction)){
            resultText.text = $"What a beautiful {prediction}.\nHowever, you've already brought this to life!\n\nPress the help button to see available objects to draw!";
        }
        // Success message & adds object to alreadySpawned
        else {
            alreadySpawned.Add(prediction);
            resultText.text = $"What a great {prediction}!\nYour drawing brought it to life!";
        }
    }

    // Toggles instruction text
    public void ToggleInstructions(){
        instructionsPanel.SetActive(!instructionsPanel.activeSelf);
    }

    // Resets to default starting message
    public void ResetResult() {
        resultText.text = "Draw something, then press Submit.\n\nPress the help button to see available objects to draw!";
    }

    // Resets all UI when waking
    private void Awake() {
        ResetResult();
        instructionsPanel.SetActive(false);
    }
    
    // Sets as prediction listener
    private void OnEnable() {
        QuickDrawClassifier.PredictionMade += ShowPrediction;
    }

    // Removes as prediction listener
    private void OnDisable() {
        QuickDrawClassifier.PredictionMade -= ShowPrediction;
    }
}
