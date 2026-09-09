using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBuilder;

namespace AssetBuilder
{
    public class OutlineAnimation : MonoBehaviour
    {
        bool pingPong = false;
        int counter = 0;

        // Use this for initialization
        void OnEnable()
        {
            counter = 0;
        }

        // Update is called once per frame
        void Update()
        {
            Color c = GetComponent<OutlineEffect>().lineColor0;

            if (pingPong)
            {
                c.a += Time.deltaTime;

                if (c.a >= 1)
                {
                    pingPong = false;
                    counter++;

                    }
            }
            else
            {
                c.a -= Time.deltaTime;

                if (c.a <= 0)
                {
                    pingPong = true;
                }
            }

            c.a = Mathf.Clamp01(c.a);
            GetComponent<OutlineEffect>().lineColor0 = c;
            GetComponent<OutlineEffect>().UpdateMaterialsPublicProperties();

            if (counter > 2)
            {
                this.enabled = false;
                GetComponent<OutlineEffect>().lineColor0 = Color.red;
            GetComponent<OutlineEffect>().UpdateMaterialsPublicProperties();

            }
        }
    }
}