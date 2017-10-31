using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace CANTest
{
    #region 转换成十六进制字符串以及其他转换
    public class HexToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string _str = "";
            if (parameter != null && value != null)
            {
                if (parameter.ToString() == "timer")
                {
                    _str = String.Format("{0:X2}", value);
                }
                else if (parameter.ToString() == "Acc")
                {
                    _str = String.Format("{0:X8}", value);// System.Convert.ToString(System.Convert.ToUInt32(value), 16).PadLeft(8, '0');
                }
                else if (parameter.ToString() == "baudRate")
                {
                    _str = (int)value + "Kbps";
                }
            }
            return _str;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value.ToString() != null)
            {
                if (parameter.ToString() == "timer")
                {
                    byte _byte = System.Convert.ToByte(value.ToString(), 16);
                    return _byte;
                }
                else if (parameter.ToString() == "Acc")
                {
                    UInt32 _uint32 = System.Convert.ToUInt32(value.ToString(), 16);
                    return _uint32;
                }
                else if (parameter.ToString() == "baudRate")
                {
                    BaudRate _baud = (BaudRate)System.Convert.ToInt32(value.ToString().Substring(0, value.ToString().Length - 4));
                    return _baud;
                }
            }
            return null;
        }
    }
    #endregion

    #region 字节转换成整型以及下拉框默认选择索引
    public class ByteToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int _int = 0;
            if (parameter != null)
            {
                if (parameter.ToString() == "index")
                {
                    return _int;
                }
            }
            if (value != null)
            {
                _int = System.Convert.ToUInt16(value);
            }
            return _int;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte state = System.Convert.ToByte(value);
            return state;
        }
    }
    #endregion

    #region 取反转换器
    public class ReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool state = !(bool)value;
            return state;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool state = !(bool)value;
            return state;
        }
    }
    #endregion

    #region 根据CAN类型选择索引绑定源
    public class TypeToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int _conn = System.Convert.ToInt16(value);
            int _indexNum = GetArrByType(_conn);
            List<uint> _arrdevIndex = new List<uint> { 0, 1, 2, 3 };
            //初始索引数为4个，根据当前已选择的通道和索引来返回索引的source
            for (int i = 0; i < 4; i++)
            {
                int _removeFlag = 0;
                foreach (var item in MainWindow.tabSource)
                {
                    if (item.TypeIndex == _conn && item.DevIndex == i)
                    {
                        _removeFlag++;
                        //若当前索引对应的通道号已选择完，则移除此索引
                        if (_indexNum == _removeFlag)
                        {
                            _arrdevIndex.Remove((uint)i);
                            break;
                        }
                    }
                }
            }
            return _arrdevIndex;
        }
        /// <summary>
        /// 获取可打开的通道数目
        /// </summary>
        private int GetArrByType(int _index)
        {
            if (_index >= 0 && _index < 8)
            {
                return 1;
            }
            else if (_index >= 8 && _index < 20)
            {
                return 2;
            }
            else
            {
                return 4;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region 根据CAN类型选择通道绑定源
    public class TypeToChannelConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int _type = System.Convert.ToInt16(values[0]);
            uint _index = System.Convert.ToUInt16(values[1]);
            List<uint> _channelNum = GetArrByType(_type);
            for (int i = 0; i < _channelNum.Count; i++)
            {
                foreach (var item in MainWindow.tabSource)
                {
                    if (item.TypeIndex == _type && item.DevIndex == _index && item.DevChannel == _channelNum[i])
                    {
                        _channelNum.Remove(_channelNum[i]);
                        break;
                    }
                }
            }
            return _channelNum;
        }

        private List<uint> GetArrByType(int _index)
        {
            if (_index >= 0 && _index < 8)
            {
                return new List<uint> { 0 }; ;
            }
            else if (_index >= 8 && _index < 20)
            {
                return new List<uint> { 0, 1 }; ;
            }
            else
            {
                return new List<uint> { 0, 1, 2, 3 };
            }
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region 发送，停止等按键使能convert
    public class FlagToIsEnableConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool _flag = false;
            if (parameter != null)
            {
                try
                {
                    if (parameter.ToString() == "send")
                    {
                        if ((bool)values[0]&&(bool)values[1])
                        {
                            _flag = true;
                        }
                    }
                    else
                    {
                        if ((bool)values[0] && !(bool)values[1])
                        {
                            _flag = true;
                        }
                    }
                }
                catch (Exception)
                {

                    //为什么在从TabControl中删除Item的时候回执行此Convert
                }

            }
            return _flag;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region 表格数据增加时让选中最后一行从而实现滚动条自动下拉
    public class RowToFocusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int _focusRow = (int)value;
            _focusRow--;
            return _focusRow;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
