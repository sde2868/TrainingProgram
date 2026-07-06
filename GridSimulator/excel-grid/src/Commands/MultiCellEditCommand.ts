import type { Command } from "./Command";
import { DataStore } from "../Data/DataStore";
import type { CellEdit } from "./CellEdit";

export class MultiCellEditCommand implements Command {
    private dataStore: DataStore;
    private edits: CellEdit[];

    constructor(
        dataStore: DataStore,
        edits: CellEdit[]
    ) {
        this.dataStore = dataStore;
        this.edits = edits;
    }

    execute(): void {
        for (const edit of this.edits) {
            this.dataStore.setCell(edit.row, edit.column, edit.newValue);
        }
    }

    undo(): void {
        for (const edit of this.edits) {
            this.dataStore.setCell(edit.row, edit.column, edit.oldValue);
        }
    }
}