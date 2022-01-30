using NUnit.Framework;
using Debug.Like.A.Scientist;
using System;

namespace Debug.Like.A.Scientist.Tests
{
    [TestFixture]
    public class Tests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_NormalUsage()
        {
            double a = 5.8;

            for (int i = 0; i < 100; i++)
            {
                Dlas.Report(Math.Sin(i), "a")
                    .AsTimeSeries()
                    .Send();
            }
            
            Assert.Pass();
        }
    }
}