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
		public IReadOnlySet<GOperation> SelectedOperations { get => selectedOperations; }

		public event Action? OperationsChanged;
		public event Action? SelectedOperationsChanged;

		private GProgram program;
		private List<GOperation> operations;
		private HashSet<GOperation> selectedOperations;

		public ViewState()
		{
			program = new GProgram();
			operations = new List<GOperation>();
			selectedOperations = new HashSet<GOperation>(GOperation.ByGLineEqualityComparer);
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
			var newSelection = new List<GOperation>();
			foreach (var operation in operations)
			{
				if (selectedOperations.Contains(operation))
					newSelection.Add(operation);
			}
			selectedOperations.Clear();
			foreach (var operation in newSelection)
				selectedOperations.Add(operation);
			SelectedOperationsChanged?.Invoke();
		}

		public void UpdateSelection(IEnumerable<GOperation> operationsToSelect, IEnumerable<GOperation> operationsToUnselect)
		{

		}

	}
}
