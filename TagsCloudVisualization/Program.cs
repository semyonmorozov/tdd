using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TagsCloudVisualization
{
    static class Program
    {
        static void Main(string[] args)
        {
            var bgColor = Color.DarkMagenta;
            var textColor = Color.White;
            var tags = new Dictionary<string, int>();
            var screenBounds = Screen.PrimaryScreen.Bounds;
            var vizualizator = new TagsCloudVisualizator(screenBounds.Width,screenBounds.Height);
            using (var file = new System.IO.StreamReader(@"wordsStats.txt"))
            {
                for (int i=0;i<400;i++)
                {
                    var line = file.ReadLine();
                    if (line == null) break;
                    line = Regex.Replace(line, "\t", " ");
                    string[] splitedLine = line.Split(' ');
                    string word = splitedLine[1];
                    var frequency = Int32.Parse(splitedLine[2]);
                    tags.Add(word,frequency);
                }
            }
            vizualizator.AddTags(tags);
            var bitmap = vizualizator.Visualize(bgColor, textColor);
            string path = @"C:\Temp\result.png";
            bitmap.Save(path);
        }
    }
}
