using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PuzzleControllerTests
{
    private GameObject go;
    private PuzzleController controller;

    [SetUp]
    public void Setup()
    {
        go = new GameObject();
        controller = go.AddComponent<PuzzleController>();

        controller.puzzlePanels = new GameObject[3]; // Simulate 3 puzzles
        controller.puzzleCanvas = new GameObject();

        controller.Awake(); // Manually call because Unity won't auto-call it in Edit Mode
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    [Test]
    public void ResetPuzzleState_SetsPuzzleClosed()
    {
        PuzzleController.Instance.ResetPuzzleState();
        Assert.IsFalse(PuzzleController.IsPuzzleOpen);
    }

    [Test]
    public void MarkPuzzleIncomplete_WorksCorrectly()
    {
        controller.MarkPuzzleIncomplete(1);
        Assert.IsFalse(controller.IsPuzzleCompleted(1));
    }

    [Test]
    public void SetPuzzleCompletionStates_UpdatesCorrectly()
    {
        var newStates = new List<bool> { true, false, true };
        controller.SetPuzzleCompletionStates(newStates);

        var states = controller.GetPuzzleCompletionStates();
        Assert.AreEqual(newStates, states);
    }
}