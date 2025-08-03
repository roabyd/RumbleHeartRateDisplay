using Il2CppTMPro;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace RumbleHeartRateDisplay
{
    internal class ModResources
    {
        private static AssetBundle bundle;

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

            bundle = LoadAssetBundleFromFile(@"UserData/heartratebundle");
            tmpFont = GameObject.Instantiate(bundle.LoadAsset<TMP_FontAsset>("TMP_GoodDogPlain"));
            heartImageObject = GameObject.Instantiate(bundle.LoadAsset<GameObject>("HeartImageObject"));
            heartImageObject.name = "RumbleHeartHud_heartImageObject";

            GameObject.DontDestroyOnLoad(heartImageObject);

            initialized = true;
        }

        public static AssetBundle LoadAssetBundleFromFile(string filePath)
        {
            Stream stream = StreamFromFile(filePath);
            Il2CppSystem.IO.Stream il2CppStream = ConvertToIl2CppStream(stream);
            AssetBundle bundle = AssetBundle.LoadFromStream(il2CppStream);
            stream.Close();
            il2CppStream.Close();
            return bundle;
        }

        public static MemoryStream StreamFromFile(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            return new MemoryStream(fileBytes);
        }

        public static Il2CppSystem.IO.Stream ConvertToIl2CppStream(Stream stream)
        {
            Il2CppSystem.IO.MemoryStream Il2CppStream = new Il2CppSystem.IO.MemoryStream();

            const int bufferSize = 4096;
            byte[] managedBuffer = new byte[bufferSize];
            Il2CppStructArray<byte> il2CppBuffer = new(managedBuffer);

            int bytesRead;
            while ((bytesRead = stream.Read(managedBuffer, 0, managedBuffer.Length)) > 0)
            {
                il2CppBuffer = managedBuffer;
                Il2CppStream.Write(il2CppBuffer, 0, bytesRead);
            }
            Il2CppStream.Flush();
            return Il2CppStream;
        }
    }

}
