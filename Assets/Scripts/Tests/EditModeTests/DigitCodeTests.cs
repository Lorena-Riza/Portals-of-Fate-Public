using NUnit.Framework;
using UnityEngine;
using TMPro;

public class DigitCodeTest
{
    private GameObject digitCodeGO;
    private DigitCode digitCode;

    private GameObject codeDisplayGO;
    private TMP_Text codeDisplay;

    private GameObject inventoryGO;
    private InventoryController inventoryController;

    private GameObject inventoryItemPrefab;

    [SetUp]
    public void Setup()
    {
        // Create DigitCode GameObject & component
        digitCodeGO = new GameObject("DigitCode");
        digitCode = digitCodeGO.AddComponent<DigitCode>();

        // Create TMP_Text for display
        codeDisplayGO = new GameObject("CodeDisplay");
        codeDisplay = codeDisplayGO.AddComponent<TextMeshProUGUI>();
        digitCode.codeDisplay = codeDisplay;

        // Create InventoryController GameObject & component
        inventoryGO = new GameObject("InventoryController");
        inventoryController = inventoryGO.AddComponent<InventoryController>();

        // Set inventoryController in DigitCode (private field via reflection)
        var invCtrlField = typeof(DigitCode).GetField("inventoryController", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        invCtrlField.SetValue(digitCode, inventoryController);

        // Create inventory item prefab with Item component
        inventoryItemPrefab = new GameObject("InventoryItemPrefab");
        inventoryItemPrefab.AddComponent<Item>();

        var itemPrefabField = typeof(DigitCode).GetField("inventoryItemPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        itemPrefabField.SetValue(digitCode, inventoryItemPrefab);

        // Set correct code
        digitCode.correctCode = "1234";

        // Set puzzle index
        var puzzleIndexField = typeof(DigitCode).GetField("myPuzzleIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        puzzleIndexField.SetValue(digitCode, 1);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(digitCodeGO);
        Object.DestroyImmediate(codeDisplayGO);
        Object.DestroyImmediate(inventoryGO);
        Object.DestroyImmediate(inventoryItemPrefab);
    }

    [Test]
    public void SubmitIncorrectCode_ResetsInputAndDisplay()
    {
        digitCode.OnDigitPressed("9");
        digitCode.OnDigitPressed("9");

        digitCode.OnSubmitPressed();

        // Input should reset
        Assert.AreEqual("", digitCode.codeDisplay.text);

        // GameObject should still be active (not disabled)
        Assert.IsTrue(digitCode.gameObject.activeSelf);
    }
}