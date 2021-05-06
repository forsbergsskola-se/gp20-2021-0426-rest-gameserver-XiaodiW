using System;
using System.Linq;
using System.Threading.Tasks;

namespace LameScooter
{
    class Program
    {
        private static void Main(string[] args) {
            Console.WriteLine("The app is running with arguments as following");
            for(var i = 0; i < args.Length; i++) Console.WriteLine($"{i}: {args[i]}");
            try {
                if(args[0].Any(char.IsDigit)) throw new ArgumentException("Please Type in a valid StationName");
                ILameScooterRental offlineData = new OfflineLameScooterRental();
                var result = offlineData.GetScooterCountInStation(args[0]).Result;
                Console.WriteLine($"The available bike count in Station {args[0]} is {result}");
            }
            catch(ArgumentException e) {
                Console.WriteLine(e.ToString());
                throw;
            }
        }
    }
}
