using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeepButtonSelected : MonoBehaviour
{
    private GameObject lastselect;
    public bool isMouseClicked = false;

    private void Start()
    {
#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
#endif

        lastselect = new GameObject();
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            isMouseClicked = true;
            EventSystem.current.SetSelectedGameObject(lastselect);
        }
        else
        {
            lastselect = EventSystem.current.currentSelectedGameObject;
        }
    }
}
