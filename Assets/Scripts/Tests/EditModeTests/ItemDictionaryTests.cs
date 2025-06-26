using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ItemDictionaryTests
{
    private GameObject gameObject;
    private ItemDictionary itemDictionary;

    [SetUp]
    public void Setup()
    {
        gameObject = new GameObject("ItemDictionaryGO");
        itemDictionary = gameObject.AddComponent<ItemDictionary>();

        var itemList = new List<Item>();

        for (int i = 0; i < 3; i++)
        {
            var itemGO = new GameObject("Item" + i);
            var itemComp = itemGO.AddComponent<Item>();  // Real Item from your project
            itemList.Add(itemComp);
        }

        itemDictionary.itemPrefabs = itemList;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameObject);
        foreach (var item in itemDictionary.itemPrefabs)
        {
            Object.DestroyImmediate(item.gameObject);
        }
    }

    [Test]
    public void Awake_AssignsIDsAndPopulatesDictionary()
    {
        itemDictionary.Awake();

        for (int i = 0; i < itemDictionary.itemPrefabs.Count; i++)
        {
            Assert.AreEqual(i + 1, itemDictionary.itemPrefabs[i].ID, $"Item at index {i} has incorrect ID");
        }

        for (int i = 1; i <= itemDictionary.itemPrefabs.Count; i++)
        {
            var prefab = itemDictionary.GetItemPrefab(i);
            Assert.IsNotNull(prefab, $"Prefab for ID {i} should not be null");
            Assert.AreEqual("Item" + (i - 1), prefab.name);
        }
    }

    [Test]
    public void GetItemPrefab_InvalidID_LogsErrorAndReturnsNull()
    {
        itemDictionary.Awake();

        LogAssert.Expect(LogType.Error, "Item with ID 99 not found in the dictionary.");

        var prefab = itemDictionary.GetItemPrefab(99);
        Assert.IsNull(prefab);
    }
}