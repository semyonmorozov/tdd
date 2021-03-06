﻿using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    class UniquePositivePointsFromSpiral : IEnumerable, IEnumerator
    {
        private readonly Spiral spiral;
        private int index=-1;
        public UniquePositivePointsFromSpiral(Point spiralCenter)
        {
            spiral = new Spiral(spiralCenter);
        }

        public UniquePositivePointsFromSpiral(Point spiralCenter,double spreading)
        {
            spiral = new Spiral(spiralCenter,spreading);
        }
        public object Current => spiral.GetPoint(index);

        public bool MoveNext()
        {
            var oldPoint = Current;
            index++;
            while(oldPoint.Equals(Current)||!IsPositive((Point)Current))
                    index++;
            return true;
        }

        private bool IsPositive(Point point)
        {
            return point.X > 0 && point.Y > 0;
        }

        public void Reset()
        {
            index = -1;
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }
    }

    [TestFixture]
    public class UniquePositivePointsFromSpiral_Should
    {
        private UniquePositivePointsFromSpiral points;
        [SetUp]
        public void Init()
        {
            points = new UniquePositivePointsFromSpiral(new Point(0, 0));
        }

        [Test]
        public void ReturnUniquePoints()
        {
            var uniquePoints = new List<Point>();
            for (var i = 0; i < 40; i++)
            {
                points.MoveNext();
                uniquePoints.Add((Point)points.Current);
            }
            uniquePoints.Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void ReturnPositivePoints()
        {
            for (var i = 0; i < 40; i++)
            {
                points.MoveNext();
                var point = (Point) points.Current;
                point.X.Should().BePositive();
                point.Y.Should().BePositive();
            }
        }
    }
}
