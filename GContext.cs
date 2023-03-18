using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class GContext
	{
		public decimal X { get; set; }
		public decimal Y { get; set; }
		public decimal Z { get; set; }

		public decimal F { get; set; }
		public decimal S { get; set; }

		public bool Absolute { get; set; }
		public bool Inches { get; set; }
		public bool Active { get; set; }
		public bool Dynamic { get; set; }

		public GContext()
		{
			X = Y = Z = 0;
			F = 1000;
			S = 0;
			Absolute = true;
			Inches = false;
			Active = false;
			Dynamic = false;
		}

		public decimal ToMM(decimal value) => Inches ? value * 2.54m : value;
	}
}
