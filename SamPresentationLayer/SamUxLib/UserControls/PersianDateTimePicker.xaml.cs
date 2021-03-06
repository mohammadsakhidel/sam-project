using RamancoLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamUxLib.UserControls
{
    public partial class PersianDateTimePicker : UserControl
    {
        public PersianDateTimePicker()
        {
            InitializeComponent();
        }

        #region Methods:
        public bool IsTimeSpecicied()
        {
            return Regex.IsMatch(tbTime.Text, @"\d{2}:\d{2}");
        }
        public bool IsCompleted()
        {
            var selectedDate = persianDatePicker.SelectedDate;
            if (selectedDate.HasValue && (!IncludeTime || IsTimeSpecicied()))
                return true;

            return false;
        }
        public DateTime? GetMiladyDateTime()
        {
            var selectedDate = persianDatePicker.SelectedDate;
            if (!selectedDate.HasValue)
                return null;

            if (!IsTimeSpecicied())
                return new DateTime(selectedDate.Value.Year, selectedDate.Value.Month, selectedDate.Value.Day);
            else
            {
                var time = tbTime.Text.Split(':');
                return new DateTime(selectedDate.Value.Year, selectedDate.Value.Month, selectedDate.Value.Day,
                                    Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
            }
        }
        public void SetMiladyDate(DateTime miladyDate)
        {
            var shamsi = DateTimeUtils.ToShamsi(miladyDate.Year, miladyDate.Month, miladyDate.Day);
            persianDatePicker.SelectedDate = new DateTime(shamsi.Year, shamsi.Month, shamsi.Day);

            tbTime.Text = miladyDate.ToString("HH:mm");
        }
        #endregion

        #region Properties:
        bool includeTime = true;
        public bool IncludeTime
        {
            get
            {
                return includeTime;
            }
            set
            {
                includeTime = value;

                if (!includeTime)
                {
                    tbTime.Visibility = Visibility.Collapsed;
                }
                else
                {
                    tbTime.Visibility = Visibility.Visible;
                }
            }
        }
        #endregion
    }
}
