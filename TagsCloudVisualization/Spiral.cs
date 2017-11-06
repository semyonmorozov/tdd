using System;
using NUnit.Framework;
using System.Drawing;
using FluentAssertions;

namespace TagsCloudVisualization
{
    class Spiral
    {
        private readonly Point center;
        private readonly double spreading;

        public Spiral(Point center = default(Point), double spreading=0.1)
        {
            this.center = center;
            this.spreading = spreading;
        }

        public Point GetPoint(double angle)
        {
            var x = (int) (spreading * angle * Math.Cos(angle)) + center.X;
            var y = (int) (spreading * angle * Math.Sin(angle)) + center.Y;
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
        public void ReturnCorrectPoint_WithCustomAlpha_AndDefaultCenter(double spreading, double angle, int expectedX, int expectedY)
        {
            var spiral = new Spiral(spreading: spreading);
            spiral.GetPoint(angle).ShouldBeEquivalentTo(new Point(expectedX, expectedY));
        }

        [TestCase(2, 0, 0, 0, 0, 0)]
        [TestCase(2, 1, 0, 0, 1, 1)]
        [TestCase(2, 1, 1, 1, 2, 2)]
        [TestCase(2, 1, -1, -1, 0, 0)]
        [TestCase(2, 7, -5, 6, 5, 15)]
        public void ReturnCorrectPoint_WithCustomCenter_AndCustomAlpha(double spreading, double angle, int centerX, int centerY, int expectedX, int expectedY)
        {
            var spiral = new Spiral(new Point(centerX,centerY), spreading);
            spiral.GetPoint(angle).ShouldBeEquivalentTo(new Point(expectedX, expectedY));
        }
    }
}
