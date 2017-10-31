using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;


namespace CANTest
{
    /// <summary>
    /// Interaction logic for SelectCAN.xaml
    /// </summary>
    public partial class SelectCAN : DXWindow
    {
        ZLGEntity _conn = new ZLGEntity();
        UInt32[] m_arrdevtype = new UInt32[30];
        public bool IsSelected = false;

        public SelectCAN(ZLGEntity conn)
        {
            _conn = conn;
            InitializeComponent();
            selectGrid.DataContext = _conn;
        }

        private void SelectCAN_Loaded(object sender, RoutedEventArgs e)
        {
            AddItemsToType();
        }

        #region 加载类型下拉框
        /// <summary>
        /// 添加CAN设备类型到列表中
        /// </summary>
        private void AddItemsToType()
        {
            Int32 curindex = 0;
            comboBox_DevType.Items.Clear();
            //1个通道号
            curindex = comboBox_DevType.Items.Add("VCI_USBCAN_E_U");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_USBCAN_E_U;

            curindex = comboBox_DevType.Items.Add("VCI_PCI5010U");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_PCI5010U;

            curindex = comboBox_DevType.Items.Add("VCI_PCI9810");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_PCI9810;

            curindex = comboBox_DevType.Items.Add("VCI_USBCAN1(I+)");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_USBCAN1;

            curindex = comboBox_DevType.Items.Add("VCI_PCI5110");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_PCI5110;

            curindex = comboBox_DevType.Items.Add("VCI_CANLITE");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_CANLITE;


            curindex = comboBox_DevType.Items.Add("VCI_PC104CAN");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_PC104CAN;

            curindex = comboBox_DevType.Items.Add("VCI_DNP9810");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_DNP9810;



            //2个通道号
            curindex = comboBox_DevType.Items.Add("VCI_USBCAN_2E_U");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_USBCAN_2E_U;

            curindex = comboBox_DevType.Items.Add("VCI_PCI5020U");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_PCI5020U;

            curindex = comboBox_DevType.Items.Add("VCI_PCI5121");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_PCI5121;

            curindex = comboBox_DevType.Items.Add("VCI_USBCAN2(II+)");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_USBCAN2;

            curindex = comboBox_DevType.Items.Add("VCI_USBCAN2A");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_USBCAN2A;

            curindex = comboBox_DevType.Items.Add("VCI_PCI9820");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_PCI9820;

            curindex = comboBox_DevType.Items.Add("VCI_ISA9620");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_ISA9620;

            curindex = comboBox_DevType.Items.Add("VCI_ISA5420");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_ISA5420;

            curindex = comboBox_DevType.Items.Add("VCI_PC104CAN2");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_PC104CAN2;

            curindex = comboBox_DevType.Items.Add("VCI_PCI9820I");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_PCI9820I;

            curindex = comboBox_DevType.Items.Add("VCI_PEC9920");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_PEC9920;

            curindex = comboBox_DevType.Items.Add("VCI_PCIE9221");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_PCIE9221;


            //4个通道号
            curindex = comboBox_DevType.Items.Add("VCI_PCI9840");
            m_arrdevtype[curindex] = (uint)ZLGType.VCI_PCI9840;
        }
        #endregion

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_DevIndex.SelectedIndex == -1 || comboBox_DevChannel.SelectedIndex == -1)
            {
                DXMessageBox.Show((string)Application.Current.Resources["tePromptText1"], (string)FindResource("tePrompt"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                IsSelected = true;
                _conn.DevType = m_arrdevtype[_conn.TypeIndex];
                _conn.ConnObject = comboBox_DevType.SelectedItem.ToString();
                _conn.RunDevice();
                //保存上次打开的CAN盒类型
                CANTest.Properties.Settings.Default.TypeIndex = _conn.TypeIndex;
                CANTest.Properties.Settings.Default.Save();

                //保存设置


                this.Close();
            }
        }

        private void Canel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
