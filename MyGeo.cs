using System;
using System.Collections.Generic;
using System.Drawing;

namespace IDEA.Library
{
	/// <summary>
	/// Knihovna geometrických funkcí.
	/// </summary>
	public class MyGeo
	{
		/// <summary>
		/// Označuje kvadrant.
		/// </summary>
		/// <remarks>
		/// Jedná se o určení počátku souřadnic [0, 0].
		/// </remarks>
		public enum Quadrant
		{
			/// <summary>
			/// Kvadrant nelze určit (šířka nebo výška jsou nulové).
			/// </summary>
			Unknown,

			/// <summary>
			/// První kvadrant, počátek souřadnic je vlevo dole.
			/// </summary>
			Quadrant1, 

			/// <summary>
			/// Druhý kvadrant, počátek souřadnic je vpravo dole.
			/// </summary>
			Quadrant2,

			/// <summary>
			/// Třetí kvadrant, počátek souřadnic je vpravo nahoře.
			/// </summary>
			Quadrant3,

			/// <summary>
			/// Čtvrtý kvadrant, počátek souřadnic je vlevo nahoře.
			/// </summary>
			Quadrant4
		}

		/// <summary>
		/// Velikost jednoho radiánu ve stupních (RAD - DEG).
		/// </summary>
		public const double OneRAD = 180d / Math.PI;

		private MyGeo()
		{}

        /// <summary>
        /// Převede úhel ve stupních do radiánů.
        /// </summary>
        /// <param name="angleInDEG"></param>
        /// <returns></returns>
	    public static double ToRAD(double angleInDEG)
	    {
            return angleInDEG / OneRAD;
	    }

        /// <summary>
        /// Převede úhel v radiánech do stupňů.
        /// </summary>
        /// <param name="angleInRAD"></param>
        /// <returns></returns>
        public static double ToDEG(double angleInRAD)
        {
            return angleInRAD * OneRAD;
        }
        
        /// <summary>
		/// Určí kvadrant zadané plochy (bounds je dle Geo).
		/// </summary>
		/// <param name="bounds">Plocha.</param>
		/// <remarks>Pozor! Win kreslí v Q4.</remarks>
		/// <returns>Kvadrant.</returns>
		public static Quadrant GetQuadrant(RectangleF bounds)
		{
			int xSign = Math.Sign(bounds.Width);
			int ySign = Math.Sign(bounds.Height);

			if (xSign == 1 && ySign == 1)
				return Quadrant.Quadrant1;

			if (xSign == -1 && ySign == 1)
				return Quadrant.Quadrant2;

			if (xSign == -1 && ySign == -1)
				return Quadrant.Quadrant3;

			if (xSign == 1 && ySign == -1)
				return Quadrant.Quadrant4;

			return Quadrant.Unknown;
			
		}

		/// <summary>
		/// Určí kvadrant zadané plochy.
		/// </summary>
		/// <param name="bounds">Plocha.</param>
		/// <returns>Kvadrant.</returns>
		public static Quadrant GetQuadrant(Rectangle bounds)
		{
			return GetQuadrant(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height));
		}

		/// <summary>
		/// Úhel mezi dvěma body.
		/// </summary>
		/// <remarks>
		/// Úhel, který svírá určitý směr (směr k pozorovanému objektu, ...)
		/// od směru východního. Úhel je orientovaný, zaleží tedy na směru měření úhlu.
		/// Měří se proti směru pohybu hodinových ručiček, tj. od východu k severu. 
		/// Měří se ve stupních.
		/// </remarks>
		/// <param name="basePoint">Výchozí bod (bod ze kterého se díváme do cíle).</param>
		/// <param name="targetPoint">Cílový bod (bod na který se díváme z cíle).</param>
		/// <returns>Úhel.</returns>
		public static double GetAngle(PointF basePoint, PointF targetPoint)
		{
			float xDelta = targetPoint.X - basePoint.X;
			float yDelta = targetPoint.Y - basePoint.Y;

			if (xDelta == 0) //protože v Atan se pak dělí nulou
				return 180 - 90 * Math.Sign(yDelta); //90 nebo 180
			else if (xDelta < 0) //2. a 3. kvadrant
				return 180 + Math.Atan(yDelta / xDelta) * OneRAD;
			else if (yDelta < 0) //4.kvadrant
				return 360 + Math.Atan(yDelta / xDelta) * OneRAD;
			else //1.kvadrant
				return Math.Atan(yDelta / xDelta) * OneRAD;
		}

		/// <summary>
		/// Azimut mezi dvěma body.
		/// </summary>
		/// <remarks>
		/// <seealso cref="Angle2Azimuth"/>
		/// </remarks>
		/// <param name="basePoint">Výchozí bod (bod ze kterého se díváme do cíle).</param>
		/// <param name="targetPoint">Cílový bod (bod na který se díváme z cíle).</param>
		/// <returns>Azimut.</returns>
		public static double GetAzimuth(PointF basePoint, PointF targetPoint)
		{
			return Angle2Azimuth( GetAngle(basePoint, targetPoint) );
		}

		/// <summary>
		/// Převede geometrický úhel na azimut.
		/// </summary>
		/// <remarks>
		/// Azimut je orientovaný úhel, který svírá určitý směr (směr k pozorovanému objektu, ...)
		/// od směru severního. Úhel je orientovaný, zaleží tedy na směru měření úhlu.
		/// Měří se po směru pohybu hodinových ručiček, tj. od severu k východu. 
		/// Měří se ve stupních.
		/// Z definice vyplývá, že sever má azimut 0°, východ 90°, jih 180° a západ 270°.
		/// </remarks>
		/// <param name="angle">Úhel.</param>
		/// <returns>Azimut.</returns>
		public static double Angle2Azimuth(double angle)
		{
			if(angle > 90)
				return 450-angle;
			else //if (angle<=90)
				return 90-angle;
		}

		/// <summary>
		/// Dopočítá podle zadaných parametrů další body do úseku.
		/// </summary>
		/// <remarks>
		/// Do úseku mezi zadanými body (...Point) vloží v zadané vzdálenosti (step)
		/// další body, které vrátí v zadaném poli.
		/// Vzdálenost posledního bodu je ponechána tak, jak vyjde.
		/// Hraniční body (...Point) nejsou zahrnuty.
		/// </remarks>
		/// <param name="basePoint">Výchozí bod (bod ze kterého se díváme do cíle).</param>
		/// <param name="targetPoint">Cílový bod (bod na který se díváme z cíle).</param>
		/// <param name="step">Krok.</param>
		/// <returns>Pole bodů.</returns>
		public static PointF[] GetPoints(PointF basePoint, PointF targetPoint, double step)
		{
			if (step == 0)
				return null;
			else
			{
				double angle = GetAngle(basePoint, targetPoint);
				double distanceBT = Distance(basePoint, targetPoint);
				int count = (int)(distanceBT/step)-1;
				PointF[] result = null;

				if (count > 0)
					result = new PointF[count];

				for(int index=1; index <= count; index++)
					result[index-1] = GetPoint(basePoint, angle, step*index);

				return result;
			}
		}

		/// <summary>
		/// Dopočítá podle zadaných parametrů další body do úseku.
		/// Body jsou však posunuty o step/2 směrem k basePoint.
		/// </summary>
		/// <remarks>
		/// Do úseku mezi zadanými body (...Point) vloží v zadané vzdálenosti (step)
		/// další body, které vrátí v zadaném poli.
		/// Vzdálenost posledního bodu je ponechána tak, jak vyjde.
		/// Hraniční body (...Point) nejsou zahrnuty.
		/// Používá se např. k výpočtu plochy nad výškopisem.
		/// </remarks>
		/// <param name="basePoint">Výchozí bod (bod ze kterého se díváme do cíle).</param>
		/// <param name="targetPoint">Cílový bod (bod na který se díváme z cíle).</param>
		/// <param name="step">Krok.</param>
		/// <returns>Pole bodů.</returns>
		public static PointF[] GetPointsForIntegral(PointF basePoint, PointF targetPoint, double step)
		{
			if (step == 0)
				return null;
			else
			{
				double angle = GetAngle(basePoint, targetPoint); //úhel mezi krajními body
				double distanceBT = Distance(basePoint, targetPoint); //vzdálenost mezi krajními body
				int count = (int)(distanceBT/step); //počet bodů

				PointF[] result = new PointF[count];

				for(int index=1; index <= count; index++)
				{
					result[index-1] = GetPoint(basePoint, angle, step*(index-0.5));
				}

				return result;
			}
		}

		/// <summary>
		/// Vrací bod vzdálený od výchozího bodu o zadanou vzdálenost pod zadaným úhlem.
		/// </summary>
		/// <param name="basePoint">Výchozí bod (bod ze kterého se díváme do cíle).</param>
		/// <param name="angle">Úhel.</param>
		/// <param name="distance">Vzdálenost bodu od 'basePoint'.</param>
		/// <returns>Bod.</returns>
		public static PointF GetPoint(PointF basePoint, double angle, double distance)
		{
			return new PointF(
				Convert.ToSingle( Math.Round( basePoint.X + Math.Cos(angle/OneRAD)*distance, 5))
				, Convert.ToSingle( Math.Round( basePoint.Y + Math.Sin(angle/OneRAD)*distance, 5))
				);
		}

		/// <summary>
		/// Vypočítá vzdálenost mezi dvěma zadanými body.
		/// </summary>
		/// <param name="basePoint">Výchozí bod (bod ze kterého se díváme do cíle).</param>
		/// <param name="targetPoint">Cílový bod (bod na který se díváme z cíle).</param>
		/// <returns>Vzdálenost mezi zadanými body.</returns>
		public static double Distance(PointF basePoint, PointF targetPoint)
		{
			float xDelta = targetPoint.X - basePoint.X;
			float yDelta = targetPoint.Y - basePoint.Y;

			return Math.Sqrt( Math.Pow(xDelta, 2) + Math.Pow(yDelta, 2) );
		}

		/// <summary>
		/// Lineární interpolace.
		/// </summary>
		/// <remarks>
		/// Lineárně interpoluje hodnoty mezi 'startValue' a 'endValue'
		/// do pole o počtu prvků 'count'.
		/// Pole obsahuje 'startValue' s indexem 0 a 'endValue'
		/// s indexem 'count'-1.
		/// </remarks>
		/// <param name="startValue">Počáteční hodnota interpolace.</param>
		/// <param name="endValue">Koncová hodnota interpolace.</param>
		/// <param name="count">Počet prvků pole interpolovaných hodnot.</param>
		/// <returns>Pole interpolovaných hodnot.</returns>
		public static double[] LinearInterpolation(double startValue, double endValue, int count)
		{
			double delta = endValue - startValue; //rozdíl hodnot
			double k = delta / (count-1); //koeficient lineární interpolace
			double[] result = new double[count]; //pole s výsledky

			for(int index=0; index < count; index++)
				result[index] = startValue + k*index;

			return result;
		}


		/// <summary>
		/// Lineárně interpolovaná hodnota mezi dvěma body.
		/// </summary>
		/// <param name="x1">X-ová souřadnice výchozího bodu.</param>
		/// <param name="y1">Y-ová souřadnice výchozího bodu.</param>
		/// <param name="x2">X-ová souřadnice koncového bodu.</param>
		/// <param name="y2">Y-ová souřadnice koncového bodu.</param>
		/// <param name="x">X-ová souřadnice bodu pro který zjišťujeme Y.</param>
		/// <returns>Y.</returns>
		public static double LinearInterpolationValue(double x1, double y1, double x2, double y2, double x)
		{
			return y1 + ((y2 - y1) * (x-x1)) / (x2 - x1);
		}

		/// <summary>
		/// Vrací střed úseku zadaného koncovými body.
		/// </summary>
		/// <param name="basePoint">Výchozí bod.</param>
		/// <param name="targetPoint">Koncový bod.</param>
		/// <returns>Střed úseku.</returns>
		public static PointF GetCenterPoint(PointF basePoint, PointF targetPoint)
		{
			double distance = Distance(basePoint, targetPoint);
			double angle = GetAngle(basePoint, targetPoint);

			return GetPoint(basePoint, angle, distance / 2f);
		}

		/// <summary>
		/// Vytvoří pole bodů vyplňující zadanou plochu.
		/// </summary>
		/// <param name="startPoint">Počáteční souřadnice.</param>
		/// <param name="countX">Počet kroků v ose X.</param>
		/// <param name="countY">Počet kroků v ose Y.</param>
		/// <param name="stepX">Krok v ose X.</param>
		/// <param name="stepY">Krok v ose Y.</param>
		/// <returns>Pole bodů.</returns>
		public static PointF[] GeneratePoints(PointF startPoint, int countX, int countY, double stepX, double stepY)
		{
			//--- příprava
			countX = Convert.ToInt32(Math.Abs(countX));
			countY = Convert.ToInt32(Math.Abs(countY));

			PointF[] result = new PointF[countX * countY];
			int index = 0;
			//---

			for (int indexY = 0; indexY < countY; indexY++)
				for (int indexX = 0; indexX < countX; indexX++)
					result[index++] = new PointF(
						Convert.ToSingle(startPoint.X + (indexX * stepX))
						, Convert.ToSingle(startPoint.Y + (indexY * stepY))
						);

			return result;
		}

		/// <summary>
		/// Vytvoří pole bodů vyplňující zadanou plochu.
		/// </summary>
		/// <param name="startPoint">Počáteční souřadnice.</param>
		/// <param name="endPoint">Koncová souřadnice.</param>
		/// <param name="countX">Počet bodů v ose X.</param>
		/// <param name="countY">Počet bodů v ose Y.</param>
		/// <returns>Pole bodů.</returns>
		public static PointF[] GeneratePoints(PointF startPoint, PointF endPoint, int countX, int countY)
		{
			//--- příprava
			double stepX = (endPoint.X - startPoint.X) / (Math.Abs(countX) - 1);
			double stepY = (endPoint.Y - startPoint.Y) / (Math.Abs(countY) - 1);
			//---

			return GeneratePoints(startPoint, countX, countY, stepX, stepY);
		}

		/// <summary>
		/// Vytvoří pole bodů vyplňující zadanou plochu.
		/// </summary>
		/// <param name="startPoint">Počáteční souřadnice.</param>
		/// <param name="endPoint">Koncová souřadnice.</param>
		/// <param name="deltaX">Krok v ose X (použije se jeho absolutní hodnota).</param>
		/// <param name="deltaY">Krok v ose Y (použije se jeho absolutní hodnota).</param>
		/// <returns>Pole bodů.</returns>
		public static PointF[] GeneratePoints(PointF startPoint, PointF endPoint, double deltaX, double deltaY)
		{
			//--- příprava
			List<PointF> points = new List<PointF>();			

			double stepX = (startPoint.X <= endPoint.X) ? Math.Abs(deltaX) : -Math.Abs(deltaX);
			double stepY = (startPoint.Y <= endPoint.Y) ? Math.Abs(deltaY) : -Math.Abs(deltaY);
			//---

			for (double y = startPoint.Y; (stepY > 0 && y < endPoint.Y) || (stepY < 0 && y > endPoint.Y); y += stepY)
			    for (double x = startPoint.X; (stepX > 0 && x < endPoint.X) || (stepX < 0 && x > endPoint.X); x += stepX)
			        points.Add(new PointF(Convert.ToSingle(x), Convert.ToSingle(y)));

			PointF[] result = new PointF[points.Count];
			
			points.CopyTo(result);

			return result;
		}

		/// <summary>
		/// Vrací normalizovaný obdélník.
		/// </summary>
		/// <remarks>
		/// Normalizovaný obdélník má všechny rozměry kladné (Width i Height).
		/// </remarks>
		/// <param name="rectangle">Obdélník.</param>
		/// <returns>Normalizovaný obdélník.</returns>
		public static RectangleF GetNormalizeRectangle(RectangleF rectangle)
		{
			float x1 = (rectangle.Width >= 0) ? rectangle.Left : rectangle.Right;
			float x2 = (rectangle.Width >= 0) ? rectangle.Right : rectangle.Left;

			float y1 = (rectangle.Height >= 0) ? rectangle.Top : rectangle.Bottom;
			float y2 = (rectangle.Height >= 0) ? rectangle.Bottom : rectangle.Top;

			return RectangleF.FromLTRB(x1, y1, x2, y2);
		}

		/// <summary>
		/// Vrací normalizovaný obdélník.
		/// </summary>
		/// <remarks>
		/// Normalizovaný obdélník má všechny rozměry kladné (Width i Height).
		/// </remarks>
		/// <param name="rectangle">Obdélník.</param>
		/// <returns>Normalizovaný obdélník.</returns>
		public static Rectangle GetNormalizeRectangle(Rectangle rectangle)
		{
			int x1 = (rectangle.Width >= 0) ? rectangle.Left : rectangle.Right;
			int x2 = (rectangle.Width >= 0) ? rectangle.Right : rectangle.Left;

			int y1 = (rectangle.Height >= 0) ? rectangle.Top : rectangle.Bottom;
			int y2 = (rectangle.Height >= 0) ? rectangle.Bottom : rectangle.Top;

			return Rectangle.FromLTRB(x1, y1, x2, y2);
		}
	}
}

//#if DEBUG

//namespace NUnit.Tests
//{
//    using System;
//    using System.Data;
//    using NUnit.Framework;
	
//    using IDEA.Library; //testovaný namespace

//    [TestFixture]
//    public class MyGeoTests
//    {
//        [Test]
//        [Category("MyGeo")]
//        public void GetAngle2AzimuthTest()
//        {
//            for(int angle=0; angle<=90; angle++)
//                Assert.AreEqual(90-angle, MyGeo.Angle2Azimuth(angle));

//            for(int angle=91; angle<360; angle++)
//                Assert.AreEqual(450-angle, MyGeo.Angle2Azimuth(angle));
//        }

//        [Test]
//        [Category("MyGeo")]
//        public void GetAngleTest()
//        {
//            PointF basePoint = new PointF(0,0);
			
//            //--- Kontrola postupným vytvářením souřadnic bodů pod známým úhlem,
//            // a porovnáním vypočítaného úhlu k těmto bodům.
//            for(int deg=0; deg<360; deg++)
//            {
//                double angle = deg / (MyGeo.OneRAD);
//                PointF targetPoint = new PointF(Convert.ToSingle(Math.Cos(angle)), Convert.ToSingle(Math.Sin(angle)));

//                Assert.AreEqual(deg, Math.Round( MyGeo.GetAngle(basePoint, targetPoint), 5 ) );
//            }
//            //---

//            //--- Testy na -0 v souřadnících
//            Assert.AreEqual(90, MyGeo.GetAngle(basePoint, new PointF(-0f,  10f)));
//            Assert.AreEqual(270, MyGeo.GetAngle(basePoint, new PointF(-0f,  -10f)));
//            //---

//        }

//        [Test]
//        [Category("MyGeo")]
//        public void DistanceTest()
//        {
//            PointF basePoint = new PointF(1,3);
//            PointF targetPoint = new PointF(3+1,4+3);

//            Assert.AreEqual(5, MyGeo.Distance(basePoint, targetPoint));
//            Assert.AreEqual(5, MyGeo.Distance(targetPoint, basePoint));
//        }

//        [Test]
//        [Category("MyGeo")]
//        public void GetPointTest()
//        {
//            PointF basePoint = new PointF(2,3);

//            for(int angle = 0; angle<360; angle++)
//            {
//                PointF targetPoint = MyGeo.GetPoint(basePoint, angle, 10);

//                int controlAngle = Convert.ToInt32( MyGeo.GetAngle(basePoint, targetPoint) );

//                Assert.AreEqual(angle, controlAngle);

//                //kontrolní výpis
//                //System.Console.Out.WriteLine(String.Format("angle:{0}; point:{1}; cntrl.angle:{2}", angle, targetPoint, controlAngle));
//            }
//        }

//        [Test]
//        [Category("MyGeo")]
//        public void GetPointsTest()
//        {
//            PointF basePoint = new PointF(0+1,0+2);
			
//            PointF targetPoint;
//            PointF[] aPoints;

////			//--- test 1
////			targetPoint = new PointF(10+1,10+2);
////			aPoints = MyGeo.GetPoints(basePoint, targetPoint, 1);
////
////			foreach(PointF point in aPoints)
////			{
////				Assert.AreEqual(point.X, point.Y, point.ToString() + " (x=y)");
//////				System.Console.Out.WriteLine(String.Format("{0}", point));
////			}
////			//---

//            //--- test 2
//            targetPoint = new PointF(0+1,10+2);
//            aPoints = MyGeo.GetPoints(basePoint, targetPoint, 1);
//            Assert.AreEqual(9, aPoints.Length, " test 2: aPoints.Length");

//            int y=3;

//            foreach(PointF point in aPoints)
//            {
//                Assert.AreEqual(1, point.X, point.ToString() + " test 3: (x)");
//                Assert.AreEqual(y++, point.Y, point.ToString() + " test 3: (y)");
//            }
//            //---

//            //--- test 3
//            basePoint = new PointF(0,0);

//            GetPointsSubTest(basePoint, 10, 5, 2.0,  4);
//            GetPointsSubTest(basePoint, 10, 0, 0.5, 19);
//            //---
//        }

//        private void GetPointsSubTest(PointF basePoint, float value1, float value2, double step, double count)
//        {
//            string message = String.Format(" val1: {0}; val2: {1}; step: {2}; count: {3}", value1, value2, step, count);

//            //--- vytvoření inverzní hvězdice
//            PointF[] aPoints0 = MyGeo.GetPoints(basePoint,new PointF( value1,  value2), step); // 10,  5
//            PointF[] aPoints1 = MyGeo.GetPoints(basePoint,new PointF( value2,  value1), step); //  5, 10
//            PointF[] aPoints2 = MyGeo.GetPoints(basePoint,new PointF(-value2,  value1), step); // -5, 10
//            PointF[] aPoints3 = MyGeo.GetPoints(basePoint,new PointF(-value1,  value2), step); //-10,  5
//            PointF[] aPoints4 = MyGeo.GetPoints(basePoint,new PointF(-value1, -value2), step); //-10, -5
//            PointF[] aPoints5 = MyGeo.GetPoints(basePoint,new PointF(-value2, -value1), step); // -5,-10
//            PointF[] aPoints6 = MyGeo.GetPoints(basePoint,new PointF( value2, -value1), step); //  5,-10
//            PointF[] aPoints7 = MyGeo.GetPoints(basePoint,new PointF( value1, -value2), step); // 10, -5
//            //---

//            //--- testy délky
//            Assert.AreEqual(count, aPoints0.Length, "length 0");
//            Assert.AreEqual(count, aPoints1.Length, "length 1");
//            Assert.AreEqual(count, aPoints2.Length, "length 2");
//            Assert.AreEqual(count, aPoints3.Length, "length 3");
//            Assert.AreEqual(count, aPoints4.Length, "length 4");
//            Assert.AreEqual(count, aPoints5.Length, "length 5");
//            Assert.AreEqual(count, aPoints6.Length, "length 6");
//            Assert.AreEqual(count, aPoints7.Length, "length 7");
//            //---

////			System.Console.Out.WriteLine("--------------------------");
////
////			for(int index=0; index<count; index++)
////			{
////				System.Console.Out.WriteLine(String.Format("aPoints0[{0}]: {1}", index, aPoints0[index]));
////				System.Console.Out.WriteLine(String.Format("aPoints1[{0}]: {1}", index, aPoints1[index]));
////				System.Console.Out.WriteLine(String.Format("aPoints2[{0}]: {1}", index, aPoints2[index]));
////			}

//            //--- testy hodnot
//            for(int index=0; index<count; index++)
//            {
//                //test bodu 1
//                Assert.AreEqual( aPoints0[index].X,  aPoints1[index].Y, "t1Y" + message + String.Format("; index: {0}", index) );
//                Assert.AreEqual( aPoints0[index].Y,  aPoints1[index].X, "t1X" + message + String.Format("; index: {0}", index) );

//                //test bodu 2
//                Assert.AreEqual( aPoints0[index].X,  aPoints2[index].Y, "t2Y" + message + String.Format("; index: {0}", index) ); //*
//                Assert.AreEqual(-aPoints0[index].Y,  aPoints2[index].X, "t2X" + message + String.Format("; index: {0}", index) );

//                //test bodu 3
//                Assert.AreEqual( aPoints0[index].Y,  aPoints3[index].Y, "t3Y" + message + String.Format("; index: {0}", index) );
//                Assert.AreEqual(-aPoints0[index].X,  aPoints3[index].X, "t3X" + message + String.Format("; index: {0}", index) );

//                //test bodu 4
//                Assert.AreEqual(-aPoints0[index].Y,  aPoints4[index].Y, "t4Y" + message + String.Format("; index: {0}", index) );
//                Assert.AreEqual(-aPoints0[index].X,  aPoints4[index].X, "t4X" + message + String.Format("; index: {0}", index) );

//                //test bodu 5
//                Assert.AreEqual(-aPoints0[index].X,  aPoints5[index].Y, "t5Y" + message + String.Format("; index: {0}", index) );
//                Assert.AreEqual(-aPoints0[index].Y,  aPoints5[index].X, "t5X" + message + String.Format("; index: {0}", index) );

//                //test bodu 6
//                Assert.AreEqual(-aPoints0[index].X,  aPoints6[index].Y, "t6Y" + message + String.Format("; index: {0}", index) );
//                Assert.AreEqual( aPoints0[index].Y,  aPoints6[index].X, "t6X" + message + String.Format("; index: {0}", index) );

//                //test bodu 7
//                Assert.AreEqual(-aPoints0[index].Y,  aPoints7[index].Y, "t7Y" + message + String.Format("; index: {0}", index) );
//                Assert.AreEqual( aPoints0[index].X,  aPoints7[index].X, "t7X" + message + String.Format("; index: {0}", index) );
//            }
//            //---

//        }
		
//        /// <summary>
//        /// Azimut mezi zadanými body počítaný původním algoritmem od p.Beňo.
//        /// Funkce je zahrnuta kvůli kontrole nově napsané funkce.
//        /// </summary>
//        /// <param name="point1">První bod (zdroj).</param>
//        /// <param name="point2">Druhy bod (ref.bod).</param>
//        /// <returns>Azimut.</returns>
//        private float GetAzimuthByBeno(PointF point1, PointF point2)
//        {
//            float X_delta = point1.X - point2.X;
//            float Y_delta = point1.Y - point2.Y;
//            float SgnX;
//            float SgnY;
//            float Delta = 0;

//            if (X_delta<0)
//                SgnX=-1;
//            else
//                SgnX=1;
//            if (X_delta==0)
//                SgnX=0;

//            if (Y_delta<0)
//                SgnY=-1;
//            else
//                SgnY=1;
//            if (Y_delta==0)
//                SgnY=0;

//            if (X_delta==0)
//                Delta=90 - (90*SgnY);
//            if (Y_delta==0)
//                Delta=180 - (90*SgnX);
//            if ((X_delta!=0) && (Y_delta!=0))
//                Delta=Convert.ToSingle( (Math.Atan(X_delta/Y_delta)*180/Math.PI) + 90*(2-(SgnX*(1+SgnY))) );

//            return Delta;

						
//            //záložní výpočet, duplicitní s GetAngle, 
//            //použít jen v případě rychlostních problémů

//            //			float xDelta = targetPoint.X - basePoint.X;
//            //			float yDelta = targetPoint.Y - basePoint.Y;
//            //
//            //			if (xDelta==0)
//            //				return 90 - 90 * Math.Sign(yDelta);
//            //			else if (yDelta==0)
//            //				return 180 - 90 * Math.Sign(xDelta);
//            //			else //((X_delta!=0) && (Y_delta!=0))
//            //				return Convert.ToSingle( 
//            //					Math.Atan(xDelta / yDelta) * (180f / Math.PI) 
//            //					+ 90 * (2-(Math.Sign(xDelta) * (1+Math.Sign(yDelta))))
//            //					);

//        }

//        [Test]
//        [Category("MyGeo")]
//        public void LinearInterpolationTest()
//        {
//            double[] interpolateValues = MyGeo.LinearInterpolation(6, 24, 10);
//            double[] validValues = new double[] {6,8,10,12,14,16,18,20,22,24};
			
//            //--- kontrola
//            for(int index=0; index < interpolateValues.Length; index++)
//                Assert.AreEqual(validValues[index], interpolateValues[index]);
//            //---
//        }

//        [Test]
//        [Category("MyGeo")]
//        public void LinearInterpolationValueTest()
//        {
//            double[] validValues = new double[] {-0.5,0,0.5,1,1.5,2,2.5,3,3.5,4,4.5,5,5.5,6};
			
//            for(int x=0; x<=13; x++)
//                Assert.AreEqual(validValues[x], MyGeo.LinearInterpolationValue(3,1,13,6,x));
//        }

//        [Test]
//        [Category("MyGeo")]
//        public void GetCenterPointTest()
//        {
//            for(double angle = 0; angle < 2*Math.PI; angle += 0.1)
//            {
//                PointF basePoint = new PointF( (float)Math.Cos(angle), (float)Math.Sin(angle));
//                PointF targetPoint = new PointF(-(float)Math.Cos(angle), -(float)Math.Sin(angle));

//                PointF centerPoint = MyGeo.GetCenterPoint(basePoint, targetPoint);

//                Assert.AreEqual(0f, centerPoint.X, String.Format("Chyba (x) vznikla při úhlu {0} rad.", angle));
//                Assert.AreEqual(0f, centerPoint.Y, String.Format("Chyba (y) vznikla při úhlu {0} rad.", angle));
//            }
//        }

//        [Test]
//        [Category("MyGeo")]
//        public void GetQuadrantTest()
//        {
//            GetQuadrantTest(11, 22, MyGeo.Quadrant.Quadrant1);
//            GetQuadrantTest(-11, 22, MyGeo.Quadrant.Quadrant2);
//            GetQuadrantTest(-11, -22, MyGeo.Quadrant.Quadrant3);
//            GetQuadrantTest(11, -22, MyGeo.Quadrant.Quadrant4);

//            GetQuadrantTest(0, 22, MyGeo.Quadrant.Unknown);
//            GetQuadrantTest(0, -22, MyGeo.Quadrant.Unknown);
//            GetQuadrantTest(11, 0, MyGeo.Quadrant.Unknown);
//            GetQuadrantTest(-11, 0, MyGeo.Quadrant.Unknown);
//            GetQuadrantTest(0, 0, MyGeo.Quadrant.Unknown);
//        }

//        private void GetQuadrantTest(int width, int height, MyGeo.Quadrant expectedQuadrant)
//        {
//            RectangleF recF = new RectangleF(0, 0, width, height);
//            Rectangle rec = new Rectangle(0, 0, width, height);

//            Assert.AreEqual(expectedQuadrant, MyGeo.GetQuadrant(recF));
//            Assert.AreEqual(expectedQuadrant, MyGeo.GetQuadrant(rec));
//        }

//        [Test]
//        [Category("MyGeo")]
//        public void GetNormalizeRectangleF()
//        {

//            Assert.IsTrue(IsNormalizeRectangle(MyGeo.GetNormalizeRectangle(RectangleF.FromLTRB( 1, 20, 3, 40))),"Step 1");
//            Assert.IsTrue(IsNormalizeRectangle(MyGeo.GetNormalizeRectangle(RectangleF.FromLTRB(10, 2, 3, 40))), "Step 2");
//            Assert.IsTrue(IsNormalizeRectangle(MyGeo.GetNormalizeRectangle(RectangleF.FromLTRB(1, 20, 30, 4))), "Step 3");
//            Assert.IsTrue(IsNormalizeRectangle(MyGeo.GetNormalizeRectangle(RectangleF.FromLTRB(10, 2, 30, 4))), "Step 4");
//        }

//        private bool IsNormalizeRectangle(RectangleF rectangle)
//        {
//            return (rectangle.Width >= 0 && rectangle.Height >= 0);
//        }

//        [Test]
//        [Category("MyGeo")]
//        public void GetNormalizeRectangle()
//        {

//            Assert.IsTrue(IsNormalizeRectangle(MyGeo.GetNormalizeRectangle(Rectangle.FromLTRB(1, 20, 3, 40))), "Step 1");
//            Assert.IsTrue(IsNormalizeRectangle(MyGeo.GetNormalizeRectangle(Rectangle.FromLTRB(10, 2, 3, 40))), "Step 2");
//            Assert.IsTrue(IsNormalizeRectangle(MyGeo.GetNormalizeRectangle(Rectangle.FromLTRB(1, 20, 30, 4))), "Step 3");
//            Assert.IsTrue(IsNormalizeRectangle(MyGeo.GetNormalizeRectangle(Rectangle.FromLTRB(10, 2, 30, 4))), "Step 4");
//        }

//        private bool IsNormalizeRectangle(Rectangle rectangle)
//        {
//            return (rectangle.Width >= 0 && rectangle.Height >= 0);
//        }

//    }
//}


//#endif