using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;

namespace RumbleHeartRateDisplay
{
    public class Core : MelonMod
    {
        //private BLEManagerOld _bleManager;
        private HeartRateReceiver _heartRateReceiver;

        private string settingsFilePath = @"UserData\RumbleHeartHud.xml";

        public static MelonLogger.Instance Logger { get; private set; }

        public override void OnInitializeMelon()
        {
            Logger = LoggerInstance;
            LoggerInstance.Msg("RumbleHeartRateDisplay: Initialized.");

            try
            {
                Settings.FromXmlFile(settingsFilePath);
                LoggerInstance.Msg("Loaded settings from file.");
            }
            catch
            {
                Settings.Initialize();
                LoggerInstance.Msg("Unable to load settings. Using defaults.");
            }

            _heartRateReceiver = new HeartRateReceiver();

            _ = Task.Run(() => _heartRateReceiver.StartStreamingHeartRate());
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

            if (Input.GetKeyDown(KeyCode.H))
            {
                _ = Task.Run(() => HeartRateReceiver.RestartBluetoothHandler(_heartRateReceiver));
            }
        }

        public override void OnApplicationQuit()
        {
            MelonLogger.Msg("Game is exiting...");
            // Stop the bluetooth handler process
            HeartRateReceiver.StopBluetoothHandler();
        }
    }
}
