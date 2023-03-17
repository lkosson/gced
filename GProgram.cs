using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class GProgram
	{
		public List<GLine> Lines { get; }

		public GProgram()
		{
			Lines = new List<GLine>();
		}

		public void Read(StreamReader reader)
		{
			string? line;
			Lines.Clear();
			while ((line = reader.ReadLine()) != null)
			{
				var gline = new GLine();
				gline.Parse(line);
				Lines.Add(gline);
			}
		}

		public void Read(string filename)
		{
			using var sr = new StreamReader(filename, Encoding.UTF8);
			Read(sr);
		}
	}
}
