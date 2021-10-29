using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.


/// <summary>
/// This script addresses the Unity bug that buttons stay highlighted after you select them
/// and then move off the button.
/// This results in two buttons being active at the same time.
/// https://forum.unity.com/threads/clicking-a-button-leaves-it-in-mouseover-state.285167/
/// https://answers.unity.com/questions/854724/unity-46-ui-button-highlight-color-staying-after-b.html
/// https://docs.unity3d.com/ScriptReference/UI.Selectable.OnPointerExit.html
/// My solution was based off a couple of answers together.
/// </summary>



public class ButtonHighlightfix : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler// required interface when using the OnPointerExit method. {
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EventSystem.current.alreadySelecting)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    //Do this when the cursor exits the rect area of this selectable UI object.
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("The cursor exited the selectable UI element.");
        EventSystem.current.SetSelectedGameObject(null);
    }
}