﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Interpic.Alerts
{
    /// <summary>
    /// Interaction logic for ErrorAlert.xaml
    /// </summary>
    public partial class WarningAlert : Window
    {
        public bool Result { get; set; }

        public WarningAlert(string message, bool showCancelButton = false)
        {
            InitializeComponent();
            lbMessage.Text = message;
            btnCancel.Visibility = showCancelButton ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        public static WarningAlert Show(string message, bool showCancelButton = false)
        {
            WarningAlert alert = new WarningAlert(message, showCancelButton);
            alert.ShowDialog();
            return alert;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }
    }
}
