using System.Collections;
using System.Collections.Generic;
using AvatarBuilder;
using UnityEngine;
using UnityEngine.UI;
namespace AvatarBuilder
{

    public class VariantThumbnail : ItemThumbnail
    {

        private void Start()
        {
            //AvatarConfigurator.ThumbnailDownloaderAsynchrously(LoadImage, itemDataJson.);
            button.onClick.AddListener(() => BtnClickedEvent());
        }

        private void BtnClickedEvent()
        {
            ThumbnailJsonData categoryThumbnailJsonData = new ThumbnailJsonData();
            ItemDataJson itemDataJson = new ItemDataJson();
            switch (itemType)
            {
                case ItemType.blend:
                    itemDataJson.Blend = new List<ItemProperties>();
                    itemDataJson.Blend.Add(itemProperties);
                    break;
                case ItemType.color:
                    itemDataJson.Color = new List<ItemProperties>();
                    itemDataJson.Color.Add(itemProperties);
                    break;
                case ItemType.mesh:
                    itemDataJson.Mesh = new List<ItemProperties>();
                    itemDataJson.Mesh.Add(itemProperties);
                    break;
                case ItemType.texture:
                    itemDataJson.Texture = new List<ItemProperties>();
                    itemDataJson.Texture.Add(itemProperties);
                    break;
            }

            categoryThumbnailJsonData.mongodbID = categoryID;
            categoryThumbnailJsonData.CategoryType = itemCategory.ToString();
            categoryThumbnailJsonData.Data = itemDataJson;
            AvatarParent_FbxHolder.instance.currentSelectedAvatar.AddNewVariantInAvatarSpecs(categoryThumbnailJsonData);
            GetComponentInParent<ItemSelectionPanel>().ChangeSelectableVariantThumbnail(this);
            AvatarParent_FbxHolder.instance.currentSelectedAvatar.UpdateColorsAndBlendOfAvatar();
        }

        private void Update()
        {
            if (LoadingImage.gameObject.activeInHierarchy) //To save process if disable
            {
                rotationEuler += Vector3.forward * 30 * Time.deltaTime; //increment 30 degrees every second
                LoadingImage.transform.rotation = Quaternion.Euler(rotationEuler);
            }
        }

        public void LoadProperties(ItemThumbnail item, ItemType itemType)
        {
            LoadingImage.gameObject.SetActive(true);
            this.itemType = itemType;
            this.itemCategory = item.itemCategory;
            this.categoryID = item.categoryID;
            if (this.itemType == ItemType.color)
            {
                Color newCol;
                if (ColorUtility.TryParseHtmlString(itemProperties.Value, out newCol))
                    buttonSprite.color = newCol;

                LoadingImage.gameObject.SetActive(false);

            }
        }
    }
}