export class DataStore {
    private data: Record<string, any>[] = [];
    setData(data: Record<string, any>[]): void {
        this.data = data;
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
        if (this.data.length === 0) {
            return [];
        }
        return Object.keys(this.data[0]);
    }
    getRowCount(): number {
        return this.data.length;
    }
}