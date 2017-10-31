using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace CANTest
{
    /// <summary>
    /// 具体的ZLG接口卡类型定义
    /// </summary>
    public enum ZLGType {
        VCI_PCI5121 = 1,
        VCI_PCI9810 = 2,
        VCI_USBCAN1 = 3,
        VCI_USBCAN2 = 4,
        VCI_USBCAN2A = 4,
        VCI_PCI9820 = 5,
        VCI_CAN232 = 6,
        VCI_PCI5110 = 7,
        VCI_CANLITE = 8,
        VCI_ISA9620 = 9,
        VCI_ISA5420 = 10,
        VCI_PC104CAN = 11,
        VCI_CANETUDP = 12,
        VCI_CANETE = 12,
        VCI_DNP9810 = 13,
        VCI_PCI9840 = 14,
        VCI_PC104CAN2 = 15,
        VCI_PCI9820I = 16,
        VCI_CANETTCP = 17,
        VCI_PEC9920 = 18,
        VCI_PCI5010U = 19,
        VCI_USBCAN_E_U = 20,
        VCI_USBCAN_2E_U = 21,
        VCI_PCI5020U = 22,
        VCI_EG20T_CAN = 23,
        VCI_PCIE9221 = 24
    };

    /// <summary>
    /// 波特率定义
    /// </summary>
    public enum BaudRate { _5Kbps = 5, _10Kbps = 10, _20Kbps = 20, _40Kbps = 40,
    _50Kbps = 50, _80Kbps = 80, _100Kbps = 100, _125Kbps = 125, _200Kbps = 200,
    _250Kbps = 250, _400Kbps = 400, _500Kbps = 500, _666Kbps = 666, _800Kbps = 800, _1000Kbps = 1000
    };

    /// <summary>
    /// 定义该类库支持访问的ZLG接口卡的类型
    /// </summary>
    public class Providier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private List<string> _arrdevPattern = new List<string> { };
        /// <summary>
        /// 模式下拉框绑定源
        /// </summary>
        public List<string> _ArrdevPattern
        {
            get { return _arrdevPattern; }
            set
            {
                _arrdevPattern = value;
                OnPropertyChanged(new PropertyChangedEventArgs("_ArrdevPattern"));
            }
        }

        private string[] _arrdevBaudrate;
        /// <summary>
        /// 模式下拉框绑定源
        /// </summary>
        public string[] _ArrdevBaudrate
        {
            get { return _arrdevBaudrate; }
            set
            {
                _arrdevBaudrate = value;
                OnPropertyChanged(new PropertyChangedEventArgs("_ArrdevBaudrate"));
            }
        }
        public Providier() 
        {
            _arrdevPattern.Add((string)Application.Current.Resources["teNormal"]);
            _arrdevPattern.Add((string)Application.Current.Resources["teListenonly"]);

             _arrdevBaudrate = new string[] {"5Kbps","10Kbps","20Kbps","40Kbps","50Kbps","80Kbps","100Kbps",
                "125Kbps","200Kbps","250Kbps","400Kbps","500Kbps","666Kbps","800Kbps","1000Kbps" };

        }
    }
}
