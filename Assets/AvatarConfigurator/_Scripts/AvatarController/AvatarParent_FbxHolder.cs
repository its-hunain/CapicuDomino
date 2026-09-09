using System;
using UnityEngine;

namespace AvatarBuilder
{
    public class AvatarParent_FbxHolder : MonoBehaviour
    {
        public AvatarFBXController[] avatars;

        [SerializeField]
        public CachedItemThumbnailJson cachedSelecteditem = new CachedItemThumbnailJson();

        public AvatarFBXController currentSelectedAvatar;

        public static AvatarParent_FbxHolder instance;

        public void Awake()
        {
            instance = this;
            //FindFBX();

            ResetData();
        }

        [ContextMenu("Find Data")]
        public void FindFBX()
        {
            instance = this;
            avatars = GetComponentsInChildren<AvatarFBXController>(true);
            foreach (var item in avatars)
            {
                if (item.gameObject.name.ToLower().Contains("female"))
                {
                    item.tag = "female";
                }
                else
                {
                    item.tag = "male";
                }
                item.FindData();
                item.gameObject.SetActive(false);
            }
        }

       
        public CachedItemThumbnailJson GetAvatarSpecs(string json)
        {
            CachedItemThumbnailJson getSavedAvatarSpecs = CachedItemThumbnailJson.FromJson(json);
            cachedSelecteditem.Message = getSavedAvatarSpecs.Message;
            cachedSelecteditem.femaleDefaultDataList = getSavedAvatarSpecs.femaleDefaultDataList;
            cachedSelecteditem.maleDefaultDataList = getSavedAvatarSpecs.maleDefaultDataList;
            cachedSelecteditem.gender = getSavedAvatarSpecs.gender.ToLower();
            cachedSelecteditem.IsNewUser = getSavedAvatarSpecs.IsNewUser;
            cachedSelecteditem.maleAvatarPrice = getSavedAvatarSpecs.maleAvatarPrice;
            cachedSelecteditem.femaleAvatarPrice = getSavedAvatarSpecs.femaleAvatarPrice;
            cachedSelecteditem.maleAvatarCode = getSavedAvatarSpecs.maleAvatarCode;
            cachedSelecteditem.femaleAvatarCode = getSavedAvatarSpecs.femaleAvatarCode;

            //Default Character
            if (getSavedAvatarSpecs.IsNewUser)
            {
                //if (getSavedAvatarSpecs.gender == "male")
                //{
                //    cachedSelecteditem.DataList = getSavedAvatarSpecs.maleDefaultDataList;
                //}
                //else
                //{
                //    cachedSelecteditem.DataList = getSavedAvatarSpecs.femaleDefaultDataList;
                //}
            }
            else //Previous Cached Character
            {
                cachedSelecteditem.DataList = getSavedAvatarSpecs.DataList;
            }
            return cachedSelecteditem;
        }

        internal void ResetData()
        {
            foreach (var item in avatars)
            {
                item.ResetAvatarData();
            }
        }


        public void UpdateDataOnModel()
        {
            Debug.Log("UpdateDataOnModel()");
            //Setting Texture of Facial Hair and Eye Brows

            Debug.Log("cachedSelecteditem.DataList: " + cachedSelecteditem.DataList.Count);
            foreach (var item in cachedSelecteditem.DataList)
            {
                Debug.Log("item CategoryType: " + item.CategoryType);
                Debug.Log("item: " + item.Data.ToString());
                CategoryType categoryType;
                if (Enum.TryParse(item.CategoryType, out categoryType))
                {
                    if (categoryType == CategoryType.eyeBrows)
                    {
                        if (item.Data.Texture.Count > 0)
                        {
                            Debug.Log("1++++++++++++++");
                            currentSelectedAvatar.TextureDownload(categoryType, null, item.Data.Texture[0].Url, true);
                        }
                    }
                    if (categoryType == CategoryType.facialHair)
                    {
                        if (item.Data.Texture.Count > 0)
                        {
                            Debug.Log("2++++++++++++++");
                            currentSelectedAvatar.TextureDownload(categoryType, null, item.Data.Texture[0].Url, true);
                        }
                    }
                }
            }
            //Setting Remaining Categories Data
            currentSelectedAvatar.UpdateColorsAndBlendOfAvatar();

        }
    }
}