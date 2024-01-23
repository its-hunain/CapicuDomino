using AvatarBuilder;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarBuilder
{
    public class CategoryThumbnail : MonoBehaviour
    {
        public string mongoDBCategory_ID;
        public string category_ID;
        public string category_Name;
        public string class_ID;
        public CategoryType categoryType;
        public GameObject selectedObj;
        public ItemSelectionPanel itemSelectionPanel;
        public List<ItemThumbnail> itemsThumbnailList;
        public List<VariantThumbnail> variantsThumbnailList;
        public Button btn;
        AvatarScreenManager avatarScreenManager;

        private void Start()
        {
            btn = GetComponent<Button>();
            btn.onClick.AddListener(() => LoadThumbnailsDataByCategoryID());
        }


        public void EnableDisableSelectable(bool value) { selectedObj.SetActive(value); }

        public void LoadThumbnailsDataByCategoryID()
        {
            if (categoryType == CategoryType.skin)
                CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarTargetPositions.bodyView);
            else
                CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarTargetPositions.faceView);


            Debug.Log("Category Name: " + category_Name);
            avatarScreenManager = AvatarScreenManager.instance;

            AvatarScreenManager.instance.categorySelectionPanel.ChangeSelectableIcon(this);

            if (!itemSelectionPanel.gameObject.activeInHierarchy)
            {
                avatarScreenManager.DisableAllItemThumbnailSelectionPanels();
                itemSelectionPanel.EnableDisablePanel(true);
            }
            else
            {
                AvatarScreenManager.instance.categorySelectionPanel.ChangeSelectableIcon();
                avatarScreenManager.DisableAllItemThumbnailSelectionPanels();
            }

            if (itemsThumbnailList.Count == 0)
            {
                AvatarConfigurator.ItemsDownloaderAsynchrously(FillCategoryItems, category_Name, string.Format($"{WebServiceManager.instance.getAvatarBuilderCategories}/{mongoDBCategory_ID}/items"));
            }
        }

        public void FillCategoryItems(ItemDataJson itemsThumbnailData)
        {
            Debug.Log("FillCategoryItems");
            Debug.Log("category_ID: " + category_ID);
            ItemThumbnail itemThumb = null;
            switch (categoryType)
            {
                case CategoryType.skin: //Skin
                    foreach (var item in itemsThumbnailData.Color)
                    {
                        Debug.Log("item.ID: " + item.itemID);
                        Debug.Log("item.Value: " + item.Value);
                        itemThumb = Instantiate(avatarScreenManager.itemThumbnailPrefab, itemSelectionPanel.contentTransform).GetComponent<ItemThumbnail>();
                        itemThumb.categoryID = mongoDBCategory_ID;
                        itemThumb.itemProperties = item;
                        itemsThumbnailList.Add(itemThumb);
                        itemThumb.LoadProperties(ItemType.color, CategoryType.skin);
                        AvatarConfigurator.ThumbnailDownloaderAsynchrously(itemThumb.SetThumbnail, item.ThumbnailUrl);
                    }
                    break;

                case CategoryType.faceCut: //Face Cut
                    foreach (var item in itemsThumbnailData.Blend)
                    {
                        itemThumb = Instantiate(avatarScreenManager.itemThumbnailPrefab, itemSelectionPanel.contentTransform).GetComponent<ItemThumbnail>();
                        itemThumb.categoryID = mongoDBCategory_ID;
                        itemThumb.itemProperties = item;
                        itemsThumbnailList.Add(itemThumb);
                        itemThumb.LoadProperties(ItemType.blend, CategoryType.faceCut);
                        AvatarConfigurator.ThumbnailDownloaderAsynchrously(itemThumb.SetThumbnail, item.ThumbnailUrl);
                    }
                    break;

                case CategoryType.eyeBrows: //EyeBrow
                    foreach (var item in itemsThumbnailData.Texture)
                    {
                        itemThumb = Instantiate(avatarScreenManager.itemThumbnailPrefab, itemSelectionPanel.contentTransform).GetComponent<ItemThumbnail>();
                        itemThumb.categoryID = mongoDBCategory_ID;
                        itemThumb.itemProperties = item;
                        itemsThumbnailList.Add(itemThumb);
                        itemThumb.LoadProperties(ItemType.texture, CategoryType.eyeBrows);
                        if(item.ThumbnailUrl!=null)     AvatarConfigurator.ThumbnailDownloaderAsynchrously(itemThumb.SetThumbnail, item.ThumbnailUrl);
                        if (item.Url != null) AvatarParent_FbxHolder.instance.currentSelectedAvatar.TextureDownload(CategoryType.eyeBrows, itemThumb , null , false);
                    }
                    foreach (var variant in itemsThumbnailData.Color)
                    {
                        VariantThumbnail variantThumb = Instantiate(avatarScreenManager.variantThumbnailPrefab, itemSelectionPanel.variantContentTransform).GetComponent<VariantThumbnail>();
                        variantThumb.itemProperties = variant;
                        variantsThumbnailList.Add(variantThumb);
                        variantThumb.LoadProperties(itemThumb, ItemType.color);
                    }
                    break;

                case CategoryType.eyes: //Eye
                    foreach (var item in itemsThumbnailData.Blend)
                    {
                        itemThumb = Instantiate(avatarScreenManager.itemThumbnailPrefab, itemSelectionPanel.contentTransform).GetComponent<ItemThumbnail>();
                        itemThumb.categoryID = mongoDBCategory_ID;
                        itemThumb.itemProperties = item;
                        itemsThumbnailList.Add(itemThumb);
                        itemThumb.LoadProperties(ItemType.blend, CategoryType.eyes);
                        AvatarConfigurator.ThumbnailDownloaderAsynchrously(itemThumb.SetThumbnail, item.ThumbnailUrl);
                    }
                    foreach (var variant in itemsThumbnailData.Color)
                    {
                        VariantThumbnail variantThumb = Instantiate(avatarScreenManager.variantThumbnailPrefab, itemSelectionPanel.variantContentTransform).GetComponent<VariantThumbnail>();
                        variantThumb.itemProperties = variant;
                        variantsThumbnailList.Add(variantThumb);
                        variantThumb.LoadProperties(itemThumb, ItemType.color);
                    }
                    break;

                case CategoryType.hair: //Hair
                    foreach (var item in itemsThumbnailData.Mesh)
                    {
                        itemThumb = Instantiate(avatarScreenManager.itemThumbnailPrefab, itemSelectionPanel.contentTransform).GetComponent<ItemThumbnail>();
                        itemThumb.categoryID = mongoDBCategory_ID;
                        itemThumb.itemProperties = item;
                        itemsThumbnailList.Add(itemThumb);
                        itemThumb.LoadProperties(ItemType.mesh, CategoryType.hair);
                        AvatarConfigurator.ThumbnailDownloaderAsynchrously(itemThumb.SetThumbnail, item.ThumbnailUrl);
                    }
                    foreach (var variant in itemsThumbnailData.Color)
                    {
                        VariantThumbnail variantThumb = Instantiate(avatarScreenManager.variantThumbnailPrefab, itemSelectionPanel.variantContentTransform).GetComponent<VariantThumbnail>();
                        variantThumb.itemProperties = variant;
                        variantsThumbnailList.Add(variantThumb);
                        variantThumb.LoadProperties(itemThumb, ItemType.color);
                    }
                    break;

                case CategoryType.nose: //Nose
                    foreach (var item in itemsThumbnailData.Blend)
                    {
                        itemThumb = Instantiate(avatarScreenManager.itemThumbnailPrefab, itemSelectionPanel.contentTransform).GetComponent<ItemThumbnail>();
                        itemThumb.categoryID = mongoDBCategory_ID;
                        itemThumb.itemProperties = item;
                        itemsThumbnailList.Add(itemThumb);
                        itemThumb.LoadProperties(ItemType.blend, CategoryType.nose);
                        AvatarConfigurator.ThumbnailDownloaderAsynchrously(itemThumb.SetThumbnail, item.ThumbnailUrl);
                    }
                    break;

                case CategoryType.lips: //Lips
                    foreach (var item in itemsThumbnailData.Blend)
                    {
                        itemThumb = Instantiate(avatarScreenManager.itemThumbnailPrefab, itemSelectionPanel.contentTransform).GetComponent<ItemThumbnail>();
                        itemThumb.categoryID = mongoDBCategory_ID;
                        itemThumb.itemProperties = item;
                        itemsThumbnailList.Add(itemThumb);
                        itemThumb.LoadProperties(ItemType.blend, CategoryType.lips);
                        AvatarConfigurator.ThumbnailDownloaderAsynchrously(itemThumb.SetThumbnail, item.ThumbnailUrl);
                    }
                    foreach (var variant in itemsThumbnailData.Color)
                    {
                        VariantThumbnail variantThumb = Instantiate(avatarScreenManager.variantThumbnailPrefab, itemSelectionPanel.variantContentTransform).GetComponent<VariantThumbnail>();
                        variantThumb.itemProperties = variant;
                        variantsThumbnailList.Add(variantThumb);
                        variantThumb.LoadProperties(itemThumb, ItemType.color);
                    }
                    break;

                case CategoryType.facialHair: //FacialHair
                    foreach (var item in itemsThumbnailData.Texture)
                    {
                        itemThumb = Instantiate(avatarScreenManager.itemThumbnailPrefab, itemSelectionPanel.contentTransform).GetComponent<ItemThumbnail>();
                        itemThumb.categoryID = mongoDBCategory_ID;
                        itemThumb.itemProperties = item;
                        itemsThumbnailList.Add(itemThumb);
                        itemThumb.LoadProperties(ItemType.texture, CategoryType.facialHair);
                        if (item.ThumbnailUrl != null)  AvatarConfigurator.ThumbnailDownloaderAsynchrously(itemThumb.SetThumbnail, item.ThumbnailUrl);
                        if (item.Url != null) AvatarParent_FbxHolder.instance.currentSelectedAvatar.TextureDownload(CategoryType.facialHair, itemThumb, null , false);
                    }
                    foreach (var variant in itemsThumbnailData.Color)
                    {
                        VariantThumbnail variantThumb = Instantiate(avatarScreenManager.variantThumbnailPrefab, itemSelectionPanel.variantContentTransform).GetComponent<VariantThumbnail>();
                        variantThumb.itemProperties = variant;
                        variantsThumbnailList.Add(variantThumb);
                        variantThumb.LoadProperties(itemThumb, ItemType.color);
                    }
                    break;
            }

            itemSelectionPanel.itemThumbnails = itemsThumbnailList;
            if (itemSelectionPanel.currentSelectedItem == null)
            {
                itemSelectionPanel.itemThumbnails[0].EnableDisableSelectable(true);
            }
            AvatarParent_FbxHolder.instance.currentSelectedAvatar.SetAvatarSpecsOn_UI();  //Reflect Old Selected Data
        }

        public void EnableDisableCategory(bool doEnable = false)
        {
            //Debug.Log("this: " + this.name, this.gameObject);
            gameObject.SetActive(doEnable);
        }
    }
}