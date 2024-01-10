using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// The name is a lie, this only TRANSLATES the things bcoz I'm too lazy to add rotation scaling and such
// Partially based on wheeeUI and Visual Studio's autocomplete feature
namespace ZUI
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class Transformer : MonoBehaviour
    {
        internal static Transformer instance;
        internal UrlDir.UrlConfig[] transforms;
        internal GameObject[] transformObjects;
        public Vector3[] transformAmounts;

        public void Start()
        {
            instance = this;
            transforms = GameDatabase.Instance.GetConfigs("ZUITransform");

            transformObjects = new GameObject[transforms.Length];
            transformAmounts = new Vector3[transforms.Length];

            int i = 0;
            foreach (UrlDir.UrlConfig config in transforms)
            {
                if (!config.config.HasValue("target")) {
                    Debug.Log("[ZUI] Node does not have a transform target!");
                    continue;
                }
                string target = config.config.GetValue("target");
                float[] translate = new float[3];
                GameObject gameObject = GameObject.Find(target);
                if (gameObject != null) {
                    transformObjects[i] = gameObject;
                    if (config.config.HasValue("translate_x"))
                    {
                        translate[0] = Convert.ToSingle(config.config.GetValue("translate_x"));
                    }
                    if (config.config.HasValue("translate_y"))
                    {
                        translate[1] = Convert.ToSingle(config.config.GetValue("translate_y"));
                    }
                    if (config.config.HasValue("translate_z"))
                    {
                        translate[2] = Convert.ToSingle(config.config.GetValue("translate_z"));
                    }
                    transformAmounts[i] = new Vector3(translate[0], translate[1], translate[2]);
                    Debug.Log("[ZUI] target: " + gameObject.ToString() + ", transformAmounts: " + transformAmounts[i]);
                } else
                {
                    Debug.Log("[ZUI] Invalid transform target! (" + target + ")");
                    continue;
                }
                i++;
            }

            i = 0;
            foreach (GameObject gameObject in transformObjects)
            {
                if (gameObject != null)
                {
                    gameObject.transform.localPosition = transformAmounts[i];
                }
                i++;
            }
        }
        //public void Update()
        //{
        //    if (Input.GetKeyUp(KeyCode.E))
        //    {
        //        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        //        foreach (GameObject gameObject in gameObjects)
        //        {
        //            if (gameObject != null)
        //            {
        //                Debug.Log("[ZUI] Name: " + gameObject.name + ", " + gameObject.ToString());
        //            }
        //        }
        //    }
        //}
    }
}