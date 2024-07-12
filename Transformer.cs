using System;
using UnityEngine;

// Partially based on wheeeUI and Visual Studio's autocomplete feature
namespace ZUI
{
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class Transformer : MonoBehaviour
	{
		internal static Transformer instance;
		internal UrlDir.UrlConfig[] transforms;
		internal GameObject[] transformObjects;
		internal string[] relativeTransform; // this will turn out okay
		public Vector3[] translateAmounts;
		public Vector3[] rotateAmounts; // rotate will always be !relative
		// i'll add scale eventually

		private bool debugMode = false;

		public void Start()
		{
			instance = this;
			transforms = GameDatabase.Instance.GetConfigs("ZUITransform");
			transformObjects = new GameObject[transforms.Length];
			translateAmounts = new Vector3[transforms.Length];
			rotateAmounts = new Vector3[transforms.Length];
			relativeTransform = new string[transforms.Length];

			int i = 0;
			foreach (UrlDir.UrlConfig config in transforms)
			{
				if (!config.config.HasValue("target"))
				{
					Debug.Log("[ZUI] Node does not have a transform target!");
					continue;
				}
				string target = config.config.GetValue("target");
				float[] translate = new float[3];
				GameObject gameObject = GameObject.Find(target);

				// Some GameObjects return null even tho they exist. Calling the Start() function again fixes this.
				if (gameObject != null)
				{
					transformObjects[i] = gameObject;

					if (config.config.HasValue("relative"))
					{
						relativeTransform[i] = config.config.GetValue("relative");
					}
					translate[0] = config.config.HasValue("translate_x") ? Convert.ToSingle(config.config.GetValue("translate_x")) : 0f;
					translate[1] = config.config.HasValue("translate_y") ? Convert.ToSingle(config.config.GetValue("translate_y")) : 0f;
					translate[2] = config.config.HasValue("translate_z") ? Convert.ToSingle(config.config.GetValue("translate_z")) : 0f;
					rotateAmounts[i] = config.config.HasValue("rotate")
						? new Vector3(0f, 0f, Convert.ToSingle(config.config.GetValue("rotate")))
						: new Vector3(0f, 0f, 0f);
					translateAmounts[i] = new Vector3(translate[0], translate[1], translate[2]);
					Debug.Log("[ZUI] target: " + gameObject.ToString() + ", translateAmounts: " + translateAmounts[i] + ", rotateAmounts: " + rotateAmounts[i]);
				} else
				{
					Debug.Log("[ZUI] Invalid transform target! (" + target + ")");
					continue;
				}
				i++;
			}

			// This could probably be merged with the loop above but just in
			// case I find a gameObject that can only be moved in Update() or
			// something then it would be very easy to move this loop there.
			i = 0;
			foreach (GameObject gameObject in transformObjects)
			{
				if (gameObject != null)
				{
					switch (relativeTransform[i])
					{
						case "yes":
						case "YES":
						case "true":
						case "TRUE":
							gameObject.transform.localPosition += translateAmounts[i];
							break;
						default:
							gameObject.transform.localPosition = translateAmounts[i];
							break;
					}
					gameObject.transform.localEulerAngles = rotateAmounts[i];
				}
				i++;
			}

			UrlDir.UrlConfig[] ZUISettings = GameDatabase.Instance.GetConfigs("ZUISettings");
			foreach (UrlDir.UrlConfig config in ZUISettings)
			{
				if (config.config.HasValue("debug_mode"))
				{
					if (config.config.GetValue("debug_mode") == "true") // too lazy to find out if this doesn't error when these if statements are combined.
					{
						debugMode = true;
					}
				}
			}
		}
		public void Update()
		{
			if (debugMode)
			{
				if (Input.GetKeyUp(KeyCode.E))
				{
					GameObject[] gameObjects = FindObjectsOfType<GameObject>();
					foreach (GameObject gameObject in gameObjects)
					{
						if (gameObject != null)
						{
							Debug.Log("[ZUI] Name: " + gameObject.name + " | Translate: " + gameObject.transform.localPosition.ToString() + " | Rotate: " + gameObject.transform.localPosition.ToString());
						}
					}
				}
				//if (Input.GetKeyUp(KeyCode.Q))
				//{
				//	Start(); // Unfortunately the configs don't get reloaded so all this does is nudge the textures more
				//}
			}
		}
	}
}