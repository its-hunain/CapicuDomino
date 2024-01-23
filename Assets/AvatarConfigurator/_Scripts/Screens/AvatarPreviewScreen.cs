using UnityEngine;
using UnityEngine.UI;

namespace AvatarBuilder
{
    public class AvatarPreviewScreen : MonoBehaviour
    {
        public CamerControllerforConfigurator cameraControllerforConfigurator;
        [SerializeField] private Button createNewBtn;
        [SerializeField] private Button editBtn;
        [SerializeField] private Button mintBtn;
        [SerializeField] private Button SaveBtn;

        Animation anim;
        AnimationClip animClip;

        public static bool isAvatarPreviewScreenEnabled = false;



        private void Awake()
        {
            anim = GetComponent<Animation>();
            animClip = anim.GetClip("AvatarPreviewScreen");
        }

        public void OnEnable()
        {
            cameraControllerforConfigurator.MoveCamera(CamerControllerforConfigurator.AvatarTargetPositions.screenCenterView);
            anim[animClip.name].speed = 1;
            anim.Play(animClip.name);
        }

        public void OnAnimationReset()
        {
            anim[animClip.name].speed = -1;
            anim[animClip.name].time = anim[animClip.name].length;
            anim.Play(animClip.name);
        }

        // Start is called before the first frame update
        void Start()
        {
            editBtn.onClick.AddListener(() => EditAvatarEvent());
            createNewBtn.onClick.AddListener(() => DiscardAvatarEvent());
            mintBtn.onClick.AddListener(() => MintAvatarEvent());
            SaveBtn.onClick.AddListener(() => SaveAvatarEvent());
        }

        private void DiscardAvatarEvent()
        {
            AvatarScreenManager.OpenClosePopUp(AvatarScreenManager.instance.discardAvatarChangesPopupScreen.gameObject, true);
        }

        private void SaveAvatarEvent()
        {
            AvatarScreenManager.OpenClosePopUp(AvatarScreenManager.instance.SaveAvatarPopUpScreen.gameObject, true);
        }

        private void MintAvatarEvent()
        {
            ObjectRotator.instance.ResetRotation();
            AvatarScreenManager.OpenClosePopUp(AvatarScreenManager.instance.avatarNamePopupScreen.gameObject, true);
        }

        private void EditAvatarEvent()
        {
            OnAnimationReset();
            AvatarScreenManager.ChangeScreen(AvatarScreenManager.instance.avatarCustomizeScreen.gameObject, this.gameObject, false);
        }
    }
}