using MelonLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumbleHeartRateDisplay
{
    internal class HeartRateReceiver
    {

        private static Process bluetoothProcess;
        public async Task StartStreamingHeartRate()
        {
            try
            {
                Core.Logger.Msg("Starting Mod Application for heart rate data...");
                StartBluetoothHandler();

                using (var pipeClient = new NamedPipeClientStream(".", "HeartRatePipe", PipeDirection.In))
                {
                    Core.Logger.Msg("Connecting to HeartRatePipe...");
                    pipeClient.Connect();
                    Core.Logger.Msg("Connected to HeartRatePipe!");

                    using (var reader = new StreamReader(pipeClient, Encoding.UTF8))
                    {
                        while (true)
                        {
                            var line = await reader.ReadLineAsync();
                            if (line != null && line.StartsWith("HeartRate:"))
                            {
                                var heartRate = line.Replace("HeartRate:", "").Trim();
                                Core.Logger.Msg($"Received Heart Rate: {heartRate} BPM");
                                HeartHud.UpdateHeartUi(Int32.Parse(heartRate));
                            }
                        }
                    }
                }

                Core.Logger.Msg("BLE scan completed.");
            }
            catch (Exception e)
            {
                Core.Logger.Msg("exception occured connecting to bluetooth: " + e.Message);
            }

        }

        static void StartBluetoothHandler()
        {
            try
            {
                bluetoothProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "UserData/BluetoothConnectionManager.exe", // Path to the Bluetooth handler executable
                        Arguments = "", // Optional arguments
                        CreateNoWindow = true, // Hide the console window if not needed
                        UseShellExecute = false
                    }
                };
                bluetoothProcess.Start();
                Core.Logger.Msg("Bluetooth handler started.");
            }
            catch (Exception ex)
            {
                Core.Logger.Msg($"Failed to start Bluetooth handler: {ex.Message}");
            }
        }

        public static void StopBluetoothHandler()
        {
            try
            {
                if (bluetoothProcess != null && !bluetoothProcess.HasExited)
                {
                    bluetoothProcess.Kill();
                    Core.Logger.Msg("Bluetooth handler stopped.");
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Msg($"Failed to stop Bluetooth handler: {ex.Message}");
            }
        }
    }
}
