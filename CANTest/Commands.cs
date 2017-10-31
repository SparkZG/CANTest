using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CANTest
{
    class Commands
    {
        /// <summary>
        /// 语言选择命令
        /// </summary>
        private static RoutedUICommand chooseLanguage = new RoutedUICommand("ChooseLanguage", "ChooseLanguage", typeof(Commands));
        public static RoutedUICommand ChooseLanguage
        {
            get { return chooseLanguage; }
        }

        /// <summary>
        /// 设备选择命令
        /// </summary>
        private static RoutedUICommand chooseDevice = new RoutedUICommand("ChooseDevice", "ChooseDevice", typeof(Commands));
        public static RoutedUICommand ChooseDevice
        {
            get { return chooseDevice; }
        }

        /// <summary>
        /// 操作设备命令
        /// </summary>
        private static RoutedUICommand operateDevice = new RoutedUICommand("OperateDevice", "OperateDevice", typeof(Commands));
        public static RoutedUICommand OperateDevice
        {
            get { return operateDevice; }
        }

        /// <summary>
        /// 处理数据命令
        /// </summary>
        private static RoutedUICommand dealData = new RoutedUICommand("DealData", "DealData", typeof(Commands));
        public static RoutedUICommand DealData
        {
            get { return dealData; }
        }

        /// <summary>
        /// 关闭软件命令
        /// </summary>
        private static RoutedUICommand exitApp = new RoutedUICommand("ExitApp", "ExitApp", typeof(Commands));
        public static RoutedUICommand ExitApp
        {
            get { return exitApp; }
        }

        /// <summary>
        /// 关于
        /// </summary>
        private static RoutedUICommand about = new RoutedUICommand("About", "About", typeof(Commands));
        public static RoutedUICommand About
        {
            get { return about; }
        }
    }
}
