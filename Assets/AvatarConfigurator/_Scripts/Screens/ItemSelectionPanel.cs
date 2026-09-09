using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarBuilder
{
    public class ItemSelectionPanel : MonoBehaviour
    {
        public CategoryType ItemCategory;
        public RectTransform contentTransform;
        public RectTransform variantContentTransform;
        public GameObject variantSelectionPanel;
        public List<ItemThumbnail> itemThumbnails;

        public ItemThumbnail currentSelectedItem = null;
        public VariantThumbnail currentSelectedVariant = null;

        public static bool isAnimationAllowed = true;

        Animation anim;
        AnimationClip animClip;

        private void Awake()
        {
            anim = GetComponent<Animation>();
            animClip = anim.GetClip("AvatarCustomizeScreenSelectionPanels");
        }

        public void OnEnable()
        {
            if (isAnimationAllowed)
            {
                isAnimationAllowed = false; // To play the animation only once.
                anim[animClip.name].speed = 1;
                anim.Play(animClip.name);
            }
        }

        public void OnReset()
        {
            anim[animClip.name].speed = -1;
            anim[animClip.name].time = anim[animClip.name].length;
            anim.Play(animClip.name);
        }

        private void OnDisable()
        {
            OnReset();
        }

        public void EnableDisablePanel(bool doEnable = false)
        {
            gameObject.SetActive(doEnable);
            if (variantContentTransform != null)
            {
                variantSelectionPanel.SetActive(doEnable);
            }
        }

        public void ChangeSelectableItemThumbnail(ItemThumbnail currentSelectable = null)
        {
            //Debug.Log(currentSelectable == null ? "yes" : "no");
            
            foreach (var item in itemThumbnails)
            {
                item.EnableDisableSelectable(false);
            }

            if (currentSelectable)
            {
                currentSelectedItem = currentSelectable == null ? itemThumbnails[0] : currentSelectable;
                currentSelectedItem.EnableDisableSelectable(true);
            }
        }

        public void ChangeSelectableVariantThumbnail(VariantThumbnail currentSelectable = null)
        {
            if (variantSelectionPanel != null)
            {
                foreach (var item in variantSelectionPanel.GetComponentsInChildren<VariantThumbnail>())
                {
                    item.EnableDisableSelectable(false);
                }
            }
            if (currentSelectable)
            {
                currentSelectedVariant = currentSelectable == null ? variantSelectionPanel.GetComponentsInChildren<VariantThumbnail>()[0] : currentSelectable;
                currentSelectedVariant.EnableDisableSelectable(true);
            }
        }

        public void DestroyOldItemThumbnail()
        {
            Debug.Log("DestroyOldItemThumbnail");
            
            foreach (var item in itemThumbnails)
            {
                Destroy(item.gameObject);
            }

            itemThumbnails.Clear();
        }

        public void DestroyOldVariantThumbnail()
        {
            if (variantSelectionPanel!=null)
            {
                foreach (var item in variantSelectionPanel.GetComponentsInChildren<VariantThumbnail>())
                {
                    Destroy(item.gameObject);
                }
            }
        }
    }
}