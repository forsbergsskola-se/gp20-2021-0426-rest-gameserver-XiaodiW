﻿using System;
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
                
                ILameScooterRental rental = args[1].ToLower() switch {
                    "offline" => new OfflineLameScooterRental(),
                    "deprecated" => new DeprecatedLameScooterRental(),
                    _ => null
                };
                if(rental == null) return;
                
                var result = rental.GetScooterCountInStation(args[0]).Result;
                Console.WriteLine($"{rental.Method}: The available bike count in Station {args[0]} is {result}");

            }
            catch(ArgumentException e) {
                Console.WriteLine("Please Type in a valid StationName");
                throw;
            }
        }
    }
}
