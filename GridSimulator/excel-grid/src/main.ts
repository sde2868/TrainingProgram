import "./style.css";
import { Grid } from "./Grid/Grid";
import { DataStore } from "./Data/DataStore";
import { SelectionManager } from "./Selection/SelectionManager";
import { StatisticsManager } from "./Stats/StatisticsManager";
import { CommandManager } from "./Commands/CommandManager";

const canvas = document.getElementById("gridCanvas") as HTMLCanvasElement;
const ctx = canvas.getContext("2d");
if (!ctx) {
  throw new Error("Canvas context not found");
}
canvas.width = window.innerWidth;
canvas.height = window.innerHeight;

function generateData(
    count: number
): Record<string, any>[] {
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
// dataStore.setCellValue(
//     0,
//     "firstName",
//     "Harsh"
// );

const grid = new Grid(canvas, ctx, dataStore, selectionManager, statisticsManager, commandManager);

grid.render();

// canvas.width = window.innerWidth;
// canvas.height = window.innerHeight;

// for (let row = 0; row < 10; row++) {
//   for (let col = 0; col < 5; col++) {
//     ctx.strokeRect(
//       col * 100,
//       row * 30,
//       100,
//       30
//     );
//   }
// }
// ctx.strokeRect(50, 50, 100, 30);
// ctx.fillText("Hello", 60, 70);