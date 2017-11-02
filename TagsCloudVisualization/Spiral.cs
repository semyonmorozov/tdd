using System;
using NUnit.Framework;
using System.Drawing;
using FluentAssertions;

namespace TagsCloudVisualization
{
    class Spiral
    {
        private Point center = new Point(0,0);
        private double alpha = 0.1;

        public Spiral(Point center, double alpha)
        {
            this.center = center;
            this.alpha = alpha;
        }

        public Spiral(Point center)
        {
            this.center = center;
        }

        public Spiral(double alpha)
        {
            this.alpha = alpha;
        }

        public Spiral()
        {
            //Initialization spiral with default value of center and alpha
        }

        public Point GetPoint(double angle)
        {
            int x = (int) (alpha * angle * Math.Cos(angle)) + center.X;
            int y = (int) (alpha * angle * Math.Sin(angle)) + center.Y;
            return new Point(x, y);
        }
    }

    [TestFixture]
    public class Spiral_Should
    {
        [Test]
        public void ReturnZeroPoint_WhenSpiralIsDefault_AndAngleIsZero()
        {
            var spiral = new Spiral();
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
        public void ReturnCorrectPoint_WithCustomAlpha_AndDefaultCenter(double alpha, double angle, int expectedX, int expectedY)
        {
            var spiral = new Spiral(alpha);
            spiral.GetPoint(angle).ShouldBeEquivalentTo(new Point(expectedX, expectedY));
        }

        [TestCase(2, 0, 0, 0, 0, 0)]
        [TestCase(2, 1, 0, 0, 1, 1)]
        [TestCase(2, 1, 1, 1, 2, 2)]
        [TestCase(2, 1, -1, -1, 0, 0)]
        [TestCase(2, 7, -5, 6, 5, 15)]
        public void ReturnCorrectPoint_WithCustomCenter_AndCustomAlpha(double alpha, double angle, int centerX, int centerY, int expectedX, int expectedY)
        {
            var spiral = new Spiral(new Point(centerX,centerY), alpha);
            spiral.GetPoint(angle).ShouldBeEquivalentTo(new Point(expectedX, expectedY));
        }
    }
}
