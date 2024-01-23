using UnityEngine;
using UnityEngine.UI;

namespace AvatarBuilder
{
    public class AvatarCustomizeScreen : MonoBehaviour
    {
        public CamerControllerforConfigurator camerControllerforConfigurator;
        public Button backBtn;
        public Button previewBtn;
        Animation anim;
        AnimationClip animClip;

        private void Awake()
        {
            anim = GetComponent<Animation>();
            animClip = anim.GetClip("AvatarCustomizeScreen");
        }

        public void OnEnable()
        {
            camerControllerforConfigurator.MoveCamera(CamerControllerforConfigurator.AvatarTargetPositions.bodyView);

            AvatarConfigurator.CategoriesDownloaderAsynchrously(WebServiceManager.instance.getAvatarBuilderCategories);

            ItemSelectionPanel.isAnimationAllowed = true; // To play the animation only once.

            anim[animClip.name].speed = 1;
            anim.Play(animClip.name);
        }

        public void OnAnimationReset()
        {
            anim[animClip.name].speed = -1;
            anim[animClip.name].time = anim[animClip.name].length;
            anim.Play(animClip.name);
        }

        void Start()
        {
            backBtn.onClick.AddListener(() => PreviousScreen());
            previewBtn.onClick.AddListener(() => PreviewData());
        }

        private void PreviewData()
        {
            OnAnimationReset();
            AvatarScreenManager.instance.DisableAllItemThumbnailSelectionPanels();
            AvatarScreenManager.instance.categorySelectionPanel.ChangeSelectableIcon();
            AvatarScreenManager.ChangeScreen(AvatarScreenManager.instance.avatarPreviewScreen.gameObject, this.gameObject, true);
        }


        private void PreviousScreen()
        {
            AvatarScreenManager.OpenClosePopUp(AvatarScreenManager.instance.discardAvatarChangesPopupScreen.gameObject, true);
        }

        //private void NextScreen()
        //{
        //    OnReset();
        //    StartCoroutine(AvatarScreenManager._ChangeScreen(AvatarScreenManager.instance.avatarSelectionScreen, this.gameObject, true));
        //}
    }
}