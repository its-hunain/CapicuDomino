using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Glauz.Blendshapes
{
    [RequireComponent(typeof(Slider))]
    public class BlendShapeSlider : MonoBehaviour
    {
        //Do not need suffix
        [Header("Do not include the suffixes of the BlendShape Name")]
        public string blendShapeName;
        private Slider slider;


        private void Start()
        {
            blendShapeName = blendShapeName.Trim();
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(value => CharacterCustomization.Instance.ChangeBlendshapeValue(blendShapeName, value));
            //slider.onValueChanged.Invoke(value);
        }

    } 
}
