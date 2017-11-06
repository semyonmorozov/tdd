using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            var screenBounds = Screen.PrimaryScreen.Bounds;
            var vizualizator = new TagsCloudVisualizator(screenBounds.Width,screenBounds.Height);
            var tags = ParseTagsFromFile(@"wordsStats.txt");
            vizualizator.AddTags(tags);
            var bitmap = vizualizator.Visualize(bgColor, textColor);
            var path = String.Concat(Path.GetTempPath(), "result", ".png");
            bitmap.Save(path);
        }

        private static Dictionary<string, int> ParseTagsFromFile(string filePath)
        {
            var tags = new Dictionary<string, int>();
            using (var file = new StreamReader(filePath))
            {
                for (var i = 0; i < 400; i++)
                {
                    var line = file.ReadLine();
                    if (line == null) break;
                    line = Regex.Replace(line, "\t", " ");
                    var splitedLine = line.Split(' ');
                    var word = splitedLine[1];
                    var frequency = Int32.Parse(splitedLine[2]);
                    tags.Add(word, frequency);
                }
            }
            return tags;
        }
    }
}
