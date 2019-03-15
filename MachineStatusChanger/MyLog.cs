using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineStatusChanger
{
    public static class MyLog
    {
        public static void WriteTraceLog(String msg)
        {
            WriteTraceLog(msg, null);
        }

        public static void WriteTraceLog(String msg, Exception ex)
        {
            try
            {
                DateTime dt = DateTime.Now;
                String logFolder = System.AppDomain.CurrentDomain.BaseDirectory + "Log";

                System.IO.Directory.CreateDirectory(logFolder);

                String logFile = logFolder + "\\Log" + dt.ToString("dd") + ".log";

                //  翌日分のログファイル削除（1ヶ月分のログファイルしか保存しないようにするため）
                String logNext = logFolder + "\\Log" + dt.AddDays(1).ToString("dd") + ".log";
                System.IO.File.Delete(logNext);

                String logStr;
                logStr = dt.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + msg;
                if (ex != null)
                {
                    logStr = logStr + "\n" + ex.ToString();
                }

                System.IO.StreamWriter sw = null;
                try
                {
                    sw = new System.IO.StreamWriter(logFile, true, System.Text.Encoding.GetEncoding("Shift-JIS"));
                    sw.WriteLine(logStr);
                }
                catch
                {
                }
                finally
                {
                    if (sw != null) sw.Close();
                }
            }
            catch{}
        }
    }
}
