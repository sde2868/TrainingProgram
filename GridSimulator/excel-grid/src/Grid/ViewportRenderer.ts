import { Renderer } from "./Renderer";
import { DataStore } from "../Data/DataStore";
import { SelectionManager } from "../Selection/SelectionManager";
import { FormulaEngine } from "../Formula/FormulaEngine";
import { RowModel } from "../Models/Row";
import { ColumnModel } from "../Models/Column";
import { ScrollManager } from "../Scroll/ScrollManager";

export class ViewportRenderer {
    private canvas: HTMLCanvasElement;
    private renderer: Renderer;
    private rowModel: RowModel;
    private columnModel: ColumnModel;
    private scrollManager: ScrollManager;
    private selectionManager: SelectionManager;
    private dataStore: DataStore;
    private formulaEngine: FormulaEngine;
    constructor(
        canvas: HTMLCanvasElement,
        renderer: Renderer,
        rowModel: RowModel,
        columnModel: ColumnModel,
        scrollManager: ScrollManager,
        selectionManager: SelectionManager,
        dataStore: DataStore,
        formulaEngine: FormulaEngine
    ) {
        this.canvas = canvas;
        this.renderer = renderer;
        this.rowModel = rowModel;
        this.columnModel = columnModel;
        this.scrollManager = scrollManager;
        this.selectionManager = selectionManager;
        this.dataStore = dataStore;
        this.formulaEngine = formulaEngine;
    }
    render(activeRow: number, activeColumn: number): void {
        this.renderer.clear(this.canvas.width, this.canvas.height);
        this.renderCells(activeRow, activeColumn);
        this.renderColumnHeaders();
        this.renderRowHeaders();
        this.renderCornerHeader();
    }
    private renderCornerHeader(): void {
        const isSelected = this.selectionManager.getSelectionType() === "all";
        this.renderer.drawHeader(0, 0, this.rowModel.getRowHeaderWidth(), this.rowModel.getHeaderRowHeight(), "", isSelected);
    }
    private renderColumnHeaders(): void {
        const selectionType = this.selectionManager.getSelectionType();
        const bounds = this.selectionManager.getBounds(this.rowModel.getTotalRows(), this.columnModel.getTotalColumns());
        if (!bounds) {
            return;
        }
        const { minColumn, maxColumn } = bounds;
        const columns = this.dataStore.getColumns();
        const viewportRight = this.scrollManager.getX() + this.canvas.width;
        const startColumn = this.columnModel.getIndex(this.scrollManager.getX());
        const endColumn = Math.min(this.columnModel.getTotalColumns(), this.columnModel.getIndex(viewportRight) + 2);
        this.ctx.save();
        this.ctx.beginPath();
        this.ctx.rect(this.rowModel.getRowHeaderWidth(), 0, this.canvas.width - this.rowModel.getRowHeaderWidth(), this.rowModel.getHeaderRowHeight());
        this.ctx.clip();
        for (let col = startColumn; col < endColumn; col++) {
            const x = this.columnModel.getOffset(col) - this.scrollManager.getX();
            const width = this.columnModel.getWidth(col);
            const isSelected = (selectionType === "column" || selectionType === "all") && col >= minColumn && col <= maxColumn;
            const headerText = col < columns.length ? columns[col] : this.getExcelColumnName(col);
            this.renderer.drawHeader(x, 0, width, this.rowModel.getHeaderRowHeight(), headerText, isSelected);
        }
        this.ctx.restore();
    }
    private renderRowHeaders(): void {
        const selectionType = this.selectionManager.getSelectionType();
        const bounds = this.selectionManager.getBounds(this.rowModel.getTotalRows(), this.columnModel.getTotalColumns());
        if (!bounds) {
            return;
        }
        const { minRow, maxRow } = bounds;
        const viewportBottom = this.scrollManager.getY() + this.canvas.height;
        const startRow = this.rowModel.getIndex(this.scrollManager.getY());
        const endRow = Math.min(this.rowModel.getTotalRows(), this.rowModel.getIndex(viewportBottom) + 2);
        this.ctx.save();
        this.ctx.beginPath();
        this.ctx.rect(0, this.rowModel.getHeaderRowHeight(), this.rowModel.getRowHeaderWidth(), this.canvas.height - this.rowModel.getHeaderRowHeight());
        this.ctx.clip();
        for (let row = startRow; row < endRow; row++) {
            const y = this.rowModel.getOffset(row) - this.scrollManager.getY();
            const height = this.rowModel.getHeight(row);
            const isSelected = (selectionType === "row" || selectionType === "all") && row >= minRow && row <= maxRow;
            this.renderer.drawHeader(0, y, this.rowModel.getRowHeaderWidth(), height, String(row + 1), isSelected);
        }
        this.ctx.restore();
    }
    private renderCells(activeRow: number, activeColumn: number): void {
        const viewportRight = this.scrollManager.getX() + this.canvas.width;
        const viewportBottom = this.scrollManager.getY() + this.canvas.height;
        const startColumn = this.columnModel.getIndex(this.scrollManager.getX());
        const endColumn = Math.min(this.columnModel.getTotalColumns(), this.columnModel.getIndex(viewportRight) + 2);
        const startRow = this.rowModel.getIndex(this.scrollManager.getY());
        const endRow = Math.min(this.rowModel.getTotalRows(), this.rowModel.getIndex(viewportBottom) + 2);
        const bounds = this.selectionManager.getBounds(this.rowModel.getTotalRows(), this.columnModel.getTotalColumns());
        if (!bounds) {
            return;
        }
        const { minRow, maxRow, minColumn, maxColumn } = bounds;
        this.ctx.save();
        this.ctx.beginPath();
        this.ctx.rect(this.rowModel.getRowHeaderWidth(), this.rowModel.getHeaderRowHeight(), this.canvas.width - this.rowModel.getRowHeaderWidth(), this.canvas.height - this.rowModel.getHeaderRowHeight());
        this.ctx.clip();
        for (let row = startRow; row < endRow; row++) {
            const y = this.rowModel.getOffset(row) - this.scrollManager.getY();
            const height = this.rowModel.getHeight(row);
            for (let col = startColumn; col < endColumn; col++) {
                const x = this.columnModel.getOffset(col) - this.scrollManager.getX();
                const width = this.columnModel.getWidth(col);
                const rawValue = String(this.dataStore.getCell(row, col) ?? "");
                const displayValue = this.formulaEngine.evaluate(rawValue, this.dataStore);
                const isSelected = row >= minRow && row <= maxRow && col >= minColumn && col <= maxColumn;
                this.renderer.drawCell(x, y, width, height, String(displayValue), isSelected);
                if (row === activeRow && col === activeColumn) {
                    this.renderer.drawActiveCell(x, y, width, height);
                }
            }
        }
        this.ctx.restore();
    }
    private getExcelColumnName(index: number): string {
        let result = "";
        let n = index + 1;
        while (n > 0) {
            const remainder = (n - 1) % 26;
            result = String.fromCharCode(65 + remainder) + result;
            n = Math.floor((n - 1) / 26);
        }
        return result;
    }
    private get ctx(): CanvasRenderingContext2D {
        return this.renderer.getContext();
    }
}