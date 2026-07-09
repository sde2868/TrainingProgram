import { RowModel } from "../Models/Row";
import { ColumnModel } from "../Models/Column";
import { ResizeColumnCommand } from "../Commands/ResizeColumnCommand";
import { ResizeRowCommand } from "../Commands/ResizeRowCommand";

export class ResizeManager {
    private rowModel: RowModel;
    private columnModel: ColumnModel;
    private isResizingColumn = false;
    private resizingColumn = -1;
    private columnStartWidth = 0;
    private isResizingRow = false;
    private resizingRow = -1;
    private rowStartHeight = 0;
    constructor(rowModel: RowModel, columnModel: ColumnModel) {
        this.rowModel = rowModel;
        this.columnModel = columnModel;
    }
    startColumnResize(column: number): void {
        this.isResizingColumn = true;
        this.resizingColumn = column;
        this.columnStartWidth = this.columnModel.getWidth(column);
    }
    updateColumnResize(worldMouseX: number): void {
        if (!this.isResizingColumn) return;
        const x = this.columnModel.getOffset(this.resizingColumn);
        const newWidth = Math.max(50, worldMouseX - x);
        this.columnModel.setWidth(this.resizingColumn, newWidth);
    }
    finishColumnResize(): ResizeColumnCommand | null {
        if (!this.isResizingColumn) return null;
        const column = this.resizingColumn;
        const newWidth = this.columnModel.getWidth(column);
        const command = new ResizeColumnCommand(this.columnModel.getColumnWidths(), column, this.columnStartWidth, newWidth);
        this.isResizingColumn = false;
        this.resizingColumn = -1;
        this.columnStartWidth = 0;
        return command;
    }
    startRowResize(row: number): void {
        this.isResizingRow = true;
        this.resizingRow = row;
        this.rowStartHeight = this.rowModel.getHeight(row);
    }
    updateRowResize(worldMouseY: number): void {
        if (!this.isResizingRow) return;
        const y = this.rowModel.getOffset(this.resizingRow);
        const newHeight = Math.max(20, worldMouseY - y);
        this.rowModel.setHeight(this.resizingRow, newHeight);
    }
    finishRowResize(): ResizeRowCommand | null {
        if (!this.isResizingRow) return null;
        const row = this.resizingRow;
        const newHeight = this.rowModel.getHeight(row);
        const command = new ResizeRowCommand(this.rowModel.getRowHeights(), row, this.rowStartHeight, newHeight);
        this.isResizingRow = false;
        this.resizingRow = -1;
        this.rowStartHeight = 0;
        return command;
    }
    isCurrentlyResizing(): boolean {
        return this.isResizingColumn || this.isResizingRow;
    }
    isColumnResizing(): boolean {
        return this.isResizingColumn;
    }
    isRowResizing(): boolean {
        return this.isResizingRow;
    }
    getResizingRow(): number {
        return this.resizingRow;
    }
    getResizingColumn(): number {
        return this.resizingColumn;
    }
}