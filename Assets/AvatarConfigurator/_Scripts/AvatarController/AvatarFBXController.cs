using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetBuilder;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace AvatarBuilder
{
    public class AvatarFBXController : MonoBehaviour
    {
        public Animator animator;

        //public static AvatarFBXController instance;
        [SerializeField]
        public AvatarDefaultData currentAvatarData;
        public SkinnedMeshRenderer avatarSKMR;
        public Texture mainSkinTexture;
        public Texture alternateTexture;

        public void FindData()
        {
            //if (avatarSKMR == null) avatarSKMR = GetComponentInChildren<SkinnedMeshRenderer>(true);
            foreach (var item in avatarSKMR.materials)
            {
                if (item.name.ToLower().Contains("eyebrow"))
                    currentAvatarData.eyeBrowsMat = item;

                if (item.name.ToLower().Contains("skin") || item.name.ToLower().Contains("avatar"))
                    currentAvatarData.skinMat = item;

                if (item.name.ToLower().Contains("facialhair"))
                    currentAvatarData.facialHairMat = item;

                if (item.name.ToLower().Contains("lips"))
                    currentAvatarData.lipsMat = item;

                if (item.name.ToLower().Contains("iris"))
                    currentAvatarData.eyeMat = item;
            }

            GameObject hairParent = this.transform.Find("Hair") == null ? this.transform.Find("Hairs").gameObject : this.transform.Find("Hair").gameObject;

            //GameObject hairParent = GetComponentsInChildren<GameObject>(true).First(r => r.tag == "HairParent");


            //GameObject hairParent = GameObject.FindGameObjectWithTag("HairParent");
            if (hairParent != null)
            {
                Debug.Log("Hair found");
                if (!hairParent.GetComponent<GetFBXChildItems>())
                    hairParent.AddComponent<GetFBXChildItems>();

                GetFBXChildItems getFBXChildItems = hairParent.GetComponent<GetFBXChildItems>();
                currentAvatarData.hairMesheParent = getFBXChildItems;
                currentAvatarData.hairMesheParent.parentFBX_Type = ItemType.mesh;
                currentAvatarData.activeHairMesh = currentAvatarData.hairMesheParent.GetChildMeshes();
            }
            else
            {
                Debug.LogError("HairParent Not Found, may be tag is missing on HairParent...");
            }


            //Transform capParent = this.transform.Find("Caps") == null ? this.transform.Find("Cap") : this.transform.Find("Caps");

            //if (capParent != null)
            //{
            //    Debug.Log("Cap found");
            //    if (!capParent.GetComponent<GetFBXChildItems>())
            //        capParent.gameObject.AddComponent<GetFBXChildItems>();

            //    capParent.gameObject.GetComponent<GetFBXChildItems>().WorkOnMasking();
            //}
            //else
            //{
            //    Debug.LogError("CapParent Not Found, may be tag is missing on CapParent...");
            //}

        }

        /// <summary>
        /// Editor Method
        /// </summary>
        public void WorkOnMasking(Transform parentTransform)
        {
            Debug.Log("Working on Masking");

            Material maskMat = null;
            if (maskMat == null)
            {
                Material material = Resources.Load("MaskMat", typeof(Material)) as Material;

                maskMat = material;
            }

            foreach (Transform childMesh in parentTransform)
            {
                if (childMesh.GetComponent<Renderer>())
                {
                    if (!childMesh.GetComponent<MaskObject>())
                        childMesh.gameObject.AddComponent<MaskObject>();


                    childMesh.gameObject.GetComponent<MaskObject>().maskMat = maskMat;
                    //Transform maskObject = childMesh.GetComponent<MaskObject>().transform.GetChild(0);
                    //maskObject.GetComponent<Renderer>().sharedMaterial = maskMat; //Extra obj for masking hair
                    //maskObject.gameObject.SetActive(true);
                }
            }
        }

        ///////////methods for changing hair styles
        public void changeHairMesh(string hairmeshShortCode = null)
        {
            if(hairmeshShortCode!=null) currentAvatarData.activeHairMesh = currentAvatarData.hairMesheParent.EnableDisableHair(hairmeshShortCode);

            if (currentAvatarData.activeHairMesh.GetComponent<Renderer>())
            {
                currentAvatarData.activeHairMesh.GetComponent<Renderer>().materials[0].color = currentAvatarData.defaultHairColor;
            }
            else
            {
                //currentAvatarData.activeHairMesh.GetComponent<Renderer>().materials[0].color = currentAvatarData.defaultHairColor;
            }
        }

        /// <summary>
        /// Methods for changing eyebrows styles/textures
        /// </summary>
        /// <param name="texture"></param>
        public void ChangeEyeBrowsTexture(Texture texture = null)
        {
            if (texture == null)
                currentAvatarData.eyeBrowsMat.mainTexture = currentAvatarData.defaultEyeBrowTexture;
            else
                currentAvatarData.defaultEyeBrowTexture = currentAvatarData.eyeBrowsMat.mainTexture = texture;

            //if (!SceneManager.GetActiveScene().name.Equals(Global.AvatarBuilderScene))
            {
                Color color = currentAvatarData.eyeBrowsMat.color;
                color.a = 1;
                currentAvatarData.eyeBrowsMat.color = color;
            }
        }

        /// <summary>
        /// Methods for changing Skin Color
        /// </summary>
        /// <param name="color"></param>
        public void ChangeSkinColor(Color? color = null)
        {
            if (color == null)
                currentAvatarData.skinMat.color = currentAvatarData.defaultSkinColor;
            else
                currentAvatarData.defaultSkinColor = currentAvatarData.skinMat.color = (Color)color;
        }

        /// <summary>
        /// Methods To ChangeEyeColors
        /// </summary>
        /// <param name="color"></param>
        public void ChangeEyeColors(Color? color = null)
        {
            if (color == null)
                currentAvatarData.eyeMat.color = currentAvatarData.defaultEyeColor;
            else
                currentAvatarData.defaultEyeColor = currentAvatarData.eyeMat.color = (Color)color;
        }

        /// <summary>
        /// Methods To ChangeEyeBrowsColors
        /// </summary>
        /// <param name="color"></param>
        public void ChangeEyeBrowsColors(Color? color = null)
        {
            if (color == null)
            {
                currentAvatarData.eyeBrowsMat.color = currentAvatarData.defaultEyeBrowsColor;
            }
            else
            {
                currentAvatarData.defaultEyeBrowsColor = currentAvatarData.eyeBrowsMat.color = (Color)color;
            }
            if (currentAvatarData.eyeBrowsMat.mainTexture == null)
            {
                Color color2 = currentAvatarData.eyeBrowsMat.color;
                color2.a = 0;
                currentAvatarData.eyeBrowsMat.color = color2;
            }
            else
            {
                Color color2 = currentAvatarData.eyeBrowsMat.color;
                color2.a = 1;
                currentAvatarData.eyeBrowsMat.color = color2;
            }
            //if (!SceneManager.GetActiveScene().name.Equals(Global.AvatarBuilderScene))
            //{
            //    Color color2 = currentAvatarData.eyeBrowsMat.color;
            //    color2.a = 0;
            //    currentAvatarData.eyeBrowsMat.color = color2;
            //}
        }

        /// <summary>
        /// Methods To changeHairColor
        /// </summary>
        /// <param name="color"></param>
        public void changeHairColor(Color? color = null)
        {

            Renderer renderer = null;
            if (currentAvatarData.activeHairMesh.GetComponent<Renderer>())
            {
                renderer = currentAvatarData.activeHairMesh.GetComponent<Renderer>();
            }
            //else
            //{
            //    Debug.Log("Error in changeHairColor...");
            //}

            if (renderer != null)
            {
                if (color == null)
                    renderer.materials[0].color = currentAvatarData.defaultHairColor;
                else
                    currentAvatarData.defaultHairColor = currentAvatarData.defaultHairColor = renderer.materials[0].color = (Color)color;
            }
            else
            { Debug.Log("Error in changeHairColor..."); }

        }

        /// <summary>
        /// Methods To changeFacialHairColor
        /// </summary>
        /// <param name="color"></param>
        public void changeFacialHairColor(Color? color = null)
        {
            if (color == null)
            {
                if (currentAvatarData.facialHairMat != null)
                    currentAvatarData.facialHairMat.color = currentAvatarData.defaultFacialHairColor;
            }
            else
            {
                currentAvatarData.defaultFacialHairColor = currentAvatarData.facialHairMat.color = (Color)color;
            }

            if (currentAvatarData.facialHairMat.mainTexture == null)
            {
                Color color2 = currentAvatarData.facialHairMat.color;
                color2.a = 0;
                currentAvatarData.facialHairMat.color = color2;
            }
            else
            {
                Color color2 = currentAvatarData.facialHairMat.color;
                color2.a = 1;
                currentAvatarData.facialHairMat.color = color2;
            }
        }


        /// <summary>
        /// Methods To ChangeLipsColor
        /// </summary>
        /// <param name="color"></param>
        public void ChangeLipsColor(Color? color = null)
        {
            if (color == null)
                currentAvatarData.lipsMat.color = currentAvatarData.defaultLipsColor;
            else
                currentAvatarData.defaultLipsColor = currentAvatarData.lipsMat.color = (Color)color;
        }

        /// <summary>
        /// Methods To ChangeFacialStyleTexture
        /// </summary>
        /// <param name="color"></param>
        public void ChangeFacialStyleTexture(Texture texture = null)
        {

            if (texture == null)
            {
                currentAvatarData.facialHairMat.mainTexture = currentAvatarData.defaultFacialHairTexture;
            }
            else
            {
                currentAvatarData.defaultFacialHairTexture = currentAvatarData.facialHairMat.mainTexture = texture;
            }

            //if (!SceneManager.GetActiveScene().name.Equals(Global.AvatarBuilderScene))
            {
                Color color2 = currentAvatarData.facialHairMat.color;
                color2.a = 1;
                currentAvatarData.facialHairMat.color = color2;
            }
        }

        /// <summary>
        /// Methods To SetBlendData
        /// </summary>
        /// <param name="data"></param>
        public void SetBlendData(string data)
        {
            //Debug.Log("SetBlendData ");
            var blendData = JsonConvert.DeserializeObject<BlendData>(data);

            foreach (var item in blendData.data)
            {
                avatarSKMR.SetBlendShapeWeight(item.index, item.value);
            }
        }

        public void AddNewItemInAvatarSpecs(ThumbnailJsonData getSavedAvatarJsons)
        {
            ThumbnailJsonData categoryThumbnailJsonData = AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList.Find(x => x.mongodbID == getSavedAvatarJsons.mongodbID);
            Debug.Log("getSavedAvatarJsons.Id: " + getSavedAvatarJsons.mongodbID);
            Debug.Log("getSavedAvatarJsons.data: " + getSavedAvatarJsons.Data.ToString());

            //Find and Update
            if (categoryThumbnailJsonData != null)
            {
                AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList[AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList.IndexOf(categoryThumbnailJsonData)] = getSavedAvatarJsons;
            }
            else//Add New
            {
                AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList.Add(getSavedAvatarJsons);
            }
        }

        public void AddNewVariantInAvatarSpecs(ThumbnailJsonData getSavedAvatarJsons)
        {
            ThumbnailJsonData categoryThumbnailJsonData = AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList.Find(x => x.mongodbID == getSavedAvatarJsons.mongodbID);
            Debug.Log("getSavedAvatarJsons.Id: " + getSavedAvatarJsons.mongodbID);
            Debug.Log("getSavedAvatarJsons.data: " + getSavedAvatarJsons.Data.ToString());
            //Find Old
            if (categoryThumbnailJsonData != null)
            {
                Debug.Log("Not Null");
                categoryThumbnailJsonData.Data.Color = getSavedAvatarJsons.Data.Color;
            }
            else
            {
                Debug.Log(" Null");
                //Add New
                AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList.Add(getSavedAvatarJsons);
            }
        }

        public void RemoveCategoryAvatarSpecs(ThumbnailJsonData getSavedAvatarJsons)
        {
            ThumbnailJsonData categoryThumbnailJsonData = AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList.Find(x => x.mongodbID == getSavedAvatarJsons.mongodbID);

            //Remove Old
            if (categoryThumbnailJsonData != null)
                AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList.Remove(categoryThumbnailJsonData);
        }


        public string SaveAvatarSpecs()
        {
            //cachedSelecteditem.Message = "Save Avatar Specs";
            CachedItemThumbnailJson cachedItemThumbnailJson = new CachedItemThumbnailJson();
            cachedItemThumbnailJson.Message = AvatarParent_FbxHolder.instance.cachedSelecteditem.Message;
            cachedItemThumbnailJson.DataList = AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList;
            cachedItemThumbnailJson.maleDefaultDataList = AvatarParent_FbxHolder.instance.cachedSelecteditem.maleDefaultDataList;
            cachedItemThumbnailJson.femaleDefaultDataList = AvatarParent_FbxHolder.instance.cachedSelecteditem.femaleDefaultDataList;
            cachedItemThumbnailJson.gender = AvatarParent_FbxHolder.instance.cachedSelecteditem.gender;
            cachedItemThumbnailJson.IsNewUser = AvatarParent_FbxHolder.instance.cachedSelecteditem.IsNewUser;

            return JsonConvert.SerializeObject(cachedItemThumbnailJson);
        }


        internal void UpdateColorsAndBlendOfAvatar()
        {
            foreach (var item in GetComponentInParent<AvatarParent_FbxHolder>().cachedSelecteditem.DataList)
            {
                CategoryType categoryType;
                if (Enum.TryParse(item.CategoryType, out categoryType))
                {
                    switch (categoryType)
                    {
                        case CategoryType.skin:
                            if (item.Data.Color.Count > 0)
                            {
                                Color newCol;
                                if (ColorUtility.TryParseHtmlString(item.Data.Color[0].Value, out newCol))
                                {
                                    ChangeSkinColor(newCol);
                                }
                            }
                            break;

                        case CategoryType.faceCut: // Done
                            if (item.Data.Blend.Count > 0)
                            {
                                //Debug.Log("item.Data.Blend: " + item.Data.Blend.Count);
                                //Debug.Log("item.Data.Blend value: " + item.Data.Blend[0].Value);
                                SetBlendData(item.Data.Blend[0].Value);
                            }
                            break;

                        case CategoryType.eyeBrows:
                            if (item.Data.Color.Count > 0)
                            {
                                Color newCol;
                                if (ColorUtility.TryParseHtmlString(item.Data.Color[0].Value, out newCol))
                                {
                                    ChangeEyeBrowsColors(newCol);
                                }
                            }
                            break;

                        case CategoryType.eyes: //Done
                            if (item.Data.Blend.Count > 0)
                            {
                                SetBlendData(item.Data.Blend[0].Value);
                            }
                            if (item.Data.Color.Count > 0)
                            {
                                Color newCol;
                                if (ColorUtility.TryParseHtmlString(item.Data.Color[0].Value, out newCol))
                                {
                                    GetComponentInParent<AvatarParent_FbxHolder>().currentSelectedAvatar.ChangeEyeColors(newCol);
                                }
                            }
                            break;

                        case CategoryType.hair:
                            if (item.Data.Mesh.Count > 0)
                            {
                                GetComponentInParent<AvatarParent_FbxHolder>().currentSelectedAvatar.changeHairMesh(item.Data.Mesh[0].Value.ToLower());
                            }
                            if (item.Data.Color.Count > 0)
                            {
                                Color newCol;
                                if (ColorUtility.TryParseHtmlString(item.Data.Color[0].Value, out newCol))
                                {
                                    GetComponentInParent<AvatarParent_FbxHolder>().currentSelectedAvatar.changeHairColor(newCol);
                                }
                            }
                            break;

                        case CategoryType.nose: //Done
                            if (item.Data.Blend.Count > 0)
                            {
                                SetBlendData(item.Data.Blend[0].Value);
                            }
                            break;

                        case CategoryType.lips: //Done
                            if (item.Data.Blend.Count > 0)
                            {
                                SetBlendData(item.Data.Blend[0].Value);
                            }
                            if (item.Data.Color.Count > 0)
                            {
                                Color newCol;
                                if (ColorUtility.TryParseHtmlString(item.Data.Color[0].Value, out newCol))
                                {
                                    GetComponentInParent<AvatarParent_FbxHolder>().currentSelectedAvatar.ChangeLipsColor(newCol);
                                }
                            }
                            break;

                        case CategoryType.facialHair:
                            if (item.Data.Color.Count > 0)
                            {
                                Color newCol;
                                if (ColorUtility.TryParseHtmlString(item.Data.Color[0].Value, out newCol))
                                {
                                    GetComponentInParent<AvatarParent_FbxHolder>().currentSelectedAvatar.changeFacialHairColor(newCol);
                                }
                            }
                            break;
                    }
                }
            }
        }


        internal void ResetAvatarData()
        {
            if (currentAvatarData.skinMat != null)          ChangeSkinColor();              //Reset Skin Color
            if (currentAvatarData.lipsMat != null)          ChangeLipsColor();              //Reset Lips Color
            if (currentAvatarData.eyeMat != null)           ChangeEyeColors();              //Reset Eye Color
            if (currentAvatarData.eyeBrowsMat != null)      ChangeEyeBrowsColors();         //Reset Eye Brows Color
            if (currentAvatarData.eyeBrowsMat != null)      ChangeEyeBrowsTexture();         //Reset Eye Brows Style
            if(currentAvatarData.facialHairMat!=null)       ChangeFacialStyleTexture();     //Reset Facial Hair Style
            if (currentAvatarData.facialHairMat != null)    changeFacialHairColor();        //Reset Facial Hair Color
            if(currentAvatarData.activeHairMesh!=null)      changeHairMesh(currentAvatarData.activeHairMesh.shortCode);               //Reset Hair Style
            if (currentAvatarData.activeHairMesh != null)   changeHairColor();              //Reset Hair Color
        }

        public void SetAvatarSpecsOn_UI()
        {
            foreach (var item in AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList)
            {
                CategoryType categoryType;
                if (Enum.TryParse(item.CategoryType, out categoryType))
                {
                    var categoryThumbnail = AvatarScreenManager.instance.categorySelectionPanel.categories.Find(x => x.categoryType.Equals(categoryType));
                    var itemSelectionPanel = categoryThumbnail.itemSelectionPanel;
                    ItemThumbnail itemThumbnail = null;
                    VariantThumbnail variantThumbnail = null;

                    switch (categoryType)
                    {
                        case CategoryType.skin:
                            //Change Color
                            itemThumbnail = (item.Data.Color.Count > 0)? categoryThumbnail.itemsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Color[0].itemID)): null;
                            itemSelectionPanel.ChangeSelectableItemThumbnail(itemThumbnail);
                            break;

                        case CategoryType.faceCut:
                            //Change Blend
                            itemThumbnail = (item.Data.Blend.Count > 0) ? categoryThumbnail.itemsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Blend[0].itemID)) : null;
                            itemSelectionPanel.ChangeSelectableItemThumbnail(itemThumbnail);
                            break;

                        case CategoryType.eyeBrows:
                            //Change Texture
                            itemThumbnail = (item.Data.Texture.Count > 0) ? categoryThumbnail.itemsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Texture[0].itemID)) : null;
                            itemSelectionPanel.ChangeSelectableItemThumbnail(itemThumbnail);

                            //Change Color
                            variantThumbnail = (item.Data.Color.Count > 0) ? categoryThumbnail.variantsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Color[0].itemID)) : null;
                            itemSelectionPanel.ChangeSelectableVariantThumbnail(variantThumbnail);
                            break;

                        case CategoryType.eyes:
                            //Change Blend
                            itemThumbnail = (item.Data.Blend.Count > 0) ? categoryThumbnail.itemsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Blend[0].itemID)) : null;
                            itemSelectionPanel.ChangeSelectableItemThumbnail(itemThumbnail);

                            //Change Color
                            variantThumbnail = (item.Data.Color.Count > 0) ? categoryThumbnail.variantsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Color[0].itemID)) : null;
                            itemSelectionPanel.ChangeSelectableVariantThumbnail(variantThumbnail);
                            break;

                        case CategoryType.hair:
                            //Change Mesh
                            itemThumbnail = (item.Data.Mesh.Count > 0) ? categoryThumbnail.itemsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Mesh[0].itemID)) : null;
                            itemSelectionPanel.ChangeSelectableItemThumbnail(itemThumbnail);
                            //Change Color
                            variantThumbnail = (item.Data.Color.Count > 0) ? categoryThumbnail.variantsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Color[0].itemID)) : null;
                            itemSelectionPanel.ChangeSelectableVariantThumbnail(variantThumbnail);
                            break;

                        case CategoryType.nose:
                            //Change Blend
                            itemThumbnail = (item.Data.Blend.Count > 0) ? categoryThumbnail.itemsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Blend[0].itemID)) : null;
                            itemSelectionPanel.ChangeSelectableItemThumbnail(itemThumbnail);
                            break;

                        case CategoryType.lips:
                            //Change Mesh
                            itemThumbnail = (item.Data.Blend.Count > 0) ? categoryThumbnail.itemsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Blend[0].itemID)) : null;
                            itemSelectionPanel.ChangeSelectableItemThumbnail(itemThumbnail);
                            //Change Color
                            variantThumbnail = (item.Data.Color.Count > 0) ? categoryThumbnail.variantsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Color[0].itemID)) : null;
                            itemSelectionPanel.ChangeSelectableVariantThumbnail(variantThumbnail);
                            break;

                        case CategoryType.facialHair:
                            //Change Texture
                            itemThumbnail = (item.Data.Texture.Count > 0) ? categoryThumbnail.itemsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Texture[0].itemID)) : null;
                            itemSelectionPanel.ChangeSelectableItemThumbnail(itemThumbnail);

                            //Change Color
                            variantThumbnail = (item.Data.Color.Count > 0) ? categoryThumbnail.variantsThumbnailList.Find(x => x.itemProperties.itemID.Equals(item.Data.Color[0].itemID)) : null;
                            itemSelectionPanel.ChangeSelectableVariantThumbnail(variantThumbnail);
                            break;
                    }
                }
            }
        }



        public void TextureDownload(CategoryType categoryType = CategoryType.none, ItemThumbnail itemThumbnail = null, string textureURL = null , bool applyTexture = false)
        {
            AvatarParent_FbxHolder.instance.StartCoroutine(_TexturesDownloaderAsynchrously(categoryType, itemThumbnail , textureURL , applyTexture));
        }

        public IEnumerator _TexturesDownloaderAsynchrously(CategoryType categoryType = CategoryType.none, ItemThumbnail itemThumbnail = null, string textureURL = null, bool applyTexture = false)
        {
            Debug.Log("Downloading Texture. . . ");
            UnityWebRequest www = null;
            if (textureURL!= null)      www = UnityWebRequestTexture.GetTexture(textureURL);
            if (itemThumbnail != null)  www = UnityWebRequestTexture.GetTexture(itemThumbnail.itemProperties.Url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D texture2D = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if (itemThumbnail != null) itemThumbnail.SetTexture(texture2D);
                if (applyTexture)
                {
                    if (categoryType == CategoryType.eyeBrows)
                    {
                        ChangeEyeBrowsTexture(texture2D);
                    }
                    if (categoryType == CategoryType.facialHair)
                    {
                        ChangeFacialStyleTexture(texture2D);
                    }
                }
                //Debug.Log("Downloaded. . . ");
            }
        }


        public void ChangeAnimation(string animationClipName)
        {
            animator.Play(animationClipName);
        }
    }

    [Serializable]
    public class AvatarDefaultData
    {
        //Default Values
        public Color defaultSkinColor;
        public Color defaultLipsColor;
        public Color defaultEyeColor;
        public Color defaultEyeBrowsColor;
        public Color defaultHairColor;
        public Color defaultFacialHairColor;

        public Texture defaultEyeBrowTexture;
        public Texture defaultFacialHairTexture;
        public int defaultHairIndex = 1;
        //Face Blend
        //Nose Blend
        //Lips Blend

        //Items
        public Material eyeMat;             //Eye
        //public Material hairMat;            //Hair will be dynamic
        public Material facialHairMat;      //Facial Hair
        public Material lipsMat;            //Lips
        public Material eyeBrowsMat;
        public Material skinMat;

        public GetFBXChildItems hairMesheParent;
        public FBXBaseItem activeHairMesh;
    }
}

