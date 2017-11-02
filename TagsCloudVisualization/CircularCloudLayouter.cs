﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    class CircularCloudLayouter
    {
        private Point center;

        private UniquePositivePointsFromSpiral uniquePositivePoints;

        private List<Rectangle> layout = new List<Rectangle>();

        public Point GetCenter()
        {
            return center;
        }

        public List<Rectangle> GetLayout()
        {
            return layout;
        }

        public CircularCloudLayouter(Point center)
        {
            this.center = center;
            this.uniquePositivePoints = new UniquePositivePointsFromSpiral(center);
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            uniquePositivePoints.MoveNext();
            var rectangle = new Rectangle((Point)uniquePositivePoints.Current, rectangleSize);
            while (layout.Any(r => r.IntersectsWith(rectangle)))
            {
                uniquePositivePoints.MoveNext();
                rectangle = new Rectangle((Point)uniquePositivePoints.Current, rectangleSize);
            }
            layout.Add(rectangle);
            return rectangle;
        }
    }

    [TestFixture]
    public class CircularCloudLayouter_Should
    {
        private CircularCloudLayouter layouter;

        [SetUp]
        public void Init()
        {
            layouter = new CircularCloudLayouter(new Point(960, 540));
        }

        [TestCase(1, 2)]
        public void ReturnCorrectCenter_AfterInitialization(int x, int y)
        {
            Point center = new Point(x, y);
            layouter = new CircularCloudLayouter(center);
            layouter.GetCenter().Should().Be(center);
        }

        [Test]
        public void AddingRectanglesToLayout()
        {
            Random rnd = new Random();
            for (int i = 0; i < 150; i++)
                layouter.PutNextRectangle(new Size(rnd.Next(10, 100), rnd.Next(10, 100)));
            layouter.GetLayout().Count.Should().Be(150);
        }

        [Test]
        public void AddRectaonglesToLayout_WithoutIntersection()
        {
            var firstRectangle = layouter.PutNextRectangle(new Size(120, 30));
            var secondRectangle = layouter.PutNextRectangle(new Size(110, 50));
            firstRectangle.IntersectsWith(secondRectangle).Should().Be(false);
        }

        [TearDown]
        public void Dispose()
        {
            if (TestContext.CurrentContext.Result.FailCount > 0)
            {
                var bitmap = new Bitmap(1920, 1080);
                var draw = Graphics.FromImage(bitmap);
                foreach (Rectangle r in layouter.GetLayout())
                    draw.DrawRectangle(new Pen(Color.Black, 1), r);
                string path = @"C:\Temp\test.png";
                bitmap.Save(path);
                Console.WriteLine("Tag cloud visualization saved to file " + path);
            }

        }
    }
}
