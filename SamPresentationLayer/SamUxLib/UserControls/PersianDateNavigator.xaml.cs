using RamancoLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
    public partial class PersianDateNavigator : UserControl
    {
        #region Fields:
        DateTime? _lastUpdatedDate;
        #endregion

        #region Ctors:
        public PersianDateNavigator()
        {
            InitializeComponent();
        }
        #endregion

        #region Events:
        public event EventHandler<DateChangedEventArgs> OnChange;
        #endregion

        #region Methods:
        public DateTime? GetMiladyDate()
        {
            var selectedDate = persianDatePicker.SelectedDate;
            if (selectedDate.HasValue)
            {
                return new DateTime(selectedDate.Value.Year, selectedDate.Value.Month, selectedDate.Value.Day);
            }
            else
            {
                return null;
            }
        }

        public void SetMiladyDate(DateTime miladyDate)
        {
            var shamsi = DateTimeUtils.ToShamsi(miladyDate.Year, miladyDate.Month, miladyDate.Day);
            persianDatePicker.SelectedDate = new DateTime(shamsi.Year, shamsi.Month, shamsi.Day);
        }
        #endregion

        #region Event Handlers:
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var current = GetMiladyDate();
                if (current.HasValue)
                {
                    SetMiladyDate(current.Value.AddDays(1));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var current = GetMiladyDate();
                if (current.HasValue)
                {
                    SetMiladyDate(current.Value.AddDays(-1));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void persianDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var newDate = GetMiladyDate();
            if (!_lastUpdatedDate.HasValue || (newDate.HasValue && newDate.Value.Date != _lastUpdatedDate.Value.Date))
            {
                _lastUpdatedDate = newDate;
                OnChange?.Invoke(sender, new DateChangedEventArgs() { NewDate = newDate.Value });
            }
        }
        #endregion
    }

    public class DateChangedEventArgs: EventArgs
    {
        public DateTime NewDate { get; set; }
    }
}
