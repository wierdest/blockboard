using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickIdentifier : MonoBehaviour
{
    public static ClickIdentifier Instance;
    private int uiLayer; // identify ui elements by layer

    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        uiLayer = LayerMask.NameToLayer("UI");
    }

    public bool IsPointerOverUI()
    {
        
        return isPointerOverUI(GetRaycastResults());
    }
    private bool isPointerOverUI(List<RaycastResult> eventSystemRaycastResults)
    {
        for(int i = 0; i < eventSystemRaycastResults.Count; i++)
        {
            RaycastResult result = eventSystemRaycastResults[i];
            if(result.gameObject.layer  == uiLayer)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsPointerOverUIObject(string objectName)
    {
        return isPointerOverUIObject(GetRaycastResults(), objectName);
    }

     private bool isPointerOverUIObject(List<RaycastResult> eventSystemRaycastResults, string objectName)
    {
        for(int i = 0; i < eventSystemRaycastResults.Count; i++)
        {
            RaycastResult result = eventSystemRaycastResults[i];
            if(result.gameObject.layer  == uiLayer && result.gameObject.name.Equals(objectName))
            {
                return true;
            }
        }
        return false;
    }

    private static List<RaycastResult> GetRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }
    
}
