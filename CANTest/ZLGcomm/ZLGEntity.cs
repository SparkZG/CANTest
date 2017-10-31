using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Data;
using System.Windows;
using DevExpress.Xpf.Core;
using Microsoft.Win32;
using System.IO;

namespace CANTest
{

    /// <summary>
    /// ZLG接口卡的函数封装类
    /// </summary>
    public class ZLGEntity : ZLG_API, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #region 定义ZLGCAN变量

        private UInt32 devType;
        /// <summary>
        /// 设备类型号
        /// </summary>
        public UInt32 DevType
        {
            get { return devType; }
            set
            {
                devType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DevType"));
            }
        }


        private UInt32 devIndex;
        /// <summary>
        /// 设备索引号
        /// </summary>
        public UInt32 DevIndex
        {
            get { return devIndex; }
            set
            {
                devIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DevIndex"));
            }
        }

        private UInt32 devChannel;
        /// <summary>
        /// 设备通道号
        /// </summary>
        public UInt32 DevChannel
        {
            get { return devChannel; }
            set
            {
                devChannel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DevChannel"));
            }
        }
        private BaudRate baudRate;
        /// <summary>
        /// 设备波特率
        /// </summary>
        public BaudRate BaudRate
        {
            get { return baudRate; }
            set
            {
                baudRate = value;
                SetBaudRateTimer(baudRate);
                OnPropertyChanged(new PropertyChangedEventArgs("BaudRate"));
            }
        }

        private UInt32 accCode;
        /// <summary>
        /// 验收码
        /// </summary>
        public UInt32 AccCode
        {
            get { return accCode; }
            set
            {
                accCode = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AccCode"));
            }
        }



        private UInt32 accMask;
        /// <summary>
        /// 屏蔽码
        /// </summary>
        public UInt32 AccMask
        {
            get { return accMask; }
            set
            {
                accMask = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AccMask"));
            }
        }


        private byte timing0;
        /// <summary>
        /// 波特率定时器0
        /// </summary>
        public byte Timing0
        {
            get { return timing0; }
            set
            {
                timing0 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Timing0"));
            }
        }


        private byte timing1;
        /// <summary>
        /// 波特率定时器1
        /// </summary>
        public byte Timing1
        {
            get { return timing1; }
            set
            {
                timing1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Timing1"));
            }
        }


        private byte mode;
        /// <summary>
        /// 模式。=0表示正常模式（相当于正常节点），=1表示只听模式（只接收，不影响总线）。
        /// </summary>
        public byte Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Mode"));
            }
        }

        #endregion

        #region 界面功能必需参数定义
        private int typeIndex;
        /// <summary>
        /// 设备选择窗体类型下拉框索引
        /// </summary>
        public int TypeIndex
        {
            get { return typeIndex; }
            set
            {
                typeIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TypeIndex"));
            }
        }

        private int sendNum;
        /// <summary>
        /// 发送帧数
        /// </summary>
        public int SendNum
        {
            get { return sendNum; }
            set
            {
                sendNum = value;
                TotalNum = sendNum + receiveNum;
                OnPropertyChanged(new PropertyChangedEventArgs("SendNum"));
            }
        }

        private int receiveNum;
        /// <summary>
        /// 接收帧数
        /// </summary>
        public int ReceiveNum
        {
            get { return receiveNum; }
            set
            {
                receiveNum = value;
                TotalNum = sendNum + receiveNum;
                OnPropertyChanged(new PropertyChangedEventArgs("ReceiveNum"));
            }
        }

        private int totalNum;
        /// <summary>
        /// 帧数总数
        /// </summary>
        public int TotalNum
        {
            get { return totalNum; }
            set
            {
                totalNum = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TotalNum"));
            }
        }

        private String connObject;
        /// <summary>
        /// TabControl Head
        /// </summary>
        public String ConnObject
        {
            get { return connObject; }
            set
            {
                connObject = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ConnObject"));
            }
        }

        private string sendID;
        /// <summary>
        /// 要发送的数据ID
        /// </summary>
        public string SendID
        {
            get { return sendID; }
            set
            {
                sendID = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SendID"));
            }
        }

        private string sendData = String.Empty;
        /// <summary>
        /// 要发送的数据内容
        /// </summary>
        public string SendData
        {
            get { return sendData; }
            set
            {
                sendData = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SendData"));
            }
        }

        private int sendTime;
        /// <summary>
        /// 发送次数
        /// </summary>
        public int SendTime
        {
            get { return sendTime; }
            set
            {
                sendTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SendTime"));
            }
        }

        private bool sendOutFlag = true;
        /// <summary>
        /// 发送数据完成标识
        /// </summary>
        public bool SendOutFlag
        {
            get { return sendOutFlag; }
            set
            {
                sendOutFlag = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SendOutFlag"));
            }
        }

        private int sendInterval;
        /// <summary>
        /// 发送次数
        /// </summary>
        public int SendInterval
        {
            get { return sendInterval; }
            set
            {
                sendInterval = value;
                OnPropertyChanged(new PropertyChangedEventArgs("sendInterval"));
            }
        }


        private bool flag = false;
        /// <summary>
        /// 标识位，记录接口卡设备是否被打开，false=关闭，true=打开
        /// </summary>
        public bool Flag
        {
            get { return flag; }
            set
            {
                flag = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Flag"));
            }
        }

        private DataTable canData = new DataTable();
        /// <summary>
        /// 从CAN设备中读取到的数据表
        /// </summary>
        public DataTable CANData
        {
            get { return canData; }
            set
            {
                canData = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CANData"));
            }
        }

        /// <summary>
        /// 数据库(过滤)
        /// </summary>
        private DataTable DataBase = new DataTable();

        /// <summary>
        /// 接收总线数据的线程
        /// </summary>
        private Thread listenThread = null;

        /// <summary>
        /// 刷新页面数据的线程
        /// </summary>
        private Thread refreshThread = null;

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">接口卡的型号</param>
        public ZLGEntity()
        {
            typeIndex = CANTest.Properties.Settings.Default.TypeIndex;
            accCode = 0x00000000;
            accMask = 0xFFFFFFFF;
            mode = 0;
            timing0 = 0x01;
            timing1 = 0x1C;
            baudRate = BaudRate._250Kbps;
            CANData.Columns.Add("dttype", typeof(int));
            CANData.Columns.Add("direction", typeof(string));
            CANData.Columns.Add("time", typeof(string));
            CANData.Columns.Add("id", typeof(string));
            CANData.Columns.Add("type", typeof(string));
            CANData.Columns.Add("data", typeof(string));
            CANData.Columns.Add("signal", typeof(int));
            DataBase = CANData.Clone();

            sendID = "12345678";
            sendData = "00 00 00 00 00 00 00 00";
            sendTime = 1;
            sendInterval = 1000;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 打开ZLG接口卡设备，此方法对于一个设备只能被调用一次
        /// </summary>
        private bool OpenDevice()
        {
            if (Flag)
            {
                return true;
            }

            uint res = VCI_OpenDevice(devType, devIndex, 0);
            if ((Status)res == Status.ERROR)
            {
                Flag = false;
                return false;
            }
            Flag = true;
            return true;
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        private bool CloseDevice()
        {
            if (Flag)
            {
                UInt32 res = VCI_CloseDevice(devType, devIndex);
                if ((Status)res == Status.SUCCESS)
                {
                    Flag = false;
                }
                else
                {
                    Flag = true;
                }
                return Flag;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 初始化某一路CAN通道
        /// </summary>
        private bool InitDevice()
        {
            VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
            UInt32 pData = 0x060007;
            if (devType > 18 && devType < 23)
            {
                config.Mode = 0;
                SetBaudRate(baudRate, ref pData);
                VCI_SetReference(devType, devIndex, 0, 0, ref pData);
            }
            else
            {
                config.AccCode = accCode;
                config.AccMask = accMask;
                config.Filter = 1;
                config.Mode = mode;
                config.Timing0 = timing0;
                config.Timing1 = timing1;
            }
            UInt32 res = VCI_InitCAN(devType, devIndex, devChannel, ref config);
            if ((Status)res == Status.ERROR)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 启动某一路CAN通道
        /// </summary>
        /// <param name="canIndex"></param>
        private bool StartDevice()
        {
            UInt32 res = VCI_StartCAN(devType, devIndex, devChannel);
            if ((Status)res == Status.ERROR)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 复位某一路CAN通道，复位后需要重新调用StartDevice方法，启动CAN的某一路通道
        /// </summary>
        /// <param name="canIndex"></param>
        private void ResetDevice(UInt32 canIndex)
        {
            UInt32 res = VCI_ResetCAN(devType, devIndex, canIndex);
            if ((Status)res == Status.ERROR)
            {

            }
        }

        /// <summary>
        /// 根据波特率设置存储参数有关数据缓冲区地址首指针2E-U等类型CAN
        /// </summary>
        private void SetBaudRate(BaudRate value, ref UInt32 pData)
        {
            switch (value)
            {
                case BaudRate._5Kbps:
                    pData = 0x1C01C1;
                    break;
                case BaudRate._10Kbps:
                    pData = 0x1C00E0;
                    break;
                case BaudRate._20Kbps:
                    pData = 0x1600B3;
                    break;
                case BaudRate._50Kbps:
                    pData = 0x1C002C;
                    break;
                case BaudRate._100Kbps:
                    pData = 0x160023;
                    break;
                case BaudRate._125Kbps:
                    pData = 0x1C0011;
                    break;
                case BaudRate._250Kbps:
                    pData = 0x1C0008;
                    break;
                case BaudRate._500Kbps:
                    pData = 0x060007;
                    break;
                case BaudRate._800Kbps:
                    pData = 0x060004;
                    break;
                case BaudRate._1000Kbps:
                    pData = 0x060003;
                    break;
                default:
                    pData = 0x060007;
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baudRate">波特率</param>
        private void SetBaudRateTimer(BaudRate value)
        {
            switch (value)
            {
                case BaudRate._5Kbps:
                    Timing0 = 0xBF;
                    Timing1 = 0xFF;
                    break;
                case BaudRate._10Kbps:
                    Timing0 = 0x31;
                    Timing1 = 0x1C;
                    break;
                case BaudRate._20Kbps:
                    Timing0 = 0x18;
                    Timing1 = 0x1C;
                    break;
                case BaudRate._40Kbps:
                    Timing0 = 0x87;
                    Timing1 = 0xFF;
                    break;
                case BaudRate._50Kbps:
                    Timing0 = 0x09;
                    Timing1 = 0x1C;
                    break;
                case BaudRate._80Kbps:
                    Timing0 = 0x83;
                    Timing1 = 0xFF;
                    break;
                case BaudRate._100Kbps:
                    Timing0 = 0x04;
                    Timing1 = 0x1C;
                    break;
                case BaudRate._125Kbps:
                    Timing0 = 0x03;
                    Timing1 = 0x1C;
                    break;
                case BaudRate._200Kbps:
                    Timing0 = 0x81;
                    Timing1 = 0xFA;
                    break;
                case BaudRate._250Kbps:
                    Timing0 = 0x01;
                    Timing1 = 0x1C;
                    break;
                case BaudRate._400Kbps:
                    Timing0 = 0x80;
                    Timing1 = 0xFA;
                    break;
                case BaudRate._500Kbps:
                    Timing0 = 0x00;
                    Timing1 = 0x1C;
                    break;
                case BaudRate._666Kbps:
                    Timing0 = 0x80;
                    Timing1 = 0xB6;
                    break;
                case BaudRate._800Kbps:
                    Timing0 = 0x00;
                    Timing1 = 0x16;
                    break;
                case BaudRate._1000Kbps:
                    Timing0 = 0x00;
                    Timing1 = 0x14;
                    break;
                default:
                    Timing0 = 0x00;
                    Timing1 = 0x1C;
                    break;

            }
        }

        #endregion

        private void ReadInformation()
        {
            VCI_BOARD_INFO info = new VCI_BOARD_INFO();
            UInt32 res = VCI_ReadBoardInfo(devType, devIndex, ref info);
        }

        #region 公有方法

        /// <summary>
        /// 运行设备的指定CAN通道，并开启线程接收CAN总线数据
        /// 如果需要多路CAN通道，此方法需要被多次调用
        /// </summary>
        public void RunDevice()
        {
            if (!OpenDevice())
            {
                return;
            }
            if (!InitDevice())
            {
                return;
            }
            if (!StartDevice())
            {
                return;
            }
            //监听线程
            listenThread = new Thread(new ThreadStart(DeviceReceive));
            listenThread.IsBackground = true;
            listenThread.Priority = ThreadPriority.AboveNormal;
            listenThread.Start();
            //刷新页面表格线程
            refreshThread = new Thread(new ThreadStart(RefreshData));
            refreshThread.IsBackground = true;
            refreshThread.Priority = ThreadPriority.AboveNormal;
            refreshThread.Start();
        }

        private void RefreshData()
        {
            while (true)
            {
                _dispatcher.BeginInvoke(new Action(delegate()
                {
                    lock (DataBase)
                    {
                        DataRow[] arrayDR = DataBase.Select("signal='0'");
                        foreach (DataRow _dr in arrayDR)
                        {
                            _dr["signal"] = 1;
                            dealDataTable(_dr, "add");
                        }
                    }
                }));

                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// 接收CAN总线上的数据
        /// </summary>
        private void DeviceReceive()
        {
            VCI_ERR_INFO pErrInfo = new VCI_ERR_INFO();
            while (true)
            {
                SpinWait.SpinUntil(() => false, 1);//80
                //返回接收缓冲区中尚未被读取的帧数
                UInt32 num = VCI_GetReceiveNum(devType, devIndex, devChannel);
                if (num == 0)
                {
                    continue;
                }
                //分配一次最多接收VCI_GetReceiveNum函数返回值的帧数的数据存储内存空间
                IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (int)num);
                //返回实际读到的帧数，如返回的为0xFFFFFFFF，则读取数据失败，有错误发生。
                UInt32 len = VCI_Receive(devType, devIndex, devChannel, pt, num, -1);
                if (len == 0xFFFFFFFF)
                {
                    VCI_ReadErrInfo(devType, devIndex, devChannel, ref pErrInfo);
                    //释放分配的内存空间
                    Marshal.FreeHGlobal(pt);
                    continue;
                }

                //获取CAN总线上的数据并触发事件
                for (int i = 0; i < len; i++)
                {
                    VCI_CAN_OBJ receData = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));

                    string _data = String.Empty;
                    byte[] temp = receData.Data;
                    for (int j = 0; j < receData.DataLen; j++)
                    {
                        _data += String.Format("{0:X2}", temp[j]) + " ";
                    }

                    //将接收的数据添加到table中
                    lock (DataBase)
                    {
                        DataRow _dr = DataBase.NewRow();
                        _dr["dttype"] = 1;
                        _dr["id"] = "0x" + string.Format("{0:X}", receData.ID);
                        _dr["direction"] = (string)Application.Current.Resources["teReceive"];
                        _dr["type"] = receData.ExternFlag == 0 ? (string)Application.Current.Resources["teStandardFrame"] : (string)Application.Current.Resources["teExtendedFrame"];
                        _dr["time"] = DateTime.Now.ToString("MM/dd HH:mm:ss:ffff");//((double)receData.TimeStamp / 10000).ToString().PadRight(9, '0');
                        _dr["data"] = _data.Trim();
                        _dr["signal"] = 0;
                        DataBase.Rows.Add(_dr);
                        if (DataBase.Rows.Count > 1000)
                        {
                            for (int n = 0; n < DataBase.Rows.Count - 1000; n++)
                            {
                                DataBase.Rows.RemoveAt(0);
                            }
                        }
                    }
                }
                Marshal.FreeHGlobal(pt);
            }
        }

        /// <summary>
        /// 发送单帧信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="canIndex"></param>
        /// <param name="data"></param>
        /// <param name="dataLength"></param>
        public void SingleTransmit()
        {
            if (sendData.ToString() == "" || sendID.ToString() == "")
            {
                return;
            }
            string[] _data = sendData.Trim().Split(' ');

            //在非托管内存中分配一个VCI_CAN_OBJ结构体大小的内存空间——————此方法似乎不行！！！
            //IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * 1);
            //VCI_CAN_OBJ pSend = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt), typeof(VCI_CAN_OBJ));

            VCI_CAN_OBJ pSend = new VCI_CAN_OBJ();
            //初始化Data为8个字节
            pSend.Data = new byte[8];
            pSend.DataLen = System.Convert.ToByte(_data.Length);
            for (int i = 0; i < _data.Length; i++)
            {
                pSend.Data[i] = System.Convert.ToByte(_data[i], 16);
            }
            pSend.ExternFlag = 1;
            pSend.RemoteFlag = 0;
            pSend.SendType = 0;
            pSend.TimeFlag = 0;
            pSend.TimeStamp = 0x00;
            pSend.ID = Convert.ToUInt32(sendID, 16);

            //返回实际发送成功的帧数
            UInt32 res = VCI_Transmit(devType, devIndex, devChannel, ref pSend, 1);
            if (res == 0)
            {
                _dispatcher.BeginInvoke(new Action(delegate()
                    {
                        DXMessageBox.Show((string)Application.Current.Resources["tePromptText3"], (string)Application.Current.Resources["tePrompt"], MessageBoxButton.OK, MessageBoxImage.Warning);
                    }));
                AbortThread();
                return;
            }

            //将接收的数据添加到table中
            lock (DataBase)
            {
                DataRow _dr = DataBase.NewRow();
                _dr["dttype"] = 0;
                _dr["id"] = "0x" + sendID;
                _dr["direction"] = (string)Application.Current.Resources["coSend"];
                _dr["type"] = pSend.ExternFlag == 0 ? (string)Application.Current.Resources["teStandardFrame"] : (string)Application.Current.Resources["teExtendedFrame"];
                _dr["time"] = DateTime.Now.ToString("MM/dd HH:mm:ss:ffff");
                _dr["data"] = sendData;
                _dr["signal"] = 0;
                DataBase.Rows.Add(_dr);
                if (DataBase.Rows.Count > 1000)
                {
                    for (int i = 0; i < DataBase.Rows.Count - 1000; i++)
                    {
                        DataBase.Rows.RemoveAt(0);
                    }
                }
            }

            //Marshal.FreeHGlobal(pt);
        }

        private readonly object _syncRootSend = new object();

        /// <summary>
        /// 根据操作类型操作数据表
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="type"></param>
        public void dealDataTable(DataRow dr, string operateType, string dataType = "send")
        {
            lock (_syncRootSend)
            {
                if (operateType == "add")
                {
                    CANData.Rows.Add(dr.ItemArray);
                    if (dr["dttype"].ToString() == "0")
                    {
                        SendNum++;
                    }
                    else
                    {
                        ReceiveNum++;
                    }

                }
                else if (operateType == "clear")
                {
                    CANData.BeginLoadData();
                    CANData.Rows.Clear();
                    SendNum = 0;
                    ReceiveNum = 0;
                    CANData.EndLoadData();
                    CANData.AcceptChanges();
                }
                else if (operateType == "export")
                {
                    ExportData();
                }
            }
        }

        /// <summary>
        /// 发送多帧信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="canIndex"></param>
        /// <param name="list"></param>
        public void MultiTransmit(UInt32 ID, List<byte[]> list)
        {
            //在非托管内存中分配多个VCI_CAN_OBJ结构体大小的内存空间 
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                //在非托管内存中创建一个VCI_CAN_OBJ对象
                VCI_CAN_OBJ pSend = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                pSend.Data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                pSend.DataLen = 8;
                for (int index = 0; index < 8; index++)
                {
                    pSend.Data[index] = list[i][index];
                }
                pSend.ExternFlag = 0;
                pSend.RemoteFlag = 0;
                pSend.SendType = 0;
                pSend.TimeFlag = 0;
                pSend.TimeStamp = 0x00;
                pSend.ID = ID;
            }

            //返回实际发送成功的帧数
            UInt32 res = VCI_Transmit(devType, devIndex, devChannel, pt, 1);

            Marshal.FreeHGlobal(pt);
        }
        /// <summary>
        /// 替换某些字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ReplaceChars(string input)
        {
            if (input == null) return "";
            input = input.Replace(" ", "_")
                .Replace(":", "")
                .Replace("/", "");

            return input;
        }
        /// <summary>
        /// 导出数据
        /// </summary>
        private void ExportData()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "*.csv";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "csv files|*.csv";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.FileName = "MessageRecord_" + ReplaceChars(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            bool? result = saveFileDialog.ShowDialog();
            if (result == true && saveFileDialog.FileName != null) //打开保存文件对话框
            {
                string fileName = saveFileDialog.FileName;//文件名字
                using (StreamWriter streamWriter = new StreamWriter(fileName, false, Encoding.Default))
                {
                    //要写入消息的字段名
                    StringBuilder sb = new StringBuilder();
                    if (canData != null && canData.Columns.Count > 0)
                    {
                        for (int m = 0; m < canData.Columns.Count; m++)
                        {
                            string name = canData.Columns[m].Caption;
                            if (name != "保留")
                            {
                                sb.Append(name).Append(",");
                            }
                        }
                    }
                    streamWriter.WriteLine(sb.ToString());
                    //要写的数据源
                    for (int i = 0; i < canData.Rows.Count; i++)
                    {
                        StringBuilder sbline = new StringBuilder();
                        for (int j = 0; j < canData.Columns.Count; j++)
                        {
                            sbline.Append(canData.Rows[i][j].ToString() + ",");
                        }
                        streamWriter.WriteLine(sbline.ToString());
                    }
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
        }

        #region 开启、关闭发送线程
        /// <summary>
        /// 获取当前dispatcher，用来异步操作
        /// </summary>
        System.Windows.Threading.Dispatcher _dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        /// <summary>
        /// 发送CAN帧线程
        /// </summary>
        Thread _sendThread = null;
        /// <summary>
        /// 建立发送线程
        /// </summary>
        public void StartThread()
        {
            _sendNum = 0;
            SendOutFlag = false;
            _sendThread = new Thread(new ThreadStart(SendUntilOut));
            _sendThread.IsBackground = true;
            _sendThread.Priority = ThreadPriority.AboveNormal;
            _sendThread.Start();
        }
        /// <summary>
        /// 用于判断是否发送完成的比对值
        /// </summary>
        int _sendNum = 0;
        /// <summary>
        /// 开始发送数据
        /// </summary>
        private void SendUntilOut()
        {
            while (true)
            {
                SingleTransmit();
                _sendNum++;
                if (_sendNum == sendTime)
                {
                    AbortThread();
                    break;
                }
                Thread.Sleep(sendInterval);
            }
        }
        /// <summary>
        /// 关闭发送线程
        /// </summary>
        public void AbortThread()
        {
            SendOutFlag = true;
            if (_sendThread != null)
            {
                _sendThread.Abort();
            }
        }

        #endregion

        /// <summary>
        /// 关闭设备，释放线程资源
        /// </summary>
        public void StopDevice()
        {
            AbortThread();
            CloseDevice();
            if (listenThread != null)
            {
                listenThread.Abort();
            }
            if (refreshThread != null)
            {
                refreshThread.Abort();
            }
            
        }

        #endregion


    }
}
