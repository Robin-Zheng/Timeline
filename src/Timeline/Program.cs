using System;
using Starcounter;

namespace Timeline
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