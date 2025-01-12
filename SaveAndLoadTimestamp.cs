using Firebase.Firestore;
using UnityEngine;
using UnityEngine.UI;

public class SaveAndLoadTimestamp : MonoBehaviour
{
    private FirebaseFirestore db; // Reference to Firestore
    public Text statusText;       // Text field to display status or loaded timestamp

    void Start()
    {
        // Initialize Firestore
        db = FirebaseFirestore.DefaultInstance;
    }

    // Method to save the current date and time to Firestore
    public void SaveTimestamp()
    {
        // Reference to the document in Firestore
        DocumentReference docRef = db.Collection("GameSaves").Document("Save1");
        if(docRef == null)
        {
            Debug.Log("the Docref is null");
        }

        // Create a data object with the current timestamp
        var saveData = new { lastSaved = Timestamp.GetCurrentTimestamp() };

        // Save the data to Firestore
        docRef.SetAsync(saveData).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Timestamp saved successfully.");
                UpdateStatus("Game saved successfully!");
            }
            else
            {
                Debug.LogError("Error saving timestamp: " + task.Exception);
                UpdateStatus("Failed to save game.");
            }
        });
    }

    // Method to load the last saved date and time from Firestore
    public void LoadLastSavedTime()
    {
        // Reference to the document in Firestore
        DocumentReference docRef = db.Collection("GameSaves").Document("Save1");

        // Get the document from Firestore
        docRef.GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                // Retrieve the timestamp field
                Timestamp lastSaved = task.Result.GetValue<Timestamp>("lastSaved");
                string formattedTime = lastSaved.ToDateTime().ToString(); // Format timestamp to a readable string

                Debug.Log("Timestamp loaded: " + lastSaved.ToString());
                UpdateStatus("Last Saved: " + formattedTime);
            }
            else
            {
                Debug.LogError("Failed to load timestamp or no data exists.");
                UpdateStatus("No saved game found.");
            }
        });
    }

    // Method to update the status or timestamp on the UI
    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}
