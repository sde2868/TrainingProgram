import { Renderer } from "./Renderer";
import { DataStore } from "../Data/DataStore";
import { SelectionManager } from "../Selection/SelectionManager"
import { StatisticsManager } from "../Stats/StatisticsManager";
import { CommandManager } from "../Commands/CommandManager";
import { EditCellCommand } from "../Commands/EditCellCommand";
import { ResizeColumnCommand } from "../Commands/ResizeColumnCommand";
import { ResizeRowCommand } from "../Commands/ResizeRowCommand";
import type { CellEdit } from "../Commands/CellEdit";
import { MultiCellEditCommand } from "../Commands/MultiCellEditCommand";

type GridArea =
    | "corner"
    | "column-header"
    | "row-header"
    | "cell";

export class Grid {
    private canvas: HTMLCanvasElement;
    private ctx: CanvasRenderingContext2D;
    private renderer: Renderer;
    private dataStore: DataStore;
    private selectionManager: SelectionManager
    private statisticsManager: StatisticsManager
    private commandManager: CommandManager

    private isSelecting = false;
    private rowHeaderWidth = 60;
    private rowHeight = 30;

    private defaultColumnWidth = 100;
    private columnWidths: number[] = [];
    private isResizingColumn = false;
    private resizingColumn = -1;
    private totalColumns = 100;
    private totalRows = 100000;

    private defaultRowHeight = 30;
    private rowHeights: number[] = [];
    private isResizingRow = false;
    private resizingRow = -1;

    private readonly RESIZE_MARGIN = 5;
    private resizeStartWidth = 0;
    private resizeStartHeight = 0;
    private autoFitRows = new Set<number>();

    private rowOffsets: number[] = [];
    private columnOffsets: number[] = [];
    private renderScheduled = false;

    private scrollX = 0;
    private scrollY = 0;

    private editingRow = -1;
    private editingColumn = -1;
    private editor?: HTMLTextAreaElement;

    private activeRow = 0;
    private activeColumn = 0;

    private clipboard = "";

    private statusCount: HTMLSpanElement;
    private statusAverage: HTMLSpanElement;
    private statusSum: HTMLSpanElement;

    private getGridArea(x: number, y: number): GridArea {
        const isInRowHeader = x < this.rowHeaderWidth;
        const isInColumnHeader = y < this.rowHeight;

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

        const x = (event.clientX - rect.left) * scaleX;
        const y = (event.clientY - rect.top) * scaleY;

        return { x, y };
    }

    public getContext(): CanvasRenderingContext2D {
        return this.ctx;
    }

    private getColumnWidth(column: number): number {
        return this.columnWidths[column] ?? this.defaultColumnWidth;
    }

    private getRowHeight(row: number): number {
        return (this.rowHeights[row] ?? this.defaultRowHeight);
    }

    private getColumnX(column: number): number {
        return this.columnOffsets[column];
    }

    private getRowY(row: number): number {
        return this.rowOffsets[row];
    }

    private getColumnFromXVirtual(x: number): number {
        let low = 0;
        let high = this.columnOffsets.length - 1;
        while (low <= high) {
            const mid = Math.floor((low + high) / 2);
            const start = this.columnOffsets[mid];
            const end = start + this.getColumnWidth(mid);
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

    private getRowFromY(y: number): number {
        let low = 0;
        let high = this.rowOffsets.length - 1;
        while (low <= high) {
            const mid = Math.floor((low + high) / 2);
            const start = this.rowOffsets[mid];
            const end = start + this.getRowHeight(mid);
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

    private getResizeColumn(mouseX: number): number {
        const column = this.getColumnFromXVirtual(mouseX);
        if (column < 0) {
            return -1;
        }
        const right = this.getColumnX(column) + this.getColumnWidth(column);
        if (Math.abs(mouseX - right) <= this.RESIZE_MARGIN) {
            return column;
        }
        return -1;
    }

    private getResizeRow(mouseY: number): number {
        const row = this.getRowFromY(mouseY);
        if (row < 0) {
            return -1;
        }
        const bottom = this.getRowY(row) + this.getRowHeight(row);
        if (Math.abs(mouseY - bottom) <= this.RESIZE_MARGIN) {
            return row;
        }
        return -1;
    }

    private rebuildRowOffsets(): void {
        this.rowOffsets = [];
        let current = this.rowHeight;
        for (let row = 0; row < this.rowHeights.length; row++) {
            this.rowOffsets.push(current);
            current += this.getRowHeight(row);
        }
    }

    private rebuildColumnOffsets(): void {
        this.columnOffsets = [];
        let current = this.rowHeaderWidth;
        for (let col = 0; col < this.columnWidths.length; col++) {
            this.columnOffsets.push(current);
            current += this.getColumnWidth(col);
        }
    }

    private getCellFromMouse(event: MouseEvent): { row: number; column: number } {
        const mouse = this.getCanvasMousePosition(event);
        const worldX = mouse.x + this.scrollX;
        const worldY = mouse.y + this.scrollY;
        return {
            row: this.getRowFromY(worldY),
            column: this.getColumnFromXVirtual(worldX)
        };
    }

    private getTotalHeight(): number {
        if (this.rowOffsets.length === 0) {
            return 0;
        }
        const last = this.rowOffsets[this.rowOffsets.length - 1];
        return (last + this.getRowHeight(this.rowOffsets.length - 1));
    }

    private getTotalWidth(): number {
        if (this.columnOffsets.length === 0) {
            return 0;
        }
        const last = this.columnOffsets[this.columnOffsets.length - 1];
        return (last + this.getColumnWidth(this.columnOffsets.length - 1));
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

    private requestRender(): void {
        if (this.renderScheduled) {
            return;
        }
        this.renderScheduled = true;
        requestAnimationFrame(() => {
            this.renderScheduled = false;
            this.render();
        });
    }

    private updateStatistics(): void {
        const stats = this.statisticsManager.calculateSelection(this.dataStore, this.selectionManager);
        this.statusCount.textContent = `Count: ${stats.count}`;
        this.statusAverage.textContent = `Average: ${stats.average.toFixed(2)}`;
        this.statusSum.textContent = `Sum: ${stats.sum}`;
    }

    private clampScroll(): void {
        const maxScrollX = Math.max(0, this.getTotalWidth() - this.canvas.width);
        const maxScrollY = Math.max(0, this.getTotalHeight() - this.canvas.height);
        this.scrollX = Math.max(0, Math.min(this.scrollX, maxScrollX));
        this.scrollY = Math.max(0, Math.min(this.scrollY, maxScrollY));
    }

    private handleWheel(event: WheelEvent): void {
        event.preventDefault();
        this.scrollX += event.deltaX;
        this.scrollY += event.deltaY;

        this.clampScroll();
        this.requestRender();
    }

    private handleMouseDown(event: MouseEvent): void {
        if (this.editor && this.editor.style.display === "block") {
            this.editor.style.display = "none";
            this.saveEdit();
        }
        const mouse = this.getCanvasMousePosition(event);
        const worldX = mouse.x + this.scrollX;
        const worldY = mouse.y + this.scrollY;
        const area = this.getGridArea(mouse.x, mouse.y);
        const { row, column } = this.getCellFromMouse(event);
        if (area === "corner") {
            this.selectAll();
            return;
        }
        if (area === "row-header") {
            const resizeRow = this.getResizeRow(worldY);
            if (resizeRow >= 0) {
                this.isResizingRow = true;
                this.resizingRow = resizeRow;
                this.resizeStartHeight = this.getRowHeight(resizeRow);
                return;
            }
            this.selectionManager.selectRow(row);
            this.updateStatistics();
            this.requestRender();
            return;
        }
        if (area === "column-header") {
            const resizeColumn = this.getResizeColumn(worldX);
            if (resizeColumn >= 0) {
                this.isResizingColumn = true;
                this.resizingColumn = resizeColumn;
                this.resizeStartWidth = this.getColumnWidth(resizeColumn);
                return;
            }
            this.selectionManager.selectColumn(column);
            this.updateStatistics();
            this.requestRender();
            return;
        }
        this.isSelecting = true;
        this.selectionManager.startSelection(row, column);
        this.activeRow = row;
        this.activeColumn = column;
        this.updateStatistics();
        this.requestRender();
    }

    private handleMouseMove(event: MouseEvent): void {
        const mouse = this.getCanvasMousePosition(event);
        if (!this.isResizingColumn && !this.isResizingRow) {
            const mouseX = mouse.x + this.scrollX;
            const mouseY = mouse.y + this.scrollY;
            const area = this.getGridArea(mouse.x, mouse.y);
            if (area === "column-header" && this.getResizeColumn(mouseX) >= 0) {
                this.canvas.style.cursor = "col-resize";
            }
            else if (area === "row-header" && this.getResizeRow(mouseY) >= 0) {
                this.canvas.style.cursor = "row-resize";
            }
            else if (area === "cell") {
                this.canvas.style.cursor = "cell";
            }
            else {
                this.canvas.style.cursor = "default";
            }
        }
        if (this.isResizingColumn) {
            const x = this.getColumnX(this.resizingColumn);
            const newWidth = mouse.x + this.scrollX - x;
            this.columnWidths[this.resizingColumn] = Math.max(50, newWidth);
            this.rebuildColumnOffsets();
            this.requestRender();
            return;
        }
        if (this.isResizingRow) {
            const y = this.getRowY(this.resizingRow);
            const newHeight = mouse.y + this.scrollY - y;
            this.rowHeights[this.resizingRow] = Math.max(20, newHeight);
            this.rebuildRowOffsets();
            this.requestRender();
            return;
        }
        if (!this.isSelecting) {
            return;
        }
        const { row, column } = this.getCellFromMouse(event);
        if (row < 0) {
            return;
        }
        this.selectionManager.updateSelection(row, column);
        this.requestRender();
    }

    private handleMouseUp(): void {
        if (this.isResizingColumn) {
            const newWidth = this.getColumnWidth(this.resizingColumn);
            const command = new ResizeColumnCommand(this.columnWidths, this.resizingColumn, this.resizeStartWidth, newWidth);
            this.commandManager.execute(command);
            this.isResizingColumn = false;
            this.resizingColumn = -1;
            this.rebuildColumnOffsets();
            for (const row of this.autoFitRows) {
                this.autoFitRowHeight(row);
            }
            this.rebuildRowOffsets();
            this.requestRender();
            return;
        }
        if (this.isResizingRow) {
            const newHeight = this.getRowHeight(this.resizingRow);
            const command = new ResizeRowCommand(this.rowHeights, this.resizingRow, this.resizeStartHeight, newHeight);
            this.commandManager.execute(command);
            this.autoFitRows.delete(this.resizingRow);
            this.isResizingRow = false;
            this.resizingRow = -1;
            this.rebuildRowOffsets();
            this.requestRender();
            return;
        }
        this.isSelecting = false;
        this.updateStatistics();
        this.requestRender();
    }

    private handleDoubleClick(event: MouseEvent): void {
        const mouse = this.getCanvasMousePosition(event);
        if (mouse.x < this.rowHeaderWidth || mouse.y < this.rowHeight) {
            return;
        }
        const { row, column } = this.getCellFromMouse(event);
        if (row < 0) return;
        this.beginEdit(row, column);
    }

    private getWrappedLineCount(text: string, width: number): number {
        let totalLines = 0;
        for (const rawLine of text.split("\n")) {
            let currentLine = "";
            for (const word of rawLine.split(" ")) {
                const testLine = currentLine ? `${currentLine} ${word}` : word;
                if (this.ctx.measureText(testLine).width > width && currentLine) {
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

    private autoFitRowHeight(row: number): void {
        const columns = this.dataStore.getColumnCount();
        let maxRequiredHeight = this.defaultRowHeight;
        for (let col = 0; col < columns; col++) {
            const value = String(this.dataStore.getCell(row, col) ?? "");
            const totalLines = this.getWrappedLineCount(value, this.getColumnWidth(col) - 10);
            const requiredHeight = totalLines * 16 + 8;
            maxRequiredHeight = Math.max(maxRequiredHeight, requiredHeight);
        }
        this.rowHeights[row] = maxRequiredHeight;
        if (maxRequiredHeight > this.defaultRowHeight) {
            this.autoFitRows.add(row);
        } else {
            this.autoFitRows.delete(row);
        }
    }

    private saveEdit(): void {
        if (this.editingRow < 0 || this.editingColumn < 0) {
            return;
        }
        const oldValue = this.dataStore.getCell(this.editingRow, this.editingColumn);
        const command = new EditCellCommand(
            this.dataStore,
            this.editingRow,
            this.editingColumn,
            oldValue,
            this.editor!.value
        );
        this.commandManager.execute(command);
        const lineHeight = 16;
        const totalLines = this.getWrappedLineCount(this.editor!.value, this.getColumnWidth(this.editingColumn) - 10);
        const requiredHeight = Math.max(this.defaultRowHeight, totalLines * lineHeight + 8);
        this.rowHeights[this.editingRow] = requiredHeight;
        if (requiredHeight > this.defaultRowHeight) {
            this.autoFitRows.add(this.editingRow);
        } else {
            this.autoFitRows.delete(this.editingRow);
        }
        this.rebuildRowOffsets();
        this.editor!.style.display = "none";
        this.editingRow = -1;
        this.editingColumn = -1;
        this.requestRender();
    }

    private ensureActiveCellVisible(): void {
        const cellLeft = this.getColumnX(this.activeColumn);
        const cellRight = cellLeft + this.getColumnWidth(this.activeColumn);

        const cellTop = this.getRowY(this.activeRow);
        const cellBottom = cellTop + this.getRowHeight(this.activeRow);

        const viewportLeft = this.scrollX + this.rowHeaderWidth;
        const viewportRight = this.scrollX + this.canvas.width;

        const viewportTop = this.scrollY + this.rowHeight;
        const viewportBottom = this.scrollY + this.canvas.height;

        if (cellLeft < viewportLeft) {
            this.scrollX = cellLeft - this.rowHeaderWidth;
        }
        else if (cellRight > viewportRight) {
            this.scrollX = cellRight - this.canvas.width;
        }

        if (cellTop < viewportTop) {
            this.scrollY = cellTop - this.rowHeight;
        }
        else if (cellBottom > viewportBottom) {
            this.scrollY = cellBottom - this.canvas.height;
        }
        this.clampScroll();
    }

    private selectAll(): void {
        this.activeRow = 0;
        this.activeColumn = 0;
        this.selectionManager.selectAll();
        this.updateStatistics();
        this.requestRender();
    }

    private moveActiveCell(): void {
        this.selectionManager.startSelection(this.activeRow, this.activeColumn);
        this.ensureActiveCellVisible();
        this.updateStatistics();
        this.requestRender();
    }

    private extendSelectionToActiveCell(): void {
        this.selectionManager.updateSelection(this.activeRow, this.activeColumn);
        this.ensureActiveCellVisible();
        this.updateStatistics();
        this.requestRender();
    }

    private handleKeyDown(event: KeyboardEvent): void {
        if (this.editor?.style.display === "block") {
            return;
        }
        const isModifierKey = event.ctrlKey || event.metaKey;
        if (event.key === "ArrowDown") {
            event.preventDefault();
            this.activeRow = Math.max(0, Math.min(this.activeRow + 1, this.totalRows - 1));
            if (event.shiftKey) {
                this.extendSelectionToActiveCell();
            } else {
                this.moveActiveCell();
            }
            return;
        }
        if (event.key === "ArrowUp") {
            event.preventDefault();
            this.activeRow = Math.max(0, Math.min(this.activeRow - 1, this.totalRows - 1));
            if (event.shiftKey) {
                this.extendSelectionToActiveCell();
            } else {
                this.moveActiveCell();
            }
            return;
        }
        if (event.key === "ArrowRight") {
            event.preventDefault();
            this.activeColumn = Math.max(0, Math.min(this.activeColumn + 1, this.totalColumns - 1));
            if (event.shiftKey) {
                this.extendSelectionToActiveCell();
            } else {
                this.moveActiveCell();
            }
            return;
        }
        if (event.key === "ArrowLeft") {
            event.preventDefault();
            this.activeColumn = Math.max(0, Math.min(this.activeColumn - 1, this.totalColumns - 1));
            if (event.shiftKey) {
                this.extendSelectionToActiveCell();
            } else {
                this.moveActiveCell();
            }
            return;
        }
        if (event.key === "Tab") {
            event.preventDefault();
            const direction = event.shiftKey ? -1 : 1;
            this.activeColumn = Math.max(0, Math.min(this.activeColumn + direction, this.totalColumns - 1));
            this.moveActiveCell();
            return;
        }
        if (event.key === "Enter") {
            event.preventDefault();
            const direction = event.shiftKey ? -1 : 1;
            this.activeRow = Math.max(0, Math.min(this.activeRow + direction, this.totalRows - 1));
            this.moveActiveCell();
            return;
        }
        if (isModifierKey && event.key.toLowerCase() === "z" && !event.shiftKey) {
            event.preventDefault();
            const command = this.commandManager.undo();
            if (command instanceof EditCellCommand) {
                this.autoFitRowHeight(command.getRow());
            }
            this.rebuildColumnOffsets();
            this.rebuildRowOffsets();
            this.requestRender();
            return;
        }
        if ((isModifierKey && event.key.toLowerCase() === "y") || (isModifierKey && event.shiftKey && event.key.toLowerCase() === "z")) {
            event.preventDefault();
            const command = this.commandManager.redo();
            if (command instanceof EditCellCommand) {
                this.autoFitRowHeight(command.getRow());
            }
            this.rebuildColumnOffsets();
            this.rebuildRowOffsets();
            this.requestRender();
            return;
        }
        if (event.key.length === 1 && !isModifierKey && !event.altKey) {
            event.preventDefault();
            this.beginEdit(this.activeRow, this.activeColumn, event.key);
            return;
        }
        if (event.key === "Delete" || event.key === "Backspace") {
            event.preventDefault();
            const oldValue = this.dataStore.getCell(this.activeRow, this.activeColumn);
            const command = new EditCellCommand(this.dataStore, this.activeRow, this.activeColumn, oldValue, "");
            this.commandManager.execute(command);
            this.autoFitRowHeight(
                this.activeRow
            );
            this.rebuildRowOffsets();
            this.requestRender();
            return;
        }
        if (event.ctrlKey && event.key === "a") {
            event.preventDefault();
            this.selectAll();
            return;
        }
        if (isModifierKey && event.key.toLowerCase() === "c") {
            event.preventDefault();
            const bounds = this.selectionManager.getBounds(this.totalRows, this.totalColumns);
            if (!bounds) {
                return;
            }
            const { minRow, maxRow, minColumn, maxColumn } = bounds;
            const selectedRowCount = maxRow - minRow + 1;
            const selectedColumnCount = maxColumn - minColumn + 1;
            const selectedCellCount = selectedRowCount * selectedColumnCount;
            if (selectedCellCount > 100000) {
                console.warn(`Copy blocked: ${selectedCellCount} cells selected`);
                return;
            }
            const rows: string[] = [];
            for (let row = minRow; row <= maxRow; row++) {
                const cells: string[] = [];
                for (let col = minColumn; col <= maxColumn; col++) {
                    cells.push(String(this.dataStore.getCell(row, col) ?? ""));
                }
                rows.push(cells.join("\t"));
            }
            this.clipboard = rows.join("\n");
            return;
        }
        if (isModifierKey && event.key.toLowerCase() === "v") {
            event.preventDefault();
            const rows = this.clipboard.split("\n");
            const edits: CellEdit[] = [];
            for (let rowOffset = 0; rowOffset < rows.length; rowOffset++) {
                const cells = rows[rowOffset].split("\t");
                for (let colOffset = 0; colOffset < cells.length; colOffset++) {
                    const targetRow = this.activeRow + rowOffset;
                    const targetColumn = this.activeColumn + colOffset;

                    const oldValue = this.dataStore.getCell(targetRow, targetColumn);
                    edits.push({
                        row: targetRow,
                        column: targetColumn,
                        oldValue,
                        newValue: cells[colOffset]
                    });
                }
            }
            const command = new MultiCellEditCommand(this.dataStore, edits);
            this.commandManager.execute(command);
            const affectedRows = new Set<number>();
            for (const edit of edits) {
                affectedRows.add(edit.row);
            }
            for (const row of affectedRows) {
                this.autoFitRowHeight(row);
            }
            this.rebuildRowOffsets();
            this.requestRender();
            return;
        }
    }

    private beginEdit(row: number, column: number, initialValue?: string): void {
        const value = initialValue ?? String(this.dataStore.getCell(row, column) ?? "");

        this.editingRow = row;
        this.editingColumn = column;
        this.editor!.value = value;

        const rect = this.canvas.getBoundingClientRect();
        const scaleX = rect.width / this.canvas.width;
        const scaleY = rect.height / this.canvas.height;

        const canvasX = (this.getColumnX(column) - this.scrollX);
        const canvasY = (this.getRowY(row) - this.scrollY);

        const x = rect.left + canvasX * scaleX;
        const y = rect.top + canvasY * scaleY;

        const width = this.getColumnWidth(column) * scaleX;
        const height = this.getRowHeight(row) * scaleY;

        this.editor!.style.left = `${x}px`;
        this.editor!.style.top = `${y}px`;
        this.editor!.style.width = `${width}px`;
        this.editor!.style.height = `${height}px`;
        this.editor!.style.display = "block";
        this.editor!.focus();
    }

    constructor(
        canvas: HTMLCanvasElement,
        ctx: CanvasRenderingContext2D,
        dataStore: DataStore,
        selectionManager: SelectionManager,
        statisticsManager: StatisticsManager,
        commandManager: CommandManager,
        statusCount: HTMLSpanElement,
        statusAverage: HTMLSpanElement,
        statusSum: HTMLSpanElement
    ) {
        this.canvas = canvas;
        this.statusCount = statusCount;
        this.statusAverage = statusAverage;
        this.statusSum = statusSum;
        this.ctx = ctx;
        this.renderer = new Renderer(ctx);
        this.dataStore = dataStore;
        this.selectionManager = selectionManager;
        this.statisticsManager = statisticsManager;
        this.commandManager = commandManager;
        this.canvas.addEventListener("wheel", this.handleWheel.bind(this), { passive: false });
        this.canvas.addEventListener("mousedown", this.handleMouseDown.bind(this));
        this.canvas.addEventListener("mousemove", this.handleMouseMove.bind(this));
        this.canvas.addEventListener("mouseup", this.handleMouseUp.bind(this));
        this.canvas.addEventListener("dblclick", this.handleDoubleClick.bind(this));
        window.addEventListener("keydown", this.handleKeyDown.bind(this));

        this.editor = document.createElement("textarea");
        this.editor.style.display = "none";
        document.body.appendChild(this.editor);

        this.editor.addEventListener("keydown", (event) => {
            if (event.key === "Enter" && event.ctrlKey) {
                event.preventDefault();
                this.saveEdit();
            }
        });
        this.columnWidths = new Array(this.totalColumns).fill(this.defaultColumnWidth);
        this.rowHeights = new Array(this.totalRows).fill(this.defaultRowHeight);
        this.rebuildRowOffsets();
        this.rebuildColumnOffsets();
        this.updateStatistics();
    }

    private renderCornerHeader(): void {
        const isSelected = this.selectionManager.getSelectionType() === "all";
        this.renderer.drawHeader(0, 0, this.rowHeaderWidth, this.rowHeight, "", isSelected);
    }

    private renderColumnHeaders(): void {
        const selectionType = this.selectionManager.getSelectionType();
        const bounds = this.selectionManager.getBounds(this.totalRows, this.totalColumns);
        if (!bounds) {
            return;
        }
        const { minRow, maxRow, minColumn, maxColumn } = bounds;
        const columns = this.dataStore.getColumns();
        const viewportRight = this.scrollX + this.canvas.width;
        const startColumn = this.getColumnFromXVirtual(this.scrollX);
        const endColumn = Math.min(this.totalColumns, this.getColumnFromXVirtual(viewportRight) + 2);
        this.ctx.save();
        this.ctx.beginPath();
        this.ctx.rect(this.rowHeaderWidth, 0, this.canvas.width - this.rowHeaderWidth, this.rowHeight);
        this.ctx.clip();
        for (let col = startColumn; col < endColumn; col++) {
            const x = this.getColumnX(col) - this.scrollX;
            const width = this.getColumnWidth(col);
            const isSelected = (selectionType === "column" || selectionType === "all") && col >= minColumn && col <= maxColumn;
            const headerText = col < columns.length ? columns[col] : this.getExcelColumnName(col);
            this.renderer.drawHeader(x, 0, width, this.rowHeight, headerText, isSelected);
        }
        this.ctx.restore();
    }

    private renderRowHeaders(): void {
        const selectionType = this.selectionManager.getSelectionType();
        const bounds = this.selectionManager.getBounds(this.totalRows, this.totalColumns);
        if (!bounds) {
            return;
        }
        const { minRow, maxRow, minColumn, maxColumn } = bounds;
        const viewportBottom = this.scrollY + this.canvas.height;
        const startRow = this.getRowFromY(this.scrollY);
        const endRow = Math.min(this.totalRows, this.getRowFromY(viewportBottom) + 2);
        this.ctx.save();
        this.ctx.beginPath();
        this.ctx.rect(0, this.rowHeight, this.rowHeaderWidth, this.canvas.height - this.rowHeight);
        this.ctx.clip();
        for (let row = startRow; row < endRow; row++) {
            const y = this.getRowY(row) - this.scrollY;
            const height = this.getRowHeight(row);
            const isSelected = (selectionType === "row" || selectionType === "all") && row >= minRow && row <= maxRow;
            this.renderer.drawHeader(0, y, this.rowHeaderWidth, height, String(row + 1), isSelected);
        }
        this.ctx.restore();
    }

    private renderCells(): void {
        const viewportRight = this.scrollX + this.canvas.width;
        const viewportBottom = this.scrollY + this.canvas.height;
        const startColumn = this.getColumnFromXVirtual(this.scrollX);
        const endColumn = Math.min(this.totalColumns, this.getColumnFromXVirtual(viewportRight) + 2);
        const startRow = this.getRowFromY(this.scrollY);
        const endRow = Math.min(this.totalRows, this.getRowFromY(viewportBottom) + 2);
        const bounds = this.selectionManager.getBounds(this.totalRows, this.totalColumns);
        if (!bounds) {
            return;
        }
        const { minRow, maxRow, minColumn, maxColumn } = bounds;
        this.ctx.save();
        this.ctx.beginPath();
        this.ctx.rect(
            this.rowHeaderWidth,
            this.rowHeight,
            this.canvas.width - this.rowHeaderWidth,
            this.canvas.height - this.rowHeight
        );
        this.ctx.clip();
        for (let row = startRow; row < endRow; row++) {
            const y = this.getRowY(row) - this.scrollY;
            const height = this.getRowHeight(row);
            for (let col = startColumn; col < endColumn; col++) {
                const x = this.getColumnX(col) - this.scrollX;
                const width = this.getColumnWidth(col);
                const value = String(this.dataStore.getCell(row, col) ?? "");
                const isSelected =
                    row >= minRow &&
                    row <= maxRow &&
                    col >= minColumn &&
                    col <= maxColumn;
                this.renderer.drawCell(x, y, width, height, value, isSelected);
                if (row === this.activeRow && col === this.activeColumn) {
                    this.renderer.drawActiveCell(x, y, width, height);
                }
            }
        }
        this.ctx.restore();
    }

    render(): void {
        this.renderer.clear(this.canvas.width, this.canvas.height);
        this.renderCells();
        this.renderColumnHeaders();
        this.renderRowHeaders();
        this.renderCornerHeader();
    }
}