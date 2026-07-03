import type { Command } from "./Command";
import { DataStore } from "../Data/DataStore";

export class EditCellCommand implements Command {
    private dataStore: DataStore;
    private row: number;
    private columnName: string;
    private oldValue: any;
    private newValue: any;

    constructor(
        dataStore: DataStore,
        row: number,
        columnName: string,
        oldValue: any,
        newValue: any
    ) {
        this.dataStore = dataStore;
        this.row = row;
        this.columnName = columnName;
        this.oldValue = oldValue;
        this.newValue = newValue;
    }

    execute(): void {
        this.dataStore.setCellValue(this.row, this.columnName, this.newValue);
    }

    undo(): void {
        this.dataStore.setCellValue(this.row, this.columnName, this.oldValue);
    }
}