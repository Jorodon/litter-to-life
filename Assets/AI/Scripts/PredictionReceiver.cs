using UnityEngine;
using UnityEngine.Events;

public class PredictionReceiver : MonoBehaviour
{
    [SerializeField] private string expectedPrediction;
    [SerializeField] private UnityEvent onPredictionMatched;
    [SerializeField, Range(0f, 1f)] private float minConfidence = 0.55f;

    // Checks that prediction matches and has at least the minimum confidence value
    private void CheckPrediction(string prediction, float confidence){
        if (prediction == expectedPrediction && confidence >= minConfidence){
            onPredictionMatched?.Invoke();
        }
    }

    // Begins listening to classifier on enable
    private void OnEnable(){
        QuickDrawClassifier.PredictionMade += CheckPrediction;
    }

    // Stops listening to classifier on disable
    private void OnDisable(){
        QuickDrawClassifier.PredictionMade -= CheckPrediction;
    }
}
