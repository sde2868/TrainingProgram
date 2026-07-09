import { SelectionManager } from "../Selection/SelectionManager";
import { DataStore } from "../Data/DataStore";
import { CommandManager } from "../Commands/CommandManager";
import { ClipboardManager } from "../Clipboard/ClipboardManager";
import { RowModel } from "../Models/Row";
import { ColumnModel } from "../Models/Column";
import { EditCellCommand } from "../Commands/EditCellCommand";

type Callbacks = {
    autoFitRow: (row: number) => void;
    rebuildOffsets: () => void;
    requestRender: () => void;
    updateStatistics: () => void;
    moveActiveCell: (row: number, column: number, extendSelection: boolean) => void;
    beginEdit: (row: number, column: number, initialValue?: string) => void;
    cancelEdit: () => void;
    isEditorOpen: () => boolean;
    selectAll: () => void;
};

export class ShortcutHandler {
    private selectionManager: SelectionManager;
    private dataStore: DataStore;
    private commandManager: CommandManager;
    private clipboardManager: ClipboardManager;
    private rowModel: RowModel;
    private columnModel: ColumnModel;
    private callbacks: Callbacks;
    constructor(
        selectionManager: SelectionManager,
        dataStore: DataStore,
        commandManager: CommandManager,
        clipboardManager: ClipboardManager,
        rowModel: RowModel,
        columnModel: ColumnModel,
        callbacks: Callbacks
    ) {
        this.selectionManager = selectionManager;
        this.dataStore = dataStore;
        this.commandManager = commandManager;
        this.clipboardManager = clipboardManager;
        this.rowModel = rowModel;
        this.columnModel = columnModel;
        this.callbacks = callbacks;
    }
    handleKeyDown(event: KeyboardEvent, activeRow: number, activeColumn: number): boolean {
        const isModifierKey = event.ctrlKey || event.metaKey;
        if (this.callbacks.isEditorOpen()) {
            if (event.key === "Escape") {
                event.preventDefault();
                this.callbacks.cancelEdit();
                this.callbacks.requestRender();
                return true;
            }
            return true;
        }
        if (event.key === "ArrowDown") {
            event.preventDefault();
            this.callbacks.moveActiveCell(Math.max(0, activeRow + 1), activeColumn, event.shiftKey);
            return true;
        }
        if (event.key === "ArrowUp") {
            event.preventDefault();
            this.callbacks.moveActiveCell(Math.max(0, activeRow - 1), activeColumn, event.shiftKey);
            return true;
        }
        if (event.key === "ArrowRight") {
            event.preventDefault();
            this.callbacks.moveActiveCell(activeRow, activeColumn + 1, event.shiftKey);
            return true;
        }
        if (event.key === "ArrowLeft") {
            event.preventDefault();
            this.callbacks.moveActiveCell(activeRow, Math.max(0, activeColumn - 1), event.shiftKey);
            return true;
        }
        if (event.key === "Tab") {
            event.preventDefault();
            const direction = event.shiftKey ? -1 : 1;
            this.callbacks.moveActiveCell(activeRow, activeColumn + direction, false);
            return true;
        }
        if (event.key === "Enter") {
            event.preventDefault();
            const direction = event.shiftKey ? -1 : 1;
            this.callbacks.moveActiveCell(activeRow + direction, activeColumn, false);
            return true;
        }
        if (event.key.length === 1 && !isModifierKey && !event.altKey) {
            event.preventDefault();
            this.callbacks.beginEdit(activeRow, activeColumn, event.key);
            return true;
        }
        if (isModifierKey && event.key.toLowerCase() === "z" && !event.shiftKey) {
            event.preventDefault();
            const command = this.commandManager.undo();
            if (command instanceof EditCellCommand) {
                this.callbacks.autoFitRow(command.getRow());
            }
            this.callbacks.rebuildOffsets();
            this.callbacks.requestRender();
            return true;
        }
        if ((isModifierKey && event.key.toLowerCase() === "y") || (isModifierKey && event.shiftKey && event.key.toLowerCase() === "z")) {
            event.preventDefault();
            const command = this.commandManager.redo();
            if (command instanceof EditCellCommand) {
                this.callbacks.autoFitRow(command.getRow());
            }
            this.callbacks.rebuildOffsets();
            this.callbacks.requestRender();
            return true;
        }
        if (isModifierKey && event.key.toLowerCase() === "c") {
            event.preventDefault();
            const bounds = this.selectionManager.getBounds(this.rowModel.getTotalRows(), this.columnModel.getTotalColumns());
            this.clipboardManager.copy(bounds, this.dataStore);
            return true;
        }
        if (isModifierKey && event.key.toLowerCase() === "v") {
            event.preventDefault();
            const pasteResult = this.clipboardManager.paste(activeRow, activeColumn, this.dataStore);
            if (!pasteResult) return true;
            this.commandManager.execute(pasteResult.command);
            for (const row of pasteResult.affectedRows) {
                this.callbacks.autoFitRow(row);
            }
            this.callbacks.rebuildOffsets();
            this.callbacks.requestRender();
            return true;
        }
        if (event.key === "Delete" || event.key === "Backspace") {
            event.preventDefault();
            const oldValue = this.dataStore.getCell(activeRow, activeColumn);
            const command = new EditCellCommand(this.dataStore, activeRow, activeColumn, oldValue, "");
            this.commandManager.execute(command);
            this.callbacks.autoFitRow(activeRow);
            this.callbacks.rebuildOffsets();
            this.callbacks.requestRender();
            return true;
        }
        if (isModifierKey && event.key.toLowerCase() === "a") {
            event.preventDefault();
            this.callbacks.selectAll();
            return true;
        }
        return false;
    }
}