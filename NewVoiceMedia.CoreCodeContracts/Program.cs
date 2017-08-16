using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NewVoiceMedia.CoreCodeContracts
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            __ContractsRuntime.Requires(true == false, null, "true == false");
            Console.WriteLine("Hello World!");
        }
    }
}
