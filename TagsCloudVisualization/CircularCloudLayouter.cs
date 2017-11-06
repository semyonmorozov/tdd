using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using System.Windows.Forms;

namespace TagsCloudVisualization
{
    class CircularCloudLayouter
    {
        private UniquePositivePointsFromSpiral uniquePositivePoints;

        public readonly Point Center;

        private List<Rectangle> layout = new List<Rectangle>();

        public List<Rectangle> Layout()
        {
            var copyOfLayout = new Rectangle[layout.Count];
            layout.CopyTo(copyOfLayout);
            return copyOfLayout.ToList();
        }

        public CircularCloudLayouter(Point center)
        {
            Center = center;
            uniquePositivePoints = new UniquePositivePointsFromSpiral(center);
        }

        public CircularCloudLayouter(Point center, double spreading)
        {
            Center = center;
            uniquePositivePoints = new UniquePositivePointsFromSpiral(center,spreading);
        }


        private static Rectangle CreateRecnagleByCenter(Point center, Size size)
        {
            var leftTopPointX = center.X - size.Width / 2;
            var leftTopPointY = center.Y - size.Height / 2;
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

        private static double GetMaxDistance(Rectangle rectangle, Point center)
        {
            var extremePoints = new List<Point>
            {
                new Point(rectangle.Left, rectangle.Top),
                new Point(rectangle.Right, rectangle.Top),
                new Point(rectangle.Left, rectangle.Bottom),
                new Point(rectangle.Right, rectangle.Top)
            };
            return extremePoints.Select(p => GetDistance(p, center)).Max();
        }

        private static double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        [SetUp]
        public void Init()
        {
            layouter = new CircularCloudLayouter(new Point(screenBounds.Width /2 , screenBounds.Height / 2));
        }

        [TestCase(1, 2)]
        public void ReturnCorrectCenter_AfterInitialization(int x, int y)
        {
            var center = new Point(x, y);
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
        [TestCase(0.4, 100)]
        [TestCase(0.5, 150)]
        [TestCase(0.6, 300)]
        public void LayOutRectangles_Tightly(double ratioOfAreas,int numOfRectangles)
        {
            var firstRectangle = layouter.PutNextRectangle(new Size(120, 30));
            var secondRectangle = layouter.PutNextRectangle(new Size(110, 50));
            firstRectangle.IntersectsWith(secondRectangle).Should().Be(false);
            var rnd = new Random();
            double radius = 0;
            for (var i = 0; i < numOfRectangles; i++)
            {
                var rectangle = layouter.PutNextRectangle(new Size(rnd.Next(10, 100), rnd.Next(10, 100)));
                var distance = GetMaxDistance(rectangle, layouter.Center);
                if (distance > radius) radius = distance;
            }
            var areaOfRectangles = layouter.Layout().Sum(r => (double)r.Height * r.Width);
            var areaOfCircle = Math.PI * Math.Pow(radius, 2);
            areaOfRectangles.Should().BeGreaterThan(areaOfCircle * ratioOfAreas);
        }

        [TearDown]
        public void Dispose()
        {
            if (TestContext.CurrentContext.Result.FailCount > 0)
            {
                var testName = TestContext.CurrentContext.Test.Name;
                var bitmap = new Bitmap(screenBounds.Width, screenBounds.Height);
                var drawer = Graphics.FromImage(bitmap);
                foreach (var r in layouter.Layout())
                    drawer.DrawRectangle(new Pen(Color.Black, 1), r);
                var path = String.Concat(@"C:\Temp\",testName,".png");
                bitmap.Save(path);
                TestContext.WriteLine("Tag cloud visualization saved to file " + path);
            }
        }
    }
}
