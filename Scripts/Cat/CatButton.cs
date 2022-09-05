using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CatButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private ColorPicker colorPicker;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private ShapeManager offlineShapeManager;
    [SerializeField] private NetworkedShapeManager networkedShapeManager;

    // long click removes category
    [SerializeField] private float clickDuration = 1.2f;
    private bool isClicking, longClick;
    private float clickDurationCount;

    private void Update()
    {
        checkClick();

        if(image.color != colorPicker.Color)
        {
            image.color = (Color32)colorPicker.Color;
        }
        
    }
    public void SetNetworkedShapeManager(NetworkedShapeManager manager)
    {
        networkedShapeManager = manager;
    }
    private void checkClick()
    {
        if(ClickIdentifier.Instance.IsPointerOverUIObject(gameObject.name))
        {
            if(Input.GetMouseButtonDown(0))
            {
                isClicking = true;
            }

            if(isClicking && Input.GetMouseButton(0))
            {
                // is holding
                clickDurationCount += Time.deltaTime;
                if(clickDurationCount >= clickDuration)
                {
                    isClicking = false;
                    longClick = true;
                    clickDurationCount = 0f;
                }
                
            }
        }
    }

    public void OnClick()
    {
        if(longClick)
        {
            Debug.Log("CatButton: Long click!");
            // remove category
            offlineShapeManager.RemoveLastSelectedShapeCategory();
            longClick = false;
            return;
        }
        Debug.Log("CatButton: Short click!");
        // color the cat & the color picker with the active face's color
        
       
        Color currentColor;
        // make & add category
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            offlineShapeManager.MakeLastSelectedShapeCatForm();
            currentColor = (Color)offlineShapeManager.GetLastSelectedShapeColor();
        }
        else
        {
            networkedShapeManager.MakeLastSelectedShapeCatForm();
            currentColor = (Color)networkedShapeManager.GetLastSelectedShapeColor();
        }
        
        if(currentColor != null)
        {
            colorPicker.Color = currentColor;
        }
        
    }
}
