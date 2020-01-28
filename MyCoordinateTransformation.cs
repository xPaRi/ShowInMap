using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;

namespace IDEA.Library
{
    /// <summary>
    /// Statická třída pro podporu práce s geografickými souřadnicemi.
    /// </summary>
    /// <remarks>
    /// Základní pojmy viz. http://www.gis.zcu.cz/studium/gen1/html/ch02s03.html
    /// </remarks>
    public static class MyCoordinateTransformation
    {
        /// <summary>
        /// Převede zeměpisnou souřadnici v systému DegDec na Stupně.
        /// </summary>
        /// <param name="value">Zeměpisná souřadnice DegDec.</param>
        /// <returns>Stupně.</returns>
        public static int DecToDegrees(double value)
        {
            return new AngleDeg(value).Degrees;
        }

        /// <summary>
        /// Převede zeměpisnou souřadnici v systému DegDec na Minuty.
        /// </summary>
        /// <param name="value">Zeměpisná souřadnice DegDec.</param>
        /// <returns>Minuty.</returns>
        public static int DecToMinutes(double value)
        {
            return new AngleDeg(value).Minutes;
        }

        /// <summary>
        /// Převede zeměpisnou souřadnici v systému DegDec na Vteřiny.
        /// </summary>
        /// <param name="value">Zeměpisná souřadnice DegDec.</param>
        /// <returns>Vteřiny.</returns>
        public static double DecToSeconds(double value)
        {
            return new AngleDeg(value).Seconds;
        }

        /// <summary>
        /// Převede zeměpisnou souřadnici ve stupních na systém DegDec.
        /// </summary>
        /// <param name="degrees">Stupně.</param>
        /// <param name="minutes">Minuty.</param>
        /// <param name="seconds">Vteřiny.</param>
        /// <returns>Zeměpisná souřadnice v systému Dec.</returns>
        public static double DegreesToDec(double degrees, double minutes, double seconds)
        {
            var angleDeg = new AngleDeg(Convert.ToInt32(degrees), Convert.ToInt32(minutes), seconds);

            return angleDeg.ToDec();
        }

        /// <summary>
        /// Převede zadanou souřadnici na řetězec Stupně, minuty, vteřiny.
        /// </summary>
        /// <param name="value">Zeměpisná souřadnice.</param>
        /// <returns>Stupně, minuty, vteřiny.</returns>
        public static string ToDegString(double? value)
        {
            return ToDegString(System.Globalization.CultureInfo.CurrentCulture, value);
        }

        /// <summary>
        /// Převede zadanou souřadnici na řetězec Stupně, minuty, vteřiny.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="value">Zeměpisná souřadnice.</param>
        /// <returns>Stupně, minuty, vteřiny.</returns>
        public static string ToDegString(IFormatProvider provider, double? value)
        {
            if (value.HasValue)
                return new AngleDeg(value.Value).ToString(provider);
            
            return string.Format(AngleDeg.FormatString, "?", "?", "?");
        }

        /// <summary>
        /// Převede zadanou souřadnici na řetězec Stupně, minuty, vteřiny.
        /// </summary>
        /// <param name="latitude">Zeměpisná šířka.</param>
        /// <param name="longitude">Zeměpisná délka.</param>
        /// <returns>Stupně, minuty, vteřiny.</returns>
        public static string ToDegString(double? latitude, double? longitude)
        {
            string latitudeAbbrev;
            string longitudeAbbrev;

            //--- určení délek a šířek
            if (latitude.HasValue)
                latitudeAbbrev = (latitude >= 0) ? "sš" : "jš";
            else
                latitudeAbbrev = string.Empty;

            if (longitude.HasValue)
                longitudeAbbrev = (longitude >= 0) ? "vd" : "zd";
            else
                longitudeAbbrev = string.Empty;
            //---

            return $"{ToDegString(latitude)} {latitudeAbbrev}; {ToDegString(longitude)} {longitudeAbbrev}";
        }


    }


    #region AngleDeg

    /// <summary>
    /// Úhel ve stupních, minutách a vteřinách.
    /// </summary>
    public struct AngleDeg
    {
        #region Fields

        internal static string FormatString = "{0}\u00b0 {1}' {2:0.0000}\"";

        private int _Degrees; //Stupně.
        private int _Minutes; //Minuty
        private double _Seconds; //Vteřiny

        #endregion //Fields

        #region Constructors

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="degrees">Stupně.</param>
        /// <param name="minutes">Minuty.</param>
        /// <param name="seconds">Vteřiny.</param>
        public AngleDeg(int degrees, int minutes, double seconds)
        {
            _Degrees = degrees;
            _Minutes = minutes;
            _Seconds = seconds;
        }

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="angleDec">Úhel v desetinném vyjádření.</param>
        public AngleDeg(double angleDec)
        {
            
            double degrees = Math.Floor(angleDec);
            double minutes = Math.Round((angleDec - degrees) * 60d, 10); //zaokrouhlení (10) je klíčové
            double seconds = Math.Round((minutes - Math.Floor(minutes)) * 60d, 8); //zaokrouhlení (8) je klíčové

            minutes = Math.Floor(minutes);

            _Degrees = Convert.ToInt32(degrees);
            _Minutes = Convert.ToInt32(minutes);
            _Seconds = seconds;
        }

        #endregion //Constructors

        #region Properties

        /// <summary>
        /// Stupně.
        /// </summary>
        public int Degrees
        {
            get
            {
                return _Degrees;
            }
            set
            {
                _Degrees = value;
            }
        }

        /// <summary>
        /// Minuty.
        /// </summary>
        public int Minutes
        {
            get
            {
                return _Minutes;
            }
            set
            {
                _Minutes = value;
            }
        }

        /// <summary>
        /// Vteřiny.
        /// </summary>
        public double Seconds
        {
            get
            {
                return _Seconds;
            }
            set
            {
                _Seconds = value;
            }
        }

        #endregion //Properties

        #region Public

        /// <summary>
        /// Úhel v desetinném vyjádření.
        /// </summary>
        /// <returns></returns>
        public double ToDec()
        {
            return Degrees + ((Minutes * 60d + Seconds) / 3600d);
        }

        /// <summary>
        /// Řetězcová reprezentace objektu.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString(System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Řetězcová reprezentace objektu.
        /// </summary>
        /// <returns></returns>
        public string ToString(IFormatProvider provider)
        {
            return string.Format(provider, FormatString, Degrees, Minutes, Seconds);
        }

        /// <summary>
        /// Operace porovnání rovnosti.
        /// </summary>
        /// <param name="obj1">První porovnávaný objekt.</param>
        /// <param name="obj2">Druhý porovnávaný objekt.</param>
        /// <returns>
        /// True - u identických objektů (i pokud jsou oba Null)
        /// , False - u nestejných objektů
        /// </returns>
        public static bool operator ==(AngleDeg obj1, AngleDeg obj2)
        {
            if ((object)obj1 != null)
                return (obj1.Equals(obj2));
            if ((object)obj2 != null)
                return (obj2.Equals(obj1));
            return (true);
        }


        /// <summary>
        /// Operace porovnání nerovnosti.
        /// </summary>
        /// <remarks>
        /// <returns>
        /// True - u nestejných objektů
        /// False - u identických objektů
        /// False - pokud jsou oba objekty null
        /// </returns>
        public static bool operator !=(AngleDeg obj1, AngleDeg obj2)
        {
            if ((object)obj1 != null)
                return (!obj1.Equals(obj2));
            if ((object)obj2 != null)
                return (!obj2.Equals(obj1));
            return (false);
        }

        /// <summary>
        /// Porovnání objektů téže třídy.
        /// </summary>
        /// <param name="obj">Objekt porovnávaný s aktuálním objektem.</param>
        /// <returns>
        /// True - u identických objektů
        /// False - u nestejných objektů
        /// </returns>
        public override bool Equals(object obj)
        {
            return
                Degrees.Equals(((AngleDeg)obj).Degrees)
                && Minutes.Equals(((AngleDeg)obj).Minutes)
                && Seconds.Equals(((AngleDeg)obj).Seconds)
                ;
        }

        public override int GetHashCode()
        {
            return Convert.ToInt32(ToDec());
        }

        #endregion //Public
    }

    #endregion //AngleDeg

    #region Elipsoid

    /// <summary>
    /// Elipsoid a jeho parametry.
    /// </summary>
    public class Elipsoid
    {
        public readonly string Name;
        public readonly double A;
        public readonly double EccSquared;

        public Elipsoid(string name, double a, double eccSquared)
        {
            Name = name;
            A = a;
            EccSquared = eccSquared;
        }

        public static Elipsoid GetElipsoid(string datumName)
        {
            switch (datumName.ToUpper())
            {
                case "AIRY":                  return new Elipsoid("Airy", 6377563, 0.00667054);
                case "AUSTRALIAN NATIONAL":   return new Elipsoid("Australian National", 6378160, 0.006694542);
                case "BESSEL 1841":           return new Elipsoid("Bessel 1841", 6377397, 0.006674372);
                case "BESSEL 1841 NAMBIA":    return new Elipsoid("Bessel 1841 Nambia", 6377484, 0.006674372);
                case "CLARKE 1866":           return new Elipsoid("Clarke 1866", 6378206, 0.006768658);
                case "CLARKE 1880":           return new Elipsoid("Clarke 1880", 6378249, 0.006803511);
                case "EVEREST":               return new Elipsoid("Everest", 6377276, 0.006637847);
                case "FISCHER 1960 MERCURY":  return new Elipsoid("Fischer 1960 Mercury", 6378166, 0.006693422);
                case "FISCHER 1968":          return new Elipsoid("Fischer 1968", 6378150, 0.006693422);
                case "GRS 1967":              return new Elipsoid("GRS 1967", 6378160, 0.006694605);
                case "GRS 1980":              return new Elipsoid("GRS 1980", 6378137, 0.00669438 );
                case "HELMERT 1906":          return new Elipsoid("Helmert 1906", 6378200, 0.006693422);
                case "HOUGH":                 return new Elipsoid("Hough", 6378270, 0.00672267);
                case "INTERNATIONAL":         return new Elipsoid("International", 6378388, 0.00672267);
                case "KRASSOVSKY":            return new Elipsoid("Krassovsky", 6378245, 0.006693422);
                case "MODIFIED AIRY":         return new Elipsoid("Modified Airy", 6377340, 0.00667054);
                case "MODIFIED EVEREST":      return new Elipsoid("Modified Everest", 6377304, 0.006637847);
                case "MODIFIED FISCHER 1960": return new Elipsoid("Modified Fischer 1960", 6378155, 0.006693422);
                case "SOUTH AMERICAN 1969":   return new Elipsoid("South American 1969", 6378160, 0.006694542);
                case "WGS 60":                return new Elipsoid("WGS 60", 6378165, 0.006693422);
                case "WGS 66":                return new Elipsoid("WGS 66", 6378145, 0.006694542);
                case "WGS 72":                return new Elipsoid("WGS 72", 6378135, 0.006694318);
                case "ED50":                  return new Elipsoid("ED50", 6378388, 0.00672267); // International Ellipsoid
                case "WGS 84":                return new Elipsoid("WGS 84", 6378137, 0.00669438);
                case "EUREF89":               return new Elipsoid("EUREF89", 6378137, 0.00669438); // Max deviation from WGS 84 is 40 cm/km see http://ocq.dk/euref89 (in danish)
                case "ETRS89":                return new Elipsoid("ETRS89", 6378137, 0.00669438); // Same as EUREF89 
                default: throw new Exception($"Neexistující elipsoid (datum) [{datumName}]");
            }
        }

        private static Elipsoid _DefaultElipsoid;

        /// <summary>
        /// Vrací standarní elipsoid (WGS 84).
        /// </summary>
        /// <returns>Elispoid WGS 84.</returns>
        public static Elipsoid GetDefault()
        {
            return _DefaultElipsoid ?? (_DefaultElipsoid = GetElipsoid("WGS 84"));
        }
    }


    #endregion //Elipsoid

    #region Souřadnicové systémy

    /// <summary>
    /// Souřadnicový systém WGS 84.
    /// </summary>
    /// <remarks>
    /// Jedná se o vojenský souřadnicový systém používaný státy NATO. Referenční plochou je 
    /// elipsoid WGS 84 (World Geodetic System). Použité kartografické zobrazení se 
    /// nazývá UTM (Univerzální transverzální Mercatorovo). 
    /// Systém má počátek v hmotném středu Země (s přesností cca 2 m) – jedná se o geocentrický systém. 
    /// Osa Z je totožná s osou rotace Země v roce 1984. Osy X a Y leží v rovině rovníku. 
    /// Počátek a orientace jeho os X,Y,Z jsou realizovány pomocí 12 pozemských stanic se známými 
    /// přesnými souřadnicemi, které nepřetržitě monitorují dráhy družic systému GPS-NAVSTAR.
    /// </remarks>
    public class WGS84Coordinate
    {

        #region Fields

        /// <summary>
        /// Konstanta pro konverzi z DEG do RAD.
        /// </summary>
        private const double DEG_TO_RAD = Math.PI / 180d; //0.0174532925199432958;

        private double _LatitudeDec;
        private double _LongitudeDec;

        private double _LatitudeRad;
        private double _LongitudeRad;

        #endregion //Fields

        /// <summary>
        /// Vzdálenost [metry] dvou bodů zadaných geografickými souřadnicemi.
        /// </summary>
        /// <remarks>
        /// Souhrný článek o výpočtech vzdálenosti bodů:
        /// https://www.movable-type.co.uk/scripts/latlong.html
        /// </remarks>
        /// <param name="latitude1">Zeměpisná šířka v DegDec bodu 1.</param>
        /// <param name="longitude1">Zeměpisná délka v DegDec bodu 1.</param>
        /// <param name="latitude2">Zeměpisná šířka v DegDec bodu 2.</param>
        /// <param name="longitude2">Zeměpisná délka v DegDec bodu 2.</param>
        /// <returns>Vzdálenost v metrech.</returns>
        public static double GetDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            return GetDistanceByElipsoid(latitude1, longitude1, latitude2, longitude2);
        }


        /// <summary>
        /// Vzdálenost [metry] dvou bodů zadaných geografickými souřadnicemi.
        /// Odchylka max. 0.00055 m.
        /// </summary>
        /// <remarks>
        /// https://stackoverflow.com/questions/6544286/calculate-distance-of-two-geo-points-in-km-c-sharp
        /// </remarks>
        /// <param name="latitude1">Zeměpisná šířka v DegDec bodu 1.</param>
        /// <param name="longitude1">Zeměpisná délka v DegDec bodu 1.</param>
        /// <param name="latitude2">Zeměpisná šířka v DegDec bodu 2.</param>
        /// <param name="longitude2">Zeměpisná délka v DegDec bodu 2.</param>
        /// <returns>Vzdálenost v metrech.</returns>
        public static double GetDistanceByElipsoid(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            long num = 0x615299L;
            double num2 = 6356752.3142;
            double num3 = 0.0033528106647474805;
            double num4 = MyGeo.ToRAD(longitude2 - longitude1);
            double a = Math.Atan((1 - num3) * Math.Tan(MyGeo.ToRAD(latitude1)));
            double num6 = Math.Atan((1 - num3) * Math.Tan(MyGeo.ToRAD(latitude2)));
            double num7 = Math.Sin(a);
            double num8 = Math.Sin(num6);
            double num9 = Math.Cos(a);
            double num10 = Math.Cos(num6);
            double num11 = num4;
            double num12 = 6.2831853071795862;
            int num13 = 20; //počet iterací výpočtu
            double y = 0;
            double x = 0;
            double num18 = 0;
            double num20 = 0;
            double num22 = 0;

            while ((Math.Abs(num11 - num12) > 1E-12) && (--num13 > 0))
            {
                double num14 = Math.Sin(num11);
                double num15 = Math.Cos(num11);
                
                y = Math.Sqrt((num10 * num14 * num10 * num14) + (((num9 * num8) - (num7 * num10 * num15)) * ((num9 * num8) - (num7 * num10 * num15))));
                
                if (y == 0)
                    return 0;

                x = (num7 * num8) + (num9 * num10 * num15);
                num18 = Math.Atan2(y, x);
                double num19 = num9 * num10 * num14 / y;
                num20 = 1 - (num19 * num19);
                num22 = (num20 == 0) ? 0 : x - (2 * num7 * num8 / num20);
                double num21 = num3 / 16 * num20 * (4 + (num3 * (4 - (3 * num20))));
                num12 = num11;
                num11 = num4 + ((1 - num21) * num3 * num19 * (num18 + (num21 * y * (num22 + (num21 * x * (-1 + (2 * num22 * num22)))))));
            }
            
            double num23 = (num20 * ((num * num) - (num2 * num2))) / (num2 * num2);
            double num24 = 1 + (num23 / 16384 * (4096 + (num23 * (-768 + (num23 * (320 - (175 * num23)))))));
            double num25 = num23 / 1024 * (256 + (num23 * (-128 + (num23 * (74 - (47 * num23))))));
            double num26 = num25 * y * (num22 + ((num25 / 4) * ((x * (-1 + (2 * num22 * num22))) - (num25 / 6 * num22 * (-3 + (4 * y * y)) * (-3 + (4 * num22 * num22))))));
            
            return num2 * num24 * (num18 - num26);
        }

        /// <summary>
        /// Vzdálenost [metry] mezi dvěma body.
        /// Odchylka max. 900 m.
        /// </summary>
        /// <remarks>
        /// Algoritmus: https://www.geodatasource.com/developers/sql
        /// </remarks>
        /// <param name="latitude1">Zeměpisná šířka v DegDec bodu 1.</param>
        /// <param name="longitude1">Zeměpisná délka v DegDec bodu 1.</param>
        /// <param name="latitude2">Zeměpisná šířka v DegDec bodu 2.</param>
        /// <param name="longitude2">Zeměpisná délka v DegDec bodu 2.</param>
        /// <returns>Vzdálenost v metrech.</returns>
        public static double GetDistanceByCircle(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double RADIUS = 6378137; //přesnost 900 m

            var lat1 = MyGeo.ToRAD(latitude1);
            var lon1 = MyGeo.ToRAD(longitude1);
            var lat2 = MyGeo.ToRAD(latitude2);
            var lon2 = MyGeo.ToRAD(longitude2);

            return RADIUS * Math.Acos((Math.Sin(lat1) * Math.Sin(lat2)) + (Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1)));
        }

        /// <summary>
        /// Vzdálenost [metry] od zadaného bodu.
        /// </summary>
        /// <param name="coordinate">Bod, ke kterému je počítána vzdálenost.</param>
        /// <returns>Vzdálenost v metrech.</returns>
        public double Distance(WGS84Coordinate coordinate)
        {
            return GetDistance(this.LatitudeDec, this.LongitudeDec, coordinate.LatitudeDec, coordinate.LongitudeDec);
        }

        /// <summary>
        /// Vzdálenost [metry] od zadaného bodu.
        /// </summary>
        /// <param name="coordinate">Bod, ke kterému je počítána vzdálenost.</param>
        /// <param name="latitude1">Zeměpisná šířka bodu, ke kterému je počítána vzdálenost.</param>
        /// <param name="longitude1">Zeměpisná délka bodu, ke kterému je počítána vzdálenost.</param>
        /// <returns>Vzdálenost v metrech.</returns>
        public double Distance(double latitude, double longitude)
        {
            return GetDistance(this.LatitudeDec, this.LongitudeDec, latitude, longitude);
        }

        /// <summary>
        /// Konstruktor pro zadání v systému DegDec.
        /// </summary>
        /// <param name="latitudeDec">Zeměpisná šířka v DegDec.</param>
        /// <param name="longitudeDec">Zeměpisná délka v DegDec.</param>
        public WGS84Coordinate(double latitudeDec, double longitudeDec)
        {
            LatitudeDec = latitudeDec;
            LongitudeDec = longitudeDec;
        }

        /// <summary>
        /// Konstruktor pro zadání v systému DegGeo.
        /// </summary>
        /// <param name="latitudeDegrees">Stupně zeměpisné šířky.</param>
        /// <param name="latitudeMinutes">Minuty zeměpisné šířky.</param>
        /// <param name="latitudeSeconds">Sekundy zeměpisné šířky.</param>
        /// <param name="longitudeDegrees">Stupně zeměpisné délky.</param>
        /// <param name="longitudeMinutes">Minuty zeměpisné délky.</param>
        /// <param name="longitudeSeconds">Sekundy zeměpisné délky.</param>
        public WGS84Coordinate(double latitudeDegrees, double latitudeMinutes, double latitudeSeconds, double longitudeDegrees, double longitudeMinutes, double longitudeSeconds)
        {
            LatitudeDec = MyCoordinateTransformation.DegreesToDec(latitudeDegrees, latitudeMinutes, latitudeSeconds);
            LongitudeDec = MyCoordinateTransformation.DegreesToDec(longitudeDegrees, longitudeMinutes, longitudeSeconds);
        }


        #region Methods

        /// <summary>
        /// Otevře prohlížeč s mapou pro aktuální souřadnici.
        /// </summary>
        /// <param name="mapServer">Mapový server [B]ing, [O]pen Street Map, [S]eznam, [G]oogle</param>
        public void OpenMap(string mapServer)
        {
            try
            {
                switch (mapServer.ToUpper())
                {
                    case "B": //Bing
                        const string FORMAT_B = @"https://bing.com/maps/default.aspx?sp=point.{0}_{1}";
                        Process.Start(string.Format(CultureInfo.InvariantCulture, FORMAT_B, LatitudeDec, LongitudeDec));
                        break;
                    case "O": //OSM (open street map)
                        const string FORMAT_O = @"https://www.openstreetmap.org/?mlat={0}&mlon={1}#map=16/{0}/{1}";
                        Process.Start(string.Format(CultureInfo.InvariantCulture, FORMAT_O, LatitudeDec, LongitudeDec));
                        break;
                    case "S": //Seznam
                        const string FORMAT_S = @"https://mapy.cz/zakladni?x={1}&y={0}&z=16&source=coor&id={1}%2C{0}";
                        Process.Start(string.Format(CultureInfo.InvariantCulture, FORMAT_S, LatitudeDec, LongitudeDec));
                        break;
                    default: //Google
                        const string FORMAT_G = @"https://maps.google.com/maps?q={0},{1}";
                        Process.Start(string.Format(CultureInfo.InvariantCulture, FORMAT_G, LatitudeDec, LongitudeDec));
                        break;
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Pokusí se ze zadaného řetězce vydolovat souřadnice WGS84.
        /// Pokud uspěje, vrací true a v out parametru souřadnici.
        /// </summary>
        /// <remarks>
        /// Podporuje následující formáty:
        ///  GoogleDec:  49.4593683, 18.3572658
        ///  SeznamDec:  49.4593683N, 18.3572658E
        ///  SeznamDeg1: N 49°27.56210', E 18°21.43595'
        ///  SeznamDeg2: 49°27'33.726"N, 18°21'26.157"E
        /// 
        ///  šířka může být: N,S
        ///  délka může být: E,W
        /// </remarks>
        /// <param name="value">Konverotvaný řetězec.</param>
        /// <param name="wgs84Coordinate">Nová souřadnice.</param>
        /// <returns></returns>
        public static bool TryParse(string value, out WGS84Coordinate wgs84Coordinate)
        {
            wgs84Coordinate = null;

            if (string.IsNullOrEmpty(value?.Trim()))
                return false;

            if (TryParseGoogleDec(value, out wgs84Coordinate))
                return true;

            if (TryParseSeznamDec(value, out wgs84Coordinate))
                return true;

            if (TryParseSeznamDegMin(value, out wgs84Coordinate))
                return true;

            if (TryParseSeznamDegMinSec(value, out wgs84Coordinate))
                return true;

            return false;
        }

        /// <summary>
        /// Konverze řetězce Googlu "49.4593683, 18.3572658"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="wgs84Coordinate"></param>
        /// <returns></returns>
        public static bool TryParseGoogleDec(string value, out WGS84Coordinate wgs84Coordinate)
        {
            const RegexOptions REGX_OPTION = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace;
            const string REGX = @"(?<lat>[+|-]?\d{1,3}.\d+)\s*,\s*(?<lng>[+|-]?\d{1,3}.\d+)\s*";

            var match = new Regex(REGX, REGX_OPTION).Match(value);

            double lat;
            double lng;

            wgs84Coordinate = null;

            if (match.Success
                && double.TryParse(match.Groups["lat"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out lat)
                && double.TryParse(match.Groups["lng"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out lng)
                )

            {
                wgs84Coordinate = new WGS84Coordinate(lat, lng);
            }

            return wgs84Coordinate != null;
        }

        public static bool TryParseSeznamDec(string value, out WGS84Coordinate wgs84Coordinate)
        {
            const RegexOptions REGX_OPTION = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace;
            const string REGX = @"(?<lat>\d{1,3}.\d*)\s*(?<latSign>N|S)\s*,\s*(?<lng>\d{1,3}.\d*)\s*(?<lngSign>E|W)";

            var match = new Regex(REGX, REGX_OPTION).Match(value);

            double lat;
            double lng;

            wgs84Coordinate = null;

            if (match.Success
                && double.TryParse(match.Groups["lat"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out lat)
                && double.TryParse(match.Groups["lng"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out lng)
                )

            {
                if (match.Groups["latSign"].Value.Equals("S", StringComparison.OrdinalIgnoreCase))
                    lat = -lat;

                if (match.Groups["lngSign"].Value.Equals("W", StringComparison.OrdinalIgnoreCase))
                    lng = -lng;

                wgs84Coordinate = new WGS84Coordinate(lat, lng);
            }

            return wgs84Coordinate != null;
        }

        public static bool TryParseSeznamDegMin(string value, out WGS84Coordinate wgs84Coordinate)
        {
            const RegexOptions REGX_OPTION = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace;
            const string REGX = @"(?<latSign>[N|S])\s*(?<latDeg>\d{1,3})°(?<latMin>\d{1,2}.\d*)'\s*,\s*(?<lngSign>[E|W])\s*(?<lngDeg>\d{1,3})°(?<lngMin>\d{1,2}.\d*)'";

            var match = new Regex(REGX, REGX_OPTION).Match(value);

            double latDeg;
            double latMin;

            double lngDeg;
            double lngMin;

            wgs84Coordinate = null;

            if (match.Success
                && double.TryParse(match.Groups["latDeg"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out latDeg)
                && double.TryParse(match.Groups["latMin"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out latMin)
                && double.TryParse(match.Groups["lngDeg"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out lngDeg)
                && double.TryParse(match.Groups["lngMin"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out lngMin)
                )

            {
                var lat = latDeg + latMin / 60d;
                if (match.Groups["latSign"].Value.Equals("S", StringComparison.OrdinalIgnoreCase))
                    lat = -lat;

                var lng = lngDeg + lngMin / 60d;
                if (match.Groups["lngSign"].Value.Equals("W", StringComparison.OrdinalIgnoreCase))
                    lng = -lng;

                wgs84Coordinate = new WGS84Coordinate(lat, lng);
            }

            return wgs84Coordinate != null;
        }

        public static bool TryParseSeznamDegMinSec(string value, out WGS84Coordinate wgs84Coordinate)
        {
            const RegexOptions REGX_OPTION = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace;
            const string REGX = @"(?<latDeg>\d{1,3})°(?<latMin>\d{1,2})'(?<latSec>\d{1,2}.\d*)""\s*(?<latSign>N|S)\s*,\s* (?<lngDeg>\d{1,3})°(?<lngMin>\d{1,2})'(?<lngSec>\d{1,2}.\d*)""\s*(?<lngSign>E|W)";

            var match = new Regex(REGX, REGX_OPTION).Match(value);

            double latDeg;
            double latMin;
            double latSec;

            double lngDeg;
            double lngMin;
            double lngSec;

            wgs84Coordinate = null;

            if (match.Success
                && double.TryParse(match.Groups["latDeg"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out latDeg)
                && double.TryParse(match.Groups["latMin"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out latMin)
                && double.TryParse(match.Groups["latSec"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out latSec)
                && double.TryParse(match.Groups["lngDeg"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out lngDeg)
                && double.TryParse(match.Groups["lngMin"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out lngMin)
                && double.TryParse(match.Groups["lngSec"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out lngSec)
                )

            {
                var lat = latDeg + latMin / 60d + latSec / 3600d;
                if (match.Groups["latSign"].Value.Equals("S", StringComparison.OrdinalIgnoreCase))
                    lat = -lat;

                var lng = lngDeg + lngMin / 60d + lngSec / 3600d;
                if (match.Groups["lngSign"].Value.Equals("W", StringComparison.OrdinalIgnoreCase))
                    lng = -lng;

                wgs84Coordinate = new WGS84Coordinate(lat, lng);
            }

            return wgs84Coordinate != null;
        }

        #endregion //Methods

        #region Properties

        /// <summary>
        /// Zeměpisná šířka ve stupních (X).
        /// </summary>
        public double LatitudeDec
        {
            get => _LatitudeDec;
            set
            {
                _LatitudeDec = value;
                _LatitudeRad = _LatitudeDec * DEG_TO_RAD;
            }
        }

        /// <summary>
        /// Zeměpisná šířka v radiánech (X).
        /// </summary>
        public double LatitudeRad => _LatitudeRad;

        /// <summary>
        /// Zeměpisná šířka v geografických stupních.
        /// </summary>
        public int LatitudeDegrees => MyCoordinateTransformation.DecToDegrees(LatitudeDec);

        /// <summary>
        /// Zeměpisná šířka v geografických minutách.
        /// </summary>
        public int LatitudeMinutes => MyCoordinateTransformation.DecToMinutes(LatitudeDec);

        /// <summary>
        /// Zeměpisná šířka v geografických vteřinách.
        /// </summary>
        public double LatitudeSeconds => MyCoordinateTransformation.DecToSeconds(LatitudeDec);

        /// <summary>
        /// Zeměpisná délka ve stupních (Y).
        /// </summary>
        public double LongitudeDec
        {
            get => _LongitudeDec;
            set
            {
                _LongitudeDec = value;
                _LongitudeRad = _LongitudeDec * DEG_TO_RAD;
            }
        }

        /// <summary>
        /// Zeměpisná délka v radiánech (Y).
        /// </summary>
        public double LongitudeRad => _LongitudeRad;

        /// <summary>
        /// Zeměpisná délka v geografických stupních.
        /// </summary>
        public int LongitudeDegrees => MyCoordinateTransformation.DecToDegrees(LongitudeDec);

        /// <summary>
        /// Zeměpisná délka v geografických minutách.
        /// </summary>
        public int LongitudeMinutes => MyCoordinateTransformation.DecToMinutes(LongitudeDec);

        /// <summary>
        /// Zeměpisná délka v geografických vteřinách.
        /// </summary>
        public double LongitudeSeconds => MyCoordinateTransformation.DecToSeconds(LongitudeDec);

        /// <summary>
        /// Souřadnice v S42.
        /// </summary>
        public S42Coordinate S42Coordinate => Transformation.TransformS42(this);

        /// <summary>
        /// Souřadnice v JTSK EPSG:2065 (+).
        /// </summary>
        public JTSK2065Coordinate JTSK2065Coordinate => Transformation.TransformJTSK2065(this);

        /// <summary>
        /// Souřadnice v JTSK EPSG:5514 (-).
        /// </summary>
        public JTSK5514Coordinate JTSK5514Coordinate => Transformation.TransformJTSK5514(this);

        /// <summary>
        /// Souřadnice v EMEPGrid50x50.
        /// </summary>
        public EMEPGrid50x50Coordinate EMEPGrid50x50Coordinate => Transformation.TransformEMEPGrid50x50(this);

        /// <summary>
        /// Souřadnice v EMEPGrid01x01.
        /// </summary>
        public EMEPGrid01x01Coordinate EMEPGrid01x01Coordinate => Transformation.TransformEMEPGrid01x01(this);

        /// <summary>
        /// Souřadnice v UTM.
        /// </summary>
        public UTMCoordinate UTMCoordinate => Transformation.TransformUTM(this);

        /// <summary>
        /// Souřadnice v UTM33N.
        /// </summary>
        public UTMZoneCoordinate UTM33NCoordinate => Transformation.TransformUTMzone(this, 33);

        /// <summary>
        /// Zeměpisná souřadnice v geografickém zápisu.
        /// </summary>
        /// <returns></returns>
        public string ToDegString()
        {
            return ToDegString(System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Zeměpisná souřadnice v geografickém zápisu.
        /// </summary>
        /// <returns></returns>
        public string ToDegString(IFormatProvider provider)
        {
            return $"{MyCoordinateTransformation.ToDegString(provider, LatitudeDec)} {MyCoordinateTransformation.ToDegString(provider, LongitudeDec)}";
        }

        /// <summary>
        /// Řetězcová reprezentace objektu.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString(System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Řetězcová reprezentace objektu.
        /// </summary>
        /// <returns></returns>
        public string ToString(IFormatProvider provider)
        {
            return string.Format(provider, "{0}\u00b0 {1}\u00b0", LatitudeDec, LongitudeDec);
        }

        #endregion //Properties
    }

    /// <summary>
    /// Seznam souřadnic WGS84.
    /// </summary>
    public class WGS84CoordinateList : List<WGS84Coordinate>
    {

        #region Helpers

        private string GetPointsAsString()
        {
            const string format = @"|{0},{1}";
            string result = string.Empty;

            foreach (WGS84Coordinate wgs84 in this)
            {
                result += string.Format(System.Globalization.CultureInfo.InvariantCulture, format, wgs84.LatitudeDec, wgs84.LongitudeDec);
            }

            return result; 
        }

        #endregion //Helpers

        /// <summary>
        /// Otevře prohlížeč s mapou a zobrazí seznam bodů pro zadané souřadnice.
        /// </summary>
        public void OpenMapAsPoints()
        {
            var commandFormat = @"http://maps.google.com/maps/api/staticmap?size=640x640{1}&sensor=false&markers=color:yellow{0}";
            var itemFormat = @"|{0},{1}";
            var coordinates = string.Empty;
            var zoom = (this.Count > 1) ? string.Empty : "&zoom=15";

            foreach (var wgs84 in this)
            {
                coordinates += string.Format(System.Globalization.CultureInfo.InvariantCulture, itemFormat, wgs84.LatitudeDec, wgs84.LongitudeDec);
            }

            try
            {
                var command = string.Format(commandFormat, coordinates, zoom);
                System.Diagnostics.Process.Start(command);
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Otevře prohlížeč s mapou a zobrazí seznam bodů jako trasu.
        /// </summary>
        /// <remarks>
        /// http://maps.google.com/maps/api/staticmap?size=640x640&path=color:0xff0000FF|weight:10|50.699308,13.970686|50.515775,14.046808|50.319946,13.545316|50.360336,13.785165&sensor=false
        /// </remarks>
        public void OpenMapAsTrace()
        {
            var commandFormat = @"http://maps.google.com/maps/api/staticmap?size=640x640&sensor=false&path=color:0x0000ff90|weight:3{0}&markers=color:yellow|size:small{0}";
            var itemFormat = @"|{0},{1}";
            var coordinates = string.Empty;

            foreach (WGS84Coordinate wgs84 in this)
            {
                coordinates += string.Format(System.Globalization.CultureInfo.InvariantCulture, itemFormat, wgs84.LatitudeDec, wgs84.LongitudeDec);
            }

            try
            {
                var command = string.Format(commandFormat, coordinates);
                System.Diagnostics.Process.Start(command);
            }
            catch
            {
                // ignored
            }
        }
    }

    /// <summary>
    /// Souřadnicový systém Jednotné trigonometrické sítě katastrální (EPSG 2065 S-JTSK/Krovak South-West).
    /// Tzv. kladné
    /// </summary>
    /// <remarks>
    /// Souřadnicový systém jednotné trigonometrické sítě katastrální (S-JTSK) je definován 
    /// Besselovým elipsoidem s referenčním bodem Hermannskogel, Křovákovým zobrazením 
    /// (dvojité konformní kuželové zobrazení v obecné poloze), převzatými prvky sítě vojenské 
    /// triangulace (orientací, rozměrem i polohou na elipsoidu) a jednotnou trigonometrickou 
    /// sítí katastrální. Křovákovo zobrazení je jednotné pro celý stát. Navrhl a propracoval 
    /// jej Ing. Josef Křovák roku 1922.
    /// 
    /// Vztah mezi souřadnicemi „záporného“ X ,Y a „kladného“ x,y Křováka (tedy mezi EPSG 5514 a EPSG 2065) je tento: X = -y a Y = -x.
    /// 
    /// Platí, že pro ČR je X > Y.
    /// Zobrazují se v pořadí Y, X
    /// Používají je geodeti pro svá měření v terénu.
    /// Zdroj: http://geoportal.cuzk.cz/(S(un51vr0e1spynjiydli1bnri))/Default.aspx?mode=TextMeta&amp;text=about_FAQ&amp;side=about&amp;menu=6
    /// </remarks>
    public class JTSK2065Coordinate
    {
        public double X { get; }
        public double Y { get; }

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="x">Souřadnice X.</param>
        /// <param name="y">Souřadnice Y.</param>
        public JTSK2065Coordinate(double x, double y)
        {
            /* kontrola dočasně vypnuta 2017-11-30 
            if (x <= y)
                throw new ArgumentOutOfRangeException($"Hodnota x musí být větší jak y. (x={x}; y={y})");

            if (x < 0)
                throw new ArgumentOutOfRangeException($"Hodnota x musí být kladná. (x={x})");

            if (y < 0)
                throw new ArgumentOutOfRangeException($"Hodnota y musí být kladná. (y={y})");
            */

            X = x;
            Y = y;
        }

        /// <summary>
        /// Souřadnice ve WGS84.
        /// </summary>
        public WGS84Coordinate WGS84Coordinate => Transformation.TransformWGS84(this);

        /// <summary>
        /// Řetězcová reprezentace objektu.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"JTSK2065: {{{Y}m; {X}m}}";
        }
    }

    /// <summary>
    /// Souřadnicový systém Jednotné trigonometrické sítě katastrální (EPSG 5514 S-JTSK/Krovak East-North).
    /// Tzv. Záporné
    /// </summary>
    /// <remarks>
    /// Souřadnicový systém jednotné trigonometrické sítě katastrální (S-JTSK) je definován 
    /// Besselovým elipsoidem s referenčním bodem Hermannskogel, Křovákovým zobrazením 
    /// (dvojité konformní kuželové zobrazení v obecné poloze), převzatými prvky sítě vojenské 
    /// triangulace (orientací, rozměrem i polohou na elipsoidu) a jednotnou trigonometrickou 
    /// sítí katastrální. Křovákovo zobrazení je jednotné pro celý stát. Navrhl a propracoval 
    /// jej Ing. Josef Křovák roku 1922.
    /// 
    /// Vztah mezi souřadnicemi „záporného“ X ,Y a „kladného“ x,y Křováka (tedy mezi EPSG 5514 a EPSG 2065) je tento: X = -y a Y = -x.
    /// 
    /// Platí, že pro ČR je X > Y.
    /// Zobrazují se v pořadí X, Y
    /// Používá je Geoportál ČÚZK.
    /// Zdroj: http://geoportal.cuzk.cz/(S(un51vr0e1spynjiydli1bnri))/Default.aspx?mode=TextMeta&amp;text=about_FAQ&amp;side=about&amp;menu=6
    /// </remarks>
    public class JTSK5514Coordinate
    {
        public double X { get; }
        public double Y { get; }

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="x">Souřadnice X.</param>
        /// <param name="y">Souřadnice Y.</param>
        public JTSK5514Coordinate(double x, double y)
        {
            /* kontrola dočasně vypnuta 2017-11-30 
            if (x <= y)
                throw new ArgumentOutOfRangeException($"Hodnota x musí být větší jak y. (x={x}; y={y})");

            if (x >= 0)
                throw new ArgumentOutOfRangeException($"Hodnota x musí být záporná. (x={x})");

            if (y >= 0)
                throw new ArgumentOutOfRangeException($"Hodnota y musí být záporná. (y={y})");
            */

            X = x;
            Y = y;
        }

        /// <summary>
        /// Souřadnice ve WGS84.
        /// </summary>
        public WGS84Coordinate WGS84Coordinate => Transformation.TransformWGS84(this);

        /// <summary>
        /// Řetězcová reprezentace objektu.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"JTSK5514: {{{X}m; {Y}m}}";
        }
    }


    /// <summary>
    /// Souřadnicový systém S-42.
    /// </summary>
    /// <remarks>
    /// Souřadnicový systém S-42 používá Krasovského elipsoid s referenčním bodem v Pulkavu. 
    /// Souřadnice bodů jsou vyjádřené v 6° a 3° pásech Gaussova zobrazení. Geodetickým základem 
    /// je astronomicko-geodetická síť (AGS), která byla vyrovnána v mezinárodním spojení a do ní 
    /// byla transformovaná Jednotná trigonometrická síť katastrální (JTSK).
    /// </remarks>
    public class S42Coordinate
    {
        public double X;
        public double Y;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="x">Souřadnice X.</param>
        /// <param name="y">Souřadnice Y.</param>
        public S42Coordinate(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Souřadnice ve WGS84.
        /// </summary>
        public WGS84Coordinate WGS84Coordinate => Transformation.TransformWGS84(this);

        /// <summary>
        /// Řetězcová reprezentace objektu.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"S42: {{{X}m; {Y}m}}";
        }
    }

    /// <summary>
    /// Celoevropský program EMEP rozděluje území do sítě čtverců 50 x 50 km v doméně 132x159 bodů.
    /// Síť je založena na polární stereografické projekci v reálném prostoru.
    /// 
    /// Poznámka 1:
    /// Platnost tohoto gridu je dočasně do roku 2012, kdy má být nahrazen jiným systémem.
    /// 
    /// Poznámka 2:
    /// EMEP - Evropský program monitorování a hodnocení (European Monitoring and Evaluation Program) 
    /// byl zřízen organizacemi UNECE (United Economic Commission for Europe), 
    /// WMO (World Meteorological Organization) a UNEP (United Nations Environment Programme) 
    /// v roce 1977.
    /// 
    /// Další informace na http://www.emep.int
    /// </summary>
    public class EMEPGrid50x50Coordinate
    {
        /// <summary>
        /// X-ová vzdálenost gridu od severního pólu.
        /// </summary>
        public const int NorthPoleX = 8;

        /// <summary>
        /// Y-ová vzdálenost gridu od severního pólu.
        /// </summary>
        public const int NorthPoleY = 110;

        /// <summary>
        /// Vzdálenost gridu mezi severním pólem a rovníkem.
        /// </summary>
        public static double M
        {
            get
            {
                const double d = 50;                        // velikost gridu na 60 st. sš
                const double FI0 = Math.PI * 60d / 180d;    // definování šířky - 60 stupňů [RAD]
                const double R = 6370;                      // poloměr zeměkoule
                
                return R / d * (1 + Math.Sin(FI0));         // počet vzdáleností gridu mezi sev. polem a rovníkem
            }
        }

        public int X;
        public int Y;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="x">Souřadnice X.</param>
        /// <param name="y">Souřadnice Y.</param>
        public EMEPGrid50x50Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Řetězcová reprezentace objektu.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"EMEP 50x50: {{{X}; {Y}}}";
        }
    }

    /// <summary>
    /// Geografické vymezení čtverců EMEP podle pokynů uvedených v dokumentu Guidelines: 
    /// http://www.ceip.at/fileadmin/inhalte/emep/2014_Guidelines/ece.eb.air.125_ADVANCE_VERSION_reporting_guidelines_2013.pdf  
    /// Relevantní pasáže(čl. 14, 28, 47-50)
    /// 
    /// Stručně: 
    /// 1. čtverce jsou definovány velikostí 0.1 x 0.1 geografického stupně souřadnic WGS
    /// 2. čtverec je definován souřadnicemi sváho středu
    /// 3. dle http://www.ceip.at/new_emep-grid se udávý jako lng-lat
    /// 4. ale zde je zase lan-lng http://www.unece.org/fileadmin/DAM/env/documents/2012/air/EMEP_36th/n_3_EMEP_note_on_grid_scale__projection_and_reporting.pdf
    /// 
    /// Příklad: 
    /// čtverec 18.15; 49.05
    /// má souřadnice [18.10; 19.00] - (18.20; 49.10)
    /// 
    /// Poznámka 2:
    /// EMEP - Evropský program monitorování a hodnocení (European Monitoring and Evaluation Program) 
    /// byl zřízen organizacemi UNECE (United Economic Commission for Europe), 
    /// WMO (World Meteorological Organization) a UNEP (United Nations Environment Programme) 
    /// v roce 1977.
    /// </summary>
    public class EMEPGrid01x01Coordinate
    {
        /// <summary>
        /// Velikost hrany gridu ve stupních
        /// </summary>
        public const double GRID_HALF_SIZE = 0.05; //velikost hrany gridu ve stupních

        /// <summary>
        /// Zeměpisná délka.
        /// </summary>
        public double LatitudeDec { get; }

        /// <summary>
        /// Zeměpisná šířka.
        /// </summary>
        public double LongitudeDec { get; }

        /// <summary>
        /// Pokud jsou zadané hodnoty středy EMEP čtverců, vrtací True.
        /// </summary>
        /// <remarks>
        /// Technicky se musíé jednat o násobky 0.1 zvětšené o 0.05
        /// </remarks>
        public bool IsValid => IsGridable(LatitudeDec) && IsGridable(LongitudeDec);

        /// <summary>
        /// Konstruktor. 
        /// </summary>
        /// <param name="latitudeDec">Zeměpisná délka.</param>
        /// <param name="longitudeDec">Zeměpisná šířka.</param>
        public EMEPGrid01x01Coordinate(double latitudeDec, double longitudeDec)
        {
            LatitudeDec = latitudeDec;
            LongitudeDec = longitudeDec;
        }

        /// <summary>
        /// Wgs souřadnice středu EMEP čtverce.
        /// </summary>
        /// <param name="wgs84Coordinate"></param>
        public EMEPGrid01x01Coordinate(WGS84Coordinate wgs84Coordinate)
            : this(wgs84Coordinate.LatitudeDec, wgs84Coordinate.LongitudeDec)
        {}

        /// <summary>
        /// Souřadnice levého horního rohu.
        /// </summary>
        public WGS84Coordinate LeftTopCorner => new WGS84Coordinate(LatitudeDec + GRID_HALF_SIZE, LongitudeDec - GRID_HALF_SIZE);

        /// <summary>
        /// Souřadnice pravého horního rohu.
        /// </summary>
        public WGS84Coordinate RightTopCorner => new WGS84Coordinate(LatitudeDec + GRID_HALF_SIZE, LongitudeDec + GRID_HALF_SIZE);

        /// <summary>
        /// Souřadnice levého dolního rohu.
        /// </summary>
        public WGS84Coordinate LeftBottomCorner => new WGS84Coordinate(LatitudeDec - GRID_HALF_SIZE, LongitudeDec - GRID_HALF_SIZE);

        /// <summary>
        /// Souřadnice pravého dolního rohu.
        /// </summary>
        public WGS84Coordinate RightBottomCorner => new WGS84Coordinate(LatitudeDec - GRID_HALF_SIZE, LongitudeDec + GRID_HALF_SIZE);

        /// <summary>
        /// Řetězcová reprezentace objektu.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString(System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Řetězcová reprezentace objektu.
        /// </summary>
        /// <returns></returns>
        public string ToString(IFormatProvider provider)
        {
            return string.Format(provider, "EMEP 0.1\u00b0x0.1\u00b0: {0:0.00}; {1:0.00}{2}", LatitudeDec, LongitudeDec, ((IsValid) ? string.Empty : " (nevalidní)"));
        }

        internal static double Gridable(double value)
        {
            return (Math.Floor(value * 10) / 10) + 0.05;
        }

        /// <summary>
        /// Vrací True v případě, že číslo je GRIDABLE (tj. je o 0.05 větší násobku 0.10)
        /// </summary>
        /// <param name="value">Kontrolovaná hodnota.</param>
        /// <returns></returns>
        private static bool IsGridable(double value)
        {
            return Math.Abs(value - Gridable(value)) < 0.0000001;
        }
    }

    /// <summary>
    /// Souřadnice UTM (Universal Transverse Mercator coordinate system)
    /// </summary>
    public class UTMCoordinate
    {
        public double Easting { get; set; }
        public double UTMEasting { get; set; }
        public double Northing { get; set; }
        public double UTMNorthing { get; set; }
        public int ZoneNumber { get; set; }
        public string ZoneLetter { get; set; }
        public string Zona => $"{ZoneNumber}{ZoneLetter}";

        //Něco tady nehraje, tak to zatím nechávám tak...
        //public UTMCoordinate(double easting, double northing, int zoneNumber, string zoneLetter)
        //{
        //    //Easting = utmEasting, Northing = utmNorthing, ZoneNumber = zoneNumber, ZoneLetter = utmZone
        //}

        public override string ToString()
        {
            return $"{Zona} {Easting}{Northing}";
        }

        /// <summary>
        /// Vrací písmeno zóny za základě zeměpisné délky.
        /// </summary>
        /// <param name="latitude">Zeměpisná délka.</param>
        /// <returns></returns>
        public static string GetUtmZoneLetter(double latitude)
        {
            if (84 >= latitude && latitude >= 72)  return "X";
            if (72 > latitude && latitude >= 64)   return "W";
            if (64 > latitude && latitude >= 56)   return "V";
            if (56 > latitude && latitude >= 48)   return "U";
            if (48 > latitude && latitude >= 40)   return "T";
            if (40 > latitude && latitude >= 32)   return "S";
            if (32 > latitude && latitude >= 24)   return "R";
            if (24 > latitude && latitude >= 16)   return "Q";
            if (16 > latitude && latitude >=  8)   return "P";
            if ( 8 > latitude && latitude >=  0)   return "N";
            if ( 0 > latitude && latitude >= -8)   return "M";
            if (-8 > latitude && latitude >= -16)  return "L";
            if (-16 > latitude && latitude >= -24) return "K";
            if (-24 > latitude && latitude >= -32) return "J";
            if (-32 > latitude && latitude >= -40) return "H";
            if (-40 > latitude && latitude >= -48) return "G";
            if (-48 > latitude && latitude >= -56) return "F";
            if (-56 > latitude && latitude >= -64) return "E";
            if (-64 > latitude && latitude >= -72) return "D";
            if (-72 > latitude && latitude >= -80) return "C";

            return "Z";
        }

        /// <summary>
        /// Vrací číslo UTM zóny na základě zeměpisné polohy.
        /// </summary>
        /// <param name="latitude">Zeměpisná délka.</param>
        /// <param name="longitude">Zeměpisná šířka.</param>
        /// <returns></returns>
        public static int GetUtmZoneNumber(double latitude, double longitude)
        {
            if (longitude >= 8 && longitude <= 13 && latitude > 54.5 && latitude < 58)
                return 32;
 
            if (latitude >= 56.0 && latitude < 64.0 && longitude >= 3.0 && longitude < 12.0)
                return 32;
 
            if (latitude >= 72.0 && latitude < 84.0)
            {
                if (longitude >= 0.0 && longitude < 9.0)
                    return 31;
                
                if (longitude >= 9.0 && longitude < 21.0)
                    return 33;
                
                if (longitude >= 21.0 && longitude < 33.0)
                    return 35;
                
                if (longitude >= 33.0 && longitude < 42.0)
                    return 37;
            }

            return (int) ((longitude + 180) / 6) + 1;
        }

        /// <summary>
        /// Vrací číslo UTM zóny na základě zeměpisné polohy.
        /// </summary>
        /// <param name="wgs84Coordinate">Zeměpisná poloha.</param>
        /// <returns></returns>
        public static int GetUtmZoneNumber(WGS84Coordinate wgs84Coordinate)
        {
            return GetUtmZoneNumber(wgs84Coordinate.LatitudeDec, wgs84Coordinate.LongitudeDec);
        }
    }

    /// <summary>
    /// Souřadnice s fixní zónou. 
    /// </summary>
    /// <remarks>
    /// Požívá se například pro přepočet WGS84 na UTM33N    '
    /// Řeší to, že ČR je na dvou dlaždicích UTM33N a UTM34N.
    /// Teď se to bude tvářit jako by byla na 33N
    /// </remarks>
    public class UTMZoneCoordinate
    {
        public double X { get; }
        public double Y { get; }
        public int ZoneNumber { get; }

        public UTMZoneCoordinate(double x, double y, int zoneNumber)
        {
            X = x;
            Y = y;
            ZoneNumber = zoneNumber;
        }

        public override string ToString()
        {
            return $"{X}; {Y}; zone: {ZoneNumber}";
        }
    }

    #endregion //Souřadnicové systémy

    #region Transformation

    /// <summary>
    /// Transformuje souřadnice ze systému ITRF (v podstatě WGS84) na S42.
    /// </summary>
    /// <remarks>
    /// Výpočet byl oproti vzoru dosti brutálně optimalizován, tj. není tak názorný
    /// a množství různých volání funkcí bylo zjednodušeno až na úroveň konstanty.
    /// </remarks>
    public class Transformation
    {
        private const double DEG_TO_RAD = Math.PI / 180d; //0.0174532925199432958   rad = deg * DEG_TO_RAD
        private const double RAD_TO_DEG = 1.0 / DEG_TO_RAD; //565.48667764616278292   deg = rad / DEG_TO_RAD

        /// <summary>
        /// Transformuje souřadnice ze systému ITRF (v podstatě WGS84) na S42.
        /// </summary>
        /// <param name="wgs84Coordinate">Transformované souřadnice.</param>
        /// <returns></returns>
        public static S42Coordinate TransformS42(WGS84Coordinate wgs84Coordinate)
        {
            double o3 = 6378245d / Math.Sqrt(1 - 0.0066934216229659511d * Math.Pow(Math.Sin(wgs84Coordinate.LatitudeRad), 2)); // N
            double p3 = Math.Pow(Math.Tan(wgs84Coordinate.LatitudeRad), 2); // T
            double q3 = 0.0067385254146834989d * Math.Pow(Math.Cos(wgs84Coordinate.LatitudeRad), 2);// C
            double r3 = (wgs84Coordinate.LongitudeRad - 0.26179938779914941d) * Math.Cos(wgs84Coordinate.LatitudeRad);// A
            double s3 =
                  6367558.4970123032d * wgs84Coordinate.LatitudeRad
                - 16036.479939776922d * Math.Sin(2d * wgs84Coordinate.LatitudeRad)
                + 16.827654579200246d * Math.Sin(4d * wgs84Coordinate.LatitudeRad)
                - 0.02179177355292761d * Math.Sin(6d * wgs84Coordinate.LatitudeRad)
                ; // M

            return new S42Coordinate(
                  3500123.2862402d + o3 * (r3 + (1d - p3 + q3) * Math.Pow(r3, 3) / 6d + (5d - 18d * p3 + Math.Pow(p3, 2) + 72d * q3 - 0.57277466024809742d) * Math.Pow(r3, 5) / 120d)
                , 42.93530495d + (s3 + o3 * Math.Tan(wgs84Coordinate.LatitudeRad) * (Math.Pow(r3, 2) / 2d + (5d - p3 + 9d * q3 + 4d * Math.Pow(q3, 2)) * Math.Pow(r3, 4) / 24d + (58.776286613154447d - 58d * p3 + Math.Pow(r3, 2) + 600d * q3) * Math.Pow(r3, 6) / 720d))
                );
        }

        /// <summary>
        /// Transformuje souřadnice ze systému ITRF (v podstatě WGS84) na JTSK ESP:2065.
        /// Převod používá Hrdninův algoritmus 
        /// </summary>
        /// <remarks>
        /// Výpočet byl oproti vzoru dosti brutálně optimalizován, tj. není tak názorný
        /// a množství různých volání funkcí bylo zjednodušeno až na úroveň konstanty.
        /// </remarks>
        /// <param name="wgs84Coordinate">Transformované souřadnice.</param>
        public static JTSK2065Coordinate TransformJTSK2065(WGS84Coordinate wgs84Coordinate)
        {
            //--- Výpočet pravoúhlých souřadnic z geodetických souřadnic.
            double ro = 6378137d / Math.Sqrt(1d - 0.00669437999014133d * Math.Pow(Math.Sin(wgs84Coordinate.LatitudeRad), 2));

            double x1 = ro * Math.Cos(wgs84Coordinate.LatitudeRad) * Math.Cos(wgs84Coordinate.LongitudeRad);
            double y1 = ro * Math.Cos(wgs84Coordinate.LatitudeRad) * Math.Sin(wgs84Coordinate.LongitudeRad);
            double z1 = ro * Math.Sin(wgs84Coordinate.LatitudeRad) * 0.99330562000985867d;
            //--- 

            //--- transformace pravoúhlých souřadnic
            double x2 = -570.69d + 0.999996457d * (0.0000255065325768538d * y1 + x1 - 0.0000076928295663736721d * z1);
            double y2 = -85.69d + 0.999996457d * (-0.0000255065325768538d * x1 + y1 + 0.00002423200589058494d * z1);
            double z2 = -462.84d + 0.999996457d * (0.0000076928295663736721d * x1 + z1 - 0.00002423200589058494d * y1);
            //---

            //--- Výpočet geodetických souřadnic z pravoúhlých souřadnic.
            double p = Math.Sqrt(Math.Pow(x2, 2) + Math.Pow(y2, 2));
            double theta = Math.Atan(z2 * 1.0033539847919968d / p);
            double B = Math.Atan((z2 + 42707.884210431082d * Math.Pow(Math.Sin(theta), 3)) / (p - 42565.121440450319d * Math.Pow(Math.Cos(theta), 3)));
            double L = 2 * Math.Atan(y2 / (p + x2));
            //---

            //--- finální výpočet
            const double e = 0.081696831215303d;
            const double n = 0.97992470462083d;
            const double sinVQ = 0.420215144586493d;
            const double cosVQ = 0.907424504992097d;
            const double alfa = 1.000597498371542d;

            double sinB = Math.Sin(B);
            double t = 1.00685001861538d * Math.Exp(alfa * Math.Log(Math.Pow(1d + sinB, 2) / (1 - Math.Pow(sinB, 2)) * Math.Exp(e * Math.Log((1d - e * sinB) / (1d + e * sinB)))));
            double sinU = (t - 1d) / (t + 1d);
            double cosU = Math.Sqrt(1 - Math.Pow(sinU, 2));
            double V = alfa * L;
            double sinS = 0.863499969506341d * sinU + 0.504348889819882d * cosU * (cosVQ * Math.Cos(V) + sinVQ * Math.Sin(V));
            double cosS = Math.Sqrt(1 - Math.Pow(sinS, 2));
            double sinD = (sinVQ * Math.Cos(V) - cosVQ * Math.Sin(V)) * cosU / cosS;
            double D = Math.Atan(sinD / Math.Sqrt(1 - Math.Pow(sinD, 2)));
            double ro2 = 12310230.12797036d * Math.Exp(-n * Math.Log((1d + sinS) / cosS));
            //---

            return new JTSK2065Coordinate(ro2 * Math.Cos(n * D), ro2 * Math.Sin(n * D));
        }

        /// <summary>
        /// Transformuje souřadnice ze systému ITRF (v podstatě WGS84) na JTSK ESP:5514.
        /// Převod používá Hrdninův algoritmus 
        /// </summary>
        /// <remarks>
        /// Výpočet byl oproti vzoru dosti brutálně optimalizován, tj. není tak názorný
        /// a množství různých volání funkcí bylo zjednodušeno až na úroveň konstanty.
        /// </remarks>
        /// <param name="wgs84Coordinate">Transformované souřadnice.</param>
        public static JTSK5514Coordinate TransformJTSK5514(WGS84Coordinate wgs84Coordinate)
        {
            return TransformJTSK5514(TransformJTSK2065(wgs84Coordinate));
        }

        /// <summary>
        /// Transformuje souřadnice ze systému ITRF (v podstatě WGS84) na EMEPGrid50x50.
        /// Převod použivá oficiální algoritmus.
        /// </summary>
        /// <param name="wgs84Coordinate">Transformované souřadnice.</param>
        /// <returns></returns>
        public static EMEPGrid50x50Coordinate TransformEMEPGrid50x50(WGS84Coordinate wgs84Coordinate)
        {
            double FI = Math.PI * wgs84Coordinate.LatitudeDec / 180d;
            double Lambda = Math.PI * wgs84Coordinate.LongitudeDec / 180d;
            
            const double LAMBDA0 = Math.PI * (-32d) / 180d;     // úhel rotace -32 stupnu, tj. délky rovnoběžné s osou Y [RAD]

            double indexX = EMEPGrid50x50Coordinate.NorthPoleX + EMEPGrid50x50Coordinate.M * Math.Tan(Math.PI / 4d - FI / 2d) * Math.Sin(Lambda - LAMBDA0);
            double indexY = EMEPGrid50x50Coordinate.NorthPoleY - EMEPGrid50x50Coordinate.M * Math.Tan(Math.PI / 4d - FI / 2d) * Math.Cos(Lambda - LAMBDA0);

            return new EMEPGrid50x50Coordinate(Convert.ToInt32(indexX), Convert.ToInt32(indexY));
        }

        /// <summary>
        /// Transformuje souřadnice ze systému ITRF (v podstatě WGS84) na EMEPGrid 0.1 x 0.1 st.
        /// Velikost buňky je zhruba 7 x 18 km (délka, šířka).
        /// </summary>
        /// <param name="wgs84Coordinate">Transformované souřadnice.</param>
        /// <returns></returns>
        public static EMEPGrid01x01Coordinate TransformEMEPGrid01x01(WGS84Coordinate wgs84Coordinate)
        {
            return new EMEPGrid01x01Coordinate(EMEPGrid01x01Coordinate.Gridable(wgs84Coordinate.LatitudeDec), EMEPGrid01x01Coordinate.Gridable(wgs84Coordinate.LongitudeDec));
        }

        /// <summary>
        /// Transformuje souřadnice ze systému ITRF (v podstatě WGS84) na EMEPGrid 0.1 x 0.1 st.
        /// Velikost buňky je zhruba 7 x 18 km (délka, šířka).
        /// </summary>
        /// <param name="latitudeDec">Zeměpisná šířka transformované souřadnice.</param>
        /// <param name="longitudeDec">Zeměpisná délka transformované souřadnice.</param>
        /// <returns></returns>
        public static EMEPGrid01x01Coordinate TransformEMEPGrid01x01(double latitudeDec, double longitudeDec)
        {
            return new EMEPGrid01x01Coordinate(new WGS84Coordinate(latitudeDec, longitudeDec));
        }

        /// <summary>
        /// Transformuje souřadnice ze systému EMEPGrid50x50 (v podstatě WGS84) na ITRF (v podstatě WGS84).
        /// Převod použivá oficiální algoritmus.
        /// </summary>
        /// <param name="emepGrid50x50Coordinate">Transformované souřadnice.</param>
        /// <returns></returns>
        public static WGS84Coordinate TransformWGS84(EMEPGrid50x50Coordinate emepGrid50x50Coordinate)
        {
            double X = emepGrid50x50Coordinate.X;
            double Y = emepGrid50x50Coordinate.Y;

            const double Lambda0 = -32;  // úhel rotace -32 stupnu, tj. délky rovnoběžné s osou Y

            double r = Math.Sqrt(Math.Pow((X - EMEPGrid50x50Coordinate.NorthPoleX), 2) + Math.Pow((Y - EMEPGrid50x50Coordinate.NorthPoleY), 2));
            double FI = 90d - 360d / Math.PI * Math.Atan(r / EMEPGrid50x50Coordinate.M);
            double Lambda = Lambda0 + 180d / Math.PI * Math.Atan((X - EMEPGrid50x50Coordinate.NorthPoleX) / (EMEPGrid50x50Coordinate.NorthPoleY - Y));

            return new WGS84Coordinate(FI, Lambda);
        }

        /// <summary>
        /// Transformuje souřadnice JTSK5514Coordinate na JTSK2065Coordinate.
        /// </summary>
        /// <param name="jtskCoordinate">JTSK souřadnice EPSG 5514.</param>
        /// <returns></returns>
        public static JTSK2065Coordinate TransformJTSK2065(JTSK5514Coordinate jtskCoordinate)
        {
            return new JTSK2065Coordinate(-jtskCoordinate.Y, -jtskCoordinate.X);
        }

        /// <summary>
        /// Transformuje souřadnice JTSK2065Coordinate na JTSK5514Coordinate.
        /// </summary>
        /// <param name="jtskCoordinate">JTSK souřadnice EPSG 2065.</param>
        /// <returns></returns>
        public static JTSK5514Coordinate TransformJTSK5514(JTSK2065Coordinate jtskCoordinate)
        {
            return new JTSK5514Coordinate(-jtskCoordinate.Y, -jtskCoordinate.X);
        }

        /// <summary>
        /// Transformuje souřadnice ze systému JTSK EPSG:2065 na ITRF (v podstatě WGS84).
        /// </summary>
        /// <remarks>
        /// Výpočet byl oproti vzoru dosti brutálně optimalizován, tj. není tak názorný
        /// a množství různých volání funkcí bylo zjednodušeno až na úroveň konstanty.
        /// 
        /// Zdrojový kód: http://www.alena.ilcik.cz/gps/souradnice/JTSKtoWGS.htm
        /// používá evidentně Hrdinův algoritmus:
        ///   Hrdina, Z.: Prepocet z S-JTSK do WGS-84. 2002.
        ///   http://gpsweb.cz/JTSK-WGS.htm.
        /// </remarks>
        /// <param name="jtsk2065Coordinate">Transformované souřadnice.</param>
        /// <returns></returns>
        public static WGS84Coordinate TransformWGS84(JTSK2065Coordinate jtsk2065Coordinate)
        {
            double H = 45;

            //--- Výpočet zeměpisných souřadnic z rovinných souřadnic
            const double e = 0.081696831215303;
            const double sinVQ = 0.420215144586493; 
            const double cosVQ = 0.907424504992097;

            double ro1 = Math.Sqrt(Math.Pow(jtsk2065Coordinate.X, 2) + Math.Pow(jtsk2065Coordinate.Y, 2));
            double D = 2d * Math.Atan(jtsk2065Coordinate.Y / (ro1 + jtsk2065Coordinate.X)) / 0.97992470462083d;
            double S = 2d * Math.Atan(Math.Exp(1.0204865693093612d * Math.Log(12310230.12797036d / ro1))) - Math.PI / 2d;
            double sinU = 0.863499969506341d * Math.Sin(S) - 0.504348889819882d * Math.Cos(S) * Math.Cos(D); 
            double cosU = Math.Sqrt(1d - Math.Pow(sinU, 2));
            double sinDV = Math.Sin(D) * Math.Cos(S) / cosU;
            double cosDV = Math.Sqrt(1d - Math.Pow(sinDV, 2));
            double Ljtsk = 2d * Math.Atan((sinVQ * cosDV - cosVQ * sinDV) / (1d + cosVQ * cosDV + sinVQ * sinDV)) / 1.000597498371542d;
            double t = Math.Exp(1.9988057168391598d * Math.Log((1d + sinU) / cosU / 1.003419163966575d));
            double pom = (t - 1d) / (t + 1d);

            double sinB;

            do
            {
                sinB = pom;
                pom = t * Math.Exp(e * Math.Log((1 + e * sinB) / (1 - e * sinB)));
                pom = (pom - 1) / (pom + 1);
            }
            while (Math.Abs(pom - sinB) > 1e-15);

            double Bjtsk = Math.Atan(pom / Math.Sqrt(1 - Math.Pow(pom, 2)));
            //---

            //--- Pravoúhlé souřadnice ve S-JTSK
            double ro2 = 6377397.15508d / Math.Sqrt(1 - 0.0066743722306217279d * Math.Pow(Math.Sin(Bjtsk), 2));
            double x = (ro2 + H) * Math.Cos(Bjtsk) * Math.Cos(Ljtsk);
            double y = (ro2 + H) * Math.Cos(Bjtsk) * Math.Sin(Ljtsk);
            double z = (0.99332562776937827d * ro2 + H) * Math.Sin(Bjtsk);
            //---

            //--- Pravoúhlé souřadnice v WGS-84
            const double wx = -0.00002423200589058494d;
            const double wy = -0.0000076928295663736721d;
            const double wz = -0.0000255065325768538d;

            double xn = 570.69d + 1.000003543d * (x + wz * y - wy * z);
            double yn = 85.69d + 1.000003543d * (-wz * x + y + wx * z);
            double zn = 462.84d + 1.000003543d * (wy * x - wx * y + z);
            //---

            //--- Geodetické souřadnice v systému WGS-84
            double p = Math.Sqrt(Math.Pow(xn, 2) + Math.Pow(yn, 2));
            double theta = Math.Atan(zn * 1.0033640898209764d / p);
            double B = Math.Atan((zn + 42841.31151331366d * Math.Pow(Math.Sin(theta), 3)) / (p - 42697.672707180056d * Math.Pow(Math.Cos(theta), 3))); 
            double L = 2 * Math.Atan(yn / (p + xn)); 
            //---

            return new WGS84Coordinate(B / Math.PI * 180, L / Math.PI * 180);
        }

        /// <summary>
        /// Transformuje souřadnice ze systému JTSK EPSG:5514 na ITRF (v podstatě WGS84).
        /// </summary>
        /// <remarks>
        /// Detaily viz. TransformWGS84 pro TransformJTSK2065().
        /// </remarks>
        /// <param name="jtsk5514Coordinate">Transformované souřadnice.</param>
        /// <returns></returns>
        public static WGS84Coordinate TransformWGS84(JTSK5514Coordinate jtsk5514Coordinate)
        {
            return TransformWGS84(TransformJTSK2065(jtsk5514Coordinate));
        }

        /// <summary>
        /// Transformuje souřadnice ze systému S42 na ITRF (v podstatě WGS84).
        /// </summary>
        /// <remarks>
        /// Compiled by Gábor Timár, Eötvös University of Budapest, e-mail: timar@ludens.elte.hu
        /// References: František Kuska (1960): Matematická Kartografia. Slovenské Vydateľstvo Technickej Literatúry, Bratislava, 388 p.
        /// John P. Snyder (1987): Map projections - a working manual. USGS Prof. Paper 1395: 1-261
        /// Equations derived by József Varga (Technical University of Budapest) and Gábor Virág (FÖMI Space Geodesy Observatory, Penc, Hungary)
        /// were used for computing the Krovák projection.
        /// </remarks>
        /// <param name="s42Coordinate">Transformované souřadnice.</param>
        /// <returns></returns>
        public static WGS84Coordinate TransformWGS84(S42Coordinate s42Coordinate)
        {
            // MAXIMÁLNÍ PŘESNOST
            //  LAT (-0,0041415; 0,0050661) m
            //  LON (-0,0055585; 0,0086234) m
            double R2 = s42Coordinate.Y / 6367558.4970123032d
                + 0.0025184647775237596d * Math.Sin(s42Coordinate.Y / 3183779.2485061516)
                + 0.0000036998858962068768d * Math.Sin(s42Coordinate.Y / 1591889.6242530758d)
                + 0.0000000074446047831951984d * Math.Sin(s42Coordinate.Y / 1061259.7495020505d)
                + 0.000000000017026207045302084d * Math.Sin(s42Coordinate.Y / 795944.8121265379);

            // DOSTAČUJÍCÍ PŘESNOST
            // LAT (0,0000000; 0,0444900) m
            // LON (-0,0068361; 0,0109344) m
            //double R2 = s42Coordinate.Y / 6367558.4970123032d
            //    + 0.0025184647775237596d * Math.Sin(s42Coordinate.Y / 3183779.2485061516)
            //    + 0.0000036998858962069d * Math.Sin(s42Coordinate.Y / 1591889.6242530758d);

            double C1 = 0.0067385254146834989d * Math.Pow(Math.Cos(R2), 2);
            double T1 = Math.Pow(Math.Tan(R2), 2);
            double N1 = 6378245d / Math.Sqrt(1 - 0.0066934216229659511d * Math.Pow(Math.Sin(R2), 2));
            double D = (s42Coordinate.X - (500000 + Math.Truncate(s42Coordinate.X / 1000000) * 1000000)) / N1;
            double Fl1Rad = R2 - N1 * Math.Tan(R2) / 6335552.7170004258d * Math.Pow(1 - 0.0066934216229659511d * Math.Pow(Math.Sin(R2), 2), 1.5)
                * (0.5d * Math.Pow(D, 2) - (4.9393532712678487 + 3 * T1 + 10 * C1 - 4 * Math.Pow(C1, 2))
                    * Math.Pow(D, 4) / 24 + (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1
                    - 1.6981084045002417d - 3 * Math.Pow(C1, 2)) * Math.Pow(D, 6) / 720
                  );
            double LaRad = (0.36651914291880922d + 0.10471975511965977 * (Math.Truncate(s42Coordinate.X / 1000000) - 4))
                + (D - (1 + 2 * T1 + C1) * Math.Pow(D, 3) / 6 + (5 - 2 * C1 + 28 * T1 - 3 * Math.Pow(C1, 2) + 0.053908203317467991d + 24 * Math.Pow(T1, 2)) * Math.Pow(D, 5) / 120)
                / Math.Cos(R2);

            double dFIsec = (-26 * Math.Sin(Fl1Rad) * Math.Cos(LaRad) + 121 * Math.Sin(Fl1Rad) * Math.Sin(LaRad) - 78 * Math.Cos(Fl1Rad) + 2.7045797937100424d * Math.Sin(2 * Fl1Rad))
                / 110576.25484489677d * Math.Pow((1 - 0.0066934216229659329d * Math.Pow(Math.Sin(Fl1Rad), 2)), 1.5);
            double dLAsec = (-26 * Math.Sin(LaRad) - 121 * Math.Cos(LaRad)) / 111321.37574842962d * Math.Sqrt(1 - 0.0066934216229659329d * Math.Pow(Math.Sin(Fl1Rad), 2)) 
                / Math.Cos(Fl1Rad);

            return new WGS84Coordinate(Fl1Rad * 57.295779513082323d + dFIsec, LaRad * 57.295779513082323d + dLAsec);
        }


        /// <summary>
        /// Transformuje souřadnice WGS84 na UTM
        /// </summary>
        /// <param name="wgs84Coordinate"></param>
        /// <returns></returns>
        public static UTMCoordinate TransformUTM(WGS84Coordinate wgs84Coordinate)
        {
            return TransformUTM(wgs84Coordinate, UTMCoordinate.GetUtmZoneNumber(wgs84Coordinate));
        }

        /// <summary>
        /// Transformuje souřadnice WGS84 na UTM tak, že vnutí číslo zóny bez ohledu na
        /// zeměpisnou šířku. Metoda je použita k přepočtu na UTM33N.
        /// </summary>
        /// <remarks>
        /// Online kalkulátopr a zobrazovač:
        /// https://www.latlong.net/lat-long-utm.html
        /// </remarks>
        /// <param name="wgs84Coordinate"></param>
        /// <param name="zoneNumber">Číslo zóny.</param>
        /// <returns></returns>
        private static UTMCoordinate TransformUTM(WGS84Coordinate wgs84Coordinate, int zoneNumber)
        {
            var elipsoid = Elipsoid.GetDefault();
            var eccSquared = elipsoid.EccSquared;
            var a = elipsoid.A;

            var latRad = wgs84Coordinate.LatitudeRad; 
            var longRad = wgs84Coordinate.LongitudeRad; 

            var longOrigin = (zoneNumber - 1) * 6 - 180 + 3;  //+3 puts origin in middle of zone
            var longOriginRad = longOrigin * DEG_TO_RAD;

            var utmZone = UTMCoordinate.GetUtmZoneLetter(wgs84Coordinate.LatitudeDec);

            var eccPrimeSquared = eccSquared / (1 - eccSquared);

            var N = a / Math.Sqrt(1 - eccSquared * Math.Sin(latRad) * Math.Sin(latRad));
            var T = Math.Tan(latRad) * Math.Tan(latRad);
            var C = eccPrimeSquared * Math.Cos(latRad) * Math.Cos(latRad);
            var A = Math.Cos(latRad) * (longRad - longOriginRad);

            var M = a * ((1 - eccSquared / 4 - 3 * eccSquared * eccSquared / 64 - 5 * eccSquared * eccSquared * eccSquared / 256) * latRad
                    - (3 * eccSquared / 8 + 3 * eccSquared * eccSquared / 32 + 45 * eccSquared * eccSquared * eccSquared / 1024) * Math.Sin(2 * latRad)
                    + (15 * eccSquared * eccSquared / 256 + 45 * eccSquared * eccSquared * eccSquared / 1024) * Math.Sin(4 * latRad)
                    - (35 * eccSquared * eccSquared * eccSquared / 3072) * Math.Sin(6 * latRad));

            var utmEasting = 0.9996 * N * (A + (1 - T + C) * A * A * A / 6
                    + (5 - 18 * T + T * T + 72 * C - 58 * eccPrimeSquared) * A * A * A * A * A / 120)
                    + 500000.0;

            var utmNorthing = 0.9996 * (M + N * Math.Tan(latRad) * (A * A / 2 + (5 - T + 9 * C + 4 * C * C) * A * A * A * A / 24
                    + (61 - 58 * T + T * T + 600 * C - 330 * eccPrimeSquared) * A * A * A * A * A * A / 720));

            if (wgs84Coordinate.LatitudeDec < 0)
                utmNorthing += 10000000.0;

            return new UTMCoordinate { Easting = utmEasting, Northing = utmNorthing, ZoneNumber = zoneNumber, ZoneLetter = utmZone };
        }

        /// <summary>
        /// Transformuje souřadnice WGS84 na UTMzone
        /// </summary>
        /// <param name="wgs84Coordinate"></param>
        /// <param name="zoneNumber">Číslo 'base zóny'.</param>
        /// <returns></returns>
        public static UTMZoneCoordinate TransformUTMzone(WGS84Coordinate wgs84Coordinate, int zoneNumber)
        {
            var tempCoordinate = TransformUTM(wgs84Coordinate, zoneNumber);

            return new UTMZoneCoordinate(tempCoordinate.Easting, tempCoordinate.Northing, zoneNumber);
        }

    }

    #endregion //Transformation
}
