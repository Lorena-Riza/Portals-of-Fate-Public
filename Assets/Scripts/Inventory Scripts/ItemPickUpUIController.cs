using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This script manages the UI for item pickups in the game.
// When an Item is added to the inventory a popup will appear on the screen with the icon and name of the item.
public class ItemPickUpUIController : MonoBehaviour
{
    // Singleton instance for easy access to this controller from other scripts.
    public static ItemPickUpUIController Instance { get; private set; }

    [Header("Popup Settings")]
    public GameObject popupPrefab; // Prefab for the item pickup popup.
    public float popupDuration = 3f; // Duration for which the popup will be displayed.


    private void Awake()
    {
        // Ensure that there is only one instance of this controller in the scene.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of ItemPickUpUIController found. Destroying the new instance.");
            Destroy(gameObject); // Prevent duplicate instances.
        }
    }

    public void ShowItemPickup (string itemName, Sprite itemIcon)
    {
        // Instantiate the popup prefab and set its parent to this controller's transform.
        GameObject newPopup = Instantiate(popupPrefab, transform);
        // Set the Item name text.
        newPopup.GetComponentInChildren<TMP_Text>().text = itemName;
        // Set the Item icon image.
        Image itemImage = newPopup.transform.Find("Item Icon")?.GetComponent<Image>();
        if (itemImage)
        {
            itemImage.sprite = itemIcon;
        }

        // Start a coroutine to fade out and destroy the popup after a certain duration.
        StartCoroutine(FadeOutAndDestroy(newPopup));
    }

    // Coroutine that waits for `popupDuration`, fades out the UI, and then destroys the popup.
    private IEnumerator FadeOutAndDestroy(GameObject popup)
    {
        yield return new WaitForSeconds(popupDuration); // Wait for the specified duration before starting the fade out.
        if (popup==null) yield break; // Check in case the popup was destroyed before this line.

        // Get the CanvasGroup component to control the alpha value for fading.
        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        // Fade out over one second.
        for (float time = 0f; time < 1f; time += Time.deltaTime)
        {
            if (popup == null) yield break;
            canvasGroup.alpha = 1f - time; // Gradually decrease the alpha value.
            yield return null;
        }

        // Destroy the popup after fading out.
        Destroy(popup);
    } 
}
