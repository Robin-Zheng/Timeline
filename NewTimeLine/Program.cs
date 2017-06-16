using System;
using Starcounter;

namespace NewTimeLine
{
    class Program
    {
        static void Main()
        {
            MainHandlers handlers = new MainHandlers();

            handlers.Register();
        }
    }
}