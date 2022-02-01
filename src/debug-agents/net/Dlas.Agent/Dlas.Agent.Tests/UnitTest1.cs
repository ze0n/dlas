using NUnit.Framework;
using Debug.Like.A.Scientist;
using System;
using System.Threading;

namespace Debug.Like.A.Scientist.Tests
{
    [TestFixture]
    public class Tests
    {

        [SetUp]
        public void Setup()
        {
        }

        private double sample_normal(double mean, double stdDev)
        {
            Random rand = new Random(); //reuse this if you are generating many
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }

        [Test]
        public void Test_NormalUsage()
        {
            for (int i = 0; i < 300; i++)
            {
                var b = Math.Sin(i/3.0);

                var a = sample_normal(+1.0, 1.0);

                // (a,b,c,d,e) x<-a y<-c, bde -> selector

                Dlas.Report(a, "A")
                    .AsTimeSeries()
                    .Send();

                Dlas.Report(b, "B")
                    .AsTimeSeries()
                    .Send();

                Thread.Sleep(1000);
            }
            
            Assert.Pass();
        }
    }
}