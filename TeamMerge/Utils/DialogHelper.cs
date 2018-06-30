using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TeamMerge.Utils
{
    public static class DialogHelper
    {
        public static Window CreateDialog<TDialog, TViewModel>(TDialog dialog, TViewModel viewModel)
            where TDialog : UserControl
            where TViewModel : class
        {
            dialog.DataContext = viewModel;

            return new Window
            {
                Content = dialog,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
        }
    }
}
