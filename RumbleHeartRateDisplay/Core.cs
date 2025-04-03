using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

namespace RumbleHeartRateDisplay
{
    public class Core : MelonMod
    {
        //private BLEManagerOld _bleManager;
        private HeartRateReceiver _heartRateReceiver;

        public static MelonLogger.Instance Logger { get; private set; }

        public override void OnInitializeMelon()
        {
            Logger = LoggerInstance;
            LoggerInstance.Msg("RumbleHeartRateDisplay: Initialized.");

            //_bleManager = new BLEManagerOld();
            //_bleManager.OnDeviceDiscovered += DeviceDiscoveredHandler;
            _heartRateReceiver = new HeartRateReceiver();

            // Start scanning for devices asynchronously
            //_ = Task.Run(() => _bleManager.StartScanningAsync("Pixel Watch"));
            _ = Task.Run(() => _heartRateReceiver.StartStreamingHeartRate());
        }

        private void DeviceDiscoveredHandler(string deviceName)
        {
            LoggerInstance.Msg($"Device discovered: {deviceName}");
        }

        public override void OnUpdate()
        {
            // Load the resources if not loaded.
            if (!ModResources.Initialized)
            {
                ModResources.LoadResources();
            }

            if (!HeartHud.Initialized)
            {
                HeartHud.Initialize();
            }


        }

        public override void OnApplicationQuit()
        {
            MelonLogger.Msg("Game is exiting...");
            // Perform cleanup or save data here
            HeartRateReceiver.StopBluetoothHandler();
        }
    }
}
