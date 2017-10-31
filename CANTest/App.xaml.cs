using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using System.Threading;
using System.Globalization;

namespace CANTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppStartup_UpdateThemeName(object sender, StartupEventArgs e)
        {
            DevExpress.Xpf.Core.ApplicationThemeHelper.UpdateApplicationThemeName();
        }
        public static string Language { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            GetLanguage();
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {           
            SaveLanguage();
            //关闭所有线程，即关闭此进程
            System.Environment.Exit(0);

            //MessageBoxManager.Unregister();
        }

        #region Method
        /// <summary>
        /// 开机启动默认的语言
        /// </summary>
        private void GetLanguage()
        {
            Language = string.Empty;
            try
            {
                Language = CANTest.Properties.Settings.Default.Language.Trim();
            }
            catch (Exception)
            {
            }
            Language = string.IsNullOrEmpty(Language) ? "en-US" : Language;

            //update Language
            UpdateLanguage();
        }
        /// <summary>
        /// 保存语言设置
        /// </summary>
        private void SaveLanguage()
        {
            try
            {
                CANTest.Properties.Settings.Default.Language = Language;
                CANTest.Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 更换语言包
        /// </summary>
        public static void UpdateLanguage()
        {
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedLanguage = string.Format(@"Language\StringResource.{0}.xaml", Language);
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedLanguage));
            if (resourceDictionary == null)
            {
                requestedLanguage = @"Language\StringResource.en-US.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedLanguage));
            }
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
        #endregion
    }
}
