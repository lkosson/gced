using System;
using System.Collections.Generic;
using System.Numerics;

namespace GCEd
{
	class GOperation
	{
		public readonly static IEqualityComparer<GOperation> ByGLineEqualityComparer = new ByGLineEqualityComparerImpl();

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
		public bool Absolute { get; set; }

		public TimeSpan TimeStart { get; set; }
		public TimeSpan TimeEnd { get; set; }

		public Vector2 AbsStart => new Vector2(AbsXStart, AbsYStart);
		public Vector2 AbsEnd => new Vector2(AbsXEnd, AbsYEnd);
		public Vector2 AbsOffset => new Vector2(AbsI, AbsJ);
		public TimeSpan TimeDuration => TimeEnd - TimeStart;

		public GOperation? OriginalValues { get; private set; }

		public GOperation(GLine line)
		{
			Line = line;
		}

		public void SaveOriginalValues()
		{
			OriginalValues = new GOperation(Line)
			{
				AbsXStart = AbsXStart,
				AbsYStart = AbsYStart,
				AbsZStart = AbsZStart,

				AbsXEnd = AbsXEnd,
				AbsYEnd = AbsYEnd,
				AbsZEnd = AbsZEnd,

				AbsI = AbsI,
				AbsJ = AbsJ,
				AbsK = AbsK,

				F = F,
				S = S,
				Active = Active,
				Absolute = Absolute
			};
		}

		public void RestoreOriginalValues()
		{
			if (OriginalValues == null) return;
			AbsXStart = OriginalValues.AbsXStart;
			AbsYStart = OriginalValues.AbsYStart;
			AbsZStart = OriginalValues.AbsZStart;

			AbsXEnd = OriginalValues.AbsXEnd;
			AbsYEnd = OriginalValues.AbsYEnd;
			AbsZEnd = OriginalValues.AbsZEnd;

			AbsI = OriginalValues.AbsI;
			AbsJ = OriginalValues.AbsJ;
			AbsK = OriginalValues.AbsK;

			F = OriginalValues.F;
			S = OriginalValues.S;
			Active = OriginalValues.Active;
			Absolute = OriginalValues.Absolute;
		}

		public void Execute(GContext context)
		{
			AbsXStart = (float)context.X;
			AbsYStart = (float)context.Y;
			AbsZStart = (float)context.Z;

			AbsI = AbsXStart + (float)context.ToMM(Line.I.GetValueOrDefault());
			AbsJ = AbsYStart + (float)context.ToMM(Line.J.GetValueOrDefault());
			AbsK = AbsZStart + (float)context.ToMM(Line.K.GetValueOrDefault());

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

			Active = context.Active;
			Absolute = context.Absolute;

			AbsXEnd = (float)context.X;
			AbsYEnd = (float)context.Y;
			AbsZEnd = (float)context.Z;

			TimeStart = context.Time;
			var length = 0f;
			if (Line.IsLine) length = Geometry.LineLength(AbsStart, AbsEnd);
			else if (Line.IsArc) length = Geometry.ArcLength(AbsStart, AbsEnd, AbsOffset, Line.Instruction == GInstruction.G2);
			if (F > 0) TimeEnd = context.Time + TimeSpan.FromSeconds(length * 60 / (double)F);
			context.Time = TimeEnd;
		}

		private class ByGLineEqualityComparerImpl : IEqualityComparer<GOperation>
		{
			public bool Equals(GOperation? x, GOperation? y) => Object.ReferenceEquals(x?.Line, y?.Line);

			public int GetHashCode(GOperation obj) => obj.Line == null ? 0 : obj.Line.GetHashCode();
		}
	}
}
