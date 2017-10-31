using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Core;
using System.Windows.Input;
using System.Windows;
using System.Data;

namespace CANTest
{
    public partial class MainWindow : DXWindow
    {
        #region 中英文切换
        private void ChooseLanguage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void ChooseLanguage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //英文
            if (e.Parameter.ToString() == "en")
            {
                App.Language = "en-US";
            }
            //中文
            else if (e.Parameter.ToString() == "zh")
            {
                App.Language = "zh-CN";
            }
            App.UpdateLanguage();
        }

        #endregion

        #region 选择设备
        private void ChooseDevice_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void ChooseDevice_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZLGEntity _conn = new ZLGEntity();
            SelectCAN _selectCAN = new SelectCAN(_conn);
            _selectCAN.ShowDialog();
            if (_selectCAN.IsSelected)
            {
                tabSource.Add(_conn);
                TabGroup.SelectedItem = _conn;
                if (!_conn.Flag)
                {
                    DXMessageBox.Show((string)Application.Current.Resources["tePromptText2"], (string)Application.Current.Resources["tePrompt"], MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }            
        }
        #endregion

        #region 操作设备
        private void OperateDevice_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void OperateDevice_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZLGEntity _conn = (ZLGEntity)TabGroup.SelectedItem;
            if (e.Parameter.ToString()=="Start")
            {
                _conn.RunDevice();
                if (!_conn.Flag)
                {
                    DXMessageBox.Show((string)Application.Current.Resources["tePromptText2"], (string)Application.Current.Resources["tePrompt"], MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (e.Parameter.ToString()=="Close")
            {                
                _conn.StopDevice();
                tabSource.Remove(_conn);
            }
            else
            {               
                _conn.StopDevice();
            }
            
        }
        #endregion

        #region 发送、停止发送、清空、导出数据
        private void DealData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void DealData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZLGEntity _conn = (ZLGEntity)TabGroup.SelectedItem;
            if (e.Parameter.ToString()=="send")
            {
                _conn.StartThread();
            }
            else if (e.Parameter.ToString() == "clear")
            {    
                _conn.dealDataTable(_conn.CANData.NewRow(), "clear");
            }
            else if (e.Parameter.ToString() == "stop")
            {
                _conn.AbortThread();
            }
            else if (e.Parameter.ToString() == "export")
            {
                _conn.dealDataTable(_conn.CANData.NewRow(), "export");
            }
        }
        #endregion

        #region 关机指令
        private void ExitApp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void ExitApp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 关机指令
        private void About_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void About_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            About _about = new About();
            _about.ShowDialog();
        }

        #endregion
    }
}
