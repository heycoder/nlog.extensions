using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace HeyCoder.NLog.Extensions.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "HeyCoder.NLog.Extensions.Debug";
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            var logger1 = LogManager.GetLogger("test");
            logger1.Info("test:{0}", DateTime.Now);
            //for (int i = 0; i < 10; i++)
            //{
            //    logger1.Error("这是一个测试Error:{0}", i);
            //}

            Console.WriteLine("End:{0}", DateTime.Now);
            Console.ReadKey();
        }
    }
}
