using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;
using Il2CppTMPro;
using MelonLoader;
using System.Collections;

namespace RumbleHeartRateDisplay
{
    internal class HeartHud
    {
        private static GameObject uiContainer = null;
        private static Canvas canvas = null;
        private static string heartSpeedParamName = "BeatSpeedMultiplier";

        private static bool initialized = false;
        public static bool Initialized { get { return initialized; } }

        private static readonly int playerControllerLayerMask = LayerMask.NameToLayer("PlayerController");
        private const int playerControllerLayer = 8388608;

        private static HeartUiElements uiElements = new HeartUiElements();

        public static void Initialize()
        {
            if (initialized) return;

            uiContainer = new GameObject();
            uiContainer.name = "HeartRateHud_Canvas";
            uiContainer.AddComponent<Canvas>();

            canvas = uiContainer.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasScaler = uiContainer.AddComponent<CanvasScaler>();

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1008);

            uiContainer.AddComponent<GraphicRaycaster>();

            CreateHeartUi();

            GameObject.DontDestroyOnLoad(uiContainer);
            GameObject.DontDestroyOnLoad(canvas);

            initialized = true;
        }

        public static void ToggleVisible()
        {
            if (uiContainer == null) return;

            uiContainer.active = !uiContainer.active;
        }

        public static void CreateHeartUi()
        {
            //Create the beating heart game object
            GameObject heartImageObject = ModResources.HeartImageObject;
            heartImageObject.transform.parent = uiContainer.transform;

            var heartImagetransform = heartImageObject.GetComponent<RectTransform>();
            heartImagetransform.anchorMin = new Vector2(0, 1);
            heartImagetransform.anchorMax = new Vector2(0, 1);
            heartImagetransform.pivot = new Vector2(0.5f, 0.5f);
            heartImagetransform.position = new Vector3(120, 850, 0);

            Animator heartAnimator = heartImageObject.GetComponent<Animator>();
            heartAnimator.SetFloat(heartSpeedParamName, 0);

            //HeartRate object
            GameObject heartRateObject = new GameObject();
            heartRateObject.transform.parent = heartImageObject.transform;
            heartRateObject.name = $"RumbleHeartHud_heartRate";
            TextMeshProUGUI heartRateText = heartRateObject.AddComponent<TextMeshProUGUI>();

            heartRateText.font = ModResources.TmpFont;
            heartRateText.text = "-- BPM";

            var heartRateTextTransform = heartRateText.GetComponent<RectTransform>();
            heartRateTextTransform.position = new Vector3(0, 7);
            heartRateTextTransform.sizeDelta = new Vector2(100, 80);
            heartRateText.enableAutoSizing = true;
            heartRateText.fontSizeMax = 55;
            heartRateText.fontSizeMin = 45;

            // Anchor to top left.
            heartRateTextTransform.anchorMin = new Vector2(0.5f, 0.5f);
            heartRateTextTransform.anchorMax = new Vector2(0.5f, 0.5f);
            heartRateTextTransform.pivot = new Vector2(0.5f, 0.5f);

            heartRateTextTransform.anchoredPosition = new Vector3(0, 0);
            heartRateText.alignment = TextAlignmentOptions.Center;

            int scale = 1;
            heartImageObject.transform.localScale = new Vector3(scale, scale, scale);

            uiElements = new HeartUiElements
            {
                Container = heartImageObject,
                HeartRate = heartRateText,
                Position = 1
            };
        }

        public static void UpdateHeartUi(int heartRate)
        {
            if (uiElements == null) return;

            MelonCoroutines.Start(UpdateHeartUiCoroutine(heartRate));
        }

        private static IEnumerator UpdateHeartUiCoroutine(int heartRate)
        {
            yield return null; // Wait for the next frame (ensures main thread execution)
            PerformHeartUiUpdate(heartRate);
        }


        public static void PerformHeartUiUpdate(int heartRate)
        {
            uiElements.HeartRate.text = heartRate >= 10 ? $"{heartRate}   BPM" : "-- BPM";

            Color heartTextColor = ModResources.HeartDefaultColor;

            if (heartRate > 140)
            {
                heartTextColor = ModResources.HeartHighColor;
            }
            else if (heartRate >= 90)
            {
                heartTextColor = ModResources.HeartMediumColor;
            }
            else if (heartRate >= 10)
            {
                heartTextColor = ModResources.HeartLowColor;
            }
            else
            {
                heartTextColor = ModResources.HeartDefaultColor;
            }

            if (heartRate > 0)
            {
                uiElements.Container.GetComponent<Animator>().SetFloat(heartSpeedParamName, heartRate * 0.0166f);
            }
            else
            {

               uiElements.Container.GetComponent<Animator>().SetFloat(heartSpeedParamName, 0);
            }

            uiElements.HeartRate.color = heartTextColor;
        }
    }
}
