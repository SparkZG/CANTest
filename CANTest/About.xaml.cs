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
using System.Deployment.Application;


namespace CANTest
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : DXWindow
    {
        ApplicationDeployment appd = ApplicationDeployment.CurrentDeployment;
        public About()
        {
            InitializeComponent();
        }

        private void web_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("IEXPLORE.EXE", "http://www.richpower-china.com");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {           

            // 取得版本号
            if (appd.CheckForUpdate())
            {
                Update.Visibility = Visibility.Visible;
            }
            else
            {
                Update.Visibility = Visibility.Hidden;
            }
            version.Text = appd.CurrentVersion.ToString();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if(DXMessageBox.Show((string)Application.Current.Resources["teReOk"], (string)Application.Current.Resources["tePrompt"], MessageBoxButton.OKCancel, MessageBoxImage.Question)==MessageBoxResult.OK)
            {
                appd.Update();
                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }            
        }
    }
}
