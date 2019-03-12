using System;
using System.Collections.Generic;
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
        }

        private void Dbaccess(string machineNo)
        {
            OdbcConnection con = new OdbcConnection("Dsn=TestDataSource;id=sa;Pwd=3141592;DataBase=eqstatdb;");
            con.Open();
            try
            {
                string q = "select MACHINE_NAME from Status where MACHINE_NO='" + machineNo + "'";
                OdbcCommand oc = new OdbcCommand(q, con);
                OdbcDataReader odr = oc.ExecuteReader();
                MachineNameLabel.Content = "Machine No.異常";
                while (odr.Read() == true)
                {
                    MachineNameLabel.Content = ((string) odr[0]).Trim();
                }
                odr.Close();
            }
            finally
            {
                con.Close();
            }
        }
    }
}
