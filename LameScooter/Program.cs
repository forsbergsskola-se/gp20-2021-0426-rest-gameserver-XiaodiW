using System;
using System.Linq;
using System.Threading.Tasks;

namespace LameScooter
{
    class Program
    {
        private static void Main(string[] args) {
            // Console.WriteLine("The app is running with arguments as following");
            // for(var i = 0; i < args.Length; i++) Console.WriteLine($"{i}: {args[i]}");
            try {
                if(args[0].Any(char.IsDigit)) throw new ArgumentException();
                var validArgs = new string[] {"offline", "deprecated", "realtime"};
                if(!validArgs.Contains(args[1].ToLower()))throw new ArgumentException();

                switch(args[1].ToLower()) {
                    case "offline":
                        ILameScooterRental offlineData = new OfflineLameScooterRental();
                        var result = offlineData.GetScooterCountInStation(args[0]).Result;
                        Console.WriteLine($"Offiline: The available bike count in Station {args[0]} is {result}");
                        break;
                    case "deprecated":
                        ILameScooterRental deprecatedData = new DeprecatedLameScooterRental();
                        var result2 = deprecatedData.GetScooterCountInStation(args[0]).Result;
                        Console.WriteLine($"Deprecated: The available bike count in Station {args[0]} is {result2}");
                        break;
                }

            }
            catch(ArgumentException e) {
                Console.WriteLine("Please Type in a valid StationName");
                throw;
            }
        }
    }
}
