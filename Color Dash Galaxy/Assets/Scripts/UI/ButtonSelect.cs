using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelect : MonoBehaviour, ISelectHandler
{
    public bool isFirstDefaultButtonSelection = false;

    public void OnSelect(BaseEventData eventData)
    {
        if (isFirstDefaultButtonSelection)
        {
            isFirstDefaultButtonSelection = false;
            return;
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.buttonSelectSound);
    }
}
