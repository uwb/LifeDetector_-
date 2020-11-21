using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace RADARMRM
{    
    class Radar
    {    

        byte operationMode;
        int radarNO;

        int udpPort;

        Form1 uiForm;
        UdpClient socket;
        IPEndPoint receiveIPEnd;
        byte[] sendPacket = new byte[32];
        byte[] receivePacket;
        int sendLength;

        int oldScanLength = 0;
        int[] fullScan, fullRScan;
        int lastMessageIndex = 0;
        int thisMessageID = 0;
        int messageHandled = 1;

        bool starting, stopping;

        public bool MRMShortRange { get; set; }
        public byte MRMRange { get; set; }
        public byte MRMCodeChannel { get; set; }
        public bool MRMBackgroundReady { get; set; }
        public int CATUnDetect { get; set; }
        public UInt32 CATTarget { get; set; }
        public UInt32[] MRMTarget { get; set; }
        public bool MRMConfirmed { get; set; }
        public UInt32 MRMCloseTarget { get; set; }
        public Int16 CATSensitivity { get; set; }
        public Int16 MRMSensitivity { get; set; }
        public UInt16 DetectionNO { get; set; }
        public int LifeSensitivity { get; set; }
        public Single LifeStart { get; set; }
        public Single LifeRange { get; set; }
        public int LifeAverageDepth { get; set; }
        public bool LifeRunning { get; private set; }
        public int LifeSections { get; private set; }
        public int CurrentLifeSection { get; private set; }
        public Single LifeDetectedDistance { get; set; }

        public byte OperationMode { get { return operationMode; } set { operationMode = value; } }

        public Radar(IPEndPoint ip, Form1 ui, int no)
        {
            radarNO = no;
            LifeRunning = false;
            LifeSections = 0;
            LifeDetectedDistance = -1;
            CATUnDetect = 1;
            operationMode = 1;
            MRMConfirmed = false;
            MRMBackgroundReady = false;
            MRMTarget = new UInt32[4];
            MRMRange = 20;
            udpPort = ip.Port;
            socket = new UdpClient();
            socket.Connect(ip);
            Task.Factory.StartNew(Receiving, TaskCreationOptions.LongRunning);
            uiForm = ui;
        }

        void Receiving()
        {
            while (true)
            {
                receivePacket = socket.Receive(ref receiveIPEnd);
                Array.Reverse(receivePacket);
                UInt16 msgType = BitConverter.ToUInt16(receivePacket, receivePacket.Length - 2);
                UInt16 msgID = BitConverter.ToUInt16(receivePacket, receivePacket.Length - 4);
                switch (msgType)
                {
                    case 0x1101:
                        ConfigurationConfirm();
                        break;
                    case 0x1102:
                        ControlConfirm();
                        break;
                    case 0x1103:
                        RemoteConfirm();
                        break;
                    case 0x1111:
                        MRMBackgroundConfirm();
                        break;
                    case 0x1202:
                        if (FullScan())
                        {
                            if (radarNO == 1)
                                uiForm.UpdateData(fullScan);
                        }
                        break;
                    case 0x1203:
                        if (FullScan())
                        {
                            uiForm.UpdateStatus("Reference received.\n");
                            Buffer.BlockCopy(fullScan, 0, fullRScan, 0, fullRScan.Length * 4);
                            uiForm.UpdateRData(fullRScan);
                        }
                        break;
                    case 0x1204:
                        CATInfoConfirm();
                        break;
                    case 0x1205:
                        MRMInfoConfirm();
                        break;
                    default:
                        break;
                }
            }
        }

        void Request()
        {
            Array.Reverse(sendPacket, 0, sendLength);
            Task<int> task = Task<int>.Factory.FromAsync(socket.BeginSend, socket.EndSend, sendPacket, sendLength, null);
        }

        public void ConfigurationRequest()
        {
            UInt16 msgType = 0x1001;
            sendLength = 12;
            sendPacket[7] = operationMode;
            sendPacket[6] = MRMCodeChannel;
            BitConverter.GetBytes(CATSensitivity).CopyTo(sendPacket, 4);
            BitConverter.GetBytes(MRMSensitivity).CopyTo(sendPacket, 2);
            sendPacket[1] = MRMRange;
            BitConverter.GetBytes(msgType).CopyTo(sendPacket, 10);
            Request();
        }

        public void RemoteRequest(byte mrmRange1, byte sensitivity1, byte averageDepth1, int start1, byte mrmRange2, byte sensitivity2, byte averageDepth2, int start2)
        {
            UInt16 msgType = 0x1003;
            sendLength = 20;
            BitConverter.GetBytes(msgType).CopyTo(sendPacket, 18);
            sendPacket[15] = mrmRange1;
            sendPacket[14] = sensitivity1;
            sendPacket[13] = averageDepth1;
            BitConverter.GetBytes(start1).CopyTo(sendPacket, 8);
            sendPacket[7] = mrmRange2;
            sendPacket[6] = sensitivity2;
            sendPacket[5] = averageDepth2;
            BitConverter.GetBytes(start2).CopyTo(sendPacket, 0);            
            Request();
        }

        void ConfigurationConfirm()
        {
            UInt32 status = BitConverter.ToUInt32(receivePacket, 0);
            if (status == 0)
                uiForm.UpdateStatus(radarNO.ToString() + "号雷达配置成功");
            else
                uiForm.UpdateStatus(radarNO.ToString() + "号雷达配置失败");                           
        }
        
        void CATInfoConfirm()
        {
            CATUnDetect = 0;
            CATTarget = BitConverter.ToUInt32(receivePacket, 0);
            uiForm.UpdateStatus("CAT detected at " + CATTarget.ToString() + ".\n");
        }
        
        void MRMInfoConfirm()
        {
            UInt32 detected = BitConverter.ToUInt32(receivePacket, 12);
            CurrentLifeSection = BitConverter.ToUInt16(receivePacket, 18);
            if (detected != 0)
            {
                LifeDetectedDistance = ((float)BitConverter.ToUInt32(receivePacket, 8)) * 0.00914891f + 0.8f * (float)CurrentLifeSection + LifeStart;
                //uiForm.UpdateStatus(radarNO.ToString() + "号雷达探测到目标位于" + LifeDetectedDistance.ToString("f2") + "米\n");
                LifeRunning = false;
            }
            else
                uiForm.UpdateStatus(radarNO.ToString() + "号雷达正在扫描第" + CurrentLifeSection.ToString() + "段\n");
        }

        void ControlConfirm()
        {
            UInt32 status = BitConverter.ToUInt32(receivePacket, 0);
            if (status == 0)
            {
                uiForm.UpdateStatus(radarNO.ToString() + "号雷达正常响应");
                if (starting)
                {
                    LifeRunning = true;
                    starting = false;
                }
                if (stopping)
                {
                    LifeRunning = false;
                    stopping = false;
                }
            }
            else if (status == 3)
            {
                LifeRunning = false;
                uiForm.UpdateStatus(radarNO.ToString() + "号雷达探测完成");
            }
            else if (status == 100)
            {
                LifeRunning = false;
                uiForm.UpdateStatus("远程遥控停止\n");
            }
            else if (status > 100)  //long vehicle local start
            {
                LifeRunning = true;
                CurrentLifeSection = 0;
                LifeSections = (int)status - 100;
                LifeDetectedDistance = -1;
                uiForm.RemoteStartAsync();
                uiForm.UpdateStatus("远程遥控开始\n");
                //uiForm.UpdateReport(String.Format("{0,-24}{1,-10}{2,-22}{3,-24}\n", DateTime.Now, "XXXXXXX", "正在扫描" + LifeSections.ToString() + "段中第" + (CurrentLifeSection + 1).ToString() + "段", "XXX"));
            }
            else
                uiForm.UpdateStatus("生命探测仪控制异常\n" + status.ToString());
        }

        void RemoteConfirm()
        {
            UInt32 status = BitConverter.ToUInt32(receivePacket, 0);
            if (status == 0)
                uiForm.UpdateStatus("设置保存至雷达成功\n");
            else
                uiForm.UpdateStatus("设置保存至雷达异常\n");
        }

        void MRMBackgroundConfirm()
        {
            UInt32 status = BitConverter.ToUInt32(receivePacket, 0);
            if (status == 0)
            {
                MRMBackgroundReady = true;
                uiForm.UpdateStatus(radarNO.ToString() + "号雷达待命\n");
            }
            else
                uiForm.UpdateStatus(radarNO.ToString() + "号雷达待命\n");
        }

        public void StartLife()
        {            
            LifeSections = (int)((LifeRange + 0.7) / 0.8);//ignore last section if its length is less than 0.1 m;
            int start = 11000 + (int)(LifeStart * 6671);
            CurrentLifeSection = 0;
            LifeDetectedDistance = -1;
            starting = true;
            UInt16 msgType = 0x1002;
            sendLength = 12;
            sendPacket[7] = 6;
            sendPacket[6] = (byte)LifeSections;
            sendPacket[5] = (byte)LifeSensitivity;
            sendPacket[4] = (byte)LifeAverageDepth;
            BitConverter.GetBytes(start).CopyTo(sendPacket, 0);
            BitConverter.GetBytes(msgType).CopyTo(sendPacket, 10);
            Request();
        }        

        public void Stop()
        {
            stopping = true;
            UInt16 msgType = 0x1002;
            sendLength = 12;
            sendPacket[7] = 0;
            BitConverter.GetBytes(msgType).CopyTo(sendPacket, 10);
            Request();
        }

        bool FullScan()
        {
            int packetLength = receivePacket.Length;
            UInt16 messageID = BitConverter.ToUInt16(receivePacket, packetLength - 4);
            UInt16 messageIndex = BitConverter.ToUInt16(receivePacket, packetLength - 6);
            UInt16 totalNumberOfMessages = BitConverter.ToUInt16(receivePacket, packetLength - 8);
            UInt16 numberOfSamplesInThisMessage = BitConverter.ToUInt16(receivePacket, packetLength - 10);
            UInt16 totalNumberOfScanSamples = BitConverter.ToUInt16(receivePacket, packetLength - 12);
            if (totalNumberOfScanSamples != oldScanLength)
            {
                oldScanLength = totalNumberOfScanSamples;
                fullScan = new int[oldScanLength];
                fullRScan = new int[oldScanLength];
            }
            if (messageIndex == 0)
            {
                Buffer.BlockCopy(receivePacket, (512 - numberOfSamplesInThisMessage) * 4, fullScan, (int)totalNumberOfScanSamples * 4 - numberOfSamplesInThisMessage * 4, numberOfSamplesInThisMessage * 4);
                if (totalNumberOfMessages == 1)
                {
                    Array.Reverse(fullScan);
                    return true;
                }
                else
                {
                    lastMessageIndex = messageIndex;
                    thisMessageID = messageID;
                    messageHandled = 1;
                }
            }
            else
            {
                if (messageID == thisMessageID)
                {
                    if (messageIndex == (lastMessageIndex + 1))
                    {
                        Buffer.BlockCopy(receivePacket, (512 - numberOfSamplesInThisMessage) * 4, fullScan, (int)totalNumberOfScanSamples * 4 - messageIndex * 2048 - numberOfSamplesInThisMessage * 4, numberOfSamplesInThisMessage * 4);
                        lastMessageIndex++;
                        messageHandled++;
                        if (messageHandled == totalNumberOfMessages)
                        {
                            Array.Reverse(fullScan);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
