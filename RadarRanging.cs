using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Media;


namespace RADARMRM
{
    class RadarRanging
    {
        const int mrmOffset_Ps = 11000;

        SoundPlayer beep;
        
        UInt16 mrmMsgType;
        UInt16 mrmMsgID;
        UInt32 mrmNodeID;
        Int32 mrmScanStart_Ps;
        Int32 mrmScanStop_Ps;
        //Int32 sensitivity;
        int maxDistance_m;
        int minDistance_m;
       
        const int C_mps = 299792458;
        UInt16 mrmScanCount;
        int TransmitGain;
        UInt32 ScanIntervalTime_ms;
        int ScanRes_ps;
        const UInt16 mrmScanResolution = 32;
        UInt16 mrmBaseIntegrationIndex;
        byte mrmAntennaMode;
        byte mrmTransmitGain;
        byte mrmCodeChannel;
        byte mrmPersistFlag;
        UInt32 mrmStatus;

        UdpClient socket;
        public string IP { get; private set; }

        byte[] sendPacket = new byte[100];
        byte[] receivePacket;
        IPEndPoint receiveIPEnd;

        int sendLength;

        public bool MRMConfirmed { get; private set; }
        public bool MRMSet { get; private set; }
        public bool MRMStoped { get; private set; }
        public bool MRMRunning { get; private set; }
        public int Sensitivity { get; set; }
        public double StartDistance { get; set; }
        public double Range { get; set; }
        public double DetectedDistance { get; private set; }
        

        int threshold;

        bool changingSection;
        bool mrmSetFailed;

        Form1 uiForm;

        int mrmScanLength;
        int mrmAverageDepth;
        int mrmAverageIndex;
        int[] mrmCurrentScan;
        float[] mrmScanAverage;        
        float[] mrmScanDifference;
        float[] mrmFiltedScan;
        float[] mrmFinalScan;
        float[] mrmBackground;
        float[] mrmDifferential;
        float mrmDifferentialEnergy;

        int sections;
        int currentSection;
        public int MRMAverageDepth { get { return 0; } set { mrmAverageDepth = 1 << value; } }
        public int Sections { get { return sections; } private set { sections = value; } }
        public int CurrentSection { get { return currentSection; } private set { currentSection = value; } }
        int warmingUpScans;
        int detectionProgress = 0;
        int detectionNO;
        int detected = 0;
        
        UInt16 thisMessageID = 0;
        UInt16 lastMessageIndex = 0;
        UInt16 messageHandled = 0;

        Object detectionLock = new Object();

        public RadarRanging(string ip, Form1 ui)
        {
            beep = new SoundPlayer();
            beep.SoundLocation = "beep.wav";
            mrmScanStart_Ps = mrmOffset_Ps;
            maxDistance_m = 10;
            minDistance_m = 2;  
            ScanRes_ps = 61;
            TransmitGain = 63;
            mrmAverageDepth = 32;
            detectionNO = 8;
            MRMRunning = false;
            DetectedDistance = -1;

            IP = String.Copy(ip);
            socket = new UdpClient();
            socket.Connect(IPAddress.Parse(ip), 22000);
            Task.Factory.StartNew(Receiving, TaskCreationOptions.LongRunning);
            uiForm = ui;
        }

        public void MRMGetConfiguration()
        {
            MRMConfirmed = false;
            mrmMsgType = 0x1002;
            sendLength = 4;
            Buffer.BlockCopy(BitConverter.GetBytes(mrmMsgType), 0, sendPacket, 2, 2);
            Request();
        }

        public void MRMSetConfiguration()
        {
           // uiForm.numericUpDown2_start_ValueChanged()
            MRMSet = false;

            if (mrmSetFailed)
            {
            }
            mrmSetFailed = false;
            mrmPersistFlag = 0;
            mrmTransmitGain = (byte)TransmitGain;
            mrmAntennaMode = 2;
            mrmCodeChannel = 1;
            mrmBaseIntegrationIndex = 15;
            //mrmScanStart_Ps = mrmOffset_Ps;
            mrmScanStart_Ps = mrmOffset_Ps + (int)((StartDistance + currentSection * 0.7) * 6671);
            mrmScanStop_Ps = mrmScanStart_Ps + 1;
            // mrmScanStop_Ps = mrmScanStart_Ps + 40000;
            // mrmScanStop_Ps = (mrmScanStart_Ps + (2 * maxDistance_m / C_mps) * 1e12);          

            mrmMsgType = 0x1001;

            sendPacket[0] = mrmPersistFlag;
            sendPacket[1] = mrmCodeChannel;
            sendPacket[2] = mrmTransmitGain;
            sendPacket[3] = mrmAntennaMode;
            Array.Clear(sendPacket, 4, 12);
            BitConverter.GetBytes(mrmBaseIntegrationIndex).CopyTo(sendPacket, 16);
            BitConverter.GetBytes(mrmScanResolution).CopyTo(sendPacket, 18);
            BitConverter.GetBytes(mrmScanStop_Ps).CopyTo(sendPacket, 20);
            BitConverter.GetBytes(mrmScanStart_Ps).CopyTo(sendPacket, 24);
            //BitConverter.GetBytes(mrmNodeID).CopyTo(sendPacket, 28);
            //BitConverter.GetBytes(mrmMsgID).CopyTo(sendPacket, 32);
            BitConverter.GetBytes(mrmMsgType).CopyTo(sendPacket, 34);

            sendLength = 36;
            Request();
           // MRMGetConfiguration();
        }

        void Request()
        {
            Array.Reverse(sendPacket, 0, sendLength);
            Task<int> task = Task<int>.Factory.FromAsync(socket.BeginSend, socket.EndSend, sendPacket, sendLength, null);
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
                        UInt16 setStatus = BitConverter.ToUInt16(receivePacket, receivePacket.Length - 8);
                        if (setStatus == 0)
                        {
                            uiForm.UpdateStatusBox(IP + " MRM set configuration confirmed\n");
                            MRMSet = true;
                            MRMStartRadio();
                        }
                        else 
                        {
                            uiForm.UpdateStatusBox(IP + " " + setStatus.ToString() + " MRM set configuration failed\n");
                            mrmSetFailed = true;
                        }                        
                        break;
                    case 0x1102:
                        uiForm.UpdateStatusBox(IP + " MRM get configuration confirmed\n");
                        uiForm.MRMStoped();
                        break;
                    case 0x1103:
                        if (msgID == 21)
                        {
                            MRMRunning = false;
                            uiForm.UpdateStatusBox(IP + " MRM stoped\n");
                            if (!MRMStoped)
                            {
                                MRMStoped = true;
                                MRMGetConfiguration();
                            } 
                        }
                        if (msgID == 20)
                        {
                            MRMRunning = true;
                            uiForm.UpdateStatusBox(IP + " MRM started\n");
                            if (changingSection)
                            {
                                changingSection = false;
                            }
                            uiForm.MRMStarted();
                        }
                        break;                    
                    case 0xf201:
                        if (changingSection)
                        {                            
                        }
                        else if (MRMFullScan())
                        {
                            
                            for (int i = 0; i < mrmScanLength; i++)
                            {
                                mrmScanAverage[i] += mrmCurrentScan[i];
                            }
                            if (++mrmAverageIndex < mrmAverageDepth)
                            {
                                break;
                            }
                            else
                            {
                                mrmAverageIndex = 0;
                            }
                            MeasureDifference();
                            Array.Clear(mrmScanAverage, 0, mrmScanLength);
                            if (warmingUpScans > 0)
                            {
                                warmingUpScans--;
                                break;
                            }
                            Differentiate();                            
                            Dedrift();
                            MatchingFilt();
                            Detect();

                            //threshold();
                            //Finalscan();
                            //detection();

                            

                           
                            //uiForm.UpdateChart2_threshold(mrmScanThreshold);
                           // uiForm.UpdateChart2_threshold(mrmDifferential);
                            //uiForm.UpdateChart2(mrmBackground);   
                            uiForm.UpdateChart2(mrmScanDifference);
                          // uiForm.UpdateChart2(mrmScanscanEnvelope);
                            //uiForm.UpdateChart2(mrmFinalScan);                            
                           // uiForm.UpdateStatusBox(" scan\n");
                           // detection();
                        }
                        break;
                    default:
                        uiForm.UpdateStatusBox(IP + msgType.ToString() + " Unknown incoming message\n");
                        break;
                }
            }
        }

        void MRMGetConfigurationConfirm()
        {
            mrmNodeID = BitConverter.ToUInt32(receivePacket, 36);
            mrmScanStart_Ps = BitConverter.ToInt32(receivePacket, 32);
            mrmScanStop_Ps = BitConverter.ToInt32(receivePacket, 28);
            mrmBaseIntegrationIndex = BitConverter.ToUInt16(receivePacket, 24);
            mrmAntennaMode = receivePacket[11];
            mrmTransmitGain = receivePacket[10];
            mrmCodeChannel = receivePacket[9];
            mrmPersistFlag = receivePacket[8];
            mrmStatus = BitConverter.ToUInt32(receivePacket, 0);
            if (mrmStatus != 0)
            {
                uiForm.UpdateStatusBox(IP + " MRM get configuration confirmed with error\n");
            }
            else
            {
                MRMConfirmed = true;
            }
        }

        public void MRMStartRadio()
        {
            UInt16 msgType = 0x1003;
            UInt32 ScanIntervalTime_ms = 0;
            UInt16 mrmScanCount = 65535;
            UInt16 msgID = 20;
            sendLength = 12;
            Buffer.BlockCopy(BitConverter.GetBytes(mrmScanCount), 0, sendPacket, 6, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(ScanIntervalTime_ms * 1000), 0, sendPacket, 0, 4);
            Array.Clear(sendPacket, 4, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(msgType), 0, sendPacket, 10, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(msgID), 0, sendPacket, 8, 2);
            Request();
            // uiForm.UpdateStatusBox(msgType + " incoming MRM scan\n");
        }

        public void MRMStopRadio()
        {
            MRMStoped = false;
            UInt16 msgType = 0x1003;
            UInt16 mrmScanCount = 0;
            UInt16 msgID = 21;
            sendLength = 12;
            Buffer.BlockCopy(BitConverter.GetBytes(mrmScanCount), 0, sendPacket, 6, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(msgID), 0, sendPacket, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(msgType), 0, sendPacket, 10, 2);
            Request();
        }        

        bool MRMFullScan()
        {
            int packetLength = receivePacket.Length;
            UInt16 messageID = BitConverter.ToUInt16(receivePacket, packetLength - 4);
            UInt32 NodeID = BitConverter.ToUInt32(receivePacket, packetLength - 8);
            UInt32 Timestamp = BitConverter.ToUInt32(receivePacket, packetLength - 12);
            Int32 ScanStart = BitConverter.ToInt32(receivePacket, packetLength - 32);
            Int32 Scanstop = BitConverter.ToInt32(receivePacket, packetLength - 36);
            Int16 ScanStepbins = BitConverter.ToInt16(receivePacket, packetLength - 38);
            // ScanType=receivePacket.Length
            UInt16 numberOfSamplesInThisMessage = BitConverter.ToUInt16(receivePacket, packetLength - 44);
            UInt32 totalNumberOfScanSamples = BitConverter.ToUInt32(receivePacket, packetLength - 48);
            UInt16 messageIndex = BitConverter.ToUInt16(receivePacket, packetLength - 50);
            UInt16 totalNumberOfMessages = BitConverter.ToUInt16(receivePacket, packetLength - 52);           
            mrmScanLength = (int)totalNumberOfScanSamples;  
            if (mrmCurrentScan == null)
            {
                uiForm.Updatechart2_XAxis(mrmScanLength);
                mrmCurrentScan = new int[mrmScanLength];
                mrmScanDifference = new float[mrmScanLength];
                mrmBackground = new float[mrmScanLength];
                mrmFiltedScan = new float[mrmScanLength];
                mrmFinalScan = new float[mrmScanLength];
                mrmScanAverage = new float[mrmScanLength];
                mrmDifferential = new float[mrmScanLength];
            }                        
            if (messageIndex == 0)
            {
                Buffer.BlockCopy(receivePacket, (350 - numberOfSamplesInThisMessage) * 4, mrmCurrentScan, (int)totalNumberOfScanSamples * 4 - numberOfSamplesInThisMessage * 4, numberOfSamplesInThisMessage * 4);
                if (totalNumberOfMessages == 1)
                {
                    Array.Reverse(mrmCurrentScan);
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
                        Buffer.BlockCopy(receivePacket, (350 - numberOfSamplesInThisMessage) * 4 , mrmCurrentScan, (int)totalNumberOfScanSamples * 4 - messageIndex * 1400 - numberOfSamplesInThisMessage * 4, numberOfSamplesInThisMessage * 4);
                        lastMessageIndex++;
                        messageHandled++;
                        if (messageHandled == totalNumberOfMessages)
                        {
                            Array.Reverse(mrmCurrentScan);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        void MeasureDifference()
        {
            for (int i = 0; i < mrmScanLength; i++)
            {
                mrmScanDifference[i] = mrmScanAverage[i] - mrmBackground[i];
                mrmBackground[i] = mrmScanAverage[i];
            }
        }

        void Differentiate()
        {
            mrmDifferentialEnergy = 0;
            for (int i = 7; i < mrmScanLength - 7; i++)
            {
                mrmDifferential[i] = (mrmBackground[i - 1] - mrmBackground[i + 1]) * 0.37659f + (mrmBackground[i + 2] - mrmBackground[i - 2]) * 0.158788f + (mrmBackground[i - 3] - mrmBackground[i + 3]) * 0.0779779f + (mrmBackground[i + 4] - mrmBackground[i - 4]) * 0.0358675f + (mrmBackground[i - 5] - mrmBackground[i + 5]) * 0.0127406f + (mrmBackground[i + 6] - mrmBackground[i - 6]) * 0.0013936f + (mrmBackground[i + 7] - mrmBackground[i - 7]) * 0.00102285f;
                mrmDifferentialEnergy += mrmDifferential[i] * mrmDifferential[i];
            }
        }

        void Dedrift()
        {
            float mrmCrossEnergy = 0;
            for (int i = 7; i < mrmScanLength - 7; i++)
            {
                mrmCrossEnergy += mrmScanDifference[i]  * mrmDifferential[i];
            }
            float crossCoefficient = mrmCrossEnergy / mrmDifferentialEnergy;
            if (crossCoefficient > 0.5f)
                crossCoefficient = 0.5f;
            for (int i = 7; i < mrmScanLength - 7; i++)
            {
                mrmScanDifference[i] -= mrmDifferential[i] * crossCoefficient;
            }
            float mrmBackgroundEnergy = 0;
            float mrmCrossBackgroundEnergy = 0;
            for (int i = 7; i < mrmScanLength - 7; i++)
            {
                mrmBackgroundEnergy += mrmBackground[i] * mrmBackground[i];
                mrmCrossBackgroundEnergy += mrmScanDifference[i] * mrmBackground[i];
            }
            float crossCoefficientBackground = mrmCrossBackgroundEnergy / mrmBackgroundEnergy;
            if (crossCoefficientBackground > 0.5f)
                crossCoefficientBackground = 0.5f;
            for (int i = 7; i < mrmScanLength - 7; i++)
            {
                mrmScanDifference[i] -= mrmBackground[i] * crossCoefficientBackground;
            }
            for (int i = 0; i < 7; i++)
            {
                mrmScanDifference[i] = 0;
                mrmScanDifference[mrmScanLength - 1 - i] = 0;
            }

        }

        void MatchingFilt()
        {
            for (int i = 7; i < mrmScanLength - 7; i++)
            {
                mrmFinalScan[i] = mrmScanDifference[i + 1] + mrmScanDifference[i + 5] + mrmScanDifference[i - 3] + mrmScanDifference[i - 7] / 2 - mrmScanDifference[i - 1] - mrmScanDifference[i - 5] - mrmScanDifference[i + 3] - mrmScanDifference[i + 7] / 2 ;
            }
            //for (int i = 7; i < mrmScanLength - 7; i++)
            //{
            //    mrmFinalScan[i] = (mrmFiltedScan[i - 7] - mrmFiltedScan[i - 5] + mrmFiltedScan[i - 3] - mrmFiltedScan[i - 1] + mrmFiltedScan[i + 1] - mrmFiltedScan[i + 3] + mrmFiltedScan[i + 5] - mrmFiltedScan[i + 7]) / 8;
            //}
        }

        void Detect()
        {
            float currentThreshold = threshold;
            if ((StartDistance + 0.7 * currentSection) < 1)
            {
                currentThreshold *= 10;
            }
            int index = Array.FindIndex(mrmFinalScan, (Predicate<float>)((float x) => Math.Abs(x) > currentThreshold));
            if (index != -1)
            {
                if (++detected > detectionNO / 2)
                {
                    beep.Play();
                    DetectedDistance = 0.7 * (currentSection + (double)index / 96);
                    uiForm.UpdateStatusBox(DetectedDistance.ToString() + "\n");
                    uiForm.Stopping = true;
                    MRMStopRadio();
                    return;
                }                    
            }
            if (++detectionProgress == detectionNO)
            {
                detectionProgress = 0;
                detected = 0;
                changingSection = true;
                if (++currentSection > sections)
                {
                    currentSection = 0;
                }
                warmingUpScans = 1;
                detectionProgress = 0;
                MRMSetConfiguration();
            }
        }

        public void ClearAll()
        {
            mrmCurrentScan = null;
            mrmAverageIndex = 0;
        }

        public void MRMStart()
        {
            detected = 0;
            detectionProgress = 0;
            DetectedDistance = -1;
            warmingUpScans = 8;
            sections = (int)(Range / 0.7);
            currentSection = 0;
            threshold = 65536;
            if (Sensitivity < 0)
            {
                threshold = threshold << (-Sensitivity);
            }
            else
            {
                threshold = threshold >> Sensitivity;
            }
            MRMSetConfiguration();
        }          
    }
      
  }    
                       
            
        
       
         
   
