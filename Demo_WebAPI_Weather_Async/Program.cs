using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace Demo_WebAPI_Weather
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayOpeningScreen();
            DisplayMenu();
            DisplayClosingScreen();
        }


        static void DisplayMenu()
        {
            bool quit = false;
            LocationZip zip = new LocationZip();

            while (!quit)
            {
                DisplayHeader("Main Menu");

                Console.WriteLine("Enter the number of your menu choice.");
                Console.WriteLine();
                Console.WriteLine("1) Set the Location");
                Console.WriteLine("2) Display the Current Weather");
                Console.WriteLine("3) Exit");
                Console.WriteLine();
                Console.Write("Enter Choice:");
                string userMenuChoice = Console.ReadLine();

                switch (userMenuChoice)
                {
                    case "1":
                        zip = DisplayGetLocationZip();
                        break;

                    case "2":
                        DisplayCurrentWeatherZipAsync(zip);
                        break;

                    case "3":
                        quit = true;
                        break;

                    default:
                        Console.WriteLine("You must enter a number from the menu.");
                        break;
                }
            }
        }


        static void DisplayOpeningScreen()
        {
            //
            // display an opening screen
            //
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Weather Reporter");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("This application uses the Web API for OpenWeatherMap.");

            DisplayContinuePrompt();
        }

        static void DisplayClosingScreen()
        {
            //
            // display an closing screen
            //
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Thank you for using my application!");
            Console.WriteLine();
            Console.WriteLine();

            //
            // display continue prompt
            //
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Console.WriteLine();
        }

        static void DisplayContinuePrompt()
        {
            //
            // display continue prompt
            //
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
            Console.WriteLine();
        }

        static void DisplayHeader(string headerText)
        {
            //
            // display header
            //
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(headerText);
            Console.WriteLine();
        }

        static LocationCoordinates DisplayGetLocationLongLat()
        {
            DisplayHeader("Set Location by Coordinates");

            LocationCoordinates coordinates = new LocationCoordinates();

            Console.Write("Enter Latitude: ");
            coordinates.Latitude = double.Parse(Console.ReadLine());

            Console.Write("Enter longitude: ");
            coordinates.Longitude = double.Parse(Console.ReadLine());

            Console.WriteLine();
            Console.WriteLine($"Location Coordinates: ({coordinates.Latitude}, {coordinates.Longitude})");
            Console.WriteLine();

            DisplayContinuePrompt();

            return coordinates;
        }
        
        static LocationZip DisplayGetLocationZip()
        {
            DisplayHeader("Set Location by Zip Code");

            LocationZip zip = new LocationZip();

            Console.Write("Enter Zip Code: ");
            zip.Zip = double.Parse(Console.ReadLine());

            Console.WriteLine();
            Console.WriteLine($"Location by Zip Code: ({zip.Zip})");
            Console.WriteLine();

            DisplayContinuePrompt();

            return zip;
        }


        static async Task<WeatherData> GetCurrentWeatherDataLongLatAsync(LocationCoordinates coordinates)
        {
            string url;

            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("http://api.openweathermap.org/data/2.5/weather?");
            sb.Append("&lat=" + coordinates.Latitude.ToString());
            sb.Append("&lon=" + coordinates.Longitude.ToString());
            sb.Append("&appid=864d252afc928abff4010abe732617a1");

            url = sb.ToString();

            WeatherData currentWeather = new WeatherData();

            Task<WeatherData> getCurrentWeather = HttpGetCurrentWeatherByLocationAsync(url);

            //
            // Note: The Wait() method is necessary so that the app does not proceed
            //       to the menu before returning the weather data. If there is an issue
            //       returning the data, the Wait() will create a deadlock in the app flow.
            //
            getCurrentWeather.Wait();

            currentWeather = await getCurrentWeather;

            return currentWeather;
        }
        
        static async Task<WeatherData> GetCurrentWeatherDataZipAsync(LocationZip zip)
        {
            string url;

            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("http://api.openweathermap.org/data/2.5/weather?");
            sb.Append("&zip=" + zip.Zip.ToString());
            sb.Append("&appid=864d252afc928abff4010abe732617a1");

            url = sb.ToString();

            WeatherData currentWeather = new WeatherData();

            Task<WeatherData> getCurrentWeather = HttpGetCurrentWeatherByLocationAsync(url);

            //
            // Note: The Wait() method is necessary so that the app does not proceed
            //       to the menu before returning the weather data. If there is an issue
            //       returning the data, the Wait() will create a deadlock in the app flow.
            //
            getCurrentWeather.Wait();

            currentWeather = await getCurrentWeather;

            return currentWeather;
        }

        static async Task<WeatherData> HttpGetCurrentWeatherByLocationAsync(string url)
        {
            string result = null;

            using (HttpClient syncClient = new HttpClient())
            {
                var response = await syncClient.GetAsync(url);
                result = await response.Content.ReadAsStringAsync();
            }

            //Console.WriteLine(result);

            WeatherData currentWeather = JsonConvert.DeserializeObject<WeatherData>(result);

            return currentWeather;
        }

        static async Task DisplayCurrentWeatherLongLatAsync(LocationCoordinates coordinates)
        {
            Console.Clear();

            DisplayHeader("Current Weather");

            WeatherData currentWeatherData = await GetCurrentWeatherDataLongLatAsync(coordinates);
            
            Console.WriteLine(String.Format("Temperature (Fahrenheit): {0:0.0}", ConvertToFahrenheit(currentWeatherData.Main.Temp)));

            DisplayContinuePrompt();
        }
        
        static async Task DisplayCurrentWeatherZipAsync(LocationZip zip)
        {
            Console.Clear();

            DisplayHeader("Current Weather");

            WeatherData currentWeatherData = await GetCurrentWeatherDataZipAsync(zip);
            
            Console.WriteLine(String.Format("Temperature (Fahrenheit): {0:0.0}", ConvertToFahrenheit(currentWeatherData.Main.Temp)));
            Console.WriteLine(String.Format("Longitude: {0}", currentWeatherData.Coord.Lon));
            Console.WriteLine(String.Format("Latitude: {0}", currentWeatherData.Coord.Lat));
            
            
            DisplayContinuePrompt();
        }

        static double ConvertToFahrenheit(double degreesKalvin)
        {
            return (degreesKalvin - 273.15) * 1.8 + 32;
        }
    }
}
