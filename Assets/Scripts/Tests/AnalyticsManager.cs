using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            await InitializeAnalytics();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async Task InitializeAnalytics()
    {
        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("Unity UGS Analytics initialized.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Analytics initialization failed: {e.Message}");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        TrackSessionEnd();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrackLevelStart(scene.name, scene.buildIndex);
    }

    public void TrackLevelStart(string levelName, int levelIndex)
    {
        SendEvent("level_start", new Dictionary<string, object>
        {
            { "level_name", levelName },
            { "level_index", levelIndex }
        });
    }

    public void TrackLevelComplete(string levelName, int levelIndex)
    {
        SendEvent("level_complete", new Dictionary<string, object>
        {
            { "level_name", levelName },
            { "level_index", levelIndex }
        });
    }

    public void TrackPuzzleComplete(int puzzleIndex, string puzzleType, float timeTaken)
    {
        SendEvent("puzzle_complete", new Dictionary<string, object>
        {
            { "puzzle_index", puzzleIndex },
            { "puzzle_type", puzzleType },
            { "time_taken", timeTaken }
        });
    }

    public void TrackPuzzleAttempt(int puzzleIndex, string puzzleType)
    {
        SendEvent("puzzle_attempt", new Dictionary<string, object>
        {
            { "puzzle_index", puzzleIndex },
            { "puzzle_type", puzzleType }
        });
    }

    public void TrackItemUsed(string itemName, string context)
    {
        SendEvent("item_used", new Dictionary<string, object>
        {
            { "item_name", itemName },
            { "context", context }
        });
    }

    public void TrackItemCollected(string itemName, string source)
    {
        SendEvent("item_collected", new Dictionary<string, object>
        {
            { "item_name", itemName },
            { "source", source }
        });
    }

    public void TrackDialogueEvent(string npcName, int dialogueIndex, string choice = "")
    {
        var parameters = new Dictionary<string, object>
        {
            { "npc_name", npcName },
            { "dialogue_index", dialogueIndex }
        };

        if (!string.IsNullOrEmpty(choice))
        {
            parameters.Add("choice", choice);
        }

        SendEvent("dialogue_event", parameters);
    }

    public void TrackChestChoice(bool isTruthChest, bool choseCorrectly)
    {
        SendEvent("chest_choice", new Dictionary<string, object>
        {
            { "chest_type", isTruthChest ? "truth" : "false" },
            { "choice_correct", choseCorrectly }
        });
    }

    public void TrackSessionStart()
    {
        SendEvent("session_start");
    }

    public void TrackSessionEnd()
    {
        SendEvent("session_end");
    }

    public void TrackCustomEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        SendEvent(eventName, parameters ?? new Dictionary<string, object>());
    }

    private void SendEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        try
        {
            var custom = new CustomEvent(eventName);
            if (parameters != null)
            {
                foreach (var kv in parameters)
                    custom.Add(kv.Key, kv.Value);
            }
            AnalyticsService.Instance.RecordEvent(custom);
            Debug.Log($"Analytics Event: {eventName} - {ToReadableString(parameters)}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to send analytics event '{eventName}': {e.Message}");
        }
    }

    private string ToReadableString(Dictionary<string, object> dict)
    {
        if (dict == null) return "";
        var list = new List<string>();
        foreach (var kv in dict)
        {
            list.Add($"{kv.Key}:{kv.Value}");
        }
        return string.Join(", ", list);
    }
}