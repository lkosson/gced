using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCEd
{
	class ViewState
	{
		public GProgram Program { get => program; }
		public IEnumerable<GOperation> Operations { get => operations; }
		public IEnumerable<GOperation> SelectedOperations { get => selectedOperations; }

		public event Action? OperationsChanged;

		private GProgram program;
		private List<GOperation> operations;
		private HashSet<GOperation> selectedOperations;

		public ViewState()
		{
			program = new GProgram();
			operations = new List<GOperation>();
			selectedOperations = new HashSet<GOperation>();
		}

		public void LoadProgram(string file)
		{
			program = new GProgram();
			program.Load(file);
			RunProgram();
		}

		public void RunProgram()
		{
			var newOperations = program.Run();
			operations.Clear();
			operations.AddRange(newOperations);
			OperationsChanged?.Invoke();
		}

	}
}
