using UnityEngine;
using UnityEngine.UI;

namespace AvatarBuilder
{
    public class GenderSelectionScreen : MonoBehaviour
    {
        //public CamerControllerforConfigurator cameraControllerforConfigurator;
        public Button maleBtn;
        public Button femaleBtn;
        public GameObject maleSelectable;
        public GameObject femaleSelectable;
        public Button nextBtn;
        Animation anim;
        AnimationClip animClip;

        public const string gender_PrefKey = "gender";

        private void Awake()
        {
            anim = GetComponent<Animation>();
            animClip = anim.GetClip("AvatarBuilderGenderSelectionScreen");
        }

        public void OnEnable()
        {
            anim[animClip.name].speed = 1;
            anim.Play(animClip.name);
        }

        public void OnReset()
        {
            anim[animClip.name].speed = -1f;
            anim[animClip.name].time = anim[animClip.name].length;
            anim.Play(animClip.name);
        }

        // Start is called before the first frame update
        void Start()
        {
            CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarTargetPositions.genderSelectionScreenView);

            maleBtn.onClick.AddListener(() => SelectAvatar(1));
            femaleBtn.onClick.AddListener(() => SelectAvatar(0));
            nextBtn.onClick.AddListener(() => NextScreen());
        }

        private void NextScreen()
        {
            OnReset();
            //Update Cache Categories
            //Default Character
            if (AvatarParent_FbxHolder.instance.cachedSelecteditem.IsNewUser)
            {
                if (AvatarConfigurator.instance.currentSelectedGender.ToString().ToLower() == "male")
                {
                    AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList = AvatarParent_FbxHolder.instance.cachedSelecteditem.maleDefaultDataList;
                }
                else
                {
                    AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList = AvatarParent_FbxHolder.instance.cachedSelecteditem.femaleDefaultDataList;
                }
                AvatarParent_FbxHolder.instance.UpdateDataOnModel();
            }
            else //Previous Cached Character
            {
                //Do Nothing
            }

            if (AvatarParent_FbxHolder.instance.currentSelectedAvatar != null)
            {
                AvatarParent_FbxHolder.instance.currentSelectedAvatar.gameObject.SetActive(true);
            }
            AvatarScreenManager.instance.ResetAvatarConfiguratorDataFromUI();

            AvatarScreenManager.ChangeScreen(AvatarScreenManager.instance.avatarCustomizeScreen.gameObject, this.gameObject, true);
        }

        /// <summary>
        /// 0 = Female
        /// 1 = Male
        /// </summary>
        /// <param name="gender"></param>
        public void SelectAvatar(int gender)
        {
            AvatarConfigurator.instance.currentSelectedGender = (gender == 0) ? Gender.female : Gender.male;
            AvatarParent_FbxHolder.instance.cachedSelecteditem.gender = AvatarConfigurator.instance.currentSelectedGender.ToString().ToLower();
            //Reset Selectable
            GameObject currentSelectable = (gender == 0) ? femaleSelectable : maleSelectable;

            Debug.Log("gender: " + gender);
            foreach (var item in AvatarParent_FbxHolder.instance.avatars)
            {
                if (item.tag.ToLower() == AvatarConfigurator.instance.currentSelectedGender.ToString().ToLower())
                {
                    AvatarParent_FbxHolder.instance.currentSelectedAvatar = item;
                    break;
                }
                //AvatarParent_FbxHolder.instance.currentSelectedAvatar = (gender == 0) ? AvatarParent_FbxHolder.instance.avatars. : AvatarParent_FbxHolder.instance.avatars[1];
            }
            UpdateSelectable(currentSelectable);
        }

        /// <summary>
        /// Update Selection
        /// </summary>
        /// <param name="currentSelectable"></param>
        private void UpdateSelectable(GameObject currentSelectable)
        {
            maleSelectable.SetActive(false);
            femaleSelectable.SetActive(false);

            currentSelectable.SetActive(true);
        }



        public static int SetInt(string KeyName, int Value)
        {
            PlayerPrefs.SetInt(KeyName, Value);
            PlayerPrefs.Save();
            return Value;
        }

        public static int GetInt(string KeyName)
        {
            return PlayerPrefs.GetInt(KeyName);
        }

        public static bool DeleteInt(string KeyName)
        {
            PlayerPrefs.DeleteKey(KeyName);
            return true;
        }
    }
}