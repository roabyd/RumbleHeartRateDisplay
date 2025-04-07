# RumbleHeartRateDisplay

A simple modification to display your current heart rate in your stream for others to view.

## Overview

RumbleHeartRateDisplay is a lightweight, user-friendly mod designed for real-time heart rate monitoring. The mod consists of 2 parts, the first is the rumble mod to take care or updating the UI, and the second is responsible for connecting to a bluetooth device and sending the heart rate data into the mod. For this to work I am using the androidOS app "Heart for Bluetooth," which uses a standand service and charatertisic to share heart rate. In theory, this could also work with other bluetooth heart rate sharing devices, but I have not tested this.

## Features

- **Real-time heart rate display**: Monitors and displays your current heart rate in Real-time.
- **Lightweight**: Designed for efficiency, ensuring minimal impact on system performance.

## Configuration

Standand setup for Rumble mods, copy the Mods, UserLibs and UserData folders into your Rumble folder (Melon Loader should already be setup for this to work, see https://discord.gg/fsbcnZgzfa for help)

There is a settings file included with the files, which needs to be updated with the name of your bluetooth device. 

To determine the name of your Bluetooth heart rate monitor:

1. Turn on your Bluetooth heart rate monitor and ensure it's in pairing mode.
2. I used the android app "nRF Connect" to look for the device in the list of available devices.
3. Note the name of your heart rate monitor as it appears in the list and connect to it (For your device to be compatible, it should expose the "Heart Rate" primary service UUID: 0x180D)
4. If this is exposed, before opening Rumble, you can run the BluetoothConnectionManager.exe. This will prompt you to input the name of the device and if everything is working, it will start logging heart rate data. Press any key to close this program.
5. Once you have confirmed the above is working, update the RumbleHeartHud.xml file with the name of the device, and you can now open Rumble as you would normally.


If the exe does not start logging heart rate data, it should output some error message to help find the issue.

Enjoy, and start streaming and sharing Rumble!
