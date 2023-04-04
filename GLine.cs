using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class GLine
	{
		private static Dictionary<(char, decimal), GInstruction> instructions;

		public GInstruction Instruction { get; set; }
		public string RawText { get; set; }
		public string Comment { get; set; }
		public string? Error { get; set; }
		public int ErrorPosition { get; set; }
		public decimal? X { get; set; }
		public decimal? Y { get; set; }
		public decimal? Z { get; set; }
		public decimal? I { get; set; }
		public decimal? J { get; set; }
		public decimal? K { get; set; }
		public decimal? F { get; set; }
		public decimal? S { get; set; }

		static GLine()
		{
			instructions = new Dictionary<(char, decimal), GInstruction>();
			foreach (var instruction in Enum.GetValues<GInstruction>())
			{
				if (instruction < GInstruction.G0) continue;
				var str = instruction.ToString();
				instructions[(str[0], Decimal.Parse(str.Substring(1), CultureInfo.InvariantCulture))] = instruction;
			}
		}

		public GLine()
		{
			Instruction = GInstruction.Empty;
			RawText = "";
			Comment = "";
		}

		public void Parse(string text)
		{
			Instruction = GInstruction.Empty;
			RawText = text;
			Comment = "";
			Error = null;
			X = Y = Z = I = J = K = F = S = null;
			char? currentField = null;
			int? valueStart = null;
			bool hasDecimalPoint = false;
			for (var i = 0; i <= text.Length; i++)
			{
				var ch = i == text.Length ? '\0' : text[i];
				if (Char.IsWhiteSpace(ch) || ch == ';' || ch == '(' || ch == '\0')
				{
					if (currentField.HasValue)
					{
						if (!valueStart.HasValue)
						{
							Instruction = GInstruction.Invalid;
							Error = $"Field '{currentField}' don't have a value.";
							ErrorPosition = i;
							return;
						}

						var stringValue = text[valueStart.Value..i];
						if (!Decimal.TryParse(stringValue, NumberStyles.Integer | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value))
						{
							Instruction = GInstruction.Invalid;
							Error = $"Invalid value '{stringValue}' in '{currentField}' field.";
							ErrorPosition = i;
							return;
						}

						if (currentField == 'X') X = value;
						else if (currentField == 'Y') Y = value;
						else if (currentField == 'Z') Z = value;
						else if (currentField == 'I') I = value;
						else if (currentField == 'J') J = value;
						else if (currentField == 'K') K = value;
						else if (currentField == 'F') F = value;
						else if (currentField == 'S') S = value;
						else if (instructions.TryGetValue((currentField.Value, value), out var instruction)) Instruction = instruction;
						else if (currentField == 'G' || currentField == 'M') Instruction = GInstruction.Unknown;
						else
						{
							Instruction = GInstruction.Invalid;
							Error = $"Unknown field '{currentField}'.";
							ErrorPosition = i;
							return;
						}

						currentField = null;
					}

					if (ch == ';' || ch == '(')
					{
						if (Instruction == GInstruction.Empty) Instruction = GInstruction.Comment;
						Comment = text[i..];
						break;
					}

					if (ch == '\0') break;
				}
				else if (Char.IsLetter(ch))
				{
					if (currentField.HasValue)
					{
						Instruction = GInstruction.Invalid;
						Error = $"Invalid character '{ch}' in '{currentField}' field.";
						ErrorPosition = i;
						return;
					}

					currentField = ch;
					valueStart = null;
				}
				else if (Char.IsDigit(ch))
				{
					if (!currentField.HasValue)
					{
						Instruction = GInstruction.Invalid;
						Error = $"Unexpected digit '{ch}' outside of a field.";
						ErrorPosition = i;
						return;
					}

					if (!valueStart.HasValue)
					{
						valueStart = i;
						hasDecimalPoint = false;
					}
				}
				else if (ch == '-' || ch == '.')
				{
					if (!currentField.HasValue)
					{
						Instruction = GInstruction.Invalid;
						Error = $"Unexpected character '{ch}' outside of a field.";
						ErrorPosition = i;
						return;
					}

					if (valueStart.HasValue)
					{
						if ((ch == '-' && i != valueStart.Value + 1) || (ch == '.' && hasDecimalPoint))
						{
							Instruction = GInstruction.Invalid;
							Error = $"Unexpected character '{ch}' in numeric constant.";
							ErrorPosition = i;
							return;
						}
					}
					else
					{
						valueStart = i;
						hasDecimalPoint = false;
					}

					if (ch == '.') hasDecimalPoint = true;
				}
			}
		}

		public override string ToString()
		{
			if (Instruction == GInstruction.Invalid) return "\"" + RawText + "\"";
			if (Instruction == GInstruction.Unknown) return "\"" + RawText + "\"";
			if (Instruction == GInstruction.Comment) return RawText;
			if (Instruction == GInstruction.Empty) return "";

			var sb = new StringBuilder();
			sb.Append(Instruction.ToString());
			if (X.HasValue) { sb.Append(" X"); sb.Append(X.Value.ToString("0.0", CultureInfo.InvariantCulture)); }
			if (Y.HasValue) { sb.Append(" Y"); sb.Append(Y.Value.ToString("0.0", CultureInfo.InvariantCulture)); }
			if (Z.HasValue) { sb.Append(" Z"); sb.Append(Z.Value.ToString("0.0", CultureInfo.InvariantCulture)); }
			if (I.HasValue) { sb.Append(" I"); sb.Append(I.Value.ToString("0.0", CultureInfo.InvariantCulture)); }
			if (J.HasValue) { sb.Append(" J"); sb.Append(J.Value.ToString("0.0", CultureInfo.InvariantCulture)); }
			if (K.HasValue) { sb.Append(" K"); sb.Append(K.Value.ToString("0.0", CultureInfo.InvariantCulture)); }
			if (F.HasValue) { sb.Append(" F"); sb.Append(F.Value.ToString("0.0", CultureInfo.InvariantCulture)); }
			if (S.HasValue) { sb.Append(" S"); sb.Append(S.Value.ToString("0.0", CultureInfo.InvariantCulture)); }
			if (!String.IsNullOrEmpty(Comment)) { sb.Append(" "); sb.Append(Comment); }

			return sb.ToString();
		}

		public GLine Clone()
		{
			var newLine = new GLine();
			newLine.Parse(ToString());
			return newLine;
		}
	}

	enum GInstruction
	{
		Empty,
		Unknown,
		Invalid,
		Comment,

		/// <summary>
		/// Rapid move
		/// </summary>
		G0,

		/// <summary>
		/// Move
		/// </summary>
		G1,

		/// <summary>
		/// Clockwise arc
		/// </summary>
		G2,

		/// <summary>
		/// Counterclockwise arc
		/// </summary>
		G3,

		/// <summary>
		/// Set inches
		/// </summary>
		G20,

		/// <summary>
		/// Set millimeters
		/// </summary>
		G21,

		/// <summary>
		/// Set coordinate system
		/// </summary>
		G53,

		/// <summary>
		/// Set coordinate system
		/// </summary>
		G54,

		/// <summary>
		/// Set coordinate system
		/// </summary>
		G55,

		/// <summary>
		/// Set coordinate system
		/// </summary>
		G56,

		/// <summary>
		/// Set coordinate system
		/// </summary>
		G57,

		/// <summary>
		/// Set coordinate system
		/// </summary>
		G58,

		/// <summary>
		/// Set coordinate system
		/// </summary>
		G59,

		/// <summary>
		/// Absolute positioning
		/// </summary>
		G90,

		/// <summary>
		/// Relative positioning
		/// </summary>
		G91,

		/// <summary>
		/// Spindle on (constant power laser)
		/// </summary>
		M3,

		/// <summary>
		/// Spindle on (dynamic power laser)
		/// </summary>
		M4,

		/// <summary>
		/// Spindle off
		/// </summary>
		M5
	}
}
