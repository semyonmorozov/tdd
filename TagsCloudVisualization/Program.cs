using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace TagsCloudVisualization
{
    static class Program
    {
        static void Main(string[] args)
        {
            var layouter = new CircularCloudLayouter(new Point(960, 960),0.04);
            var bitmap = new Bitmap(1920, 1920);
            var drawer = Graphics.FromImage(bitmap);
            drawer.Clear(Color.DarkMagenta);
            
            string line;
            using (var file = new System.IO.StreamReader(@"wordsStats.txt"))
            {
                
                for (int i=0;i<250;i++)
                {
                    line = file.ReadLine();
                    line = Regex.Replace(line, "\t", " ");
                    string[] splitedLine = line.Split(' ');
                    string word = splitedLine[1];
                    float frequency = float.Parse(splitedLine[2])/4000;
                    var font = new Font("Arial", frequency);
                    var rectangle = layouter.PutNextRectangle(Size.Ceiling(drawer.MeasureString(word, font)));
                    var br = new SolidBrush(Color.White);
                    drawer.DrawString(word, font, br, rectangle);
                }
            }
            string path = @"C:\Temp\result4.png";
            bitmap.Save(path);

        }

        public static Size Ceiling(this SizeF floatSize)
        {
            int height = (int) Math.Ceiling(floatSize.Height);
            int width = (int)Math.Ceiling(floatSize.Width);
            return new Size(width,height);
        }
    }
}
