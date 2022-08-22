using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NexusButton : MonoBehaviour
{
   [SerializeField] private Sprite off, on;
   [SerializeField] private Image buttonImage;
   [SerializeField] private ShapeManager shapeManager;

   private void Update()
   {
        if(shapeManager.HasShape)
        {
            buttonImage.sprite =  shapeManager.LastSelectedShapeHasNexus() ? on : off;
        }
   }
}
