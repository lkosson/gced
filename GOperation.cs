﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class GOperation
	{
		public GLine Line { get; set; }

		public float AbsXStart { get; set; }
		public float AbsYStart { get; set; }
		public float AbsZStart { get; set; }

		public float AbsXEnd { get; set; }
		public float AbsYEnd { get; set; }
		public float AbsZEnd { get; set; }

		public float AbsI { get; set; }
		public float AbsJ { get; set; }
		public float AbsK { get; set; }

		public decimal F { get; set; }
		public decimal S { get; set; }
		public bool Active { get; set; }

		public GOperation(GLine line)
		{
			Line = line;
		}

		public void Execute(GContext context)
		{
			AbsXStart = (float)context.X;
			AbsYStart = (float)context.Y;
			AbsZStart = (float)context.Z;

			Active = context.Active;

			if (Line.Instruction == GInstruction.G0
				|| Line.Instruction == GInstruction.G1
				|| Line.Instruction == GInstruction.G2
				|| Line.Instruction == GInstruction.G3)
			{
				if (Line.F.HasValue) context.F = Line.F.Value;
				if (Line.S.HasValue) context.S = Line.S.Value;

				F = context.F;
				S = context.S;

				if (context.Absolute)
				{
					if (Line.X.HasValue) context.X = context.ToMM(Line.X.Value);
					if (Line.Y.HasValue) context.Y = context.ToMM(Line.Y.Value);
					if (Line.Z.HasValue) context.Z = context.ToMM(Line.Z.Value);
				}
				else
				{
					if (Line.X.HasValue) context.X += context.ToMM(Line.X.Value);
					if (Line.Y.HasValue) context.Y += context.ToMM(Line.Y.Value);
					if (Line.Z.HasValue) context.Z += context.ToMM(Line.Z.Value);
				}

				AbsI = AbsXStart + (float)context.ToMM(Line.I.GetValueOrDefault());
				AbsJ = AbsYStart + (float)context.ToMM(Line.J.GetValueOrDefault());
				AbsK = AbsZStart + (float)context.ToMM(Line.K.GetValueOrDefault());
			}
			else if (Line.Instruction == GInstruction.G20)
			{
				context.Inches = true;
			}
			else if (Line.Instruction == GInstruction.G21)
			{
				context.Inches = false;
			}
			else if (Line.Instruction == GInstruction.G90)
			{
				context.Absolute = true;
			}
			else if (Line.Instruction == GInstruction.G91)
			{
				context.Absolute = false;
			}
			else if (Line.Instruction == GInstruction.M3)
			{
				context.Active = true;
				context.Dynamic = false;
			}
			else if (Line.Instruction == GInstruction.M4)
			{
				context.Active = true;
				context.Dynamic = true;
			}
			else if (Line.Instruction == GInstruction.M5)
			{
				context.Active = false;
				context.Dynamic = false;
			}

			AbsXEnd = (float)context.X;
			AbsYEnd = (float)context.Y;
			AbsZEnd = (float)context.Z;
		}
	}
}