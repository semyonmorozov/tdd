﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    class CircularCloudLayouter
    {
        

        private UniquePositivePointsFromSpiral uniquePositivePoints;

        private Point center;

        public Point Center => center;

        private List<Rectangle> layout = new List<Rectangle>();

        public List<Rectangle> Layout => layout;

        public CircularCloudLayouter(Point center)
        {
            this.center = center;
            this.uniquePositivePoints = new UniquePositivePointsFromSpiral(center);
        }

        public CircularCloudLayouter(Point center, double spreading)
        {
            this.center = center;
            this.uniquePositivePoints = new UniquePositivePointsFromSpiral(center,spreading);
        }


        private Rectangle CreateRecnagleByCenter(Point center, Size size)
        {
            int leftTopPointX = center.X - size.Width / 2;
            int leftTopPointY = center.Y - size.Height / 2;
            var leftTopPoint = new Point(leftTopPointX,leftTopPointY);
            return new Rectangle(leftTopPoint,size);
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            uniquePositivePoints.MoveNext();
            var rectangle = CreateRecnagleByCenter((Point)uniquePositivePoints.Current, rectangleSize);
            while (layout.Any(r => r.IntersectsWith(rectangle)))
            {
                uniquePositivePoints.MoveNext();
                rectangle = CreateRecnagleByCenter((Point)uniquePositivePoints.Current, rectangleSize);
            }
            layout.Add(rectangle);
            uniquePositivePoints.Reset();
            return rectangle;
        }
    }

    [TestFixture]
    public class CircularCloudLayouter_Should
    {
        private CircularCloudLayouter layouter;

        private Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
        [SetUp]
        public void Init()
        {
            layouter = new CircularCloudLayouter(new Point(screenBounds.Width /2 , screenBounds.Height / 2));
        }

        [TestCase(1, 2)]
        public void ReturnCorrectCenter_AfterInitialization(int x, int y)
        {
            Point center = new Point(x, y);
            layouter = new CircularCloudLayouter(center);
            layouter.Center.Should().Be(center);
        }

        [Test]
        public void AddingRectanglesToLayout()
        {
            Random rnd = new Random();
            for (int i = 0; i < 150; i++)
                layouter.PutNextRectangle(new Size(rnd.Next(10, 100), rnd.Next(10, 100)));
            layouter.Layout.Count.Should().Be(150);
        }

        [Test]
        public void AddRectanglesToLayout_WithoutIntersection()
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
                var testName = TestContext.CurrentContext.Test.Name;
                var bitmap = new Bitmap(screenBounds.Width, screenBounds.Height);
                var drawer = Graphics.FromImage(bitmap);
                foreach (Rectangle r in layouter.Layout)
                    drawer.DrawRectangle(new Pen(Color.Black, 1), r);
                string path = @"C:\Temp\"+testName+".png";
                bitmap.Save(path);
                TestContext.WriteLine("Tag cloud visualization saved to file " + path);
            }

        }
    }
}
