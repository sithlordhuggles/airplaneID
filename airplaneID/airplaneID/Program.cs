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

            Console.WriteLine("Select from one of the following options:");
            Console.WriteLine("1: Gather Flights Enroute to Specified Airport");
            Console.WriteLine("2: Get METAR (Weather Info) for Specified Airport");
            Console.WriteLine("Select 1 or 2");

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
            MainMenu();
        }

        public static void GetMETARForSpecifiedAirport(FlightXML2SoapClient client)
        {
            Console.WriteLine("What Airport ID would you like to search? Example: KIAD, KSFO, KJFK");
            string airportID = Console.ReadLine();

            string currentMETAR = client.Metar(airportID);

            Console.Write(currentMETAR);
            Console.ReadLine();

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
