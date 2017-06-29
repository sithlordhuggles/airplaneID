using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using airplaneID.FlightXML2_Soap;

namespace airplaneID
{
    class Program
    {
        
        static void Main(string[] args)
        {
            MainMenu();
        }

        public static void MainMenu()
        {
            FlightXML2SoapClient client = new FlightXML2SoapClient();
            client.ClientCredentials.UserName.UserName = "jakevanvl";
            client.ClientCredentials.UserName.Password = "e3b0ab0a12eebe31f5ad997f896240cc57c5864d";

            Console.Clear();
            Console.WriteLine("Select from one of the following options:");
            Console.WriteLine("1: Gather Flights Enroute to Specified Airport");
            Console.WriteLine("2: Get METAR (Weather Info) for Specified Airport");
            Console.WriteLine("3: Get Enhanced METAR for Specified Airport");
            Console.WriteLine("Select 1, 2, or 3:");

            int response = new int();

            try
            {
                response = int.Parse(Console.ReadLine());
            }
            catch
            {
                response = 0;
                Console.WriteLine("Invalid Response!");
                MainMenu();
            }

            switch (response)
            {
                case 1:
                    GetFlightsEnrouteToAirport(client);
                    break;
                case 2:
                    GetMETARForSpecifiedAirport(client);
                    break;
                case 3:
                    GetMetarExforSpecifiedAirport(client);
                    break;
                default:
                    Console.WriteLine("Invalid Response!");
                    MainMenu();
                    break;

            }
        }

        public static void GetFlightsEnrouteToAirport(FlightXML2SoapClient client)
        {
            Console.WriteLine("What Airport ID would you like to search? Example: KIAD, KSFO, KJFK");
            string airportID = Console.ReadLine();
            int resultsReturned = 10; //Max number of results returned
            string aircraftFilter = "airline"; //Limit results to airline traffic only
            int offest = 0; //Soonest results first

            EnrouteStruct flightsEnroute = client.Enroute(airportID, resultsReturned, aircraftFilter, offest);

            foreach (EnrouteFlightStruct e in flightsEnroute.enroute)
            {
                DateTime departureTimestamp = UnixTimeStampToDateTime(Double.Parse(e.actualdeparturetime.ToString()));
                DateTime arrivalTimestamp = UnixTimeStampToDateTime(Double.Parse(e.estimatedarrivaltime.ToString()));

                Console.WriteLine("Identifier: " + e.ident);
                Console.WriteLine("Origin: " + e.originCity + " (" + e.origin + ")");
                Console.WriteLine("Departure Time: " + departureTimestamp.ToString());
                Console.WriteLine("Destination: " + e.destinationCity + " (" + e.destination + ")");
                Console.WriteLine("Estimated Arrival Time: " + arrivalTimestamp.ToString());
                Console.WriteLine("Press Enter to Continue...");
                Console.ReadLine();
            }
            Console.WriteLine("Press Enter to return to the main menu...");
            Console.ReadLine();
            MainMenu();
        }

        public static void GetMETARForSpecifiedAirport(FlightXML2SoapClient client)
        {
            Console.WriteLine("What Airport ID would you like to search? Example: KIAD, KSFO, KJFK");
            string airportID = Console.ReadLine();

            string currentMETAR = client.Metar(airportID);

            Console.Write(currentMETAR);
            Console.WriteLine("Press Enter to return to the main menu...");
            Console.ReadLine();

            MainMenu();
        }

        public static void GetMetarExforSpecifiedAirport(FlightXML2SoapClient client)
        {
            Console.WriteLine("What Airport ID would you like to search? Example: KIAD, KSFO, KJFK");
            string airportID = Console.ReadLine();
            int resultsReturned = 1; //Max number of results returned
            int startTime = 0; //Limit results to airline traffic only
            int offest = 0; //Soonest results first

            ArrayOfMetarStruct currentMETAREx = client.MetarEx(airportID, startTime, resultsReturned, offest);

            foreach(MetarStruct e in currentMETAREx.metar)
            {
                DateTime timestamp = UnixTimeStampToDateTime(Double.Parse(e.time.ToString()));
                Console.WriteLine("Identifier: " + e.airport);
                Console.WriteLine("Report Time: " + timestamp.ToString());
                Console.WriteLine("Temperature: " + e.temp_air.ToString() + "C");
                Console.WriteLine("Humidity: " + e.temp_relhum.ToString() + "%");
                Console.WriteLine("Dew Point: " + e.temp_dewpoint.ToString() + "C");

                if (e.wind_speed_gust == 0)
                {
                    Console.WriteLine("Wind: " + e.wind_direction.ToString() + " deg @ " + e.wind_speed.ToString() + "kts");
                }
                else if (e.wind_speed_gust > 0)
                {
                    Console.WriteLine("Wind: " + e.wind_direction.ToString() + " deg @ " + e.wind_speed.ToString() + "kts, gusting to " + e.wind_speed_gust.ToString() + "kts");
                }
                Console.WriteLine("Visibility: " + e.visibility.ToString() + " miles");
                Console.WriteLine("Press Enter to return to the main menu...");
                Console.ReadLine();
            }
            MainMenu();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
