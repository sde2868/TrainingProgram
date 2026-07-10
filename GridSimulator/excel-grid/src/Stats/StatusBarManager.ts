import { StatisticsManager } from "./StatisticsManager";
import { DataStore } from "../Data/DataStore";
import { SelectionManager } from "../Selection/SelectionManager";
import type { RowModel } from "../Models/Row";
import type { ColumnModel } from "../Models/Column";

export class StatusBarManager {
    private statusCount: HTMLSpanElement;
    private statusAverage: HTMLSpanElement;
    private statusSum: HTMLSpanElement;
    private statusMin: HTMLSpanElement;
    private statusMax: HTMLSpanElement;
    private statisticsManager: StatisticsManager;
    private dataStore: DataStore;
    private selectionManager: SelectionManager;
    private rowModel: RowModel;
    private columnModel: ColumnModel;
    constructor(
        statusCount: HTMLSpanElement,
        statusAverage: HTMLSpanElement,
        statusSum: HTMLSpanElement,
        statusMin: HTMLSpanElement,
        statusMax: HTMLSpanElement,
        statisticsManager: StatisticsManager,
        dataStore: DataStore,
        selectionManager: SelectionManager,
        rowModel: RowModel,
        columnModel: ColumnModel
    ) {
        this.statusCount = statusCount;
        this.statusAverage = statusAverage;
        this.statusSum = statusSum;
        this.statusMin = statusMin;
        this.statusMax = statusMax;
        this.statisticsManager = statisticsManager;
        this.dataStore = dataStore;
        this.selectionManager = selectionManager;
        this.rowModel = rowModel;
        this.columnModel = columnModel;
    }
    update(): void {
        const stats = this.statisticsManager.calculateSelection(this.dataStore, this.selectionManager, this.rowModel, this.columnModel);
        this.statusCount.textContent = `Count: ${stats.count}`;
        this.statusAverage.textContent = `Average: ${stats.average.toFixed(2)}`;
        this.statusSum.textContent = `Sum: ${stats.sum}`;
        this.statusMin.textContent = `Min: ${stats.min}`;
        this.statusMax.textContent = `Max: ${stats.max}`;
    }
}