using System.Drawing;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;

namespace OptimisedImages;

public class Optimg
{
	private readonly (int, int) dimensions;
	private readonly int[] pixels;

	public Optimg((int, int) dimensions)
	{
		this.dimensions = dimensions;
		pixels = Enumerable.Repeat(0, dimensions.Item1 * dimensions.Item2).ToArray();
	}

	public static Color[] Colors { get; } =
	[
		Color.White, Color.Black, Color.Gray, Color.Yellow, Color.Red, Color.Blue, Color.LightBlue, Color.Orange,
		Color.Green, Color.MidnightBlue, Color.Pink, Color.DarkRed, Color.Crimson, Color.CornflowerBlue,
		Color.DarkOliveGreen, Color.Magenta, Color.Beige, Color.Chocolate, Color.Lime, Color.OrangeRed, Color.DarkGray,
		Color.DarkOrange, Color.Turquoise, Color.MediumTurquoise, Color.DarkTurquoise, Color.DimGray, Color.LightGray,
		Color.DarkSlateGray
	];

	public void SetPixel(int x, int y, int color)
	{
		if (x > dimensions.Item1 || y > dimensions.Item2 || x < 0 || y < 0)
		{
			throw new ArgumentException("Coordinates out of bounds.");
		}

		if (color > Colors.Length)
		{
			throw new ArgumentException("Color out of bounds.");
		}

		pixels[y * dimensions.Item1 + x] = color;
	}

	public int ReadPixel(int x, int y)
	{
		if (x > dimensions.Item1 || y > dimensions.Item2 || x < 0 || y < 0)
		{
			throw new ArgumentException("Coordinates out of bounds.");
		}

		return pixels[y * dimensions.Item2 + x];
	}

	public string Compress()
	{
		string compr = $"{dimensions.Item1};{dimensions.Item2};";
		int count = 1;
		int prevColor = pixels[0];

		foreach (int pix in pixels)
		{
			int color = pix;

			if (color == prevColor)
			{
				count++;
			}
			else
			{
				compr += $"({count}:{prevColor})";
				count = 1;
			}

			prevColor = color;
		}

		compr += $"({count}:{prevColor})";
		return Convert.ToBase64String(DeflateCompression.Zip(compr));
	}

	public Bitmap ToImage()
	{
		Bitmap bmp = new(dimensions.Item1, dimensions.Item2);

		for (int y = 0; y < dimensions.Item2; y++)
		{
			for (int x = 0; x < dimensions.Item1; x++)
			{
				bmp.SetPixel(x, y, Colors[pixels[y * dimensions.Item1 + x]]);
			}
		}

		return bmp;
	}

	public static Optimg FromImage(Image img, (int width, int height) dimensions)
	{
		object bmpLocker = new();
		Bitmap bmp_o = (Bitmap)img;
		Bitmap bmp = new(bmp_o, new Size(dimensions.width, dimensions.height));
		int w = bmp.Width;
		Optimg result = new((bmp.Width, bmp.Height));
		Cie1976Comparison colComp = new();
		Parallel.For(0, bmp.Height, y =>
		{
			for (int x = 0; x < w; x++)
			{
				Color color;

				lock (bmpLocker)
				{
					color = bmp.GetPixel(x, y);
				}

				double smallestDistance = 999;
				int closestColor = 0;

				for (int color2 = 0; color2 < Colors.Length; color2++)
				{
					Color c2 = Colors[color2];
					Rgb rgb1 = new() { R = color.R, G = color.G, B = color.B };
					Rgb rgb2 = new() { R = c2.R, G = c2.G, B = c2.B };
					double deltaE = rgb1.Compare(rgb2, colComp);

					if (deltaE < smallestDistance)
					{
						smallestDistance = deltaE;
						closestColor = color2;
					}
				}

				result.SetPixel(x, y, closestColor);
			}
		});
		return result;
	}

	public static Optimg FromString(string str)
	{
		string s = DeflateCompression.Unzip(Convert.FromBase64String(str));
		int dimensionX = int.Parse(s.Split(";")[0]);
		int dimensionY = int.Parse(s.Split(";")[1].Split("(")[0]);
		Optimg result = new((dimensionX, dimensionY));
		string pixelData = s.Substring(s.IndexOf('(') + 1).TrimEnd(')');
		(int, int)[] pixels = pixelData.Split(")(")
									   .Select(pair => pair.Split(":")
														   .Select(el => int.Parse(el)))
									   .Select(arr => (arr.ToArray()[0], arr.ToArray()[1]))
									   .ToArray();
		int x = 0;
		int y = 0;

		foreach ((int, int) pair in pixels)
		{
			for (int i = 0; i < pair.Item1; i++)
			{
				result.SetPixel(x, y, pair.Item2);
				x++;

				if (x >= dimensionX)
				{
					y++;
					x = 0;

					if (y >= dimensionY)
					{
						return result;
					}
				}
			}
		}

		return result;
	}
}