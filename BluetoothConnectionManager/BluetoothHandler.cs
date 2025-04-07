using System.IO.Pipes;
using System.Text;
using InTheHand.Bluetooth;

namespace BluetoothConnectionManager
{
    class BluetoothHandler
    {
        // The name of the device to connect to. This will be populated by an inocming argument
        private static string bluetoothDeviceName = ""; 
        private static bool runningStandalone = false;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Bluetooth Handler Application...");
            if (args.Length > 0)
            {
                bluetoothDeviceName = args[0]; // The first argument
                runningStandalone = false;
            }
            else
            {
                while (string.IsNullOrEmpty(bluetoothDeviceName))
                {
                    // Prompt the user for input
                    Console.WriteLine("Please enter your name of your bluetooth heart rate tracking device:");

                    // Read the input from the user
                    bluetoothDeviceName = Console.ReadLine();
                    runningStandalone = true;
                }
            }            

            // Scan for devices
            Console.WriteLine($"Scanning for devices named {bluetoothDeviceName}...");
            var devices = await Bluetooth.ScanForDevicesAsync();

            // Locate Pixel Watch
            var bluetoothHeartRateTracker = devices.FirstOrDefault(d => d.Name != null && d.Name.Contains(bluetoothDeviceName, StringComparison.OrdinalIgnoreCase));
            if (bluetoothHeartRateTracker == null)
            {
                Console.WriteLine($"{bluetoothDeviceName} not found. Please confirm the device name and try again.");
                Thread.Sleep(10000); // Pause for 10,000 milliseconds (10 seconds)
                return;
            }

            Console.WriteLine($"Found {bluetoothDeviceName}: {bluetoothHeartRateTracker.Name}. Connecting...");

            // Connect to bluetooth device
            var device = await BluetoothDevice.FromIdAsync(bluetoothHeartRateTracker.Id);

            if (device == null)
            {
                Console.WriteLine($"Failed to connect. Check if {bluetoothHeartRateTracker.Name} is in pairing mode and try again.");
                Thread.Sleep(10000); // Pause for 10,000 milliseconds (10 seconds)
                return;
            }
            Console.WriteLine($"Connected to {bluetoothHeartRateTracker.Name}");

            try
            {
                // Get Heart Rate Service and Characteristic
                int attempt = 1;
                GattService heartRateService = null;
                while (heartRateService == null && attempt < 10)
                {
                    heartRateService = await device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new Guid("0000180d-0000-1000-8000-00805f9b34fb")));
                    attempt++;
                }
                if (heartRateService == null)
                {
                    Console.WriteLine("Failed to get Heart Rate Service. Please confirm the device exposes the correct service.");
                    Thread.Sleep(10000); // Pause for 10,000 milliseconds (10 seconds)
                    return;
                }

                var characteristics = await heartRateService.GetCharacteristicsAsync();
                var heartRateCharacteristic = characteristics.FirstOrDefault(c => c.Properties.HasFlag(GattCharacteristicProperties.Notify));
                attempt = 1;
                while (heartRateCharacteristic == null && attempt < 5)
                {
                    heartRateCharacteristic = characteristics.FirstOrDefault(c => c.Properties.HasFlag(GattCharacteristicProperties.Notify));
                    attempt++;
                }
                if (heartRateCharacteristic == null)
                {
                    Console.WriteLine("Failed to get Heart Rate Characteristic. Please confirm the device exposes the correct characteristic.");
                    Thread.Sleep(10000); // Pause for 10,000 milliseconds (10 seconds)
                    return;
                }
                Console.WriteLine("Subscribing to heart rate notifications...");

                int lastSentHeartRate = 0;
                // Open named pipe for communication
                using (var pipeServer = new NamedPipeServerStream("HeartRatePipe", PipeDirection.Out))
                {
                    if (!runningStandalone)
                    {
                        Console.WriteLine("Waiting for client connection...");
                        pipeServer.WaitForConnection();
                        Console.WriteLine("Client connected!");
                    }
                    // Handle value changes for notifications
                    heartRateCharacteristic.CharacteristicValueChanged += async (sender, args) =>
                    {
                        var heartRate = args.Value[1]; // Extract heart rate from notification data
                        Console.WriteLine($"Heart Rate: {heartRate} BPM");
                        if (heartRate != lastSentHeartRate && !runningStandalone)
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
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Task canceled: {ex.Message}");
                Thread.Sleep(10000); // Pause for 10,000 milliseconds (10 seconds)
                return;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid operation: {ex.Message}");
                Thread.Sleep(10000); // Pause for 10,000 milliseconds (10 seconds)
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"A problem has occured: {ex.Message}");
                Thread.Sleep(10000); // Pause for 10,000 milliseconds (10 seconds)
                return;
            }            

            Console.WriteLine("Exiting...");
        }
    }
}
