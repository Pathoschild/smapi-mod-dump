/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DryIcedTea/Buttplug-Valley
**
*************************************************/

using Buttplug.Client;
using Buttplug.Client.Connectors.WebsocketConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace ButtplugValley
{
    internal class BPManager
    {
        private ButtplugClient client = new ButtplugClient("ButtplugValley");
        private ModEntry _modEntry;
        private string _intifaceIP;
        private IMonitor monitor;
        public ModConfig config;
        
        private Queue<Task> vibrationQueue = new Queue<Task>();
                private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);


        public async Task ScanForDevices()
        {
            // If we're not connected, don't even run
            if (!client.Connected)
            {
                monitor.Log("Buttplug not connected, cannot scan for devices", LogLevel.Debug);
                return;
            }
            monitor.Log("Scanning for devices", LogLevel.Info);
            await client.StartScanningAsync();
            await Task.Delay(30000);
            monitor.Log("Stopping scanning for devices", LogLevel.Info);
            await client.StopScanningAsync();
        }
        public async Task ConnectButtplug(IMonitor meMonitor, string meIntifaceIP)
        {
            monitor = meMonitor;
            _intifaceIP = meIntifaceIP;
            // Don't stomp our client if it's already connected.
            if (client.Connected)
            {
                monitor.Log("Buttplug already connected, skipping", LogLevel.Debug);
                return;
            }
            monitor.Log("Buttplug Client Connecting", LogLevel.Info);
            client.Dispose();
            client = new ButtplugClient("ButtplugValley");
            await client.ConnectAsync(new ButtplugWebsocketConnector(new Uri($"ws://{_intifaceIP}")));
            monitor.Log($"Connecting to {_intifaceIP}", LogLevel.Debug);
            monitor.Log($"{client.Devices.Count()} devices found on startup.", LogLevel.Info);
            foreach (var device in client.Devices)
            {
                monitor.Log($"- {device.Name} ({device.DisplayName} : {device.Index})", LogLevel.Info);
            }
            client.DeviceAdded += HandleDeviceAdded;
            client.DeviceRemoved += HandleDeviceRemoved;
            client.ServerDisconnect += (object o, EventArgs e) => monitor.Log("Intiface Server disconnected.", LogLevel.Warn);
            monitor.Log("Buttplug Client Connected", LogLevel.Info);
            // Add other event handlers as needed
        }

        public async Task DisconnectButtplug()
        {
            // Doesn't *really* matter but saves an extra exception from being thrown.
            if (!client.Connected)
            {
                monitor.Log("Buttplug not connected, skipping", LogLevel.Debug);
                return;
            }
            vibrationQueue.Clear();
            monitor.Log("Disconnecting Buttplug Client", LogLevel.Info);
            // Disconnect from the buttplug.io server
            await client.DisconnectAsync();
        }

        private void HandleDeviceAdded(object sender, DeviceAddedEventArgs e)
        {
            monitor.Log($"Buttplug Device {e.Device.Name} ({e.Device.DisplayName} : {e.Device.Index}) Added", LogLevel.Info);
        }

        private void HandleDeviceRemoved(object sender, DeviceRemovedEventArgs e)
        {
            monitor.Log($"Buttplug Device {e.Device.Name} ({e.Device.DisplayName} : {e.Device.Index}) Removed", LogLevel.Info);
        }

        private bool HasVibrators()
        {
            if (!client.Connected)
            {
                // Noop, this ends up being way too spammy if someone is playing with the mod installed but not connected.
                return false;
            }
            else if (client.Devices.Count() == 0)
            {
                monitor.Log("Either buttplug is not connected or no devices are available");
                return false;
            }
            else if (!client.Devices.Any(device => device.VibrateAttributes.Count > 0))
            {
                monitor.Log("No connected devices have vibrators available.", LogLevel.Warn);
                return false;
            }
            return true;
        }

        public async Task VibrateDevice(float level)
        {
            // This implicited works as a Connected check, as Buttplug clears the device list on disconnect.
            if (!HasVibrators())
            {
                return;
            }
            float intensity = MathHelper.Clamp(level, 0f, 100f) / 100f;
            foreach (var device in client.Devices) 
            {
                if (device.VibrateAttributes.Count > 0)
                {
                    monitor.Log($"Vibrating at {intensity}", LogLevel.Trace);
                    await device.VibrateAsync(intensity);
                }
                else
                {
                    monitor.Log($"No vibrators on device {device.Name}", LogLevel.Trace);
                }
            }
        }

        //Short Vibration pulse. Intensity from 1-100
        public async Task VibrateDevicePulse(float level)
        {
            if (vibrationQueue.Count >= config.QueueLength)
            {
                monitor.Log("Vibration queue is full, skipping", LogLevel.Debug);
                
                // return;
            }
            
            await VibrateDevicePulse(level, 400);
        }

        //Vibration with customizable duration. Intensity from 1-100
        public async Task VibrateDevicePulse(float level, int duration)
        {
            if (!HasVibrators())
            {
                return;
            }
            
            // Check if the queue has reached its limit.
            if (vibrationQueue.Count >= config.QueueLength)
            {
                monitor.Log("Vibration queue is full, skipping", LogLevel.Trace);
                return;
            }

            float intensity = MathHelper.Clamp(level, 0f, 100f);
            monitor.Log($"VibrateDevicePulse {intensity}", LogLevel.Trace);

            // Create a new task that performs the vibration and add it to the queue
            vibrationQueue.Enqueue(VibrateDeviceWithDuration(intensity, duration));

            // If the semaphore is not already locked, start a new task that processes the queue
            if (semaphore.CurrentCount > 0)
            {
                _ = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    
                    while (vibrationQueue.Count > 0)
                    {
                        var task = vibrationQueue.Dequeue();
                        await task;
                        
                        if (vibrationQueue.Count == 0)
                        {
                            await VibrateDevice(0);
                        }
                    }

                    semaphore.Release();
                });
            }
        }

        private async Task VibrateDeviceWithDuration(float intensity, int duration)
        {
            await VibrateDevice(intensity);
            await Task.Delay(duration);
        }

        public async Task StopDevices()
        {
            // Once Buttplug C# v3.0.1 is out, just use this line.
            // 
            // await client.StopAllDevicesAsync();
            vibrationQueue.Clear();
            await VibrateDevice(0);
        }
    }
}