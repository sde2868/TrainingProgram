# EXCEL GRID SIMULATOR
This project simulates a Microsoft Excel-style grid rendered on Canvas using OOP and SOLID principles in TypeScript.

## Installation and Running
1. Go to the [Github Repo](https://github.com/sde2868/TrainingProgram/) of this project.
2. Download ZIP file.
3. Extract it and open /GridSimulator/excel-grid
4. Open terminal and run
```bash
npm install
```
5. Run
```bash
npm run dev
```

## Features
1. Excel-like Grid view on Canvas with separate highlighting for selected cell(s) and active cell.
1. Headers for rows and column showing row-number and column-name.
1. Cell edit on double-click or direct typing (Ctrl+Enter to save).
1. Resizable rows and columns by drag-holding edges in header.
1. Word-wrap for long text in a cell.
1. Auto-fit / auto-resize for row if a cell invloves multiple line text.
1. Copy / paste content for a cell / range of selected cell(s) using <kbd>Ctrl</kbd>+<kbd>C</kbd> / <kbd>Ctrl</kbd>+<kbd>V</kbd> respectively.
1. Undo / redo command applicable on row / column resize, copy / paste content, edit cell using <kbd>Ctrl</kbd>+<kbd>Z</kbd> / <kbd>Ctrl</kbd>+<kbd>Y</kbd> respectively.
1. Vertical navigation using <kbd>Enter</kbd> key (<kbd>Enter</kbd> for ⬇️ and <kbd>Shift</kbd>+<kbd>Enter</kbd> for ⬆️).
1. Horizontal navigation using <kbd>Tab</kbd> key (<kbd>Tab</kbd> for ➡️ and <kbd>Shift</kbd>+<kbd>Tab</kbd> for ⬅️).
1. Navigation using `Arrow` (⬆️, ⬇️, ⬅️, ➡️) keys.
1. Select a single cell / row / column by tapping on cell / row-header / column-header respectively.
1. Select a range of cells using <kbd>Shift</kbd>+`Arrow` (⬆️, ⬇️, ⬅️, ➡️) keys.
1. Select a range of cells by drag-holding mouse.
1. Select all cells (entire spreadsheet) using <kbd>Ctrl</kbd>+<kbd>A</kbd> or clicking on the corner cell at the start of the spreadsheet.
1. Auto-scroll on mouse drag selection.
1. Status bar showing stats like count, sum, average for selected cell(s).
1. Directly formula typing in a cell (start with <kbd>=</kbd>). Types implemented:
    1. Simple formula involving individual cells. Example:
        ```
        =A1+A10
        ```
    1. Range formula (COUNT, SUM, AVERAGE, MIN, MAX) involving range of cells. Example:
        ```
        =SUM(A10:C35)
        ```
    1. Nested (recursive) formula. Example:
        ```
        =SUM(A10:C35)
        ```
        in F5 and
        ```
        =SUM(A11:C45)
        ```
        in F6 and
        ```
        =SUM(F5:F6)
        ```
        in G5
1. Auto update (recalculate) formula results on change in cell values.
1. Appropriate error handling for formulas including:
    1. `#NAME?` error for unidentified formula / incorrent usage of formula.
    1. `#REF!` error for incorrect refernce passed.
    1. `#CIRC!` error for circular referencing in nested formulas.
    1. `#DIV/0!` error for division by zero when using formulas like average and division.

## Folder Structure
```
.
├── README.md
└── excel-grid
    ├── index.html
    ├── package-lock.json
    ├── package.json
    ├── src
    │   ├── Clipboard
    │   │   └── ClipboardManager.ts
    │   ├── Commands
    │   │   ├── CellEdit.ts
    │   │   ├── Command.ts
    │   │   ├── CommandManager.ts
    │   │   ├── EditCellCommand.ts
    │   │   ├── MultiCellEditCommand.ts
    │   │   ├── ResizeColumnCommand.ts
    │   │   └── ResizeRowCommand.ts
    │   ├── Data
    │   │   └── DataStore.ts
    │   ├── Editor
    │   │   └── EditorManager.ts
    │   ├── Formula
    │   │   └── FormulaEngine.ts
    │   ├── Grid
    │   │   ├── Grid.ts
    │   │   ├── InputManager.ts
    │   │   ├── Renderer.ts
    │   │   ├── ResizeManager.ts
    │   │   ├── RowSizingManager.ts
    │   │   ├── ShortcutHandler.ts
    │   │   └── ViewportRenderer.ts
    │   ├── Models
    │   │   ├── Cell.ts
    │   │   ├── Column.ts
    │   │   └── Row.ts
    │   ├── Scroll
    │   │   └── ScrollManager.ts
    │   ├── Selection
    │   │   └── SelectionManager.ts
    │   ├── Stats
    │   │   ├── StatisticsManager.ts
    │   │   └── StatusBarManager.ts
    │   ├── main.ts
    │   └── style.css
    └── tsconfig.json
```

## Application of OOP concepts
1. The project uses classes to represent real-world spreadsheet concepts such as rows, columns, cells, selection state, and rendering behavior.
1. Managers such as Grid, SelectionManager, EditorManager, ClipboardManager, and FormulaEngine encapsulate related responsibilities and keep the code organized.
1. Data is separated from presentation, with DataStore holding spreadsheet data while Renderer and ViewportRenderer handle drawing on the canvas.

## Application of SOLID concepts
1. Single Responsibility Principle is followed by giving each class a focused role, such as rendering, formula evaluation, selection handling, or command execution.
1. Open/Closed Principle is reflected in the command-based design, where new actions can be added by introducing new command classes without changing the manager logic.
1. Dependency Inversion is applied by injecting dependencies such as DataStore, SelectionManager, and FormulaEngine into the Grid and its subcomponents instead of hard-coding them.

## Application of Command Pattern
1. Editing a cell, resizing a row/column, and applying multi-cell paste actions are wrapped in command classes like EditCellCommand, ResizeRowCommand, ResizeColumnCommand, and MultiCellEditCommand.
1. Every command exposes execute() and undo() methods so state changes can be reversed consistently.
1. CommandManager centralizes the execution flow and maintains separate undo and redo stacks.

## Working of Undo / Redo
1. Every mutating action is first converted into a command object before being executed.
1. Undo pops the latest command from the undo stack and restores the previous state of the affected cell or dimension.
1. Redo replays the command from the redo stack after an undo, allowing users to recover the last reverted change.

## Working of Virtual Rendering
1. Only the visible portion of the sheet is painted, which makes the grid responsive even for a very large number of rows and columns.
1. RowModel and ColumnModel calculate offsets and sizes so the renderer can determine which rows and columns are currently in the viewport.
1. ViewportRenderer clips drawing to the visible area and renders only the cells, headers, and selection highlights that fall inside the current screen region.

## Data Generation and Loading
1. The app generates a synthetic dataset of 50,000 records in main.ts using a simple schema with fields such as firstName, lastName, age, and salary.
1. DataStore stores the generated records and derives the visible column names from the first item in the dataset.
1. The grid can dynamically update cell values, add new cells when needed, and recalculate formula-based outputs as the user edits the sheet.

## Covered Test Cases
1. Cell editing works for normal values as well as formula entries, and the entered value is saved correctly through the editor overlay.
1. Keyboard navigation is tested using arrow keys, Enter, Tab, Shift+Tab, and shortcut-based movement across the sheet.
1. Single-cell selection, row selection, column selection, range selection, select-all, and drag-selection are validated through both mouse and keyboard interaction.
1. Row and column resizing are checked by dragging the header borders and confirming the new size is applied correctly.
1. Copy and paste operations are tested for a single cell as well as a selected range of cells.
1. Undo and redo are verified for cell edits, resizing actions, and clipboard actions.
1. Formula evaluation is tested for simple arithmetic expressions, range functions such as SUM and AVERAGE, and nested formula references.
1. Formula error handling is checked for invalid names, invalid references, circular references, and division by zero.
1. Auto-fit row behavior is verified when cell content wraps across multiple lines.
1. Status bar calculations are checked for count, sum, and average values on selected cells.

## Performance Observations
1. The canvas-based implementation performs better than a traditional DOM grid for very large datasets because it only renders what is visible.
1. Virtual rendering and requestAnimationFrame-based redraws reduce unnecessary work during scrolling and selection updates.
1. Copy operations are intentionally limited to avoid excessive memory use for very large selections.

## Accessibility Considerations
1. The spreadsheet is rendered on a canvas, so it does not provide native accessibility semantics like a real table or form control.
1. Keyboard shortcuts are available for editing and navigation, but the experience is less discoverable than a standard spreadsheet UI.
1. Future improvements could include better focus handling, screen-reader support, and more visible keyboard guidance.

## Known Limitations and Next Improvements
1. Implemented on Canvas which hampers accessibility.
1. Unavailability of scroll bar (both vertical and horizontal).
1. Impersistant data, currently uses in-memory storage.
1. Formula restricted by current implementation types, mixing formulas (simple + range), long arithmetic expressions and brackets not supported.
1. Status bar updates triggers after mousedown when selecting a range of cells using mouse drag-holding.
1. Performance optimizations for larger datasets.
1. Better clipboard using system clipboard.