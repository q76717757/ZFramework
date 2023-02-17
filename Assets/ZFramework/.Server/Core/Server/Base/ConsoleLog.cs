using System;

namespace ZFramework
{
    public class ConsoleLog : ILog
    {
        public void Info(object obj)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(obj.ToString());
        }
        public void Warning(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(obj.ToString());
        }

        public void Error(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(obj.ToString());
        }
    }
}
