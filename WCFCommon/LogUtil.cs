using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFCommon
{
    public class LogUtil
    {
        private static Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public static void Info(string message)
        {
            _log.Info(message);
        }

        public static void Debug(string message)
        {
            _log.Debug(message);
        }

        public static void Error(string message)
        {
            _log.Error(message);
        }

        public static void Error(Exception ex, string message)
        {
            StringBuilder msg = new StringBuilder();

            msg.Append(message + "：");

            while (ex != null)
            {
                msg.Append(ex.Message + "\r\n" + ex.StackTrace + "\r\n");
                ex = ex.InnerException;
            }

            _log.Error(msg.ToString().TrimEnd());
        }
    }
}
