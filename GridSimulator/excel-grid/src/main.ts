import "./style.css";
import { Grid } from "./Grid/Grid";
import { DataStore } from "./Data/DataStore";
import { SelectionManager } from "./Selection/SelectionManager";
import { StatisticsManager } from "./Stats/StatisticsManager";
import { CommandManager } from "./Commands/CommandManager";
import { FormulaEngine } from "./Formula/FormulaEngine";

const canvas = document.getElementById("gridCanvas") as HTMLCanvasElement;
const statusCount = document.getElementById("statusCount") as HTMLSpanElement;
const statusAverage = document.getElementById("statusAverage") as HTMLSpanElement;
const statusSum = document.getElementById("statusSum") as HTMLSpanElement;
const statusMin = document.getElementById("statusMin") as HTMLSpanElement;
const statusMax = document.getElementById("statusMax") as HTMLSpanElement;
const ctx = canvas.getContext("2d");
if (!ctx) {
  throw new Error("Canvas context not found");
}
canvas.width = window.innerWidth;
canvas.height = window.innerHeight - 28;

function generateData(count: number): Record<string, any>[] {
    const data = [];

    for (let i = 1; i <= count; i++) {
        data.push({
            id: i,
            firstName: `User${i}`,
            lastName: `Last${i}`,
            age: 20 + (i % 40),
            salary: 50000 + i * 10
        });
    }

    return data;
}

const dataStore = new DataStore();
dataStore.setData(generateData(50000));
const selectionManager = new SelectionManager();
const statisticsManager = new StatisticsManager();
const commandManager = new CommandManager();
const formulaEngine = new FormulaEngine();
const grid = new Grid(canvas, ctx, dataStore, selectionManager, statisticsManager, commandManager, statusCount, statusAverage, statusSum, statusMin, statusMax, formulaEngine);

grid.render();