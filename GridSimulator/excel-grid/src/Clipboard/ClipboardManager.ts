import { DataStore } from "../Data/DataStore";
import type { SelectionBounds } from "../Selection/SelectionManager";
import { MultiCellEditCommand } from "../Commands/MultiCellEditCommand";
import type { CellEdit } from "../Commands/CellEdit";

export type ClipboardPasteResult = {
    command: MultiCellEditCommand;
    affectedRows: Set<number>;
};

export class ClipboardManager {
    private content = "";
    copy(bounds: SelectionBounds, dataStore: DataStore): void {
        if (!bounds) {
            this.content = "";
            return;
        }
        const { minRow, maxRow, minColumn, maxColumn } = bounds;
        const selectedRowCount = maxRow - minRow + 1;
        const selectedColumnCount = maxColumn - minColumn + 1;
        const selectedCellCount = selectedRowCount * selectedColumnCount;
        if (selectedCellCount > 100000) {
            console.warn(`Copy blocked: ${selectedCellCount} cells selected`);
            this.content = "";
            return;
        }
        const rows: string[] = [];
        for (let row = minRow; row <= maxRow; row++) {
            const cells: string[] = [];
            for (let col = minColumn; col <= maxColumn; col++) {
                cells.push(String(dataStore.getCell(row, col) ?? ""));
            }
            rows.push(cells.join("\t"));
        }
        this.content = rows.join("\n");
    }
    paste(activeRow: number, activeColumn: number, dataStore: DataStore): ClipboardPasteResult | null {
        if (!this.content) {
            return null;
        }
        const rows = this.content.split(/\r?\n/);
        const edits: CellEdit[] = [];
        const affectedRows = new Set<number>();
        for (let rowOffset = 0; rowOffset < rows.length; rowOffset++) {
            const cells = rows[rowOffset].split("\t");
            for (let colOffset = 0; colOffset < cells.length; colOffset++) {
                const targetRow = activeRow + rowOffset;
                const targetColumn = activeColumn + colOffset;
                const oldValue = dataStore.getCell(targetRow, targetColumn);
                edits.push({
                    row: targetRow,
                    column: targetColumn,
                    oldValue,
                    newValue: cells[colOffset]
                });
                affectedRows.add(targetRow);
            }
        }
        return {
            command: new MultiCellEditCommand(dataStore, edits),
            affectedRows
        };
    }
    hasContent(): boolean {
        return this.content.length > 0;
    }
    getContent(): string {
        return this.content;
    }
    setContent(text: string): void {
        this.content = text;
    }
    clear(): void {
        this.content = "";
    }
}