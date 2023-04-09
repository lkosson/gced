using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class GProgram
	{
		public LinkedList<GLine> Lines { get; }

		public GProgram()
		{
			Lines = new LinkedList<GLine>();
		}

		public void New()
		{
			Lines.Clear();
			Lines.AddLast(new GLine("G17 ; Set XY plane"));
			Lines.AddLast(new GLine("G21 ; Set unit to mm"));
			Lines.AddLast(new GLine("G90 ; Set absolute positioning"));
			Lines.AddLast(new GLine("G53 ; Set work offset"));
			Lines.AddLast(new GLine("G0 Z0 S0 F1000 ; Reset position, feed and power"));
			Lines.AddLast(new GLine("M4 ; Start laser, dynamic power"));
			Lines.AddLast(new GLine("M5 ; Stop laser"));
		}

		public void Load(StreamReader reader)
		{
			string? line;
			Lines.Clear();
			while ((line = reader.ReadLine()) != null)
			{
				Lines.AddLast(new GLine(line));
			}
		}

		public void Load(string filename)
		{
			using var sr = new StreamReader(filename, Encoding.UTF8);
			Load(sr);
		}

		public void Save(StreamWriter writer)
		{
			foreach (var line in Lines)
			{
				writer.WriteLine(line.ToString());
			}
		}

		public void Save(string filename)
		{
			using var sw = new StreamWriter(filename, false, Encoding.UTF8);
			Save(sw);
		}

		public List<GOperation> Run()
		{
			var result = new List<GOperation>(Lines.Count);
			var context = new GContext();
			foreach (var line in Lines)
			{
				var goperation = new GOperation(line);
				goperation.Execute(context);
				result.Add(goperation);
			}
			return result;
		}

		public GProgram Clone()
		{
			var newProgram = new GProgram();
			foreach (var line in Lines)
				newProgram.Lines.AddLast(line.Clone());
			return newProgram;
		}
	}
}
