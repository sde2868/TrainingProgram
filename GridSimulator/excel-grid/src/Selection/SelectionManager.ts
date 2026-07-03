type SelectionType =
    | "cell"
    | "row"
    | "column";

export class SelectionManager {
    private startRow = -1;
    private startColumn = -1;

    private endRow = -1;
    private endColumn = -1;

    private selectionType: SelectionType = "cell";

    selectRow(row: number): void {
        this.selectionType = "row";
        this.startRow = row;
        this.endRow = row;
    }
    selectColumn(column: number): void {
        this.selectionType = "column";
        this.startColumn = column;
        this.endColumn = column;
        this.startRow = 0;
        this.endRow = Number.MAX_SAFE_INTEGER;
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