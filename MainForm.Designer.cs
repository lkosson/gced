namespace GCEd
{
	partial class MainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			canvas = new Canvas();
			operationProperties = new OperationProperties();
			operationsList = new OperationsList();
			lineEditor = new LineEditor();
			openFileDialog = new System.Windows.Forms.OpenFileDialog();
			saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			splitContainer = new System.Windows.Forms.SplitContainer();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			menuStrip = new System.Windows.Forms.MenuStrip();
			fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			convertToAbsoluteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			convertToRelativeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			moveEndpointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			moveOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			translateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			snapToGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			snapToItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			snapToAxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showCoordinatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			showPerformanceStatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			panzoomToFitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			panViewToSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newLineAfterCurrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			rapidToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			lineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			clockwiseArcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			counterclockwiseArcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			commentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			commentBeforeCurrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			backgroundImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			textShapeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			convertToOutlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
			splitContainer.Panel1.SuspendLayout();
			splitContainer.Panel2.SuspendLayout();
			splitContainer.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			menuStrip.SuspendLayout();
			SuspendLayout();
			// 
			// canvas
			// 
			canvas.Dock = System.Windows.Forms.DockStyle.Fill;
			canvas.Location = new System.Drawing.Point(0, 0);
			canvas.Name = "canvas";
			canvas.ShowCursorCoords = true;
			canvas.ShowFPS = false;
			canvas.ShowItemCoords = true;
			canvas.ShowMajorGrid = true;
			canvas.ShowMinorGrid = true;
			canvas.ShowOriginGrid = true;
			canvas.Size = new System.Drawing.Size(547, 514);
			canvas.SnapToAxes = true;
			canvas.SnapToGrid = true;
			canvas.SnapToItems = true;
			canvas.TabIndex = 0;
			// 
			// operationProperties
			// 
			operationProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			operationProperties.Location = new System.Drawing.Point(0, 0);
			operationProperties.Margin = new System.Windows.Forms.Padding(0);
			operationProperties.Name = "operationProperties";
			operationProperties.Size = new System.Drawing.Size(249, 334);
			operationProperties.TabIndex = 1;
			// 
			// operationsList
			// 
			operationsList.Dock = System.Windows.Forms.DockStyle.Fill;
			operationsList.Location = new System.Drawing.Point(3, 373);
			operationsList.Name = "operationsList";
			operationsList.Size = new System.Drawing.Size(243, 138);
			operationsList.TabIndex = 2;
			// 
			// lineEditor
			// 
			lineEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			lineEditor.Location = new System.Drawing.Point(3, 337);
			lineEditor.Name = "lineEditor";
			lineEditor.Size = new System.Drawing.Size(243, 30);
			lineEditor.TabIndex = 3;
			// 
			// openFileDialog
			// 
			openFileDialog.Filter = "G-Code files (*.nc, *.gcode)|*.nc;*.gcode|All files|*.*";
			openFileDialog.RestoreDirectory = true;
			// 
			// saveFileDialog
			// 
			saveFileDialog.Filter = "G-Code files (*.nc, *.gcode)|*.nc;*.gcode|All files|*.*";
			saveFileDialog.RestoreDirectory = true;
			// 
			// splitContainer
			// 
			splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			splitContainer.Location = new System.Drawing.Point(0, 24);
			splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			splitContainer.Panel1.Controls.Add(canvas);
			// 
			// splitContainer.Panel2
			// 
			splitContainer.Panel2.Controls.Add(tableLayoutPanel1);
			splitContainer.Size = new System.Drawing.Size(800, 514);
			splitContainer.SplitterDistance = 547;
			splitContainer.TabIndex = 5;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 1;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(operationProperties, 0, 0);
			tableLayoutPanel1.Controls.Add(operationsList, 0, 2);
			tableLayoutPanel1.Controls.Add(lineEditor, 0, 1);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 3;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Size = new System.Drawing.Size(249, 514);
			tableLayoutPanel1.TabIndex = 0;
			// 
			// menuStrip
			// 
			menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, insertToolStripMenuItem, aboutToolStripMenuItem });
			menuStrip.Location = new System.Drawing.Point(0, 0);
			menuStrip.Name = "menuStrip";
			menuStrip.Size = new System.Drawing.Size(800, 24);
			menuStrip.TabIndex = 6;
			menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripSeparator1, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			fileToolStripMenuItem.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			newToolStripMenuItem.Name = "newToolStripMenuItem";
			newToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+N";
			newToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
			newToolStripMenuItem.Text = "New";
			newToolStripMenuItem.Click += newToolStripMenuItem_Click;
			// 
			// openToolStripMenuItem
			// 
			openToolStripMenuItem.Name = "openToolStripMenuItem";
			openToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+O";
			openToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
			openToolStripMenuItem.Text = "Open ...";
			openToolStripMenuItem.Click += openToolStripMenuItem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(193, 6);
			// 
			// saveToolStripMenuItem
			// 
			saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			saveToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+S";
			saveToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
			saveToolStripMenuItem.Text = "Save";
			saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
			// 
			// saveAsToolStripMenuItem
			// 
			saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			saveAsToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+S";
			saveAsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
			saveAsToolStripMenuItem.Text = "Save as ...";
			saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(193, 6);
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.ShortcutKeyDisplayString = "Alt+F4";
			exitToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
			exitToolStripMenuItem.Text = "Exit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem, toolStripSeparator3, cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, deleteToolStripMenuItem, toolStripSeparator4, selectAllToolStripMenuItem, toolStripSeparator5, convertToAbsoluteToolStripMenuItem, convertToRelativeToolStripMenuItem, toolStripSeparator6, moveEndpointToolStripMenuItem, moveOffsetToolStripMenuItem, translateToolStripMenuItem, convertToOutlineToolStripMenuItem });
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			editToolStripMenuItem.Text = "Edit";
			// 
			// undoToolStripMenuItem
			// 
			undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			undoToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Z";
			undoToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			undoToolStripMenuItem.Text = "Undo";
			undoToolStripMenuItem.Click += undoToolStripMenuItem_Click;
			// 
			// redoToolStripMenuItem
			// 
			redoToolStripMenuItem.Name = "redoToolStripMenuItem";
			redoToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Y";
			redoToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			redoToolStripMenuItem.Text = "Redo";
			redoToolStripMenuItem.Click += redoToolStripMenuItem_Click;
			// 
			// toolStripSeparator3
			// 
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new System.Drawing.Size(215, 6);
			// 
			// cutToolStripMenuItem
			// 
			cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			cutToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+X";
			cutToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			cutToolStripMenuItem.Text = "Cut";
			cutToolStripMenuItem.Click += cutToolStripMenuItem_Click;
			// 
			// copyToolStripMenuItem
			// 
			copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			copyToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
			copyToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			copyToolStripMenuItem.Text = "Copy";
			copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
			// 
			// pasteToolStripMenuItem
			// 
			pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			pasteToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+V";
			pasteToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			pasteToolStripMenuItem.Text = "Paste";
			pasteToolStripMenuItem.Click += pasteToolStripMenuItem_Click;
			// 
			// deleteToolStripMenuItem
			// 
			deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			deleteToolStripMenuItem.ShortcutKeyDisplayString = "Del";
			deleteToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			deleteToolStripMenuItem.Text = "Delete";
			deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
			// 
			// toolStripSeparator4
			// 
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new System.Drawing.Size(215, 6);
			// 
			// selectAllToolStripMenuItem
			// 
			selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			selectAllToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+A";
			selectAllToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			selectAllToolStripMenuItem.Text = "Select all";
			selectAllToolStripMenuItem.Click += selectAllToolStripMenuItem_Click;
			// 
			// toolStripSeparator5
			// 
			toolStripSeparator5.Name = "toolStripSeparator5";
			toolStripSeparator5.Size = new System.Drawing.Size(215, 6);
			// 
			// convertToAbsoluteToolStripMenuItem
			// 
			convertToAbsoluteToolStripMenuItem.Name = "convertToAbsoluteToolStripMenuItem";
			convertToAbsoluteToolStripMenuItem.ShortcutKeyDisplayString = "A";
			convertToAbsoluteToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			convertToAbsoluteToolStripMenuItem.Text = "Convert to absolute";
			convertToAbsoluteToolStripMenuItem.Click += convertToAbsoluteToolStripMenuItem_Click;
			// 
			// convertToRelativeToolStripMenuItem
			// 
			convertToRelativeToolStripMenuItem.Name = "convertToRelativeToolStripMenuItem";
			convertToRelativeToolStripMenuItem.ShortcutKeyDisplayString = "Shift+A";
			convertToRelativeToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			convertToRelativeToolStripMenuItem.Text = "Convert to relative";
			convertToRelativeToolStripMenuItem.Click += convertToRelativeToolStripMenuItem_Click;
			// 
			// toolStripSeparator6
			// 
			toolStripSeparator6.Name = "toolStripSeparator6";
			toolStripSeparator6.Size = new System.Drawing.Size(215, 6);
			// 
			// moveEndpointToolStripMenuItem
			// 
			moveEndpointToolStripMenuItem.Name = "moveEndpointToolStripMenuItem";
			moveEndpointToolStripMenuItem.ShortcutKeyDisplayString = "E";
			moveEndpointToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			moveEndpointToolStripMenuItem.Text = "Move endpoint";
			moveEndpointToolStripMenuItem.Click += moveEndpointToolStripMenuItem_Click;
			// 
			// moveOffsetToolStripMenuItem
			// 
			moveOffsetToolStripMenuItem.Name = "moveOffsetToolStripMenuItem";
			moveOffsetToolStripMenuItem.ShortcutKeyDisplayString = "Shift+E";
			moveOffsetToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			moveOffsetToolStripMenuItem.Text = "Move offset";
			moveOffsetToolStripMenuItem.Click += moveOffsetToolStripMenuItem_Click;
			// 
			// translateToolStripMenuItem
			// 
			translateToolStripMenuItem.Name = "translateToolStripMenuItem";
			translateToolStripMenuItem.ShortcutKeyDisplayString = "T";
			translateToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			translateToolStripMenuItem.Text = "Translate";
			translateToolStripMenuItem.Click += translateToolStripMenuItem_Click;
			// 
			// viewToolStripMenuItem
			// 
			viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { showGridToolStripMenuItem, snapToGridToolStripMenuItem, snapToItemsToolStripMenuItem, snapToAxesToolStripMenuItem, showCoordinatesToolStripMenuItem, showPerformanceStatsToolStripMenuItem, panzoomToFitToolStripMenuItem, panViewToSelectionToolStripMenuItem });
			viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			viewToolStripMenuItem.Text = "View";
			// 
			// showGridToolStripMenuItem
			// 
			showGridToolStripMenuItem.Name = "showGridToolStripMenuItem";
			showGridToolStripMenuItem.ShortcutKeyDisplayString = "G";
			showGridToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			showGridToolStripMenuItem.Text = "Show grid";
			showGridToolStripMenuItem.Click += showGridToolStripMenuItem_Click;
			// 
			// snapToGridToolStripMenuItem
			// 
			snapToGridToolStripMenuItem.Name = "snapToGridToolStripMenuItem";
			snapToGridToolStripMenuItem.ShortcutKeyDisplayString = "S";
			snapToGridToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			snapToGridToolStripMenuItem.Text = "Snap to grid";
			snapToGridToolStripMenuItem.Click += snapToGridToolStripMenuItem_Click;
			// 
			// snapToItemsToolStripMenuItem
			// 
			snapToItemsToolStripMenuItem.Name = "snapToItemsToolStripMenuItem";
			snapToItemsToolStripMenuItem.ShortcutKeyDisplayString = "Shift+S";
			snapToItemsToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			snapToItemsToolStripMenuItem.Text = "Snap to items";
			snapToItemsToolStripMenuItem.Click += snapToItemsToolStripMenuItem_Click;
			// 
			// snapToAxesToolStripMenuItem
			// 
			snapToAxesToolStripMenuItem.Name = "snapToAxesToolStripMenuItem";
			snapToAxesToolStripMenuItem.ShortcutKeyDisplayString = "";
			snapToAxesToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			snapToAxesToolStripMenuItem.Text = "Snap to axes";
			snapToAxesToolStripMenuItem.Click += snapToAxesToolStripMenuItem_Click;
			// 
			// showCoordinatesToolStripMenuItem
			// 
			showCoordinatesToolStripMenuItem.Name = "showCoordinatesToolStripMenuItem";
			showCoordinatesToolStripMenuItem.ShortcutKeyDisplayString = "C";
			showCoordinatesToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			showCoordinatesToolStripMenuItem.Text = "Show coordinates";
			showCoordinatesToolStripMenuItem.Click += showCoordinatesToolStripMenuItem_Click;
			// 
			// showPerformanceStatsToolStripMenuItem
			// 
			showPerformanceStatsToolStripMenuItem.Name = "showPerformanceStatsToolStripMenuItem";
			showPerformanceStatsToolStripMenuItem.ShortcutKeyDisplayString = "F";
			showPerformanceStatsToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			showPerformanceStatsToolStripMenuItem.Text = "Show performance stats";
			showPerformanceStatsToolStripMenuItem.Click += showPerformanceStatsToolStripMenuItem_Click;
			// 
			// panzoomToFitToolStripMenuItem
			// 
			panzoomToFitToolStripMenuItem.Name = "panzoomToFitToolStripMenuItem";
			panzoomToFitToolStripMenuItem.ShortcutKeyDisplayString = "H";
			panzoomToFitToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			panzoomToFitToolStripMenuItem.Text = "Pan/zoom to fit all";
			panzoomToFitToolStripMenuItem.Click += panzoomToFitToolStripMenuItem_Click;
			// 
			// panViewToSelectionToolStripMenuItem
			// 
			panViewToSelectionToolStripMenuItem.Name = "panViewToSelectionToolStripMenuItem";
			panViewToSelectionToolStripMenuItem.ShortcutKeyDisplayString = "Shift+H";
			panViewToSelectionToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			panViewToSelectionToolStripMenuItem.Text = "Pan view to selection";
			panViewToSelectionToolStripMenuItem.Click += panViewToSelectionToolStripMenuItem_Click;
			// 
			// insertToolStripMenuItem
			// 
			insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newLineAfterCurrentToolStripMenuItem, newLineToolStripMenuItem, toolStripSeparator7, rapidToolStripMenuItem, lineToolStripMenuItem, clockwiseArcToolStripMenuItem, counterclockwiseArcToolStripMenuItem, commentToolStripMenuItem, commentBeforeCurrentToolStripMenuItem, toolStripSeparator8, backgroundImageToolStripMenuItem, textShapeToolStripMenuItem });
			insertToolStripMenuItem.Name = "insertToolStripMenuItem";
			insertToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			insertToolStripMenuItem.Text = "Insert";
			// 
			// newLineAfterCurrentToolStripMenuItem
			// 
			newLineAfterCurrentToolStripMenuItem.Name = "newLineAfterCurrentToolStripMenuItem";
			newLineAfterCurrentToolStripMenuItem.ShortcutKeyDisplayString = "I";
			newLineAfterCurrentToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			newLineAfterCurrentToolStripMenuItem.Text = "New line after current";
			newLineAfterCurrentToolStripMenuItem.Click += newLineAfterCurrentToolStripMenuItem_Click;
			// 
			// newLineToolStripMenuItem
			// 
			newLineToolStripMenuItem.Name = "newLineToolStripMenuItem";
			newLineToolStripMenuItem.ShortcutKeyDisplayString = "Shift+I";
			newLineToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			newLineToolStripMenuItem.Text = "New line before current";
			newLineToolStripMenuItem.Click += newLineToolStripMenuItem_Click;
			// 
			// toolStripSeparator7
			// 
			toolStripSeparator7.Name = "toolStripSeparator7";
			toolStripSeparator7.Size = new System.Drawing.Size(245, 6);
			// 
			// rapidToolStripMenuItem
			// 
			rapidToolStripMenuItem.Name = "rapidToolStripMenuItem";
			rapidToolStripMenuItem.ShortcutKeyDisplayString = "0";
			rapidToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			rapidToolStripMenuItem.Text = "Rapid";
			rapidToolStripMenuItem.Click += rapidToolStripMenuItem_Click;
			// 
			// lineToolStripMenuItem
			// 
			lineToolStripMenuItem.Name = "lineToolStripMenuItem";
			lineToolStripMenuItem.ShortcutKeyDisplayString = "1";
			lineToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			lineToolStripMenuItem.Text = "Line";
			lineToolStripMenuItem.Click += lineToolStripMenuItem_Click;
			// 
			// clockwiseArcToolStripMenuItem
			// 
			clockwiseArcToolStripMenuItem.Name = "clockwiseArcToolStripMenuItem";
			clockwiseArcToolStripMenuItem.ShortcutKeyDisplayString = "2";
			clockwiseArcToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			clockwiseArcToolStripMenuItem.Text = "Clockwise arc";
			clockwiseArcToolStripMenuItem.Click += clockwiseArcToolStripMenuItem_Click;
			// 
			// counterclockwiseArcToolStripMenuItem
			// 
			counterclockwiseArcToolStripMenuItem.Name = "counterclockwiseArcToolStripMenuItem";
			counterclockwiseArcToolStripMenuItem.ShortcutKeyDisplayString = "3";
			counterclockwiseArcToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			counterclockwiseArcToolStripMenuItem.Text = "Counterclockwise arc";
			counterclockwiseArcToolStripMenuItem.Click += counterclockwiseArcToolStripMenuItem_Click;
			// 
			// commentToolStripMenuItem
			// 
			commentToolStripMenuItem.Name = "commentToolStripMenuItem";
			commentToolStripMenuItem.ShortcutKeyDisplayString = ";";
			commentToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			commentToolStripMenuItem.Text = "Comment after current";
			commentToolStripMenuItem.Click += commentToolStripMenuItem_Click;
			// 
			// commentBeforeCurrentToolStripMenuItem
			// 
			commentBeforeCurrentToolStripMenuItem.Name = "commentBeforeCurrentToolStripMenuItem";
			commentBeforeCurrentToolStripMenuItem.ShortcutKeyDisplayString = "Shift+;";
			commentBeforeCurrentToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			commentBeforeCurrentToolStripMenuItem.Text = "Comment before current";
			commentBeforeCurrentToolStripMenuItem.Click += commentBeforeCurrentToolStripMenuItem_Click;
			// 
			// toolStripSeparator8
			// 
			toolStripSeparator8.Name = "toolStripSeparator8";
			toolStripSeparator8.Size = new System.Drawing.Size(245, 6);
			// 
			// backgroundImageToolStripMenuItem
			// 
			backgroundImageToolStripMenuItem.Name = "backgroundImageToolStripMenuItem";
			backgroundImageToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+B";
			backgroundImageToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			backgroundImageToolStripMenuItem.Text = "Background image ...";
			backgroundImageToolStripMenuItem.Click += backgroundImageToolStripMenuItem_Click;
			// 
			// textShapeToolStripMenuItem
			// 
			textShapeToolStripMenuItem.Name = "textShapeToolStripMenuItem";
			textShapeToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+T";
			textShapeToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
			textShapeToolStripMenuItem.Text = "Text shape ...";
			textShapeToolStripMenuItem.Click += textShapeToolStripMenuItem_Click;
			// 
			// aboutToolStripMenuItem
			// 
			aboutToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
			aboutToolStripMenuItem.Text = "About";
			aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
			// 
			// convertToOutlineToolStripMenuItem
			// 
			convertToOutlineToolStripMenuItem.Name = "convertToOutlineToolStripMenuItem";
			convertToOutlineToolStripMenuItem.ShortcutKeyDisplayString = "O";
			convertToOutlineToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			convertToOutlineToolStripMenuItem.Text = "Convert to outline ...";
			convertToOutlineToolStripMenuItem.Click += convertToOutlineToolStripMenuItem_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(800, 538);
			Controls.Add(splitContainer);
			Controls.Add(menuStrip);
			MainMenuStrip = menuStrip;
			Name = "MainForm";
			Text = "GCEd";
			WindowState = System.Windows.Forms.FormWindowState.Maximized;
			splitContainer.Panel1.ResumeLayout(false);
			splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
			splitContainer.ResumeLayout(false);
			tableLayoutPanel1.ResumeLayout(false);
			menuStrip.ResumeLayout(false);
			menuStrip.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Canvas canvas;
		private OperationProperties operationProperties;
		private OperationsList operationsList;
		private LineEditor lineEditor;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem convertToAbsoluteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem convertToRelativeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem moveEndpointToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem moveOffsetToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newLineToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem rapidToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem lineToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clockwiseArcToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem counterclockwiseArcToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
		private System.Windows.Forms.ToolStripMenuItem backgroundImageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem textShapeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showGridToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem snapToGridToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showCoordinatesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showPerformanceStatsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem snapToItemsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem commentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newLineAfterCurrentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem commentBeforeCurrentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem panzoomToFitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem panViewToSelectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem translateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem snapToAxesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem convertToOutlineToolStripMenuItem;
	}
}