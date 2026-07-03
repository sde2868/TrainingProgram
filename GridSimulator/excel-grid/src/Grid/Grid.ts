import { Renderer } from "./Renderer";
import { DataStore } from "../Data/DataStore";
import { SelectionManager } from "../Selection/SelectionManager"
import { StatisticsManager } from "../Stats/StatisticsManager";
import { CommandManager } from "../Commands/CommandManager";
import { EditCellCommand } from "../Commands/EditCellCommand";
import { ResizeColumnCommand } from "../Commands/ResizeColumnCommand";
import { ResizeRowCommand } from "../Commands/ResizeRowCommand";

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

    // private getColumnFromX(x: number): number {
    //     let currentX = this.rowHeaderWidth;
    //     const columns = this.dataStore.getColumns();
    //     for (let col = 0; col < this.totalColumns; col++) {
    //         const width = this.getColumnWidth(col);
    //         if (x >= currentX && x < currentX + width) {
    //             return col;
    //         }
    //         currentX += width;
    //     }
    //     return -1;
    // }

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

    private getCellFromMouse(event: MouseEvent) {
        return {
            row: this.getRowFromY(event.offsetY + this.scrollY),
            column: this.getColumnFromXVirtual(event.offsetX + this.scrollX)
        }
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

    private handleWheel(event: WheelEvent): void {
        event.preventDefault();
        this.scrollX += event.deltaX;
        this.scrollY += event.deltaY;

        const maxScrollY = Math.max(0, this.getTotalHeight() - this.canvas.height);
        const maxScrollX = Math.max(0, this.getTotalWidth() - this.canvas.width);

        this.scrollY = Math.max(0, Math.min(this.scrollY, maxScrollY));
        this.scrollX = Math.max(0, Math.min(this.scrollX, maxScrollX));
        // console.log(
        //     "scrollX", this.scrollX,
        //     "scrollY", this.scrollY
        // );
        this.requestRender();
    }
    // private handleClick(event: MouseEvent): void {
    //     const startRow = Math.floor(this.scrollY / this.rowHeight);
    //     const startColumn = Math.floor(this.scrollX / this.columnWidth);
    //     const clickedColumn = startColumn + Math.floor(event.offsetX / this.columnWidth);
    //     const clickedRow = startRow + Math.floor(event.offsetY / this.rowHeight) - 1;
    //     if (clickedRow < 0) {
    //         return;
    //     }
    //     // this.selectionManager.selectCell(clickedRow, clickedColumn);
    //     console.log(clickedRow, clickedColumn);
    //     this.Render();
    // }
    private handleMouseDown(event: MouseEvent): void {
        if (this.editor && this.editor.style.display === "block") {
            this.editor.style.display = "none";
            this.saveEdit();
        }
        const { row, column } = this.getCellFromMouse(event);

        const resizeRow = this.getResizeRow(event.offsetY + this.scrollY);
        if (resizeRow >= 0 && event.offsetX < this.rowHeaderWidth) {
            this.isResizingRow = true;
            this.resizingRow = resizeRow;
            this.resizeStartHeight = this.getRowHeight(resizeRow);
            return;
        }
        if (event.offsetX < this.rowHeaderWidth) {
            if (row >= 0) {
                this.selectionManager.selectRow(row);
                this.requestRender();
            }
            return;
        }
        if (event.offsetY < this.rowHeight) {
            const resizeColumn = this.getResizeColumn(event.offsetX + this.scrollX);
            if (resizeColumn >= 0) {
                this.resizeStartWidth = this.getColumnWidth(resizeColumn);
                this.isResizingColumn = true;
                this.resizingColumn = resizeColumn;
                return;
            }
            this.selectionManager.selectColumn(column);
            this.requestRender();
            return;
        }
        // if (row < 0) {
        //     return;
        // }
        this.isSelecting = true;
        this.selectionManager.startSelection(row, column);
    }

    private handleMouseMove(event: MouseEvent): void {
        if (this.isResizingColumn) {
            const x = this.getColumnX(this.resizingColumn);
            const newWidth = event.offsetX + this.scrollX - x;
            this.columnWidths[this.resizingColumn] = Math.max(50, newWidth);
            this.rebuildColumnOffsets();
            this.requestRender();
            return;
        }
        if (this.isResizingRow) {
            const y = this.getRowY(this.resizingRow);
            const newHeight = event.offsetY + this.scrollY - y;
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
        // this.selectionManager.clearSelection();
        const values = this.statisticsManager.getSelectedNumericValues(this.dataStore, this.selectionManager);
        const stats = this.statisticsManager.calculate(values);
        console.log(stats);
        this.requestRender();
    }

    private handleDoubleClick(event: MouseEvent): void {
        if (event.offsetX < this.rowHeaderWidth) {
            return;
        }
        const { row, column } = this.getCellFromMouse(event);
        if (row < 0) return;
        const columns = this.dataStore.getColumns();
        // const value = this.dataStore.getCellValue(row, columns[column]);
        let value = "";
        if (column < columns.length) {
            value = String(this.dataStore.getCellValue(row, columns[column]) ?? "");
        }

        this.editingRow = row;
        this.editingColumn = column;
        this.editor!.value = String(value ?? "");

        const rect = this.canvas.getBoundingClientRect();

        // const startRow = this.getRowFromY(this.scrollY);
        // const startColumn = this.getColumnFromXVirtual(this.scrollX);

        // const screenRow = row - startRow;

        const x = rect.left + this.getColumnX(column) - this.scrollX;
        const y = rect.top + this.getRowY(row) - this.scrollY;

        this.editor!.style.left = `${x}px`;
        this.editor!.style.top = `${y}px`;
        this.editor!.style.width = `${this.getColumnWidth(column)}px`;
        this.editor!.style.height = `${this.getRowHeight(row)}px`;
        this.editor!.style.display = "block";
        this.editor!.focus();
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
        const columns = this.dataStore.getColumns();
        let maxRequiredHeight = this.defaultRowHeight;
        for (let col = 0; col < columns.length; col++) {
            const value = String(this.dataStore.getCellValue(row, columns[col]) ?? "");
            const totalLines = this.getWrappedLineCount(value, this.getColumnWidth(col) - 10);
            const requiredHeight = totalLines * 16 + 8;
            maxRequiredHeight = Math.max(maxRequiredHeight, requiredHeight);
        }
        this.rowHeights[row] = maxRequiredHeight;
    }

    private saveEdit(): void {
        if (this.editingRow < 0 || this.editingColumn < 0) {
            return;
        }
        const columns = this.dataStore.getColumns();
        if (this.editingColumn >= columns.length) {
            this.editor!.style.display = "none";
            this.editingRow = -1;
            this.editingColumn = -1;
            return;
        }
        const columnName = columns[this.editingColumn];
        // this.dataStore.setCellValue(this.editingRow, columns[this.editingColumn], this.editor!.value);
        const oldValue = this.dataStore.getCellValue(this.editingRow, columnName);
        const command = new EditCellCommand(
            this.dataStore,
            this.editingRow,
            columnName,
            oldValue,
            this.editor!.value
        );
        this.commandManager.execute(command);
        // const ctx = this.getContext();
        // const cellWidth = this.getColumnWidth(this.editingColumn) - 10;
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

    private handleKeyDown(event: KeyboardEvent): void {
        if (event.ctrlKey && event.key === "z") {
            this.commandManager.undo();
            this.rebuildColumnOffsets();
            this.rebuildRowOffsets();
            this.requestRender();
        }
        if (event.ctrlKey && event.key === "y") {
            this.commandManager.redo();
            this.rebuildColumnOffsets();
            this.rebuildRowOffsets();
            this.requestRender();
        }
    }

    constructor(
        canvas: HTMLCanvasElement,
        ctx: CanvasRenderingContext2D,
        dataStore: DataStore,
        selectionManager: SelectionManager,
        statisticsManager: StatisticsManager,
        commandManager: CommandManager
    ) {
        this.canvas = canvas;
        this.ctx = ctx;
        this.renderer = new Renderer(ctx);
        this.dataStore = dataStore;
        this.selectionManager = selectionManager;
        this.statisticsManager = statisticsManager;
        this.commandManager = commandManager;
        this.canvas.addEventListener("wheel", this.handleWheel.bind(this), { passive: false });
        // this.canvas.addEventListener("click", this.handleClick.bind(this));
        this.canvas.addEventListener("mousedown", this.handleMouseDown.bind(this));
        this.canvas.addEventListener("mousemove", this.handleMouseMove.bind(this));
        this.canvas.addEventListener("mouseup", this.handleMouseUp.bind(this));
        this.canvas.addEventListener("dblclick", this.handleDoubleClick.bind(this));
        window.addEventListener("keydown", this.handleKeyDown.bind(this));

        this.editor = document.createElement("textarea");
        this.editor.style.resize = "none";
        this.editor.style.overflow = "hidden";
        this.editor.style.position = "absolute";
        this.editor.style.display = "none";
        document.body.appendChild(this.editor);

        this.editor.addEventListener("keydown", (event) => {
            if (event.key === "Enter" && event.ctrlKey) {
                event.preventDefault();
                this.saveEdit();
            }
        });
        // const columns = this.dataStore.getColumns();
        this.columnWidths = new Array(this.totalColumns).fill(this.defaultColumnWidth);
        this.rowHeights = new Array(this.totalRows).fill(this.defaultRowHeight);
        this.rebuildRowOffsets();
        this.rebuildColumnOffsets();
    }

    render(): void {
        this.renderer.clear(this.canvas.width, this.canvas.height);

        const columns = this.dataStore.getColumns();

        // const visibleRows = Math.ceil(this.canvas.height / this.rowHeight);
        // const visibleColumns = Math.ceil((this.canvas.width - this.rowHeaderWidth) / this.columnWidth);
        // const visibleRows = this.dataStore.getRowCount();
        // const visibleColumns = columns.length;
        const viewportRight = this.scrollX + this.canvas.width;
        const startColumn = this.getColumnFromXVirtual(this.scrollX);
        const endColumn = Math.min(this.totalColumns, this.getColumnFromXVirtual(viewportRight) + 2);

        const startRow = this.getRowFromY(this.scrollY);
        // const startColumn = this.getColumnFromXVirtual(this.scrollX);

        const maxRows = this.totalRows;
        const viewportBottom = this.scrollY + this.canvas.height;
        const endRow = Math.min(maxRows, this.getRowFromY(viewportBottom) + 2);
        // const endColumn = this.getColumnFromXVirtual(this.scrollX + this.canvas.width) + 1;

        const minRow = Math.min(this.selectionManager.getStartRow(), this.selectionManager.getEndRow());
        const maxRow = Math.max(this.selectionManager.getStartRow(), this.selectionManager.getEndRow());

        const minColumn = Math.min(this.selectionManager.getStartColumn(), this.selectionManager.getEndColumn());
        const maxColumn = Math.max(this.selectionManager.getStartColumn(), this.selectionManager.getEndColumn());

        for (let col = startColumn; col < endColumn; col++) {
            const x = this.getColumnX(col) - this.scrollX;
            const width = this.getColumnWidth(col);
            let isSelected = false;
            if (this.selectionManager.getSelectionType() === "column") {
                isSelected = (col === minColumn);
            }
            // this.renderer.drawCell(x, 0, width, this.rowHeight, ((columns[col] !== undefined) ? columns[col] : ""), isSelected);
            const headerText = col < columns.length ? columns[col] : this.getExcelColumnName(col);
            this.renderer.drawCell(x, 0, width, this.rowHeight, headerText, isSelected);
        }
        // let currentY = this.rowHeight;
        for (let row = startRow; row < endRow; row++) {
            // const screenRow = row - startRow;
            // const height = this.getRowHeight(row);
            // const y = currentY;
            const rowHeaderY = this.getRowY(row) - this.scrollY;
            const rowHeaderHeight = this.getRowHeight(row);
            let isRowHeaderSelected = false;
            if (rowHeaderY + rowHeaderHeight < 0 || rowHeaderY > this.canvas.height) {
                continue;
            }
            if (this.selectionManager.getSelectionType() === "row") {
                isRowHeaderSelected = (row === minRow);
            }
            this.renderer.drawCell(0, rowHeaderY, this.rowHeaderWidth, rowHeaderHeight, String(row + 1), isRowHeaderSelected);
            for (let col = startColumn; col < endColumn; col++) {
                // const screenRow = row - startRow;
                const x = this.getColumnX(col) - this.scrollX;
                const width = this.getColumnWidth(col);
                if (x + width < 0 || x > this.canvas.width) {
                    continue;
                }

                const y = this.getRowY(row) - this.scrollY;
                const height = this.getRowHeight(row);
                if (y + height < 0 || y > this.canvas.height) {
                    continue;
                }

                // const value = this.dataStore.getCellValue(row, columns[col]);
                let value = "";
                if (col < columns.length) {
                    value = String(this.dataStore.getCellValue(row, columns[col]) ?? "");
                }
                let isSelected =
                    row >= minRow &&
                    row <= maxRow &&
                    col >= minColumn &&
                    col <= maxColumn;

                const selectionType = this.selectionManager.getSelectionType();
                if (selectionType === "column") {
                    isSelected = (col === minColumn);
                }
                if (selectionType === "row") {
                    isSelected = (row === minRow);
                }
                this.renderer.drawCell(x, y, width, height, value, isSelected);
            }
        }
    }
}