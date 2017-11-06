using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    class TagsCloudVisualizator
    {
        private readonly int width;
        private readonly int height;
        private Bitmap tagsCloud;
        private Dictionary<string, int> tags = new Dictionary<string, int>();

        private const int minTextSize = 6;
        private const int maxTextSize = 150;

        public TagsCloudVisualizator(int width, int height)
        {
            this.width = width;
            this.height = height;
            tagsCloud = new Bitmap(width, height);
        }

        public void AddTags(Dictionary<string, int> tags)
        {
            tags.ToList().ForEach(x => this.tags[x.Key] = x.Value);
        }

        public Bitmap Visualize(Color bgColor, Color textColor, double spreading = 0.01)
        {
            var center = new Point(width / 2, height / 2);
            var layouter = new CircularCloudLayouter(center, spreading);
            var drawer = Graphics.FromImage(tagsCloud);

            drawer.Clear(bgColor);
            float normaCoef = tags.Values.Max() / maxTextSize;

            foreach (var tag in tags)
            {
                var word = tag.Key;
                var weight = tag.Value;
                var normalizedWeight = NormalizeWeight(weight, normaCoef);
                var font = new Font("Tahoma", normalizedWeight);
                var textSize = Size.Ceiling(drawer.MeasureString(word, font));
                var rectangle = layouter.PutNextRectangle(textSize);
                var br = new SolidBrush(Color.White);
                drawer.DrawString(word, font, br, rectangle);
            }
            
            return tagsCloud;
        }

        private float NormalizeWeight(float weight, float normaCoef)
        {
            weight = weight / normaCoef;
            return weight < minTextSize ? minTextSize : weight;
        }
    }
}
