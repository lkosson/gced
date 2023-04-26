using System;
using System.Collections.Generic;

namespace GCEd
{
	class LineNodeOperation
	{
		public GLine Line { get; init; }
		public LinkedListNode<GLine> Node { get; init; }
		public GOperation Operation { get; init; }
		public GOperation? PreviousInExtent { get; init; }
		public GOperation? NextInExtent { get; init; }

		public LineNodeOperation(GLine line, LinkedListNode<GLine> node, GOperation operation, GOperation? previousInExtent, GOperation? nextInExtent)
		{
			Line = line;
			Node = node;
			Operation = operation;
			PreviousInExtent = previousInExtent;
			NextInExtent = nextInExtent;
		}

		public void Deconstruct(out GLine line, out LinkedListNode<GLine> node, out GOperation operation, out GOperation? previousInExtent, out GOperation? nextInExtent)
		{
			line = Line;
			node = Node;
			operation = Operation;
			previousInExtent = PreviousInExtent;
			nextInExtent = NextInExtent;
		}
	}
}
