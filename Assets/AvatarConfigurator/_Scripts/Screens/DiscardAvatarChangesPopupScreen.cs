using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarBuilder
{

    public class DiscardAvatarChangesPopupScreen : MonoBehaviour
    {
        [Header("PopUp Items")]
        public Button noBtn;
        public Button closeBtn;
        public Button yesBtn;

        void Start()
        {
            noBtn.onClick.AddListener(() => ClosePopUpDelegate());
            yesBtn.onClick.AddListener(() => DiscardChanges());
            closeBtn.onClick.AddListener(() => ClosePopUpDelegate());
        }

        public void DiscardChanges()
        {
            Debug.LogError("Discard Changes");
            CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarTargetPositions.genderSelectionScreenView);

            AvatarParent_FbxHolder.instance.cachedSelecteditem = new CachedItemThumbnailJson();

            if (AvatarScreenManager.instance.avatarPreviewScreen.gameObject.activeInHierarchy)
            {
                AvatarScreenManager.ChangeScreen(AvatarScreenManager.instance.genderSelectionScreen.gameObject, AvatarScreenManager.instance.avatarPreviewScreen.gameObject, false);
            }
            if (AvatarScreenManager.instance.avatarCustomizeScreen.gameObject.activeInHierarchy)
            {
                AvatarScreenManager.ChangeScreen(AvatarScreenManager.instance.genderSelectionScreen.gameObject, AvatarScreenManager.instance.avatarCustomizeScreen.gameObject, false);
            }
            string json = AvatarParent_FbxHolder.instance.currentSelectedAvatar.SaveAvatarSpecs();
            Debug.Log(json);
            WebServiceManager.instance.APIRequest(WebServiceManager.instance.getSetUserAvatarSpecs, Method.PUT, json,null, AvatarScreenManager.instance.OnSuccessGetAvatarSpecs, null,CACHEABLE.NULL,true,null); //Update
            //AvatarParent_FbxHolder.instance.ResetAvatarData();
            AvatarParent_FbxHolder.instance.currentSelectedAvatar.gameObject.SetActive(false);
            GenderSelectionScreen.DeleteInt(GenderSelectionScreen.gender_PrefKey);
            ObjectRotator.instance.ResetRotation();

            ClosePopUpDelegate();
            //OnAnimationReset();
        }

        private void ClosePopUpDelegate()
        {
            if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

            AvatarScreenManager.OpenClosePopUp(gameObject, false);
        }
    }
}