using System.Collections.Generic;
using System.Drawing;

namespace AvaloniaApplication1.ViewModels;

public class ChartColors
{
	private static Dictionary<string, Color> _colorDictionary => new()
		{
			{ "Photograph", Color.DarkSlateGray },
			{ "Painting", Color.DarkKhaki },
			{ "Comic", Color.SteelBlue },
			{ "TVShow", Color.MediumTurquoise },
			{ "Movie",  Color.Silver },
			{ "Book", Color.IndianRed },
			{ "Song", Color.Purple },
			{ "Game", Color.LimeGreen },
			{ "Music", Color.HotPink },
			{ "My work progress", Color.DarkKhaki },
			{ "Work", Color.DarkKhaki },
			{ "Year progress", Color.DimGray },
			{ "All", Color.YellowGreen}
		};

	public static Color GetColor(string name)
	{
		if (_colorDictionary.ContainsKey(name))
		{
			return _colorDictionary[name];
		}

		foreach (var key in _colorDictionary.Keys)
		{
			if (name.Contains(key, System.StringComparison.OrdinalIgnoreCase))
			{
				return _colorDictionary[key];
			}
		}

		return Color.LightYellow;
	}
}