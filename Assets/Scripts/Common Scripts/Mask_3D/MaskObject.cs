using UnityEngine;
using AvatarBuilder;
using System.Reflection;
using System;
using System.Collections.Generic;

public class MaskObject : MonoBehaviour
{
    public Material maskMat;
    GameObject child;
    public SkinnedMeshRenderer childRenderer;

    AvatarFBXController avatar;
    private void Start()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Cap"))
        {
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material = maskMat;
        }
    }
    private void OnEnable()
    {
        if (!AvatarParent_FbxHolder.instance)
        {
            Debug.LogError("1");
        }


        avatar = AvatarParent_FbxHolder.instance.currentSelectedAvatar;

        if (avatar == null)
        {
            avatar = GetComponentInParent<AvatarParent_FbxHolder>().currentSelectedAvatar;
        }


        if (gameObject.layer == LayerMask.NameToLayer("Cap"))
        {
            avatar.currentAvatarData.activeHairMesh.GetComponent<Renderer>().material.renderQueue = 3003;
        }
        else
        {
//            Debug.LogError("Not Cap");
            SkinnedMeshRenderer orignalObject = GetComponent<SkinnedMeshRenderer>();
            child = new GameObject(gameObject.name + "_child");
            child.transform.SetParent(transform);
            //child = Instantiate(EmptyObj, transform,false);
            SkinnedMeshRenderer skinnedMeshRenderer = child.AddComponent<SkinnedMeshRenderer>();

            if (skinnedMeshRenderer != null)
            {

                skinnedMeshRenderer.sharedMesh = orignalObject.sharedMesh;
                skinnedMeshRenderer.localBounds = orignalObject.localBounds;
                skinnedMeshRenderer.rootBone = orignalObject.rootBone;
                skinnedMeshRenderer.sharedMaterial = maskMat;
                skinnedMeshRenderer.bones = BuildBonesArray(orignalObject.rootBone, orignalObject.bones);
            }
            try
            {
                avatar.currentAvatarData.skinMat.renderQueue = 3002;
            }
            catch (Exception ex)
            {

            }
        }
    }

    private void OnDisable()
    {
        if(child!=null) Destroy(child);
    }


    static Transform[] BuildBonesArray(Transform rootBone, Transform[] bones)
    {
        List<Transform> boneList = new List<Transform>();
        ExtractBonesRecursively(rootBone, ref boneList);

        List<Transform> Reorder = new List<Transform>();
        foreach (Transform bone in bones)
        {
            foreach (Transform extractbone in boneList)
            {
                if (bone.name == extractbone.name)
                {
                    Reorder.Add(extractbone);
                }
            }

        }

        return Reorder.ToArray();
    }

    static void ExtractBonesRecursively(Transform bone, ref List<Transform> boneList)
    {
        boneList.Add(bone);

        for (int i = 0; i < bone.childCount; i++)
        {
            ExtractBonesRecursively(bone.GetChild(i), ref boneList);
        }
    }



}