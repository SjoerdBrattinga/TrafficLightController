using System;

namespace ControllerSDM
{
    internal class Program
    {                                              
        private static void Main(string[] args)
        {
            var controller = new Controller();
            controller.Start();
            //Console.WriteLine("Controller started");
            Console.ReadLine();
            controller.CloseConnection();
        }
    }
}
