using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class BlendManager : MonoBehaviour
{

    [SerializeField] Button lipsButton;
    [SerializeField] Button noseButton;
    [SerializeField] Button eyesButton;
    [SerializeField] Button faceCutButton;
    [Header("JSON")]
    [SerializeField] Button JsonCopyButton;
    [SerializeField] Text jsonTxt;
    [SerializeField] string json;

    public SkinnedMeshRenderer avatarSKMR;
    Mesh avatarMesh;

    public int intialSlider;
    public int finalSlider;

    public static BlendManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadAvatar();
        lipsButton.onClick.AddListener(GenerateLipsBlendData);
        noseButton.onClick.AddListener(GenerateNoseBlendData);
        eyesButton.onClick.AddListener(GenerateEyesBlendData);
        faceCutButton.onClick.AddListener(GenerateFaceCutBlendData);
        JsonCopyButton.onClick.AddListener(CopyToClipboard);
    }

    public void LoadAvatar()
    {
        avatarMesh = avatarSKMR.sharedMesh;
    }


    public void GenerateLipsBlendData()
    {
        JsonCopyButton.interactable = true;
        BlendData blendData = new BlendData();
        blendData.itemType = "Lips";

        for (int i = 0; i <= 19; i++)
        {
            BlendIndexData blendIndexData = new BlendIndexData();
            blendIndexData.index = i;
            blendIndexData.name = avatarMesh.GetBlendShapeName(i);
            blendIndexData.value = avatarSKMR.GetBlendShapeWeight(i);
            
            blendData.data.Add(blendIndexData);            
        }

        string data = JsonConvert.SerializeObject(blendData);
        //////File.WriteAllText(_path + "Lips.json", data);
        Debug.Log(data);
        jsonTxt.text = data;
    }

    public void GenerateNoseBlendData()
    {
        JsonCopyButton.interactable = true;
        BlendData blendData = new BlendData();
        blendData.itemType = "Nose";

        for (int i = 20; i <= 39; i++)
        {
            BlendIndexData blendIndexData = new BlendIndexData();
            blendIndexData.index = i;
            blendIndexData.name = avatarMesh.GetBlendShapeName(i);
            blendIndexData.value = avatarSKMR.GetBlendShapeWeight(i);

            blendData.data.Add(blendIndexData);
        }

        string data = JsonConvert.SerializeObject(blendData);
        ////File.WriteAllText(_path + "Nose.json", data);
        Debug.Log(data);
        jsonTxt.text = data;
    }

    public void GenerateEyesBlendData()
    {
        JsonCopyButton.interactable = true;
        BlendData blendData = new BlendData();
        blendData.itemType = "Eyes";

        for (int i = 40; i <= 59; i++)
        {
            BlendIndexData blendIndexData = new BlendIndexData();
            blendIndexData.index = i;
            blendIndexData.name = avatarMesh.GetBlendShapeName(i);
            blendIndexData.value = avatarSKMR.GetBlendShapeWeight(i);

            blendData.data.Add(blendIndexData);
        }

        string data = JsonConvert.SerializeObject(blendData);
        ////File.WriteAllText(_path + "Eyes.json", data);
        Debug.Log(data);
        jsonTxt.text = data;
    }

    public void GenerateFaceCutBlendData()
    {
        JsonCopyButton.interactable = true;
        BlendData blendData = new BlendData();
        blendData.itemType = "FaceCut";

        for (int i = 60; i <= 79; i++)
        {
            BlendIndexData blendIndexData = new BlendIndexData();
            blendIndexData.index = i;
            blendIndexData.name = avatarMesh.GetBlendShapeName(i);
            blendIndexData.value = avatarSKMR.GetBlendShapeWeight(i);
            blendData.data.Add(blendIndexData);
        }

        string data = JsonConvert.SerializeObject(blendData);
        ////File.WriteAllText(_path + "FaceCut.json", data);
        Debug.Log(data);
        jsonTxt.text = data;
    }


    /// <summary>
    /// Puts the string into the Clipboard.
    /// </summary>
    public void CopyToClipboard()
    {
        string json = jsonTxt.text;
        GUIUtility.systemCopyBuffer = json;
        jsonTxt.text = "Json Copied...";
        JsonCopyButton.interactable = false;
    }
}

[System.Serializable]
public class BlendIndexData
{
    public int index { get; set; }
    public string name { get; set; }
    public float value { get; set; }
}

[System.Serializable]
public class BlendData
{
    public string itemType;
    public List<BlendIndexData> data = new List<BlendIndexData>();
}

