import { StatisticsManager } from "./StatisticsManager";
import { DataStore } from "../Data/DataStore";
import { SelectionManager } from "../Selection/SelectionManager";

export class StatusBarManager {
    private statusCount: HTMLSpanElement;
    private statusAverage: HTMLSpanElement;
    private statusSum: HTMLSpanElement;
    private statisticsManager: StatisticsManager;
    private dataStore: DataStore;
    private selectionManager: SelectionManager;
    constructor(
        statusCount: HTMLSpanElement,
        statusAverage: HTMLSpanElement,
        statusSum: HTMLSpanElement,
        statisticsManager: StatisticsManager,
        dataStore: DataStore,
        selectionManager: SelectionManager
    ) {
        this.statusCount = statusCount;
        this.statusAverage = statusAverage;
        this.statusSum = statusSum;
        this.statisticsManager = statisticsManager;
        this.dataStore = dataStore;
        this.selectionManager = selectionManager;
    }
    update(): void {
        const stats = this.statisticsManager.calculateSelection(this.dataStore, this.selectionManager);
        this.statusCount.textContent = `Count: ${stats.count}`;
        this.statusAverage.textContent = `Average: ${stats.average.toFixed(2)}`;
        this.statusSum.textContent = `Sum: ${stats.sum}`;
    }
}