using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class GLine
	{
		private static Dictionary<(char, decimal), GInstruction> instructions;

		public GInstruction Instruction { get; set; }
		public Directive Directive { get; set; }
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
		public string? P { get; set; }

		public bool IsLine => Instruction == GInstruction.G0 || Instruction == GInstruction.G1;
		public bool IsArc => Instruction == GInstruction.G2 || Instruction == GInstruction.G3;
		public bool IsVisible => IsLine || IsArc;
		public bool IsEmpty => !X.HasValue && !Y.HasValue && !Z.HasValue && !I.HasValue && !J.HasValue && !K.HasValue && !F.HasValue && !S.HasValue;

		public Vector2 XY { get => new Vector2((float)X.GetValueOrDefault(), (float)Y.GetValueOrDefault()); set { X = (decimal)value.X; Y = (decimal)value.Y; } }
		public Vector2 IJ { get => new Vector2((float)I.GetValueOrDefault(), (float)J.GetValueOrDefault()); set { I = (decimal)value.X; J = (decimal)value.Y; } }

		static GLine()
		{
			instructions = new Dictionary<(char, decimal), GInstruction>();
			foreach (var instruction in Enum.GetValues<GInstruction>())
			{
				if (instruction < GInstruction.G0) continue;
				var str = instruction.ToString();
				var ch = str[0];
				var num = Decimal.Parse(str.Substring(1), CultureInfo.InvariantCulture);
				instructions[(Char.ToUpper(str[0]), num)] = instruction;
				instructions[(Char.ToLower(str[0]), num)] = instruction;
			}
		}

		public GLine()
		{
			Instruction = GInstruction.Empty;
			RawText = "";
			Comment = "";
		}

		public GLine(string text)
			: this()
		{
			Parse(text);
		}

		public void Parse(string text)
		{
			Instruction = GInstruction.Empty;
			RawText = text;
			Comment = "";
			Error = null;
			X = Y = Z = I = J = K = F = S = null;
			P = null;
			char? currentField = null;
			int? valueStart = null;
			bool hasDecimalPoint = false;
			bool isQuoted = false;
			bool wasQuoted = false;
			for (var i = 0; i <= text.Length; i++)
			{
				var ch = i == text.Length ? '\0' : text[i];
				if (isQuoted)
				{
					if (ch == '\0')
					{
						Instruction = GInstruction.Invalid;
						Error = $"Unclosed quote.";
						ErrorPosition = valueStart.GetValueOrDefault(i);
						return;
					}
					if (ch == '"')
					{
						isQuoted = false;
						wasQuoted = true;
					}
				}
				else if (Char.IsWhiteSpace(ch) || Char.IsLetter(ch) || ch == ';' || ch == '(' || ch == '\0')
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

						var stringValue = text[valueStart.Value..(wasQuoted ? i - 1 : i)];
						if (!Decimal.TryParse(stringValue, NumberStyles.Integer | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value) && currentField != 'P')
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
						else if (currentField == 'P') P = stringValue;
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
						wasQuoted = false;
					}

					if (Char.IsLetter(ch))
					{
						currentField = Char.ToUpper(ch);
						valueStart = null;
					}

					if (ch == ';' || ch == '(')
					{
						if (i == 0 && text.Length > 2 && ch == ';' && text[1] == '.' && text.Contains(' '))
						{
							i = text.IndexOf(' ');
							var directiveText = text[2..i];
							if (Enum.TryParse<Directive>(directiveText, true, out var directive))
							{
								Instruction = GInstruction.Directive;
								Directive = directive;
							}
							else
							{
								Instruction = GInstruction.Invalid;
								Error = $"Unknown directive '{directiveText}'";
								ErrorPosition = 2;
							}
						}
						else
						{
							if (Instruction == GInstruction.Empty) Instruction = GInstruction.Comment;
							Comment = text[i..];
							break;
						}
					}

					if (ch == '\0') break;
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
				else if (ch == '"')
				{
					if (!currentField.HasValue)
					{
						Instruction = GInstruction.Invalid;
						Error = $"Unexpected quote character outside of a field.";
						ErrorPosition = i;
						return;
					}

					if (valueStart.HasValue)
					{
						Instruction = GInstruction.Invalid;
						Error = $"Unexpected quote character.";
						ErrorPosition = i;
						return;
					}

					if (wasQuoted)
					{
						Instruction = GInstruction.Invalid;
						Error = $"Unexpected additional quote.";
						ErrorPosition = i;
						return;
					}

					isQuoted = true;
					valueStart = i + 1;
				}
			}

			if (IsArc && !I.HasValue && !J.HasValue)
			{
				Instruction = GInstruction.Invalid;
				Error = "Missing I and J parameters for arc.";
				ErrorPosition = 0;
			}
		}

		public override string ToString()
		{
			if (Instruction == GInstruction.Invalid) return RawText;
			if (Instruction == GInstruction.Unknown) return RawText;
			if (Instruction == GInstruction.Comment) return RawText;
			if (Instruction == GInstruction.Empty) return "";

			var sb = new StringBuilder();
			if (Instruction == GInstruction.Directive)
			{
				sb.Append(";.");
				sb.Append(Directive);
			}
			else sb.Append(Instruction.ToString());

			if (X.HasValue) { sb.Append(" X"); sb.Append(X.Value.ToString("0.0###", CultureInfo.InvariantCulture)); }
			if (Y.HasValue) { sb.Append(" Y"); sb.Append(Y.Value.ToString("0.0###", CultureInfo.InvariantCulture)); }
			if (Z.HasValue) { sb.Append(" Z"); sb.Append(Z.Value.ToString("0.0###", CultureInfo.InvariantCulture)); }
			if (I.HasValue) { sb.Append(" I"); sb.Append(I.Value.ToString("0.0###", CultureInfo.InvariantCulture)); }
			if (J.HasValue) { sb.Append(" J"); sb.Append(J.Value.ToString("0.0###", CultureInfo.InvariantCulture)); }
			if (K.HasValue) { sb.Append(" K"); sb.Append(K.Value.ToString("0.0###", CultureInfo.InvariantCulture)); }
			if (F.HasValue) { sb.Append(" F"); sb.Append(F.Value.ToString("0.0###", CultureInfo.InvariantCulture)); }
			if (S.HasValue) { sb.Append(" S"); sb.Append(S.Value.ToString("0.0###", CultureInfo.InvariantCulture)); }
			if (P != null) { sb.Append(" P\""); sb.Append(P); sb.Append("\""); }
			if (!String.IsNullOrEmpty(Comment)) { sb.Append(" "); sb.Append(Comment); }

			return sb.ToString();
		}

		public GLine Clone() => new GLine(ToString());
	}

	enum GInstruction
	{
		Empty,
		Unknown,
		Invalid,
		Comment,
		Directive,

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

	enum Directive
	{
		None,
		Background,
		Line,
		Circle
	}
}
