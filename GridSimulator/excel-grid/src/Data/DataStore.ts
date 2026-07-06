export class DataStore {
    private data: Record<string, any>[] = [];
    private columns: string[] = [];
    private extraCells = new Map<string, any>();
    private maxRowIndex = -1;
    private maxColumnIndex = -1;

    setData(data: Record<string, any>[]): void {
        this.data = data;
        this.maxRowIndex = data.length - 1;
        if (data.length > 0) {
            this.columns = Object.keys(data[0]);
            this.maxColumnIndex = this.columns.length - 1;
        }
    }
    getCellValue(rowIndex: number, columnName: string): any {
        return this.data[rowIndex]?.[columnName];
    }
    setCellValue(rowIndex: number, columnName: string, value: any): void {
        if (!this.data[rowIndex]) {
            return;
        }
        this.data[rowIndex][columnName] = value;
    }
    getColumns(): string[] {
        return this.columns;
    }
    getRowCount(): number {
        return this.maxRowIndex + 1;
    }
    getColumnCount(): number {
        return this.maxColumnIndex + 1;
    }
    getCellValueByIndex(rowIndex: number, columnIndex: number): any {
        const columnName = this.columns[columnIndex];
        if (columnName) {
            return this.getCellValue(rowIndex, columnName);
        }
        return this.extraCells.get(this.getCellKey(rowIndex, columnIndex));
    }
    setCellValueByIndex(rowIndex: number, columnIndex: number, value: any): void {
        while (this.data.length <= rowIndex) {
            this.data.push({});
        }
        this.maxRowIndex = Math.max(this.maxRowIndex, rowIndex);
        this.maxColumnIndex = Math.max(this.maxColumnIndex, columnIndex);
        const columnName = this.columns[columnIndex];
        if (columnName) {
            this.setCellValue(rowIndex, columnName, value);
            return;
        }
        this.extraCells.set(this.getCellKey(rowIndex, columnIndex), value);
    }
    private getCellKey(rowIndex: number, columnIndex: number): string {
        return `${rowIndex}:${columnIndex}`;
    }
    getCell(rowIndex: number, columnIndex: number): any {
        return this.getCellValueByIndex(rowIndex, columnIndex);
    }
    setCell(rowIndex: number, columnIndex: number, value: any): void {
        this.setCellValueByIndex(rowIndex, columnIndex, value);
    }
}