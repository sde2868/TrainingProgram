import { RowModel } from "../Models/Row";
import { DataStore } from "../Data/DataStore";
import { ColumnModel } from "../Models/Column";

export class RowSizingManager {
    private rowModel: RowModel;
    private dataStore: DataStore;
    private columnModel: ColumnModel;
    private ctx: CanvasRenderingContext2D;
    constructor(rowModel: RowModel, dataStore: DataStore, columnModel: ColumnModel, ctx: CanvasRenderingContext2D) {
        this.rowModel = rowModel;
        this.dataStore = dataStore;
        this.columnModel = columnModel;
        this.ctx = ctx;
    }
    autoFitRowHeight(row: number): void {
        this.rowModel.autoFitRowHeight(
            row,
            this.dataStore.getColumnCount(),
            (column) => String(this.dataStore.getCell(row, column) ?? ""),
            (column) => this.columnModel.getWidth(column) - 10,
            (text) => this.ctx.measureText(text).width
        );
    }
    reAutoFitRows(): void {
        this.rowModel.reAutoFitRows(
            this.dataStore.getColumnCount(),
            (row, column) => String(this.dataStore.getCell(row, column) ?? ""),
            (column) => this.columnModel.getWidth(column) - 10,
            (text) => this.ctx.measureText(text).width
        );
    }
    clearAutoFitRow(row: number): void {
        this.rowModel.clearAutoFitRow(row);
    }
}