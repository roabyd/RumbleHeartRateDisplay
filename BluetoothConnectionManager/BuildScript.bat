#!/bin/bash
dotnet publish -c Release -o ./publish/ -r win-x64 /p:PublishSingleFile=true --self-contained true
move "C:\Users\rob_d\source\repos\RumbleHeartRateDisplay\BluetoothConnectionManager\publish\BluetoothConnectionManager.exe" "C:\Program Files (x86)\Steam\steamapps\common\RUMBLE\UserData"