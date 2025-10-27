using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class CrosshairNormal : MonoBehaviour
    {
        [Header("Crosshair - Normal")]
        [SerializeField] Image Base;
        [SerializeField] Image Grabbable;
        [SerializeField] Image Interactable;
        [SerializeField] float MaxActiveSize = 90;
        [SerializeField] float MinActiveSize = 20;
        [SerializeField] float Smooth = 10;

        public bool State;
    public bool Interaction;
        private void Update()
        {
            if (State)
            {
                Color OpacityColor = Base.color;
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 0, Smooth * 2 * Time.deltaTime);
                Base.color = OpacityColor;
                Color OpacityColor1 = Grabbable.color;
                OpacityColor1.a = Mathf.Lerp(OpacityColor1.a, 1, Smooth * 2 * Time.deltaTime);
                Grabbable.color = OpacityColor1;
                float BackValue = Mathf.Lerp(Grabbable.rectTransform.rect.width, MaxActiveSize, Smooth * Time.deltaTime);
                Grabbable.rectTransform.sizeDelta = new Vector2(BackValue, BackValue);
            }
            else
            {
                Color OpacityColor1 = Grabbable.color;
                OpacityColor1.a = Mathf.Lerp(OpacityColor1.a, 0, Smooth / 2 * Time.deltaTime);
                Grabbable.color = OpacityColor1;
                float BackValue = Mathf.Lerp(Grabbable.rectTransform.rect.width, MinActiveSize, Smooth * Time.deltaTime);
                Grabbable.rectTransform.sizeDelta = new Vector2(BackValue, BackValue);
                if (Grabbable.rectTransform.rect.width < MinActiveSize * 1.5)
                {
                    Color OpacityColor = Base.color;
                    OpacityColor.a = Mathf.Lerp(OpacityColor.a, 1, Smooth * Time.deltaTime);
                    Base.color = OpacityColor;
                }
            }
        if (Interaction)
        {
            Color OpacityColor = Base.color;
            OpacityColor.a = Mathf.Lerp(OpacityColor.a, 0, Smooth * 2 * Time.deltaTime);
            Base.color = OpacityColor;
            Color OpacityColor1 = Interactable.color;
            OpacityColor1.a = Mathf.Lerp(OpacityColor1.a, 1, Smooth * 2 * Time.deltaTime);
            Interactable.color = OpacityColor1;
            float BackValue = Mathf.Lerp(Interactable.rectTransform.rect.width, MaxActiveSize, Smooth * Time.deltaTime);
            Interactable.rectTransform.sizeDelta = new Vector2(BackValue, BackValue);
        }
        else
        {
            Color OpacityColor1 = Interactable.color;
            OpacityColor1.a = Mathf.Lerp(OpacityColor1.a, 0, Smooth / 2 * Time.deltaTime);
            Interactable.color = OpacityColor1;
            float BackValue = Mathf.Lerp(Interactable.rectTransform.rect.width, MinActiveSize, Smooth * Time.deltaTime);
            Interactable.rectTransform.sizeDelta = new Vector2(BackValue, BackValue);
            if (Interactable.rectTransform.rect.width < MinActiveSize * 1.5)
            {
                Color OpacityColor = Base.color;
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 1, Smooth * Time.deltaTime);
                Base.color = OpacityColor;
            }
        }
        }
    public void StateActive()
    {
            State = true;
    }
    public void StateBase()
    {
            State = false;
            Interaction = false;
    }
    public void InteractiveActive()
    {
        Interaction = true;
    }
    public void InteractiveBase()
    {
        Interaction = false;
    }
}