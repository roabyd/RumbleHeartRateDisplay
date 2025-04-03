using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Bluetooth;

namespace BluetoothConnectionManager
{
    class BluetoothHandler
    {
        
        static async Task Main(string[] args)
        {

            Console.WriteLine("Starting Bluetooth Query Application...");

            // Scan for devices
            Console.WriteLine("Scanning for devices...");
            var devices = await Bluetooth.ScanForDevicesAsync();

            // Locate Pixel Watch
            var pixelWatch = devices.FirstOrDefault(d => d.Name != null && d.Name.Contains("Pixel Watch", StringComparison.OrdinalIgnoreCase));
            if (pixelWatch == null)
            {
                Console.WriteLine("Pixel Watch not found.");
                return;
            }

            Console.WriteLine($"Found Pixel Watch: {pixelWatch.Name}. Connecting...");

            // Connect to Pixel Watch
            var device = await BluetoothDevice.FromIdAsync(pixelWatch.Id);

            if (device == null)
            {
                Console.WriteLine("Failed to connect.");
                return;
            }
            Console.WriteLine("Connected to Pixel Watch.");

            // Get Heart Rate Service and Characteristic
            var UUID = new Guid("0000180d-0000-1000-8000-00805f9b34fb");
            var bluetoothUUID = BluetoothUuid.FromGuid(UUID);
            var heartRateService = await device.Gatt.GetPrimaryServiceAsync(bluetoothUUID);

            while (heartRateService == null)
            {
                heartRateService = await device.Gatt.GetPrimaryServiceAsync(bluetoothUUID);
            }

            var characteristics = await heartRateService.GetCharacteristicsAsync();
            var heartRateCharacteristic = characteristics.FirstOrDefault(c => c.Properties.HasFlag(GattCharacteristicProperties.Notify));

            while (heartRateCharacteristic == null)
            {
                heartRateCharacteristic = characteristics.FirstOrDefault(c => c.Properties.HasFlag(GattCharacteristicProperties.Notify));
            }

            Console.WriteLine("Subscribing to heart rate notifications...");

            int lastSentHeartRate = 0;
            // Open named pipe for communication
            using (var pipeServer = new NamedPipeServerStream("HeartRatePipe", PipeDirection.Out))
            {
                Console.WriteLine("Waiting for client connection...");
                pipeServer.WaitForConnection();
                Console.WriteLine("Client connected!");

                // Handle value changes for notifications
                heartRateCharacteristic.CharacteristicValueChanged += async (sender, args) =>
                {
                    var heartRate = args.Value[1]; // Extract heart rate from notification data
                    Console.WriteLine($"Heart Rate: {heartRate} BPM");
                    if (heartRate != lastSentHeartRate)
                    {
                        // Send heart rate data through the pipe
                        var message = $"HeartRate:{heartRate}\n";
                        var buffer = Encoding.UTF8.GetBytes(message);
                        await pipeServer.WriteAsync(buffer, 0, buffer.Length);
                        await pipeServer.FlushAsync();
                        lastSentHeartRate = heartRate;
                    }     
                };

                // Start notifications
                await heartRateCharacteristic.StartNotificationsAsync();

                Console.WriteLine("Press Enter to stop...");
                Console.ReadLine();

                await heartRateCharacteristic.StopNotificationsAsync();
            }

            Console.WriteLine("Exiting...");
        }
    }
}
