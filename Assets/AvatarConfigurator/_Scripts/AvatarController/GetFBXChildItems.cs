using System.Collections.Generic;
using UnityEngine;

namespace AvatarBuilder
{
    /// <summary>
    /// Attach this script to parent Item
    /// </summary>
    public class GetFBXChildItems : MonoBehaviour
    {
        public static CategoryType parentFBX_Category;
        public ItemType parentFBX_Type;
        public List<FBXBaseItem> childObjects = new List<FBXBaseItem>();

        public FBXBaseItem EnableDisableHair(string shortCode /*By Default: hair1*/)
        {
            //False All
            foreach (var item in childObjects)
            {
                item.gameObject.SetActive(false);
            }
            var currentObj = childObjects.Find(x => x.shortCode.Equals(shortCode.ToLower()));
            if(currentObj!=null) currentObj.gameObject.SetActive(true);
            return currentObj;
        }


        /// <summary>
        /// Editor Method
        /// </summary>
        public FBXBaseItem GetChildMeshes()
        {
            FBXBaseItem defaultFBXBaseItem = null;

            Debug.Log("GetChildrens");
            childObjects.Clear();
            foreach (Transform child in transform)
            {
                if (!child.GetComponent<FBXBaseItem>())
                    child.gameObject.AddComponent<FBXBaseItem>();
                FBXBaseItem fBXBaseItem = child.GetComponent<FBXBaseItem>();
                fBXBaseItem.shortCode = child.gameObject.name.ToLower();
                fBXBaseItem.itemType = parentFBX_Type;
                childObjects.Add(fBXBaseItem);
                fBXBaseItem.gameObject.SetActive(false);
                if (child.name.ToLower().Contains("default") && defaultFBXBaseItem == null) defaultFBXBaseItem = fBXBaseItem;
            }
            return defaultFBXBaseItem;
        }
    }
}