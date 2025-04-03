using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RumbleHeartRateDisplay
{
    internal class ModResources
    {
        private static Il2CppAssetBundle bundle;

        private static TMP_FontAsset tmpFont;
        private static GameObject heartImageObject;

        private static bool initialized = false;
        public static bool Initialized { get { return initialized; } }

        public static TMP_FontAsset TmpFont { get { return tmpFont; } }
        public static GameObject HeartImageObject { get { return heartImageObject; } }

        public static readonly Color HeartLowColor = new Color(164f / 255, 245f / 255, 130f / 255);
        public static readonly Color HeartMediumColor = new Color(255f / 255, 180f / 255, 0f / 255);
        public static readonly Color HeartHighColor = new Color(255f / 255, 0f / 255, 0f / 255);
        public static readonly Color HeartDefaultColor = new Color(255f / 255, 255f / 255, 255f / 255);

        public static void LoadResources(bool reload = false)
        { 
            if (initialized && !reload) return;

            bundle = Il2CppAssetBundleManager.LoadFromFile(@"UserData/heartratebundle");
            tmpFont = GameObject.Instantiate(bundle.LoadAsset<TMP_FontAsset>("TMP_GoodDogPlain"));
            heartImageObject = GameObject.Instantiate(bundle.LoadAsset<GameObject>("HeartImageObject"));
            heartImageObject.name = "RumbleHeartHud_heartImageObject";

            GameObject.DontDestroyOnLoad(heartImageObject);

            initialized = true;
        }
    }

}
