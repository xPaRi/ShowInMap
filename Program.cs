using System;
using System.Globalization;
using System.Reflection;
using IDEA.Library;

namespace ShowInMap
{
    internal class Program
    {
        /// <summary>
        /// Zobrazí zadané souřadnice na mapě v internetovém prohlížeči
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name} ver. {Assembly.GetExecutingAssembly().GetName().Version}");

            if (args.Length < 3 || args.Length > 4)
            {
                Console.WriteLine("Invalid parameters.\n");
                Help();
                return;
            }


            if (!double.TryParse(args[1].Replace(',', '.'), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var x))
            {
                Console.WriteLine("Invalid parsing X value.\n");
                Help();
                return;
            }


            if (!double.TryParse(args[2].Replace(',', '.'), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var y))
            {
                Console.WriteLine("Invalid parsing Y value.\n");
                Help();
                return;
            }

            WGS84Coordinate wgs84;

            switch (args[0].ToUpper())
            {
                case "JTSK2065":
                    Console.WriteLine($" IN (JTSK2065): x={x}; y={y}");
                    wgs84 = new JTSK2065Coordinate(x,y).WGS84Coordinate;
                    break;
                case "JTSK5514":
                    Console.WriteLine($" IN (JTSK5514): x={x}; y={y}");
                    wgs84 = new JTSK5514Coordinate(x,y).WGS84Coordinate;
                    break;
                case "S42":
                    Console.WriteLine($" IN (S42): x={x}; y={y}");
                    wgs84 = new S42Coordinate(x,y).WGS84Coordinate;
                    break;
                default:
                    Console.WriteLine("Invalid parsing source type.");
                    Help();
                    return;
            }

            Console.WriteLine($" OUT: Lat: {wgs84.LatitudeDec:0.000000}; Lng: {wgs84.LongitudeDec:0.000000}");

            if (args.Length == 4)
            {
                switch (args[3].ToUpper())
                {
                    case "B":
                        Console.WriteLine(" Opening Bing map in default internet browser...");
                        wgs84.OpenMap("B");
                        break;
                    case "G":
                        Console.WriteLine(" Opening Google map in default internet browser...");
                        wgs84.OpenMap("G");
                        break;
                    case "O":
                        Console.WriteLine(" Opening OSM map in default internet browser...");
                        wgs84.OpenMap("O");
                        break;
                    case "S":
                        Console.WriteLine(" Opening Seznam map in default internet browser...");
                        wgs84.OpenMap("S");
                        break;
                    default:
                        Console.WriteLine(" Unknown map server.");
                    break;
                }
            }
        }

        private static void Help()
        {
            Console.WriteLine("ShowInMap <source type> <X> <Y> [B|G|O|S]");
            Console.WriteLine();
            Console.WriteLine(" Source type");
            Console.WriteLine("  JTSK2065       - EPSG 2065 S-JTSK/Krovak South-West, positive coordinates");
            Console.WriteLine("  JTSK5514       - EPSG 5514 S-JTSK/Krovak East-North, negative coordinates");
            Console.WriteLine("  S42            - EPSG 28403 Pulkovo 1942/Gauss-Krüger zone 3");
            Console.WriteLine();
            Console.WriteLine(" X, Y            - Source coordinates. Automatic decimal point replacement.");
            Console.WriteLine("                   Example: -820800,60 -1068738,00 -> -820800.60 -1068738.00");
            Console.WriteLine();
            Console.WriteLine(" B               - Open internet browser and show coordinates in Bing maps");
            Console.WriteLine(" G               - Open internet browser and show coordinates in Google maps");
            Console.WriteLine(" O               - Open internet browser and show coordinates in Open Street Map");
            Console.WriteLine(" S               - Open internet browser and show coordinates in Seznam maps");
            Console.WriteLine();
            Console.WriteLine("Example");
            Console.WriteLine(" ShowInMap JTSK5514 -820800.60 -1068738.00 G");
        }
    }
}
