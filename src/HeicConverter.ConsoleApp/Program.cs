using Openize.Heic.Decoder;
using System.Drawing;
using System.Globalization;

namespace HeicConverter.ConsoleApp;

internal class Program
{
    internal static void Main(string[] args)
    {
        var directory = args[0];
        var ratio = args.Length > 1 ? double.Parse(args[1], CultureInfo.InvariantCulture) : 0.25;
        var prefix = args.Length > 2 ? args[2] : "out";

        var files = Directory.EnumerateFiles(directory).ToList();

        var filesToConvert = files.Count(x => x.EndsWith(".heic", StringComparison.OrdinalIgnoreCase));

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Found {filesToConvert} HEIC files to convert in {directory}.");
        Console.WriteLine($"Ratio {ratio}.");
        Console.ResetColor();

        var i = 1;
        foreach (var filePath in files)
        {
            if (!Path.GetExtension(filePath).Equals(".heic", StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }

            Console.WriteLine($"{i}. Converting {filePath}...");

            using var fs = new FileStream(filePath, FileMode.Open);
            var image = HeicImage.Load(fs);

            var pixels = image.GetInt32Array(PixelFormat.Argb32);
            var width = (int)image.Width;
            var height = (int)image.Height;
            var j = 0;

            using var myBitmap = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    myBitmap.SetPixel(x, y, Color.FromArgb(pixels[j++]));
                }
            }

            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var newFileName = $"{prefix}_{fileName}.png";
            var newFilePath = Path.Combine(directory, newFileName);

            using var resizedBitmap = new Bitmap(myBitmap, (int)(ratio * myBitmap.Width), (int)(ratio * myBitmap.Height));

            resizedBitmap.Save(newFilePath);
            i++;
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Converting done.");
        Console.ResetColor();

        Console.ReadLine();
    }
}
