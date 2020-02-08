using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace TentaDatabas
{
    class MenuClass
    {
        public static void MainMenu()
        {
            bool menu = true;
            while (menu)
            {
                Console.Clear();
                Console.WriteLine("-----MENU OPTIONS----");
                Console.WriteLine("1. Park Vehicle");
                Console.WriteLine("2. Fetch Vehicle");
                Console.WriteLine("3. Fetch Vehicle for free (incase damaged etc)");
                Console.WriteLine("4. Search for Parked Vehicle");
                Console.WriteLine("5. Move Vehicle to desired spot");
                Console.WriteLine("6. Optimize Parkinglot");
                Console.WriteLine("7. Show Vehicles being parked for longer than 48 hrs");
                Console.WriteLine("8. Show complete history");
                Console.WriteLine("9. Show history for specific date");
                Console.WriteLine("10. Show history for specific time period");
                Console.WriteLine("11. Print ParkingLot");
                Console.WriteLine("12. Exit Parkinglot");

                int userInput = InputChecker();
                switch (userInput)
                {
                    case 1:
                        var vehicle = GetUsersRegAndTypeID();
                        SQLMethods.ParkVehicle(vehicle);
                        break;
                    case 2:
                        SQLMethods.RemoveVehicle(true); //Skickar med true för att man ska betala,
                        break;
                    case 3:
                        SQLMethods.RemoveVehicle(false); //Skickar med false för att man ska hämta ut gratis. SÖK EFTER FORDON
                        break;
                    case 4:
                        SQLMethods.SearchForVehicle();
                        break;
                    case 5:
                        SQLMethods.MoveVehicle();
                        break;
                    case 6:
                        SQLMethods.OptimizeParking();
                        break;
                    case 7:
                        SQLMethods.PrintVehiclesParkedOver48Hrs();
                        break;
                    case 8:
                        SQLMethods.ShowFullHistory();
                        break;
                    case 9:
                        SQLMethods.HistoryForSpecificDate();
                        break;
                    case 10:
                        SQLMethods.HistoryForTimeSpan();
                        break;
                    case 11:
                        SQLMethods.PrintParkingLot();
                        break;
                    case 12:
                        menu = false;
                        break;

                }
                Console.ReadKey();
            }

        }
        /// <summary>
        /// Hämtar regnmr och VehicleTypeId av användare med lite veriefiering.
        /// </summary>
        /// <returns>användares fordon</returns>
        private static Vehicle GetUsersRegAndTypeID()
        {
            Console.Clear();
            Vehicle userVehicle = new Vehicle();
            char[] forbiddenChars = @"? !@#¤%&/()=+`´'¨-*:;|\<>_\".ToCharArray();
            Console.WriteLine("Type your Registrationnumber, minimum length 3 max 10");
            string regNmbr = Console.ReadLine();

            if (regNmbr.Length >= 3 && regNmbr.Length <= 10)
            {
                bool match = false;
                for (int i = 0; i < forbiddenChars.Length; i++)
                {
                    if (regNmbr.Contains(forbiddenChars[i]))
                    {
                        match = true;
                    }
                }

                if (match)
                {
                    Console.WriteLine("Cannot have special chars as regnumber");
                    Console.ReadKey();
                    MainMenu();
                }
                else
                {
                    try
                    {
                        userVehicle.RegistrationNummber = regNmbr;
                    }
                    catch
                    {
                        Console.WriteLine("För lång eller kort regnmr");
                        Console.ReadKey();
                        MainMenu();
                    }

                }
                Console.WriteLine("What type to park? 1:MC   2:CAR");
                int userInput = InputChecker();
                switch (userInput)
                {
                    case 1:
                        userVehicle.VehicleTypeID = userInput;
                        break;
                    case 2:
                        userVehicle.VehicleTypeID = userInput;
                        break;
                    default:
                        Console.WriteLine("Select Valid Type");
                        Console.ReadKey();
                        MainMenu();
                        break;
                }

            }
            else
            {
                Console.WriteLine("RegistrationNumber length must be between 3-10");
            }
            return userVehicle;
        }

        public static int InputChecker()
        {
            string input;
            bool succes = false;
            int userInput = 0;
            while (succes != true)
            {
                input = Console.ReadLine();
                succes = int.TryParse(input, out userInput);
                if (succes == false)
                {
                    Console.WriteLine("Wrong input, try again. Need integer.");
                }

            }
            return userInput;
        }
       

    }
}
