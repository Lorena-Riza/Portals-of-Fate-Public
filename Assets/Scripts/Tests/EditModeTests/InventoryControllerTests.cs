using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InventoryControllerTests
{
    private GameObject controllerGO;
    private InventoryController inventoryController;
    private GameObject inventoryPanel;
    private GameObject slotPrefab;
    private GameObject itemPrefab;

    [SetUp]
    public void SetUp()
    {
        // Setup controller GameObject
        controllerGO = new GameObject("InventoryController");
        inventoryController = controllerGO.AddComponent<InventoryController>();

        // Setup inventory panel
        inventoryPanel = new GameObject("InventoryPanel");
        inventoryPanel.transform.parent = controllerGO.transform;
        inventoryController.inventoryPanel = inventoryPanel;

        // Create fake slot prefab
        slotPrefab = new GameObject("SlotPrefab");
        slotPrefab.AddComponent<RectTransform>();
        slotPrefab.AddComponent<Slot>();
        inventoryController.slotPrefab = slotPrefab;

        inventoryController.slotCount = 3;

        // Create item prefab with int ID
        itemPrefab = new GameObject("FakeItem");
        itemPrefab.AddComponent<RectTransform>();
        var itemComponent = itemPrefab.AddComponent<Item>();
        itemComponent.ID = 101;

        // Create initial empty slots
        for (int i = 0; i < inventoryController.slotCount; i++)
        {
            var slotGO = Object.Instantiate(slotPrefab, inventoryPanel.transform);
            slotGO.name = "Slot_" + i;
        }
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(controllerGO);
        Object.DestroyImmediate(inventoryPanel);
        Object.DestroyImmediate(slotPrefab);
        Object.DestroyImmediate(itemPrefab);
    }

    [Test]
    public void HasItem_ReturnsTrue_WhenItemExists()
    {
        var slot = inventoryPanel.transform.GetChild(0).GetComponent<Slot>();
        var itemInstance = Object.Instantiate(itemPrefab, slot.transform);
        itemInstance.name = itemPrefab.name + "_Clone";
        slot.currentItem = itemInstance;

        Assert.IsTrue(inventoryController.HasItem(itemPrefab));
    }

    [Test]
    public void GetInventoryItems_ReturnsCorrectSaveData()
    {
        var slot = inventoryPanel.transform.GetChild(0).GetComponent<Slot>();
        var itemInstance = Object.Instantiate(itemPrefab, slot.transform);
        itemInstance.name = itemPrefab.name + "_Clone";
        slot.currentItem = itemInstance;

        var saveData = inventoryController.GetInventoryItems();

        Assert.AreEqual(1, saveData.Count);
        Assert.AreEqual(101, saveData[0].itemID);
        Assert.AreEqual(0, saveData[0].slotIndex);
    }
}