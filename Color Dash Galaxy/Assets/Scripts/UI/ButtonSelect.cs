using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelect : MonoBehaviour, ISelectHandler
{
    public bool isFirstDefaultButtonSelection = false;

    KeepButtonSelected keepButtonSelected;

    void Start()
    {
        keepButtonSelected = FindObjectOfType<KeepButtonSelected>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (isFirstDefaultButtonSelection)
        {
            isFirstDefaultButtonSelection = false;
            return;
        }

        if (!keepButtonSelected.isMouseClicked)
            AudioManager.Instance.PlaySound(AudioManager.Instance.buttonSelectSound);

        keepButtonSelected.isMouseClicked = false;
    }
}
