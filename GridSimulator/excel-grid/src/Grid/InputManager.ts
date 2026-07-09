import { RowModel } from "../Models/Row";
import { ColumnModel } from "../Models/Column";
import { ScrollManager } from "../Scroll/ScrollManager";
import { ResizeManager } from "./ResizeManager";

type GridArea = "corner" | "column-header" | "row-header" | "cell";

type Callbacks = {
    saveEditor: () => void;
    beginEdit: (row: number, column: number) => void;
    selectAll: () => void;
    selectRow: (row: number) => void;
    selectColumn: (column: number) => void;
    startSelection: (row: number, column: number) => void;
    updateSelection: (row: number, column: number) => void;
    updateStatistics: () => void;
    requestRender: () => void;
    setActiveCell: (row: number, column: number) => void;
    finishColumnResize: () => void;
    finishRowResize: () => void;
    finishSelection: () => void;
    clampScroll: () => void;
    onAutoScrollSelection: (row: number, column: number) => void;
};
export class InputManager {
    private canvas: HTMLCanvasElement;
    private rowModel: RowModel;
    private columnModel: ColumnModel;
    private scrollManager: ScrollManager;
    private resizeManager: ResizeManager;
    private callbacks: Callbacks;
    private isSelecting = false;
    private readonly RESIZE_MARGIN = 5;
    constructor(
        canvas: HTMLCanvasElement,
        rowModel: RowModel,
        columnModel: ColumnModel,
        scrollManager: ScrollManager,
        resizeManager: ResizeManager,
        callbacks: Callbacks
    ) {
        this.canvas = canvas;
        this.rowModel = rowModel;
        this.columnModel = columnModel;
        this.scrollManager = scrollManager;
        this.resizeManager = resizeManager;
        this.callbacks = callbacks;
    }
    handleMouseDown(event: MouseEvent): void {
        this.callbacks.saveEditor();
        const mouse = this.getCanvasMousePosition(event);
        const worldX = mouse.x + this.scrollManager.getX();
        const worldY = mouse.y + this.scrollManager.getY();
        const area = this.getGridArea(mouse.x, mouse.y);
        const { row, column } = this.getCellFromMouse(event);
        if (area === "corner") {
            this.callbacks.selectAll();
            return;
        }
        if (area === "row-header") {
            const resizeRow = this.getResizeRow(worldY);
            if (resizeRow >= 0) {
                this.resizeManager.startRowResize(resizeRow);
                return;
            }
            this.callbacks.selectRow(row);
            this.callbacks.updateStatistics();
            this.callbacks.requestRender();
            return;
        }
        if (area === "column-header") {
            const resizeColumn = this.getResizeColumn(worldX);
            if (resizeColumn >= 0) {
                this.resizeManager.startColumnResize(resizeColumn);
                return;
            }
            this.callbacks.selectColumn(column);
            this.callbacks.updateStatistics();
            this.callbacks.requestRender();
            return;
        }
        this.isSelecting = true;
        this.callbacks.startSelection(row, column);
        this.callbacks.setActiveCell(row, column);
        this.callbacks.updateStatistics();
        this.callbacks.requestRender();
    }
    handleMouseMove(event: MouseEvent): void {
        const mouse = this.getCanvasMousePosition(event);
        if (!this.resizeManager.isColumnResizing() && !this.resizeManager.isRowResizing()) {
            const mouseX = mouse.x + this.scrollManager.getX();
            const mouseY = mouse.y + this.scrollManager.getY();
            const area = this.getGridArea(mouse.x, mouse.y);
            if (area === "column-header" && this.getResizeColumn(mouseX) >= 0) {
                this.canvas.style.cursor = "col-resize";
            } else if (area === "row-header" && this.getResizeRow(mouseY) >= 0) {
                this.canvas.style.cursor = "row-resize";
            } else if (area === "cell") {
                this.canvas.style.cursor = "cell";
            } else {
                this.canvas.style.cursor = "default";
            }
        }
        if (this.resizeManager.isColumnResizing()) {
            this.resizeManager.updateColumnResize(mouse.x + this.scrollManager.getX());
            this.columnModel.rebuildOffsets(this.rowModel.getRowHeaderWidth());
            this.callbacks.requestRender();
            return;
        }
        if (this.resizeManager.isRowResizing()) {
            this.resizeManager.updateRowResize(mouse.y + this.scrollManager.getY());
            this.rowModel.rebuildOffsets();
            this.callbacks.requestRender();
            return;
        }
        if (!this.isSelecting) {
            return;
        }
        const width = this.canvas.width;
        const height = this.canvas.height;
        this.scrollManager.updateAutoScroll(mouse.x, mouse.y, width, height, this.rowModel.getRowHeaderWidth(), this.rowModel.getHeaderRowHeight());
        if (this.scrollManager.hasAutoScroll()) {
            this.scrollManager.startAutoScroll(() => {
                const row = this.rowModel.getIndex(this.scrollManager.getY() + this.canvas.height - this.rowModel.getHeaderRowHeight());
                const column = this.columnModel.getIndex(this.scrollManager.getX() + this.canvas.width - this.rowModel.getRowHeaderWidth());
                this.callbacks.onAutoScrollSelection(row, column);
            });
        } else {
            this.scrollManager.stopAutoScroll();
        }
        const { row, column } = this.getCellFromMouse(event);
        if (row < 0) {
            return;
        }
        this.callbacks.updateSelection(row, column);
        this.callbacks.requestRender();
    }
    handleMouseUp(): void {
        this.scrollManager.stopAutoScroll();
        if (this.resizeManager.isColumnResizing()) {
            this.callbacks.finishColumnResize();
            return;
        }
        if (this.resizeManager.isRowResizing()) {
            this.callbacks.finishRowResize();
            return;
        }
        this.isSelecting = false;
        this.callbacks.finishSelection();
    }
    handleDoubleClick(event: MouseEvent): void {
        const mouse = this.getCanvasMousePosition(event);
        if (mouse.x < this.rowModel.getRowHeaderWidth() || mouse.y < this.rowModel.getHeaderRowHeight()) {
            return;
        }
        const { row, column } = this.getCellFromMouse(event);
        if (row < 0) {
            return;
        }
        this.callbacks.beginEdit(row, column);
    }
    private getGridArea(x: number, y: number): GridArea {
        const isInRowHeader = x < this.rowModel.getRowHeaderWidth();
        const isInColumnHeader = y < this.rowModel.getHeaderRowHeight();
        if (isInRowHeader && isInColumnHeader) {
            return "corner";
        }
        if (isInColumnHeader) {
            return "column-header";
        }
        if (isInRowHeader) {
            return "row-header";
        }
        return "cell";
    }
    private getCanvasMousePosition(event: MouseEvent): { x: number; y: number } {
        const rect = this.canvas.getBoundingClientRect();
        const scaleX = this.canvas.width / rect.width;
        const scaleY = this.canvas.height / rect.height;
        return {
            x: (event.clientX - rect.left) * scaleX,
            y: (event.clientY - rect.top) * scaleY
        };
    }
    private getResizeColumn(mouseX: number): number {
        const column = this.columnModel.getIndex(mouseX);
        if (column < 0) {
            return -1;
        }
        const right = this.columnModel.getOffset(column) + this.columnModel.getWidth(column);
        return Math.abs(mouseX - right) <= this.RESIZE_MARGIN ? column : -1;
    }
    private getResizeRow(mouseY: number): number {
        const row = this.rowModel.getIndex(mouseY);
        if (row < 0) {
            return -1;
        }
        const bottom = this.rowModel.getOffset(row) + this.rowModel.getHeight(row);
        return Math.abs(mouseY - bottom) <= this.RESIZE_MARGIN ? row : -1;
    }
    private getCellFromMouse(event: MouseEvent): { row: number; column: number } {
        const mouse = this.getCanvasMousePosition(event);
        return {
            row: this.rowModel.getIndex(mouse.y + this.scrollManager.getY()),
            column: this.columnModel.getIndex(mouse.x + this.scrollManager.getX())
        };
    }
}