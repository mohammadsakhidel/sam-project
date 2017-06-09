﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SamUxLib.Code.Utils
{
    public class ExceptionManager
    {
        public static void Handle(Exception ex)
        {
            Dispatcher.CurrentDispatcher.Invoke(() => {
                UxUtil.ShowError($"{ex.GetType().FullName}:{Environment.NewLine}{ex.Message}");
            });
        }
    }
}
