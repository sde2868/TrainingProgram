import type { Command } from "./Command";
import { DataStore } from "../Data/DataStore";

export class EditCellCommand implements Command {
    private dataStore: DataStore;
    private row: number;
    private column: number;
    private oldValue: any;
    private newValue: any;

    constructor(
        dataStore: DataStore,
        row: number,
        column: number,
        oldValue: any,
        newValue: any
    ) {
        this.dataStore = dataStore;
        this.row = row;
        this.column = column;
        this.oldValue = oldValue;
        this.newValue = newValue;
    }

    execute(): void {
        this.dataStore.setCellValueByIndex(this.row, this.column, this.newValue);
    }

    undo(): void {
        this.dataStore.setCellValueByIndex(this.row, this.column, this.oldValue);
    }

    public getRow(): number {
        return this.row;
    }
}