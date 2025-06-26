using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindSaveLoad : MonoBehaviour
{
    public InputActionAsset actions;

    private void Awake()
    {
        LoadRebinds();
    }

    private void Start()
    {
        // Ensure rebinds are loaded after a short delay to allow other initializations
        Invoke(nameof(LoadRebinds), 0.1f);
    }

    private void OnDisable()
    {
        SaveRebinds();
    }

    private void LoadRebinds()
    {
        string rebindsFilePath = Path.Combine(Application.persistentDataPath, "rebinds.json");
        if (File.Exists(rebindsFilePath))
        {
            string rebindsJson = File.ReadAllText(rebindsFilePath);
            actions.LoadBindingOverridesFromJson(rebindsJson);
        }
    }

    private void SaveRebinds()
    {
        string rebindsJson = actions.SaveBindingOverridesAsJson();
        string rebindsFilePath = Path.Combine(Application.persistentDataPath, "rebinds.json");

        // Ensure the directory exists
        string directoryPath = Path.GetDirectoryName(rebindsFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(rebindsFilePath, rebindsJson);
    }
}
