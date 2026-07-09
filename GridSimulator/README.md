# GRID SIMULATOR
This project simulates microsoft excel grid using OOPs and SOLID principles applied in typescript.

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