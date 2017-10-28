using System;
using NUnit.Framework;
using System.Drawing;
using FluentAssertions;

namespace TagsCloudVisualization
{
    class Spiral
    {
        private double alpha;

        public Spiral(double alpha)
        {
            this.alpha = alpha;
        }

        public Point GetPoint(double angle)
        {
            return new Point((int)(alpha * angle * Math.Cos(angle)), (int)(alpha * angle * Math.Sin(angle)));
        }
    }

    [TestFixture]
    public class Spiral_Should
    {
        [Test]
        public void ReturnZeroPoint_WhenAngleIsZero()
        {
            var spiral = new Spiral(1);
            spiral.GetPoint(0).ShouldBeEquivalentTo(new Point(0,0));
        }

        [TestCase(2, 0, 0, 0)]
        [TestCase(2, 1, 1, 1)]
        [TestCase(2, 2, -1, 3)]
        [TestCase(2, 3, -5, 0)]
        [TestCase(2, 4, -5, -6)]
        [TestCase(2, 5, 2, -9)]
        [TestCase(2, 7, 10, 9)]
        [TestCase(2, 10, -16, -10)]
        public void ReturnCorrectPoint(double alpha, double angle, int expectedX, int expectedY)
        {
            var spiral = new Spiral(alpha);
            spiral.GetPoint(angle).ShouldBeEquivalentTo(new Point(expectedX, expectedY));
        }
    }
}
