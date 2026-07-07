type SelectionType =
    | "cell"
    | "row"
    | "column"
    | "all";

export type SelectionBounds = {
    minRow: number;
    maxRow: number;
    minColumn: number;
    maxColumn: number;
} | null;

export class SelectionManager {
    private startRow = -1;
    private startColumn = -1;

    private endRow = -1;
    private endColumn = -1;

    private selectionType: SelectionType = "cell";

    private clamp(value: number, min: number, max: number): number {
        return Math.max(min, Math.min(value, max));
    }

    getBounds(totalRows: number, totalColumns: number): SelectionBounds {
        const lastRow = Math.max(0, totalRows - 1);
        const lastColumn = Math.max(0, totalColumns - 1);
        if (this.selectionType === "all") {
            return {
                minRow: 0,
                maxRow: lastRow,
                minColumn: 0,
                maxColumn: lastColumn
            };
        }
        if (this.selectionType === "row") {
            const row = this.clamp(this.startRow, 0, lastRow);
            return {
                minRow: row,
                maxRow: row,
                minColumn: 0,
                maxColumn: lastColumn
            };
        }
        if (this.selectionType === "column") {
            const column = this.clamp(this.startColumn, 0, lastColumn);
            return {
                minRow: 0,
                maxRow: lastRow,
                minColumn: column,
                maxColumn: column
            };
        }
        return {
            minRow: this.clamp(Math.min(this.startRow, this.endRow), 0, lastRow),
            maxRow: this.clamp(Math.max(this.startRow, this.endRow), 0, lastRow),
            minColumn: this.clamp(Math.min(this.startColumn, this.endColumn), 0, lastColumn),
            maxColumn: this.clamp(Math.max(this.startColumn, this.endColumn), 0, lastColumn)
        };
    }
    
    selectAll(): void {
        this.selectionType = "all";
        this.startRow = -1;
        this.startColumn = -1;
        this.endRow = -1;
        this.endColumn = -1;
    }

    selectRow(row: number): void {
        this.selectionType = "row";
        this.startRow = row;
        this.endRow = row;
        this.startColumn = -1;
        this.endColumn = -1;
    }

    selectColumn(column: number): void {
        this.selectionType = "column";
        this.startColumn = column;
        this.endColumn = column;
        this.startRow = -1;
        this.endRow = -1;
    }

    getSelectionType(): SelectionType {
        return this.selectionType;
    }

    startSelection(row: number, column: number): void {
        this.selectionType = "cell";
        this.startRow = row;
        this.startColumn = column;
        this.endRow = row;
        this.endColumn = column;
    }

    updateSelection(row: number, column: number): void {
        this.endRow = row;
        this.endColumn = column;
    }

    clearSelection(): void {
        this.selectionType = "cell";
        this.startRow = -1;
        this.startColumn = -1;
        this.endRow = -1;
        this.endColumn = -1;
    }

    getStartRow(): number {
        return this.startRow;
    }

    getStartColumn(): number {
        return this.startColumn;
    }

    getEndRow(): number {
        return this.endRow;
    }

    getEndColumn(): number {
        return this.endColumn;
    }
}