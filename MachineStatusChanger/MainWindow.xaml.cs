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
using System.Configuration;

namespace MachineStatusChanger
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string MachineNoRowName = "MACHI_NAME";
        private static readonly string MachineNameRowName = "MACHINE_NAME";
        private static readonly string MachineSateRowName = "MACHINE_FLG2";
        private static readonly int MachineNoLength = 6;
        private string dsn;
        private string dbn;
        private string uid;
        private string pas;
        private string tbl;
        private int pfLength;
        private string prefixString;

        public MainWindow()
        {
            InitializeComponent();

            SetParameter();

            InitEvents();
        }

        private void SetParameter()
        {
            dsn = ConfigurationManager.AppSettings["DSN"];
            dbn = ConfigurationManager.AppSettings["Database"];
            uid = ConfigurationManager.AppSettings["User"];
            pas = ConfigurationManager.AppSettings["Password"];
            tbl = ConfigurationManager.AppSettings["Table"];
            string fabName = ConfigurationManager.AppSettings["Fab"];
            if (fabName == "L")
            {
                pfLength = 2;
                prefixString = "ML";
            }
            else if (fabName == "B")
            {
                pfLength = 2;
                prefixString = "MB";
            }
            else
            {
                pfLength = 1;
                prefixString = "M";
            }

            MachineNoLabel.Content = prefixString;
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
                            MachineNoLabel.Content = prefixString;
                            ClearMachineName();
                            break;
                        case "BS":
                            if (MachineNoLabel.Content.ToString().Length > pfLength)
                            {
                                MachineNoLabel.Content = MachineNoLabel.Content.ToString()
                                    .Remove(MachineNoLabel.Content.ToString().Length - 1);
                            }
                            ClearMachineName();
                            break;
                        default:
                            if (MachineNoLabel.Content.ToString().Length >= MachineNoLength)
                            {
                                break;
                            }
                            MachineNoLabel.Content += inKey;
                            if (MachineNoLabel.Content.ToString().Length == MachineNoLength)
                            {
                                Dbaccess(MachineNoLabel.Content.ToString());
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
            db.Connect( dsn, dbn, uid , pas, -1);
        
            string q = $"select {MachineNameRowName},{MachineSateRowName} from {tbl} where {MachineNoRowName}='{machineNo}'";
  
            tb = db.ExecuteSql(q, -1);
            if (tb.Rows.Count > 0)
            {
                MachineNameLabel.Content = tb.Rows[0][MachineNameRowName].ToString().Trim();
                int state;
                if (int.TryParse(tb.Rows[0][MachineSateRowName].ToString(), out state))
                {

                }
                else
                {
                    state = 0;
                }
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
            db.Connect(dsn, dbn, uid, pas, -1);
            db.BeginTransaction();
            string q = $"update {tbl} set {MachineSateRowName}='{state.ToString()}' where {MachineNoRowName}='{machineNo}'";

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

            if (isEnable)
            {
                S1Button.Background = Brushes.WhiteSmoke;
                S2Button.Background = Brushes.WhiteSmoke;
                S3Button.Background = Brushes.WhiteSmoke;
                S4Button.Background = Brushes.WhiteSmoke;
                S5Button.Background = Brushes.WhiteSmoke;
                S6Button.Background = Brushes.WhiteSmoke;
            }

            switch (currentState)
            {
                case 1:
//                    S1Button.IsEnabled = false;
                    S1Button.Background = Brushes.GreenYellow;
                    break;
                case 2:
                    //                    S2Button.IsEnabled = false;
                    S2Button.Background = Brushes.Purple;
                    break;
                case 3:
                    //                    S3Button.IsEnabled = false;
                    S3Button.Background = Brushes.Red;
                    break;
                case 4:
                    //                    S4Button.IsEnabled = false;
                    S4Button.Background = Brushes.Aqua;
                    break;
                case 5:
                    //                    S5Button.IsEnabled = false;
                    S5Button.Background = Brushes.DarkGray;
                    break;
                case 6:
                    //                    S6Button.IsEnabled = false;
                    S6Button.Background = Brushes.DimGray;
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
