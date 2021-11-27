﻿using System.Windows;
using System.Windows.Controls;

namespace TeamMergeBase.Utils
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