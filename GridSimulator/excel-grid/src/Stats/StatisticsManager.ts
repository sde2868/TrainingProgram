import { DataStore } from "../Data/DataStore";
import type { ColumnModel } from "../Models/Column";
import type { RowModel } from "../Models/Row";
import { SelectionManager } from "../Selection/SelectionManager";

export interface Statistics {
    count: number;
    min: number;
    max: number;
    sum: number;
    average: number;
}

export class StatisticsManager {
    calculateSelection(dataStore: DataStore, selectionManager: SelectionManager, rowModel: RowModel, columnModel: ColumnModel): Statistics {
        const bounds = selectionManager.getBounds(rowModel.getTotalRows(), columnModel.getTotalColumns());
        if (!bounds) {
            return this.getEmptyStatistics();
        }
        const { minRow, maxRow, minColumn, maxColumn } = bounds;

        let count = 0;
        let min = Infinity;
        let max = -Infinity;
        let sum = 0;

        for (let row = minRow; row <= maxRow; row++) {
            for (let col = minColumn; col <= maxColumn; col++) {
                const value = dataStore.getCell(row, col);
                const numericValue = Number(value);
                if (value === "" || value === null || value === undefined || Number.isNaN(numericValue)) {
                    continue;
                }
                count++;
                min = Math.min(min, numericValue);
                max = Math.max(max, numericValue);
                sum += numericValue;
            }
        }
        return {
            count,
            min: count > 0 ? min : 0,
            max: count > 0 ? max : 0,
            sum,
            average: count > 0 ? sum / count : 0
        }
    }

    private getEmptyStatistics(): Statistics {
        return {
            count: 0,
            min: 0,
            max: 0,
            sum: 0,
            average: 0
        };
    }
}