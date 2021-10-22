using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;

namespace ShopBridge.Services.Common
{
    public class Logger
    {
        private static Logger _Logger = null;
        private static Object _classLock = typeof(Logger);
        private static int _LogLevel = 1;
        private Logger()
        {
        }

        public static Logger Instance
        {
            get
            {
                //lock object to make it thread safe  
                lock (_classLock)
                {
                    if (_Logger == null)
                    {
                        _Logger = new Logger();
                    }
                }
                return _Logger;
            }
        }
        
        public void LogError(Exception exception, string title = "", int tryCount = 0)
        {
            try
            {
                // LOG ERROR IN DB FROM HERE....
            }
            catch (Exception ex)
            {
                if (tryCount == 0)
                {
                    Logger.Instance.LogError(ex, tryCount: 1);
                }
                Console.WriteLine(ex.Message, new object[0]);
            }
        }

    }
}
