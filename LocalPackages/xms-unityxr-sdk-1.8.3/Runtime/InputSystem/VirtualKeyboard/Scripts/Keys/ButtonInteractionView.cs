using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ximmerse.RhinoX
{
    /// <summary>
    /// Button interaction view.
    /// </summary>
    public class ButtonInteractionView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {

        public Image bg_Image;

        public Color32 color_Bg_Default = new Color32(48, 47, 55, 255);

        public Color32 color_Bg_Hover = new Color32(126, 122, 35, 255);

        public Color32 color_Bg_Highlighted = new Color32(96, 123, 41, 255);

        bool isHovered = false;

        bool isHightligthed = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            bg_Image.color = color_Bg_Hover;
            isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            bg_Image.color = color_Bg_Default;
            isHovered = false;
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            bg_Image.color = color_Bg_Highlighted;
            isHightligthed = true;
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            if(isHovered)
            {
                bg_Image.color = color_Bg_Hover;
            }
            else
            {
                bg_Image.color = color_Bg_Default;
            }
            isHightligthed = false;
        }

        void OnValidate()
        {
            if(!bg_Image)
            {
                bg_Image = GetComponent<Image>();
                color_Bg_Default = bg_Image.color;
            }
        }
    }
}