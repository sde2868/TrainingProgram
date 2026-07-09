export class ColumnModel {
    private defaultColumnWidth = 100;
    private readonly defaultRowHeaderWidth = 60;
    private columnWidths: number[] = [];
    private columnOffsets: number[] = [];
    private totalColumns = 100;
    constructor(headerWidth: number = 60) {
        this.columnWidths = new Array(this.totalColumns).fill(this.defaultColumnWidth);
        this.rebuildOffsets(headerWidth);
    }
    getTotalWidth(): number {
        if (this.columnOffsets.length === 0) {
            return 0;
        }
        const last = this.columnOffsets[this.columnOffsets.length - 1];
        return (last + this.columnWidths[this.columnOffsets.length - 1]);
    }
    getDefaultColumnWidth(): number {
        return this.defaultColumnWidth;
    }
    getTotalColumns(): number {
        return this.totalColumns;
    }
    getColumnWidths(): number[] {
        return this.columnWidths;
    }
    getWidth(column: number): number {
        return this.columnWidths[column] ?? this.defaultColumnWidth;
    }
    setWidth(column: number, width: number): void {
        this.columnWidths[column] = width;
    }
    getOffset(column: number): number {
        return this.columnOffsets[column];
    }
    getOffsets(): number[] {
        return this.columnOffsets;
    }
    getIndex(x: number): number {
        let low = 0;
        let high = this.columnOffsets.length - 1;
        while (low <= high) {
            const mid = Math.floor((low + high) / 2);
            const start = this.columnOffsets[mid];
            const end = start + this.columnWidths[mid];
            if (x >= start && x < end) {
                return mid;
            }
            if (x < start) {
                high = mid - 1;
            }
            else {
                low = mid + 1;
            }
        }
        return Math.max(0, Math.min(this.columnOffsets.length - 1, low));
    }
    rebuildOffsets(headerWidth: number = this.defaultRowHeaderWidth): void {
        this.columnOffsets = [];
        let current = headerWidth;
        for (let col = 0; col < this.columnWidths.length; col++) {
            this.columnOffsets.push(current);
            current += this.getWidth(col);
        }
    }
}