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
    │   ├── Commands
    │   │   ├── Command.ts
    │   │   ├── CommandManager.ts
    │   │   ├── EditCellCommand.ts
    │   │   ├── ResizeColumnCommand.ts
    │   │   └── ResizeRowCommand.ts
    │   ├── Data
    │   │   └── DataStore.ts
    │   ├── Grid
    │   │   ├── Grid.ts
    │   │   └── Renderer.ts
    │   ├── Models
    │   │   ├── Cell.ts
    │   │   ├── Column.ts
    │   │   └── Row.ts
    │   ├── Scroll
    │   │   └── ScrollManager.ts
    │   ├── Selection
    │   │   └── SelectionManager.ts
    │   ├── Stats
    │   │   └── StatisticsManager.ts
    │   ├── main.ts
    │   └── style.css
    └── tsconfig.json
```