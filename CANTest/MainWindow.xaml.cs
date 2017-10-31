using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Core;

namespace CANTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DXWindow
    {
        public static  ObservableCollection<ZLGEntity> tabSource = new ObservableCollection<ZLGEntity> { };
        public MainWindow()
        {        
            //让GridControl支持多线程操作
            DXGridDataController.DisableThreadingProblemsDetection = true;
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //设置软件工作集大小
            System.Diagnostics.Process.GetCurrentProcess().MaxWorkingSet = (IntPtr)750000;
            biDate.EditValue = DateTime.Now;
            //skinList.ItemsSource = Theme.Themes;
            TabGroup.ItemsSource = tabSource;
        }

        /* 皮肤选择
        private void biSkinChoose_EditValueChanged(object sender, RoutedEventArgs e)
        {
            if (biSkinChoose.EditValue == null)
            {
                return;
            }
            ThemeManager.ApplicationThemeName = biSkinChoose.EditValue.ToString();// biSkinChoose.EditValue.ToString();
            this.UpdateLayout();
        }
        */

        private void TabGroup_TabHiding(object sender, TabControlTabHidingEventArgs e)
        {
            ZLGEntity _conn = (ZLGEntity)TabGroup.SelectedItem;
            _conn.StopDevice();
            tabSource.Remove(_conn);

            //回收垃圾
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            foreach (var item in TabGroup.Items)
            {
                ZLGEntity _conn = (ZLGEntity)item;
                _conn.StopDevice();
            }
        }
    }
}
