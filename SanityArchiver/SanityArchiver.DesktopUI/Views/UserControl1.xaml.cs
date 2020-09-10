using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SanityArchiver.Application.Models
{
    public partial class MyUserControl : UserControl
    {
        public MyUserControl()
        {
            InitializeComponent();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Clipboard operation occured!");
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
        }
    }
}
