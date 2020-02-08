using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace TentaDatabas
{
    class SQLMethods
    {
        private static SqlConnection connection = new SqlConnection();
        private const string connectionString = @"Data Source=LAPTOP-Q58DHVN7;Initial Catalog=TentamenDatabas;Integrated Security=True";
        /// <summary>
        /// Skriver ut hela parkeringsplatsen
        /// </summary>
        public static void PrintParkingLot()
        {
            Console.Clear();
            connection.ConnectionString = connectionString;
            string parkinglot = "";
            using (connection)
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "dbo.PrintParkingLot";
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        parkinglot += string.Format("Plats {0,-3} {1,-20} {2} \n", (int)reader[0], (string)reader[1], (string)reader[2]);
                    }
                }
                catch
                {

                }
            }
            Console.WriteLine(parkinglot);
        }
        /// <summary>
        /// Parkerar ett fordon i databasen
        /// </summary>
        /// <param name="vehicle"> användarens fordon.</param>
        public static void ParkVehicle(Vehicle vehicle)
        {
            connection.ConnectionString = connectionString;
            using (connection)
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "dbo.AddVehicle";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@regnmbr", vehicle.RegistrationNummber);
                command.Parameters.AddWithValue("@vehicleTypeID", vehicle.VehicleTypeID);

                var parkingSpot = command.Parameters.Add("@parkingSpot", SqlDbType.Int);
                parkingSpot.Direction = ParameterDirection.ReturnValue;

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if(rowsAffected != 0)
                    {
                        int spot = (int)parkingSpot.Value;
                        Console.WriteLine("We have now parked your vehicle on spot {0}", spot);
                    }
                }
                catch
                {
                    Console.WriteLine("Couldnt park vehicle");
                }

            }
        }
        /// <summary>
        /// Ta bort fordon ur parkeringsplatsen, (hämtar ut)
        /// </summary>
        public static void RemoveVehicle(bool payForParking)
        {

            Console.Clear();
            Console.WriteLine("Type your registrationnumber to remove your vehicle");
            string userReg = Console.ReadLine();

            connection.ConnectionString = connectionString;

            bool vehicleRemoved = false;
            using (connection)
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                //Här testar vi ifall användaren ska betala eller ej för parkering. Detta är för att undvika dublicering av kod.
                if (payForParking == true)
                {
                    command.CommandText = "dbo.DeleteParkedVehicle";
                }
                else if (payForParking != true)
                {
                    command.CommandText = "dbo.RemoveVehicleForFree";
                }
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@registrationNumber", userReg);
                try
                {
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows != 0)
                    {
                        Console.WriteLine("You have now removed your vehicle");
                        vehicleRemoved = true;
                    }
                    else
                    {
                        Console.WriteLine("Couldnt find your vehicle, try search again.");
                        MenuClass.MainMenu();
                    }
                }
                catch
                {

                }
            }
            if (vehicleRemoved)
            {
                PrintRemovedVehicle(userReg);
            }
        }
        /// <summary>
        /// Skriver ut borttaget fordon ur history table.
        /// </summary>
        /// <param name="userReg">fordonet att skriva ut</param>
        public static void PrintRemovedVehicle(string userReg)
        {
            Vehicle removedVehicle = new Vehicle();
            connection.ConnectionString = connectionString;
            using (connection)
            {
                string queryString = "SELECT TOP 1 * FROM History h WHERE h.RegistrationNumber = @regnmbr ORDER BY h.ArchivedVehicleID DESC";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@regnmbr", userReg);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        removedVehicle.RegistrationNummber = reader.GetString(1);
                        removedVehicle.Arrival = reader.GetDateTime(2);
                        removedVehicle.CheckOutTime = reader.GetDateTime(3);
                        removedVehicle.TotalCost = (int)reader.GetDecimal(4);
                        removedVehicle.CurrentSpot = reader.GetInt32(5);
                        removedVehicle.VehicleTypeID = reader.GetInt32(6);

                    }
                    Console.WriteLine(removedVehicle.ToString());
                }
                catch
                {

                }
            }
        }
        /// <summary>
        /// SÖker efter fordon och skriver ut upplupen kostnad och tid
        /// </summary>
        public static void SearchForVehicle()
        {
            Console.Clear();
            Console.WriteLine("Type your registrationnumber to find vehicle");
            string userReg = Console.ReadLine();
            string outPut = "";
            connection.ConnectionString = connectionString;
            using (connection)
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "dbo.FindVehicle";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@registrationNumber", userReg);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        outPut += string.Format("Regnmbr: {0} Type: {1} Arrivaltime: {2} Current elapsed time: {3} Hours and {4} minutes.\n" +
                            "Current cost: {5} Kr\n",
                            reader.GetString(0), reader.GetString(1), reader.GetDateTime(2), reader.GetDecimal(3), reader.GetDecimal(4), (int)reader.GetDecimal(5));
                    }
                    Console.WriteLine(outPut);
                }
                catch
                {
                    Console.WriteLine("couldnt find your vehicle.");
                }
            }
        }
        /// <summary>
        /// Flyttar fordon till önskad plats.
        /// </summary>
        public static void MoveVehicle()
        {
            Console.Clear();
            Console.Write("Type your RegistrationNumber to move your vehicle: ");
            string userReg = Console.ReadLine();
            Console.Write("To which spot would you like to move the vehicle to?:");
            int userSpot = MenuClass.InputChecker();

            connection.ConnectionString = connectionString;
            using (connection)
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "dbo.MoveVehicleToSpecificSpot";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@registrationNumber", userReg);
                command.Parameters.AddWithValue("@parkingSpot", userSpot);

                try
                {
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows != 0)
                    {
                        Console.WriteLine("You have now moved your vehicle");
                        PrintWorkOrder();
                    }
                    else
                    {
                        Console.WriteLine("Couldnt find your vehicle, try search again.");
                        Console.ReadKey();
                        MenuClass.MainMenu();
                    }
                }
                catch
                {

                }
            }
        }
        public static void PrintVehiclesParkedOver48Hrs()
        {
            connection.ConnectionString = connectionString;
            string outPut = "";
            using (connection)
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "dbo.VehiclesParkedOver48Hours";
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        outPut += string.Format("Regnmbr: {0} spot: {1} CurrentTimeParked: {2} \n",
                            reader.GetString(0), reader.GetInt32(1), reader.GetString(2));
                    }
                    Console.WriteLine(outPut);
                }
                catch
                {

                }
            }
        }
        public static void ShowFullHistory()
        {
            Console.WriteLine("REGNMR        ARRIVAL                CHECKOUTTIME         TOTALCOST");
            connection.ConnectionString = connectionString;
            string queryString = "SELECT * FROM History";
            string outPut = "";
            using (connection)
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        outPut += string.Format("{0,-10}   {1,-10}    {2}   {3}kr\n",
                            reader.GetString(1), reader.GetDateTime(2), reader.GetDateTime(3), (int)reader.GetDecimal(4));
                    }
                    Console.WriteLine(outPut);
                }
                catch
                {

                }
            }
        }
        public static void OptimizeParking()
        {
            connection.ConnectionString = connectionString;
            using (connection)
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "dbo.OptimizeParkingLot";
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows != 0)
                    {
                        Console.WriteLine("ParkingLot optimized, all changes listed below \n");
                        PrintWorkOrder();
                    }
                }
                catch
                {

                }
            }
        }
        /// <summary>
        /// Skriver ut workorder för vad som skedde vi flytt av enskilt fordon eller optimering av parkeringen.
        /// </summary>
        public static void PrintWorkOrder()
        {
            string workOrder = "";
            string queryString = "SELECT * FROM Workorder DELETE FROM Workorder";
            using (connection)
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        workOrder += string.Format("Vehicle with Regnmbr: {0} and Type: {1} was moved from spot: {2} to spot: {3} \n",
                            (string)reader[1], (string)reader[2], (int)reader[3], (int)reader[4]);
                    }
                    Console.WriteLine(workOrder);
                }
                catch
                {

                }
            }
        }
    }
}
