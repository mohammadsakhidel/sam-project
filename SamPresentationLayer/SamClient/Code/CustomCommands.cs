using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamClient.Code
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand Maximize = new RoutedUICommand(
                                "Maximize", "Maximize", typeof(CustomCommands),
                                new InputGestureCollection());
    }
}
