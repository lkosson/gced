using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class Settings
	{
		public bool ShowFPS { get; set; } = false;
		public bool ShowCursorCoords { get; set; } = true;
		public bool ShowItemCoords { get; set; } = true;
		public bool ShowMinorGrid { get; set; } = true;
		public bool ShowMajorGrid { get; set; } = true;
		public bool ShowOriginGrid { get; set; } = true;
		public bool SnapToGrid { get; set; } = true;
		public bool SnapToItems { get; set; } = true;
		public bool SnapToAxes { get; set; } = true;
		public float? GridMinorStep { get; set; } = null;
		public float? GridMajorStep { get; set; } = null;
		public float HintSnapDistance { get; set; } = 2;
		public Color SelectedItemColor { get; set; } = Color.FromArgb(0xF5, 0xFA, 0xFF);
		public Color ItemColor { get; set; } = Color.FromArgb(0xA5, 0xCA, 0xFF);
		public float SelectedItemThickness { get; set; } = 4;
		public float ActiveItemThickness { get; set; } = 3;
		public bool ItemEndCaps { get; set; } = true;
		public Color GuideColor { get; set; } = Color.FromArgb(0xC5, 0xC5, 0xCF);
		public float GuideThickness { get; set; } = 1;
		public Color MinorGridColor { get; set; } = Color.FromArgb(0x30, 0x60, 0xA0);
		public float MinorGridThickness { get; set; } = 1;
		public Color MajorGridColor { get; set; } = Color.FromArgb(0x60, 0xA0, 0xD0);
		public float MajorGridThickness { get; set; } = 1;
		public Color OriginGridColor { get; set; } = Color.FromArgb(0xE0, 0xF0, 0xFF);
		public float OriginGridThickness { get; set; } = 2;
		public Color SelectionBorderColor { get; set; } = Color.FromArgb(0x40, 0xE0, 0xEA, 0xF5);
		public Color SelectionAreaColor { get; set; } = Color.FromArgb(0xAF, 0xCF, 0xFF);
		public Color TextColor { get; set; } = Color.FromArgb(0xE5, 0xF5, 0xFF);
		public Color BackgroundColor { get; set; } = Color.FromArgb(0x20, 0x4A, 0x7F);
	}
}
