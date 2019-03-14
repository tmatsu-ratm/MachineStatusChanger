using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
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
                            ClearMachineName();
                            break;
                        case "BS":
                            if (MachineNoLabel.Content.ToString().Length > 1)
                            {
                                MachineNoLabel.Content = MachineNoLabel.Content.ToString()
                                    .Remove(MachineNoLabel.Content.ToString().Length - 1);
                            }
                            ClearMachineName();
                            break;
                        default:
                            MachineNoLabel.Content += inKey;
                            if (MachineNoLabel.Content.ToString().Length == 6)
                            {
                                Dbaccess(MachineNoLabel.Content.ToString());
//                                MachineNoLabel.Content = "M";
                            }
                            else
                            {
                                ClearMachineName();
                            }
                            break;
                    }
                };
            }
        }

        private void ClearMachineName()
        {
            MachineNameLabel.Content = "";
            ChangeStateButton(false);
        }

        private void Dbaccess(string machineNo)
        {
            OdbcDbIf db = new OdbcDbIf();
            DataTable tb;
            db.Connect("TestDataSource", "eqstatdb", "sa", "3141592", -1);
            string q = "select MACHINE_NAME,MACHINE_STR from Status where MACHINE_NO='" + machineNo + "'";
            tb = db.ExecuteSql(q, -1);
            if (tb.Rows.Count > 0)
            {
                MachineNameLabel.Content = tb.Rows[0]["MACHINE_NAME"].ToString().Trim();
                int state = (int)(tb.Rows[0]["MACHINE_STR"]);
                ChangeStateButton(true, state);
            }
            else
            {
                MachineNameLabel.Content = "Machine No.異常";
            }

            db.Disconnect();

        }

        private void UpdateState(string machineNo, int state)
        {
            OdbcDbIf db = new OdbcDbIf();
            db.Connect("TestDataSource", "eqstatdb", "sa", "3141592", -1);
            db.BeginTransaction();
            string q = "update Status set MACHINE_STR=" + state + " where MACHINE_NO='" + machineNo + "'";
            db.ExecuteSql(q, -1);
            db.CommitTransaction();
            db.Disconnect();

            Dbaccess(machineNo);
        }
        
        private void ChangeStateButton(bool isEnable, int currentState = 0)
        {
            S1Button.IsEnabled = isEnable;
            S2Button.IsEnabled = isEnable;
            S3Button.IsEnabled = isEnable;
            S4Button.IsEnabled = isEnable;
            S5Button.IsEnabled = isEnable;
            S6Button.IsEnabled = isEnable;

            switch (currentState)
            {
                case 1:
                    S1Button.IsEnabled = false;
                    break;
                case 2:
                    S2Button.IsEnabled = false;
                    break;
                case 3:
                    S3Button.IsEnabled = false;
                    break;
                case 4:
                    S4Button.IsEnabled = false;
                    break;
                case 5:
                    S5Button.IsEnabled = false;
                    break;
                case 6:
                    S6Button.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }

        private void S1Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(MachineNoLabel.Content.ToString(), 1);
        }

        private void S2Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(MachineNoLabel.Content.ToString(), 2);
        }

        private void S3Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(MachineNoLabel.Content.ToString(), 3);
        }

        private void S4Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(MachineNoLabel.Content.ToString(), 4);
        }

        private void S5Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(MachineNoLabel.Content.ToString(), 5);
        }

        private void S6Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(MachineNoLabel.Content.ToString(), 6);
        }
    }
}
