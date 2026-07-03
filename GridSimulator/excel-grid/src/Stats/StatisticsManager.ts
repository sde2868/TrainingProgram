import { DataStore } from "../Data/DataStore";
import { SelectionManager } from "../Selection/SelectionManager";

export interface Statistics {
    count: number;
    min: number;
    max: number;
    sum: number;
    average: number;
}

export class StatisticsManager {

    getSelectedNumericValues(dataStore: DataStore, selectionManager: SelectionManager): number[] {
        const values: number[] = [];
        const columns = dataStore.getColumns();
        const selectionType = selectionManager.getSelectionType();

        if (selectionType === "column") {
            const selectedColumn = selectionManager.getStartColumn();
            for (let row = 0; row < dataStore.getRowCount(); row++) {
                const value = dataStore.getCellValue(row, columns[selectedColumn]);
                if (typeof value === "number") {
                    values.push(value);
                }
            }
            return values;
        }

        if (selectionType === "row") {
            const selectedRow = selectionManager.getStartRow();
            for (let col = 0; col < columns.length; col++) {
                const value = dataStore.getCellValue(selectedRow, columns[col]);
                if (typeof value === "number") {
                    values.push(value);
                }
            }
            return values;
        }

        const minRow = Math.min(selectionManager.getStartRow(), selectionManager.getEndRow());
        const maxRow = Math.max(selectionManager.getStartRow(), selectionManager.getEndRow());

        const minColumn = Math.min(selectionManager.getStartColumn(), selectionManager.getEndColumn());
        const maxColumn = Math.max(selectionManager.getStartColumn(), selectionManager.getEndColumn());

        for (let row = minRow; row <= maxRow; row++) {
            for (let col = minColumn; col <= maxColumn; col++) {
                const value = dataStore.getCellValue(row, columns[col]);
                if (typeof value === "number") {
                    values.push(value);
                }
            }
        }
        return values;
    }

    calculate(values: number[]): Statistics {
        let count = 0;
        let min = Number.MAX_SAFE_INTEGER;
        let max = Number.MIN_SAFE_INTEGER;
        let sum = 0;
        for (const value of values) {
            count++;
            min = Math.min(min, value);
            max = Math.max(max, value);
            sum += value;
        }
        return {
            count,
            min: count > 0 ? min : 0,
            max: count > 0 ? max : 0,
            sum,
            average: count > 0 ? sum / count : 0
        };
    }
}