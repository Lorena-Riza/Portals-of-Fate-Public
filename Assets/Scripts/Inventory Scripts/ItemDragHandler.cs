using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Enables drag and drop functionality for items in the inventory system.
//It also handles the swapping of items between slots when an item is dropped on an already occupied slot.
public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;//Stores the original parent of the item being dragged.
    CanvasGroup canvasGroup;//Used to control the visibility and interaction of the item being dragged.

    //Gets the CanvasGroup component attached to the item when the script starts.
    void Start ()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    //When the item is clicked and dragged, it sets the original parent to the item's current parent.
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; //Save the original slot.
        transform.SetParent(transform.root); //Temporarily set the parent to the root to avoid hierarchy issues during drag.
        canvasGroup.blocksRaycasts = false; //Allow raycasts to pass through the item being dragged.
        canvasGroup.alpha = 0.6f; //Make the item semi-transparent to indicate it's being dragged.
    }

    //While the item is being dragged, it updates the item's position to follow the mouse cursor.
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    //When the item is dropped, it checks if it was dropped on a valid slot.
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; //Re-enable raycasts for the item.
        canvasGroup.alpha = 1f; //Reset the item's transparency to full visibility.

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();

        //If the item was dropped on a slot, get that slot.
        if (dropSlot == null)
        {
            //If the item was dropped on a UI element, get the parent slot of that UI element.
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }

        //Gets the original parent slot of the item being dragged.
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot != null)
        {
            if (dropSlot.currentItem != null)
            {
                //If the drop slot already has an item, swap the items.
                dropSlot.currentItem.transform.SetParent(originalParent);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                //If the drop slot is empty, set the original slot's current item to null.
                originalSlot.currentItem = null;
            }

            //Set the item being dragged to the drop slot.
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            //If the item was dropped outside of a valid slot, return it to its original parent.
            transform.SetParent(originalParent);
        }

        //Reset the item's position to the center of the slot.
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
