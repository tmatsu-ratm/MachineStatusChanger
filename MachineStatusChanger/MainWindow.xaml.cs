using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MachineStatusChanger
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitEvents();
        }

        private void InitEvents()
        {
            foreach (var ctrl in LogicalTreeHelper.GetChildren(InputGrid))
            {
                if (!(ctrl is Button))
                {
                    continue;
                }

                (ctrl as Button).Click += (sender, e) =>
                {
                    string inKey = (sender as Button).Content.ToString();
                    switch (inKey)
                    {
                        case "CLR":
                            MachineNoLabel.Content = "M";
                            break;
                        case "BS":
                            if (MachineNoLabel.Content.ToString().Length > 1)
                            {
                                MachineNoLabel.Content = MachineNoLabel.Content.ToString()
                                    .Remove(MachineNoLabel.Content.ToString().Length - 1);
                            }

                            break;
                        default:
                            MachineNoLabel.Content += inKey;
                            break;
                    }
                };
            }
        }
    }
}
