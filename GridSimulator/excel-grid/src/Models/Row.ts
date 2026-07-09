export class RowModel {
    private defaultRowHeight = 30;
    private rowHeaderWidth = 60;
    private headerRowHeight = 30;
    private rowHeights: number[] = [];
    private rowOffsets: number[] = [];
    private totalRows = 100000;
    private autoFitRows = new Set<number>();
    constructor() {
        this.rowHeights = new Array(this.totalRows).fill(this.defaultRowHeight);
        this.rebuildOffsets();
    }
    getTotalHeight(): number {
        if (this.rowOffsets.length === 0) {
            return 0;
        }
        const last = this.rowOffsets[this.rowOffsets.length - 1];
        return (last + this.getHeight(this.rowOffsets.length - 1));
    }
    getDefaultRowHeight(): number {
        return this.defaultRowHeight;
    }
    getTotalRows(): number {
        return this.totalRows;
    }
    getRowHeights(): number[] {
        return this.rowHeights;
    }
    getRowHeaderWidth(): number {
        return this.rowHeaderWidth;
    }
    getHeaderRowHeight(): number {
        return this.headerRowHeight;
    }
    getHeight(row: number): number {
        return (this.rowHeights[row] ?? this.defaultRowHeight);
    }
    setHeight(row: number, height: number): void {
        this.rowHeights[row] = height;
    }
    getOffset(row: number): number {
        return this.rowOffsets[row];
    }
    getIndex(y: number): number {
        let low = 0;
        let high = this.rowOffsets.length - 1;
        while (low <= high) {
            const mid = Math.floor((low + high) / 2);
            const start = this.rowOffsets[mid];
            const end = start + this.getHeight(mid);
            if (y >= start && y < end) {
                return mid;
            }
            if (y < start) {
                high = mid - 1;
            }
            else {
                low = mid + 1;
            }
        }
        return Math.max(0, Math.min(this.rowOffsets.length - 1, low));
    }
    calculateWrappedLineCount(text: string, width: number, measureText: (text: string) => number): number {
        let totalLines = 0;
        for (const rawLine of text.split("\n")) {
            let currentLine = "";
            for (const word of rawLine.split(" ")) {
                const testLine = currentLine ? `${currentLine} ${word}` : word;
                if (measureText(testLine) > width && currentLine) {
                    totalLines++;
                    currentLine = word;
                }
                else {
                    currentLine = testLine;
                }
            }
            totalLines++;
        }
        return totalLines;
    }
    autoFitRowHeight(row: number, columnCount: number, getCellValue: (column: number) => string, getColumnWidth: (column: number) => number, measureText: (text: string) => number): void {
        let maxRequiredHeight = this.defaultRowHeight;
        for (let col = 0; col < columnCount; col++) {
            const value = getCellValue(col);
            const totalLines = this.calculateWrappedLineCount(value, getColumnWidth(col), measureText);
            const requiredHeight = totalLines * 16 + 8;
            maxRequiredHeight = Math.max(maxRequiredHeight, requiredHeight);
        }
        this.setHeight(row, maxRequiredHeight);
        if (maxRequiredHeight > this.defaultRowHeight) {
            this.autoFitRows.add(row);
        } else {
            this.autoFitRows.delete(row);
        }
    }
    reAutoFitRows(columnCount: number, getCellValue: (row: number, column: number) => string, getColumnWidth: (column: number) => number, measureText: (text: string) => number): void {
        for (const row of Array.from(this.autoFitRows)) {
            this.autoFitRowHeight(row, columnCount, (column) => getCellValue(row, column), getColumnWidth, measureText);
        }
    }
    clearAutoFitRow(row: number): void {
        this.autoFitRows.delete(row);
    }
    getAutoFitRows(): number[] {
        return Array.from(this.autoFitRows);
    }
    rebuildOffsets(): void {
        this.rowOffsets = [];
        let current = this.headerRowHeight;
        for (let row = 0; row < this.rowHeights.length; row++) {
            this.rowOffsets.push(current);
            current += this.getHeight(row);
        }
    }
}