using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("Visuals")]
    [Tooltip("The image that hides the pressed state. It will fade to 0 alpha on click.")]
    public Image topImage;

    [Header("State")]
    public bool interactable = true;

    [Header("Action")]
    public UnityEvent onClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable || topImage == null) return;

        // Hide the top image to reveal the pressed image underneath
        SetAlpha(0f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable || topImage == null) return;

        // Restore the top image when the mouse is released
        SetAlpha(1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;

        // Trigger the actual menu action (Start, Load, Quit)
        onClick.Invoke();
    }

    private void SetAlpha(float alpha)
    {
        Color c = topImage.color;
        c.a = alpha;
        topImage.color = c;
    }

    // Called by the Menu Manager to visually gray out the Load button if no save exists
    public void SetInteractable(bool state)
    {
        interactable = state;
        if (topImage != null)
        {
            // Dim the button to gray if disabled, or restore to full white if enabled
            topImage.color = state ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }
}