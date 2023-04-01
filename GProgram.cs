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

		public void Load(StreamReader reader)
		{
			string? line;
			Lines.Clear();
			while ((line = reader.ReadLine()) != null)
			{
				var gline = new GLine();
				gline.Parse(line);
				Lines.AddLast(gline);
			}
		}

		public void Load(string filename)
		{
			using var sr = new StreamReader(filename, Encoding.UTF8);
			Load(sr);
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
	}
}
