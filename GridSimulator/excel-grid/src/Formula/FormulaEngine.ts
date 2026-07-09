import { DataStore } from "../Data/DataStore";

export class FormulaEngine {
    // private parseCellReference(reference: string): { row: number, column: number } | null {
    //     const match = reference.match(/^([A-Z]+)(\d+)$/);
    //     if (!match) {
    //         return null;
    //     }
    //     const [, letters, numbers] = match;
    //     let column = 0;
    //     for (const letter of letters) {
    //         column = column * 26 + (letter.charCodeAt(0) - 64);
    //     }
    //     return {
    //         row: Number(numbers) - 1,
    //         column: column - 1
    //     }
    // }

    // private getCellValue(reference: string, dataStore: DataStore): number | string {
    //     const cell = this.parseCellReference(reference);
    //     if (!cell) {
    //         return "#REF!";
    //     }
    //     return dataStore.getCell(cell.row, cell.column) ?? "";
    // }

    // private toNumber(value: string | number): number | null {
    //     if (value === "") {
    //         return 0;
    //     }
    //     const number = Number(value);
    //     if (Number.isNaN(number)) {
    //         return null;
    //     }
    //     return number;
    // }

    private columnLettersToIndex(letters: string): number {
        let column = 0;
        for (const letter of letters) {
            column = column * 26 + (letter.charCodeAt(0) - 64);
        }
        return column - 1;
    }

    private columnIndexToLetters(index: number): string {
        let result = "";
        let column = index + 1;
        while (column > 0) {
            const remainder = (column - 1) % 26;
            result = String.fromCharCode(65 + remainder) + result;
            column = Math.floor((column - 1) / 26);
        }
        return result;
    }

    private parseRange(range: string, dataStore: DataStore, visitedCells: Set<string>): (string | number)[] {
        const parts = range.split(":");
        if (parts.length !== 2) {
            return [];
        }
        const startMatch = parts[0].trim().match(/^([A-Z]+)([0-9]+)$/i);
        const endMatch = parts[1].trim().match(/^([A-Z]+)([0-9]+)$/i);
        if (!startMatch || !endMatch) {
            return [];
        }
        const startColumn = this.columnLettersToIndex(startMatch[1]);
        const endColumn = this.columnLettersToIndex(endMatch[1]);
        const minColumn = Math.min(startColumn, endColumn);
        const maxColumn = Math.max(startColumn, endColumn);

        const startRow = Number(startMatch[2]) - 1;
        const endRow = Number(endMatch[2]) - 1;
        const minRow = Math.min(startRow, endRow);
        const maxRow = Math.max(startRow, endRow);

        const values: (string | number)[] = [];
        for(let row = minRow; row <= maxRow; row++) {
            for(let column = minColumn; column <= maxColumn; column++) {
                const reference = this.columnIndexToLetters(column) + String(row + 1);
                const value = this.getCellValueFromReference(reference, dataStore, visitedCells);
                values.push(value);
            }
        }
        return values;
    }

    evaluateCount(argument: string, dataStore: DataStore, visitedCells: Set<string>): string | number {
        const values = this.parseRange(argument, dataStore, visitedCells);
        let count = 0;
        for (const value of values) {
            if (value === "#CIRC!" || value === "#REF!") {
                return value;
            }
            if (value === "") {
                continue;
            }
            const number = Number(value);
            if (!isNaN(number)) {
                count++;
            }
        }
        return count;
    }

    evaluateSum(argument: string, dataStore: DataStore, visitedCells: Set<string>): string | number {
        const values = this.parseRange(argument, dataStore, visitedCells);
        let sum = 0;
        for (const value of values) {
            if (value === "#CIRC!" || value === "#REF!") {
                return value;
            }
            if (value === "") {
                continue;
            }
            const number = Number(value);
            if (!isNaN(number)) {
                sum += number;
            }
        }
        return sum;
    }

    evaluateAverage(argument: string, dataStore: DataStore, visitedCells: Set<string>): string | number {
        const values = this.parseRange(argument, dataStore, visitedCells);
        let count = 0, sum = 0;
        for (const value of values) {
            if (value === "#CIRC!" || value === "#REF!") {
                return value;
            }
            if (value === "") {
                continue;
            }
            const number = Number(value);
            if (!isNaN(number)) {
                count++;
                sum += number;
            }
        }
        if (count === 0) {
            return "#DIV/0!";
        }
        return (sum / count).toFixed(3);
    }

    evaluateMin(argument: string, dataStore: DataStore, visitedCells: Set<string>): string | number {
        const values = this.parseRange(argument, dataStore, visitedCells);
        let min = Infinity;
        for (const value of values) {
            if (value === "#CIRC!" || value === "#REF!") {
                return value;
            }
            if (value === "") {
                continue;
            }
            const number = Number(value);
            if (!isNaN(number)) {
                min = Math.min(min, number);
            }
        }
        return min === Infinity ? 0 : min;
    }

    evaluateMax(argument: string, dataStore: DataStore, visitedCells: Set<string>): string | number {
        const values = this.parseRange(argument, dataStore, visitedCells);
        let max = -Infinity;
        for (const value of values) {
            if (value === "#CIRC!" || value === "#REF!") {
                return value;
            }
            if (value === "") {
                continue;
            }
            const number = Number(value);
            if (!isNaN(number)) {
                max = Math.max(max, number);
            }
        }
        return max === -Infinity ? 0 : max;
    }
    // private getRangeValues(range: string, dataStore: DataStore): (string | number)[] {
    //     const bounds = this.parseRange(range);
    //     if (!bounds) {
    //         return [];
    //     }
    //     const values: (string | number)[] = [];
    //     for (let row = bounds.startRow; row <= bounds.endRow; row++) {
    //         for (let column = bounds.startColumn; column <= bounds.endColumn; column++) {
    //             values.push(dataStore.getCell(row, column) ?? "");
    //         }
    //     }
    //     return values;
    // }

    private getCellValueFromReference(reference: string, dataStore: DataStore, visitedCells: Set<string>): string | number {
        const match = reference.trim().match(/^([A-Z]+)([0-9]+)$/i);
        if (!match) {
            return "#REF!";
        }
        const columnLetters = match[1].toUpperCase();
        const rowNumber = Number(match[2]);
        let column = 0;
        for (const letter of columnLetters) {
            column = column * 26 + letter.charCodeAt(0) - 64;
        }
        column--;
        const row = rowNumber - 1;
        if (row < 0 || row >= dataStore.getRowCount() || column < 0 || column >= dataStore.getColumnCount()) {
            return "#REF!";
        }
        const cellKey = `${columnLetters}${rowNumber}`;
        if (visitedCells.has(cellKey)) {
            return "#CIRC!";
        }
        const rawValue = dataStore.getCell(row, column);
        if (rawValue === null || rawValue === undefined) {
            return "";
        }
        const value = String(rawValue);
        if (!value.startsWith("=")) {
            return rawValue as string | number;
        }
        const nextVisitedCells = new Set(visitedCells);
        nextVisitedCells.add(cellKey);
        return this.evaluate(value, dataStore, nextVisitedCells);
    }

    private evaluateReference(expression: string, dataStore: DataStore, visitedCells: Set<string>): string | number | null {
        if (!/^[A-Z]+[0-9]+$/i.test(expression)) {
            return null;
        }
        return this.getCellValueFromReference(expression, dataStore, visitedCells);
    }

    private evaluateFunction(expression: string, dataStore: DataStore, visitedCells: Set<string>): string | number | null {
        const match = expression.match(/^([A-Z]+)\((.*)\)$/i);
        if (!match) {
            return null;
        }
        const [, functionName, argument] = match;
        switch (functionName.toUpperCase()) {
            case "SUM":
                return this.evaluateSum(argument, dataStore, visitedCells);
            case "AVERAGE":
                return this.evaluateAverage(argument, dataStore, visitedCells);
            case "MIN":
                return this.evaluateMin(argument, dataStore, visitedCells);
            case "MAX":
                return this.evaluateMax(argument, dataStore, visitedCells);
            case "COUNT":
                return this.evaluateCount(argument, dataStore, visitedCells);
            default:
                return "#NAME?";
        }
    }

    evaluateArithmetic(expression: string, dataStore: DataStore, visitedCells: Set<string>): string | number | null {
        const match = expression.match(/^(.+?)([\+\-\*\/])(.+)$/);
        if (!match) {
            return null;
        }
        const [, leftRef, operator, rightRef] = match;
        const left = this.getCellValueFromReference(leftRef.trim(), dataStore, visitedCells);
        const right = this.getCellValueFromReference(rightRef.trim(), dataStore, visitedCells);
        if (left === "#CIRC!" || right === "#CIRC!") {
            return "#CIRC!";
        }
        if (left === "#REF!" || right === "#REF") {
            return "#REF!";
        }
        const leftNumber = Number(left || 0);
        const rightNumber = Number(right || 0);
        switch (operator) {
            case "+":
                return leftNumber + rightNumber;
            case "-":
                return leftNumber - rightNumber;
            case "*":
                return leftNumber * rightNumber;
            case "/":
                return (rightNumber === 0) ? "#DIV/0!" : (leftNumber / rightNumber);
        }
        return "#VALUE!";
    }

    evaluate(formula: string, dataStore: DataStore, visitedCells: Set<string> = new Set()): string | number {
        if (!formula.startsWith("=")) {
            return formula;
        }
        const expression = formula.slice(1).trim();
        const functionResult = this.evaluateFunction(expression, dataStore, visitedCells);
        if (functionResult !== null) {
            return functionResult;
        }
        const arithmeticResult = this.evaluateArithmetic(expression, dataStore, visitedCells);
        if (arithmeticResult !== null) {
            return arithmeticResult;
        }
        const referenceResult = this.evaluateReference(expression, dataStore, visitedCells);
        if (referenceResult !== null) {
            return referenceResult;
        }
        return "#NAME?";
    }
}