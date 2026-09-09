using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarBuilder
{
    public class ItemThumbnail : MonoBehaviour
    {
        [SerializeField]
        public ItemProperties itemProperties;
        public Image buttonSprite;
        public Button button;
        public GameObject selectedObj;
        public ItemType itemType;
        public CategoryType itemCategory;
        public string categoryID;
        public Image LoadingImage;
        public Texture2D texture;
        protected Vector3 rotationEuler;

        private void Start()
        {
            //AvatarConfigurator.ThumbnailDownloaderAsynchrously(LoadImage, itemDataJson.);
            button.onClick.AddListener(() => BtnClickedEvent());
        }

        private void BtnClickedEvent()
        {
            ItemDataJson itemDataJson = new ItemDataJson();
            ThumbnailJsonData categoryThumbnailJsonData = new ThumbnailJsonData();

            categoryThumbnailJsonData = AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList.Find(x => x.mongodbID == categoryID); //if already found then edit otherwise it's already null
            itemDataJson = categoryThumbnailJsonData.Data;

            switch (itemType)
            {
                case ItemType.blend:
                    itemDataJson.Blend.Clear();
                    itemDataJson.Blend.Add(itemProperties);
                    break;
                case ItemType.color:
                    itemDataJson.Color.Clear();
                    itemDataJson.Color.Add(itemProperties);
                    break;
                case ItemType.mesh:
                    itemDataJson.Mesh.Clear();
                    itemDataJson.Mesh.Add(itemProperties);
                    break;
                case ItemType.texture:
                    itemDataJson.Texture.Clear();
                    itemDataJson.Texture.Add(itemProperties);
                    break;
            }

            categoryThumbnailJsonData.mongodbID = categoryID;
            categoryThumbnailJsonData.CategoryType = itemCategory.ToString();
            categoryThumbnailJsonData.Data = itemDataJson;
            AvatarParent_FbxHolder.instance.currentSelectedAvatar.AddNewItemInAvatarSpecs(categoryThumbnailJsonData);
            GetComponentInParent<ItemSelectionPanel>().ChangeSelectableItemThumbnail(this);
            AvatarParent_FbxHolder.instance.currentSelectedAvatar.UpdateColorsAndBlendOfAvatar();

            if (itemCategory == CategoryType.eyeBrows) AvatarParent_FbxHolder.instance.currentSelectedAvatar.ChangeEyeBrowsTexture(this.texture);
            if (itemCategory == CategoryType.facialHair) AvatarParent_FbxHolder.instance.currentSelectedAvatar.ChangeFacialStyleTexture(this.texture);
        }


        public void EnableDisableSelectable(bool value) { selectedObj.SetActive(value); }

        private void Update()
        {
            if (LoadingImage.gameObject.activeInHierarchy) //To save process if disable
            {
                rotationEuler += Vector3.forward * 50 * Time.deltaTime; //increment 30 degrees every second
                LoadingImage.transform.rotation = Quaternion.Euler(rotationEuler);
            }
        }

        public virtual void LoadProperties(ItemType itemType, CategoryType itemCategory)
        {
            LoadingImage.gameObject.SetActive(true);
            this.itemType = itemType;
            this.itemCategory = itemCategory;

            if (this.itemCategory == CategoryType.skin)
            {
                if (this.itemType == ItemType.color)
                {
                    Color newCol;
                    if (ColorUtility.TryParseHtmlString(itemProperties.Value, out newCol))
                        buttonSprite.color = newCol;
                }
            }
        }

        public void SetThumbnail(object sprite)
        {
            Debug.Log("SetThumbnail");
            LoadingImage.gameObject.SetActive(false);
            buttonSprite.sprite = (Sprite)sprite;
        }

        public void SetTexture(Texture2D texture)
        {
            Debug.Log("SetTexture");
            this.texture = texture;
        }
    }
}