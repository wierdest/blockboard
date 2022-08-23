using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutFixerForAndroid : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup topLeft, topRight, bottomRight;
    [SerializeField] private HorizontalLayoutGroup bottomLeft;
    [SerializeField] private int padding;


    private void Awake()
    {
        // if it's android add an amount
        if(Application.platform == RuntimePlatform.Android)
        {
            //
            topLeft.padding.left = padding;
            topRight.padding.right = padding;
            bottomLeft.padding.left = padding;
            bottomRight.padding.right = padding;
            // bottomLeft.padding.bottom = padding / 2;
            // bottomRight.padding.bottom = padding / 2;
        }

        Destroy(this);
    }
}
