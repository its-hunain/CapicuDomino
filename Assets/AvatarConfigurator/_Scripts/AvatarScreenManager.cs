using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AvatarBuilder
{
    public class AvatarScreenManager : MonoBehaviour
    {
        [Header("Avatar Screens")]
        public GenderSelectionScreen genderSelectionScreen;
        public AvatarCustomizeScreen avatarCustomizeScreen;
        public AvatarPreviewScreen avatarPreviewScreen;

        [Header("PopUps Screens")]
        public AvatarNamePopUp avatarNamePopupScreen;
        public SaveAvatarPopUpScreen SaveAvatarPopUpScreen;
        public DiscardAvatarChangesPopupScreen discardAvatarChangesPopupScreen;
        public MintAvatarPopUpScreen MintAvatarPopUpScreen;
        public QuitAppPopUpScreen QuitAppPopUpScreen;
        public GameObject userNotFoundPopUpScreen;

        [Header("UI Entity Prefabs")]
        public ItemThumbnail itemThumbnailPrefab;
        public VariantThumbnail variantThumbnailPrefab;

        public Vector3 focus;
        public Quaternion focusRotate;

        [Header("Item Selection Panels")]
        public List<ItemSelectionPanel> SelectionPanelList;

        [Space]
        public CategorySelectionPanel categorySelectionPanel;

        public GetCatagories getCatagories;
        public GameObject showBlackScreen;

        public static AvatarScreenManager instance;

        public void Awake()
        {
            instance = this;
        }

        public void ResetAvatarConfiguratorDataFromUI()
        {
            DisableAllItemThumbnailSelectionPanels();
            DestroyAllItemThumbnailSelectionPanels();
            ResetSelectedItems();
        }

        internal void DisableAllItemThumbnailSelectionPanels()
        {
            foreach (var item in SelectionPanelList)
            {
                item.EnableDisablePanel(false);
            }
        }

        internal void DestroyAllItemThumbnailSelectionPanels()
        {
            foreach (var item in SelectionPanelList)
            {
                item.DestroyOldItemThumbnail();
                item.DestroyOldVariantThumbnail();
            }
        }

        private void ResetSelectedItems()
        {
            foreach (var item in SelectionPanelList)
            {
                item.ChangeSelectableItemThumbnail();
                item.ChangeSelectableVariantThumbnail();
            }
            categorySelectionPanel.ChangeSelectableIcon();

        }

        public static void ChangeScreen(GameObject nextScreen, GameObject currentScreen, bool isPush)
        {
            instance.StartCoroutine(_ChangeScreen(nextScreen, currentScreen, isPush));
        }

        public static IEnumerator _ChangeScreen(GameObject nextScreen, GameObject currentScreen, bool isPush)
        {

            //Vector3 offset = new Vector3(2000, 0, 0);
            //Vector3 reset = Vector3.zero;
            float reset = isPush ? 0 : 1;

            yield return new WaitForSeconds(isPush ? 0.1f : 0);
            //Vector3 moveValue = isPush ? offset * -1 : offset;
            float moveValue = isPush ? 1 : 0;

            nextScreen.SetActive(true);
            LeanTween.alpha(currentScreen, moveValue, 0.5f).setEaseInSine().setOnComplete(() => currentScreen.SetActive(false));
            LeanTween.alpha(nextScreen, reset, 0.5f).setEaseOutSine();
        }

        public void SpawnCategories(List<JsonCatagory> jsonCatagories)
        {
            //Debug.Log("spawing list of buttons:"+jsonCatagories.ToString());
            //Debug.Log("spawing list of buttons count:"+jsonCatagories.Count);

            foreach (var item in categorySelectionPanel.categories)
            {
                item.EnableDisableCategory(false);
            }

            foreach (var item in jsonCatagories)
            {
                Debug.Log("item.ShorCode: " + item.shortCode);
                Debug.Log("item.gender: " + item.gender);
                //Debug.Log("item.mongoDBCategory_Id: " + item.mongoDBCategory_Id);
                Debug.Log("item.mapCategoryId: " + item.mapCategoryId);
                //Debug.Log("item.mongoDBCategory_Id: " + item.mongoDBCategory_Id);
                var categoryItem = categorySelectionPanel.categories.Find(x => x.category_ID.Equals(item.mapCategoryId));
                if (categoryItem!=null)
                {
                    categoryItem.mongoDBCategory_ID = item.mongoDBCategory_Id;
                    categoryItem.EnableDisableCategory(true);
                }
            }
        }

        public static void OpenClosePopUp(GameObject panel, bool doOpen)
        {
            if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);


            if (doOpen)
            {
                panel.SetActive(true);
                if(instance && instance.showBlackScreen) instance.showBlackScreen.SetActive(doOpen);
                LeanTween.scale(panel, Vector3.one, .5f).setEaseOutBack();
            }
            else
            {
                LeanTween.scale(panel, Vector3.zero, .5f).setEaseInBack().setOnComplete(()=> OnComplete(panel, doOpen));
            }
        }

        static void OnComplete(GameObject panel, bool doOpen)
        {
            panel.SetActive(doOpen);
            if (instance && instance.showBlackScreen) instance.showBlackScreen.SetActive(doOpen);
        }

        #region CallBack Events
        internal void OnFailGetAvatarSpecs(string obj)
        {
            Debug.LogError(obj.ToString());
        }

        internal void OnSuccessGetAvatarSpecs(JObject response, long onSuccess)
        {
            if (onSuccess == 200)
            {
                Debug.Log("Response: " + response.ToString());

                //genderSelectionScreen.SelectAvatar(PlayerPrefs.HasKey(GenderSelectionScreen.gender_PrefKey) ? GenderSelectionScreen.GetInt(GenderSelectionScreen.gender_PrefKey) : GenderSelectionScreen.SetInt(GenderSelectionScreen.gender_PrefKey, 1));

                var avatarData = AvatarParent_FbxHolder.instance.GetAvatarSpecs(response.ToString());

                if (!string.IsNullOrEmpty(avatarData.gender))
                {                    
                    int gender = GenderSelectionScreen.SetInt(GenderSelectionScreen.gender_PrefKey, avatarData.gender == "female" ? 0 : 1);
                    genderSelectionScreen.SelectAvatar(gender);
                }

                if (avatarData.IsNewUser == false)
                {
                    Debug.Log("isNewUser = " + avatarData.IsNewUser);
                    genderSelectionScreen.gameObject.SetActive(false);
                    avatarPreviewScreen.gameObject.SetActive(true);
                    if (AvatarParent_FbxHolder.instance.currentSelectedAvatar != null)
                    {
                        AvatarParent_FbxHolder.instance.currentSelectedAvatar.gameObject.SetActive(true);
                    }
                }
                else
                {
                    Debug.Log("isNewUser = " + avatarData.IsNewUser);
                    //var loaded_text_file = Resources.Load("AvatarBuilder/Json/dummyJson") as TextAsset;
                    //avatarData = AvatarParent_FbxHolder.instance.cachedSelecteditem = CachedItemThumbnailJson.FromJson(loaded_text_file.text);

                    //Reset Previous Data from Model
                    //AvatarParent_FbxHolder.instance.ResetAvatarData();

                    genderSelectionScreen.gameObject.SetActive(true);
                    avatarPreviewScreen.gameObject.SetActive(false);
                }

                if (avatarData.DataList.Count > 0)
                {
                    //Setting Texture of Facial Hair and Eye Brows
                    //Setting Remaining Categories Data
                    AvatarParent_FbxHolder.instance.UpdateDataOnModel();
                }
            }
            else
            {
                Debug.LogError(response.ToString());
            }
        }
        #endregion
    }
}