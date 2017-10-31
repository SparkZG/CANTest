using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CANTest
{
    /// <summary>
    /// 定义访问ZLG接口卡的数据结构和接口函数库
    /// </summary>
    public abstract class ZLG_API
    {
        public enum Status { SUCCESS = 1, ERROR = 0 };

        #region 定义数据结构
        /// <summary>
        /// ZLGCAN系列接口卡的设备信息
        /// </summary>
        public struct VCI_BOARD_INFO
        {
            /// <summary>
            /// 硬件版本号，用16进制表示。比如0x0100表示V1.00
            /// </summary>
            public UInt16 hw_Version;
            /// <summary>
            /// 固件版本号，用16进制表示。
            /// </summary>
            public UInt16 fw_Version;
            /// <summary>
            /// 驱动版本号，用16进制表示。
            /// </summary>
            public UInt16 dr_Version;
            /// <summary>
            /// 接口库版本号，用16进制表示。
            /// </summary>
            public UInt16 in_Version;
            /// <summary>
            /// 板卡中所使用的中断号。
            /// </summary>
            public UInt16 irq_Num;
            /// <summary>
            /// 表示板卡中有几路CAN通道。
            /// </summary>
            public byte can_Num;
            /// <summary>
            /// 板卡的序列号。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] str_Serial_Num;
            /// <summary>
            /// 硬件类型。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public byte[] str_hw_Type;
            /// <summary>
            /// 系统保留。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Reserved;
        }


        /// <summary>
        /// CAN帧结构体，即1个结构体表示一个帧的数据结构。在发送和接收函数中被用来传送CAN信息帧
        /// </summary>
        public struct VCI_CAN_OBJ
        {
            /// <summary>
            /// 帧ID。32位变量
            /// </summary>
            public UInt32 ID;
            /// <summary>
            /// 设备接收到某一帧信息的时间标识，只有智能卡才有时间标识
            /// </summary>
            public UInt32 TimeStamp;
            /// <summary>
            /// 是否使用时间标识。值为1时TimeStamp有效
            /// </summary>
            public byte TimeFlag;
            /// <summary>
            /// 发送帧类型
            /// =0时为正常发送（发送失败会自动重发，重发最长时间为1.5-3秒）；
            ///=1时为单次发送（只发送一次，不自动重发）；
            ///=2时为自发自收（自测试模式，用于测试CAN卡是否损坏）；
            ///=3时为单次自发自收（单次自测试模式，只发送一次）。
            /// </summary>
            public byte SendType;
            /// <summary>
            /// 是否是远程帧。=0时为数据帧，=1时为远程帧（数据段空）。
            /// </summary>
            public byte RemoteFlag;
            /// <summary>
            /// 是否是扩展帧。=0时为标准帧（11位ID），=1时为扩展帧（29 位ID）。
            /// </summary>
            public byte ExternFlag;
            /// <summary>
            /// 数据长度DLC小于等于8，即CAN帧Data有几个字节。约束了Data[8]中的有效字节。
            /// </summary>
            public byte DataLen;
            /// <summary>
            /// CAN帧的数据
            /// 由于CAN规定了最大是8个字节，所以这里预留了8个字节的空间，受DataLen约束。如DataLen定义为3，即Data[0]、Data[1]、Data[2]是有效的。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Data;
            /// <summary>
            /// 系统保留
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Reserved;
        }

        /// <summary>
        /// CAN控制器的状态信息
        /// </summary>
        public struct VCI_CAN_STATUS
        {
            /// <summary>
            /// 中断记录，读操作会清除中断。
            /// </summary>
            public byte ErrInterrupt;
            /// <summary>
            /// CAN控制器模式寄存器值。
            /// </summary>
            public byte regMode;
            /// <summary>
            /// CAN控制器状态寄存器值。
            /// </summary>
            public byte regStatus;
            /// <summary>
            /// CAN控制器仲裁丢失寄存器值。
            /// </summary>
            public byte regALCapture;
            /// <summary>
            /// CAN控制器错误寄存器值。
            /// </summary>
            public byte regECCapture;
            /// <summary>
            /// CAN控制器错误警告限制寄存器值。默认为96。
            /// </summary>
            public byte regEWLimit;
            /// <summary>
            /// CAN控制器接收错误寄存器值。
            /// 为0-127时，为错误主动状态，为128-254为错误被动状态，为255时为总线关闭状态。
            /// </summary>
            public byte regRECounter;
            /// <summary>
            /// CAN控制器发送错误寄存器值。
            /// 为0-127时，为错误主动状态，为128-254为错误被动状态，为255时为总线关闭状态。
            /// </summary>
            public byte regTECounter;
            /// <summary>
            /// 系统保留。
            /// </summary>
            public UInt32 Reserved;
        }

        /// <summary>
        /// 记录VCI库运行时产生的错误信息
        /// </summary>
        public struct VCI_ERR_INFO
        {
            /// <summary>
            /// 错误码
            /// </summary>
            public UInt32 ErrCode;
            /// <summary>
            /// 当产生的错误中有消极错误时表示为消极错误的错误标识数据。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Passive_ErrData;
            /// <summary>
            /// 当产生的错误中有仲裁丢失错误时表示为仲裁丢失错误的错误标识数据。
            /// </summary>
            public byte ArLost_ErrData;
        }

        /// <summary>
        /// 定义初始化CAN的配置
        /// </summary>
        public struct VCI_INIT_CONFIG
        {
            /// <summary>
            /// 验收码。
            /// SJA1000的帧过滤验收码。对经过屏蔽码过滤为“有关位”进行匹配，全部匹配成功后，此帧可以被接收。否则不接收。
            /// </summary>
            public UInt32 AccCode;
            /// <summary>
            /// 屏蔽码。
            /// SJA1000的帧过滤屏蔽码。对接收的CAN帧ID进行过滤，对应位为0的是“有关位”，对应位为1的是“无关位”。
            /// 屏蔽码推荐设置为0xFFFFFFFF，即全部接收。
            /// </summary>
            public UInt32 AccMask;
            /// <summary>
            /// 系统保留。
            /// </summary>
            public UInt32 Reserved;
            /// <summary>
            /// 滤波方式。=1表示单滤波，=0表示双滤波
            /// </summary>
            public byte Filter;
            /// <summary>
            /// 波特率定时器0
            /// </summary>
            public byte Timing0;
            /// <summary>
            /// 波特率定时器1
            /// </summary>
            public byte Timing1;
            /// <summary>
            /// 模式。=0表示正常模式（相当于正常节点），=1表示只听模式（只接收，不影响总线）。
            /// </summary>
            public byte Mode;
        }

        /// <summary>
        /// 用于装载更改CANET_UDP与CANET_TCP的目标IP和端口的必要信息。
        /// 此结构体在CANETE_UDP与CANET_TCP中使用。
        /// </summary>
        public struct CHGDESIPANDPORT
        {
            /// <summary>
            /// 更改目标IP和端口所需要的密码，长度小于10，比如为“11223344”。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] szpwd;
            /// <summary>
            /// 所要更改的目标IP，比如为“192.168.0.111”。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] szdesip;
            /// <summary>
            /// 所要更改的目标端口，比如为4000。
            /// </summary>
            public UInt32 desport;
            /// <summary>
            /// 所要更改的工作模式，0表示正常模式，1表示只听模式。
            /// </summary>
            public byte blisten;
        }

        /// <summary>
        /// 定义了滤波器的滤波范围
        /// </summary>
        public struct VCI_FILTER_RECORD
        {
            /// <summary>
            /// 过滤的帧类型标志，为1代表要过滤的为扩展帧，为0代表要过滤的为标准帧。
            /// </summary>
            public UInt32 ExtFrame;
            /// <summary>
            /// 滤波范围的起始帧ID
            /// </summary>
            public UInt32 Start;
            /// <summary>
            /// 滤波范围的结束帧ID
            /// </summary>
            public UInt32 End;
        }
        #endregion

        #region 定义接口库函数

        /// <summary>
        /// 此函数用以打开设备。注意一个CAN设备只能打开一次。
        /// </summary>
        /// <param name="DevType">设备类型号。对应不同的产品型号。</param>
        /// <param name="DevIndex">设备索引号。比如当只有一个USBCAN-2E-U时，索引号为0，这时再插入一个USBCAN-2E-U，那么后面插入的这个设备索引号就是1，以此类推。</param>
        /// <param name="Reserved">保留参数，通常为0。（特例：当设备为CANET-UDP时，此参数表示要打开的本地端口号，建议在5000到40000范围内取值。当设备为CANET-TCP时，此参数固定为0。）</param>
        /// <returns>为1表示操作成功，0表示操作失败。</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_OpenDevice(UInt32 DevType, UInt32 DevIndex, UInt32 Reserved);

        /// <summary>
        /// 此函数用以关闭设备。
        /// </summary>
        /// <param name="DevType">设备类型号。</param>
        /// <param name="DevIndex">设备索引号。对应已经打开的设备。</param>
        /// <returns>为1表示操作成功，0表示操作失败。</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_CloseDevice(UInt32 DevType, UInt32 DevIndex);

        /// <summary>
        /// 初始化指定的CAN通道。有多个CAN通道时，需要多次调用。
        /// 当设备类型为PCI-5010-U、PCI-5020-U、USBCAN-E-U、USBCAN-2E-U时，必须在调用此函数之前调用VCI_SetReference对波特率进行设置
        /// </summary>
        /// <param name="DevType">设备类型号。</param>
        /// <param name="DevIndex">设备索引号。</param>
        /// <param name="CANIndex">第几路CAN。即对应卡的CAN通道号，CAN0为0，CAN1为1，以此类推。</param>
        /// <param name="pInitConfig">初始化参数结构，为一个VCI_INIT_CONFIG结构体变量。（特例：当设备类型为PCI-5010-U、PCI-5020-U、USBCAN-E-U、USBCAN-2E-U时，对滤波和波特率的设置应该放到VCI_SetReference里设置，这里pInitConfig中的成员只有Mode需要设置，其他的6个成员可以忽略</param>
        /// <returns>为1表示操作成功，0表示操作失败。</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_InitCAN(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex, ref VCI_INIT_CONFIG pInitConfig);

        /// <summary>
        /// 获取CAN设备信息
        /// </summary>
        /// <param name="DevType">设备类型号。</param>
        /// <param name="DevIndex">设备索引号。</param>
        /// <param name="pInfo">用来存储设备信息的VCI_BOARD_INFO结构指针。</param>
        /// <returns>为1表示操作成功，0表示操作失败。</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_ReadBoardInfo(UInt32 DevType, UInt32 DevIndex, ref VCI_BOARD_INFO pInfo);

        /// <summary>
        /// 以获取CAN卡发生的最近一次错误信息
        /// 特例：当调用VCI_OpenDevice，VCI_CloseDevice和VCI_ReadBoardInfo这些与特定的第几路CAN操作无关的操作函数失败后，调用此函数来获取失败错误码的时候应该把CANIndex设为－1。
        /// </summary>
        /// <param name="DevType">设备类型号</param>
        /// <param name="DevIndex">设备索引号</param>
        /// <param name="CANIndex">第几路CAN。即对应卡的CAN通道号，CAN0为0，CAN1为1，以此类推</param>
        /// <param name="pErrInfo">用来存储错误信息的VCI_ERR_INFO结构指针</param>
        /// <returns>为1表示操作成功，0表示操作失败</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_ReadErrInfo(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex, ref VCI_ERR_INFO pErrInfo);

        /// <summary>
        /// 获取CAN状态
        /// </summary>
        /// <param name="DevType">设备类型号</param>
        /// <param name="DevIndex">设备索引号</param>
        /// <param name="CANIndex">第几路CAN。即对应卡的CAN通道号，CAN0为0，CAN1为1，以此类推</param>
        /// <param name="pCANStatus">用来存储CAN状态的VCI_CAN_STATUS结构体指针</param>
        /// <returns>为1表示操作成功，0表示操作失败</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_ReadCANStatus(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex, ref VCI_CAN_STATUS pCANStatus);

        /// <summary>
        /// 获取设备的相应参数。
        /// 主要是用来针对各个不同设备的一些特定操作的
        /// </summary>
        /// <param name="DevType">设备类型号</param>
        /// <param name="DevIndex">设备索引号</param>
        /// <param name="CANIndex"></param>
        /// <param name="RefType">参数类型</param>
        /// <param name="pData"></param>
        /// <returns>为1表示操作成功，0表示操作失败</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_GetReference(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex, UInt32 RefType, ref UInt32 pData);

        /// <summary>
        /// 设置设备的相应参数。
        /// 主要是用来针对各个不同设备的一些特定操作的
        /// </summary>
        /// <param name="DevType">设备类型号</param>
        /// <param name="DevIndex">设备索引号</param>
        /// <param name="CANIndex">第几路CAN。即对应卡的CAN通道号，CAN0为0，CAN1为1，以此类推</param>
        /// <param name="RefType">参数类型</param>
        /// <param name="pData">用来存储参数有关数据缓冲区地址首指针</param>
        /// <returns>为1表示操作成功，0表示操作失败</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_SetReference(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex, UInt32 RefType, ref UInt32 pData);

        /// <summary>
        /// 启动CAN卡的某一个CAN通道。有多个CAN通道时，需要多次调用
        /// </summary>
        /// <param name="DevType">设备类型号</param>
        /// <param name="DevIndex">设备索引号</param>
        /// <param name="CANIndex">第几路CAN。即对应卡的CAN通道号，CAN0为0，CAN1为1，以此类推</param>
        /// <returns>为1表示操作成功，0表示操作失败</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_StartCAN(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex);

        /// <summary>
        /// 复位CAN。
        /// 主要用与VCI_StartCAN配合使用，无需再初始化，即可恢复CAN卡的正常状态。比如当CAN卡进入总线关闭状态时，可以调用这个函数
        /// </summary>
        /// <param name="DevType">设备类型号</param>
        /// <param name="DevIndex">设备索引号</param>
        /// <param name="CANIndex">第几路CAN。即对应卡的CAN通道号，CAN0为0，CAN1为1，以此类推</param>
        /// <returns>为1表示操作成功，0表示操作失败</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_ResetCAN(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex);

        /// <summary>
        /// 获取指定CAN通道的接收缓冲区中，接收到但尚未被读取的帧数量。
        /// 主要用途是配合VCI_Receive使用，即缓冲区有数据，再接收。用户无需一直调用VCI_Receive
        /// </summary>
        /// <param name="DevType">设备类型号</param>
        /// <param name="DevIndex">设备索引号</param>
        /// <param name="CANIndex">第几路CAN。即对应卡的CAN通道号，CAN0为0，CAN1为1，以此类推</param>
        /// <returns>返回尚未被读取的帧数</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_GetReceiveNum(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex);

        /// <summary>
        /// 清空指定CAN通道的缓冲区。
        /// 主要用于需要清除接收缓冲区数据的情况
        /// </summary>
        /// <param name="DevType">设备类型号</param>
        /// <param name="DevIndex">设备索引号</param>
        /// <param name="CANIndex">第几路CAN。即对应卡的CAN通道号，CAN0为0，CAN1为1，以此类推</param>
        /// <returns>为1表示操作成功，0表示操作失败</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_ClearBuffer(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex);

        /// <summary>
        /// 发送函数
        /// </summary>
        /// <param name="DevType">设备类型号</param>
        /// <param name="DevIndex">设备索引号</param>
        /// <param name="CANIndex">第几路CAN。即对应卡的CAN通道号，CAN0为0，CAN1为1，以此类推</param>
        /// <param name="pSend">要发送的帧结构体VCI_CAN_OBJ数组的首指针</param>
        /// <param name="Len">要发送的帧结构体数组的长度，即发送的帧数量</param>
        /// <returns>返回实际发送成功的帧数</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_Transmit(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex, IntPtr pSend, UInt32 Len);


        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);
        /// <summary>
        /// 接收函数。
        /// 此函数从指定的设备CAN通道的接收缓冲区中读取数据
        /// </summary>
        /// <param name="DevType">设备类型号</param>
        /// <param name="DevIndex">设备索引号</param>
        /// <param name="CANIndex">第几路CAN。即对应卡的CAN通道号，CAN0为0，CAN1为1，以此类推</param>
        /// <param name="pReceive">用来接收的帧结构体VCI_CAN_OBJ数组的首指针</param>
        /// <param name="Len">用来接收的帧结构体数组的长度（本次接收的最大帧数，实际返回值小于等于这个值）</param>
        /// <param name="WaitTime">缓冲区无数据，函数阻塞等待时间，以毫秒为单位。若为-1则表示无超时，一直等待</param>
        /// <returns>返回实际读取到的帧数。如果返回值为0xFFFFFFFF，则表示读取数据失败，有错误发生，请调用VCI_ReadErrInfo函数来获取错误码</returns>
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_Receive(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex, IntPtr pReceive, UInt32 Len, Int32 WaitTime);

        #endregion
    }
}
