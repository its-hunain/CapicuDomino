using UnityEngine;
using UnityEngine.UI;
namespace AvatarBuilder
{
    public class SaveAvatarPopUpScreen : MonoBehaviour
    {
        [Header("PopUp Items")]
        public Button noBtn;
        public Button closeBtn;
        public Button yesBtn;

        void Start()
        {
            noBtn.onClick.AddListener(() => ClosePopUpDelegate());
            yesBtn.onClick.AddListener(() => SaveChanges());
            closeBtn.onClick.AddListener(() => ClosePopUpDelegate());
        }

        public void SaveChanges()
        {
            //if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);
            Debug.LogError("Save Changes");
            AvatarParent_FbxHolder.instance.cachedSelecteditem.IsNewUser = false;
            string json = AvatarParent_FbxHolder.instance.currentSelectedAvatar.SaveAvatarSpecs();
            Debug.Log(json);
            WebServiceManager.instance.APIRequest(WebServiceManager.instance.getSetUserAvatarSpecs, Method.PUT, json , null,null,null,CACHEABLE.NULL,true,null); //Save
            ClosePopUpDelegate();
        }

        private void ClosePopUpDelegate()
        {
            if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

            AvatarScreenManager.OpenClosePopUp(gameObject, false);
        }

        
    }
}