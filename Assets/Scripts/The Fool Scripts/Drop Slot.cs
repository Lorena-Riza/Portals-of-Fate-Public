using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped != null)
        {
            dropped.transform.SetParent(transform);
            dropped.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // Automatically check win condition after each drop
            JigsawPuzzle puzzle = FindObjectOfType<JigsawPuzzle>();
            if (puzzle != null)
            {
                puzzle.CheckWin();
            }
        }
    }
}