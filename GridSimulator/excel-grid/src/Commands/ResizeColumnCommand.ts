import type { Command } from "./Command";

export class ResizeColumnCommand implements Command {
    private columnWidths: number[];
    private columnIndex: number;
    private oldWidth: number;
    private newWidth: number;
    constructor(
        columnWidths: number[],
        columnIndex: number,
        oldWidth: number,
        newWidth: number
    ) {
        this.columnWidths = columnWidths;
        this.columnIndex = columnIndex;
        this.oldWidth = oldWidth;
        this.newWidth = newWidth;
    }

    execute(): void {
        this.columnWidths[this.columnIndex] = this.newWidth;
    }

    undo(): void {
        this.columnWidths[this.columnIndex] = this.oldWidth;
    }
}