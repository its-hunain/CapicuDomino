using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarBuilder
{
    public class CategorySelectionPanel : MonoBehaviour
    {
        [SerializeField]
        public List<CategoryThumbnail> categories;
        public CategoryThumbnail currentSelectedCategoryThumbnail;


        public void ChangeSelectableIcon(CategoryThumbnail currentSelectable = null)
        {
            foreach (var item in categories)
            {
                item.EnableDisableSelectable(false);
            }
            if (currentSelectable != null)
            {
                currentSelectedCategoryThumbnail = currentSelectable;
                currentSelectable.EnableDisableSelectable(true);
            }
        }



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}