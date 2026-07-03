import type { Command } from "./Command";

export class ResizeRowCommand implements Command {
    private rowHeights: number[];
    private rowIndex: number;
    private oldHeight: number;
    private newHeight: number;
    constructor(
        rowHeights: number[],
        rowIndex: number,
        oldHeight: number,
        newHeight: number
    ) {
        this.rowHeights = rowHeights;
        this.rowIndex = rowIndex;
        this.oldHeight = oldHeight;
        this.newHeight = newHeight;
    }

    execute(): void {
        this.rowHeights[this.rowIndex] = this.newHeight;
    }

    undo(): void {
        this.rowHeights[this.rowIndex] = this.oldHeight;
    }
}