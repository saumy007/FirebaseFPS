using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;

public class DatabaseManager : MonoBehaviour
{
    private string userID; // Unique identifier for the user/device
    private DatabaseReference dbReference; // Reference to the Firebase database

    private void Start()
    {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                userID = SystemInfo.deviceUniqueIdentifier; // Unique identifier for this user
                Debug.Log("Firebase initialized successfully.");
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
            }
        });
    }

    public void SaveScoreToDatabase()
    {
        int currentScore = Collectible.score; // Fetch the static score from Collectible script
        if (dbReference != null)
        {
            dbReference.Child("users").Child(userID).Child("score").SetValueAsync(currentScore).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"Score successfully saved to Firebase: {currentScore}");
                }
                else
                {
                    Debug.LogError($"Error saving score to Firebase: {task.Exception}");
                }
            });
        }
        else
        {
            Debug.LogError("Database reference is null. Ensure Firebase is initialized properly.");
        }
    }

    public void LoadfromDatabase()
    {
        StartCoroutine(LoadfromEnum());
    }

    IEnumerator LoadfromEnum()
    {
        // Asynchronously fetch the data from Firebase
        var serverData = dbReference.Child("users").Child(userID).GetValueAsync();

        // Wait until the data fetching task is completed
        yield return new WaitUntil(() => serverData.IsCompleted);

        // Check if the task was successful
        if (serverData.IsCompleted)
        {
            if (serverData.Exception == null)
            {
                DataSnapshot snapshot = serverData.Result;

                if (snapshot.Exists)
                {
                    Debug.Log("Data loaded successfully from Firebase.");

                    // Assuming your data has a "score" field
                    if (snapshot.Child("score").Exists)
                    {
                        int retrievedScore = int.Parse(snapshot.Child("score").Value.ToString());
                        Debug.Log($"Score retrieved: {retrievedScore}");

                        // Optionally update your game logic/UI here
                        // Example: Update a UI element with the retrieved score
                        // scoreText.text = "Score: " + retrievedScore;
                    }
                    else
                    {
                        Debug.LogWarning("Score field does not exist in the database for this user.");
                    }
                }
                else
                {
                    Debug.LogWarning("No data exists for this user.");
                }
            }
            else
            {
                Debug.LogError($"Error fetching data: {serverData.Exception}");
            }
        }
        else
        {
            Debug.LogError("Failed to complete the task of loading data.");
        }
    }

}
