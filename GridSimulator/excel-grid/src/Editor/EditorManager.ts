import { DataStore } from "../Data/DataStore";
import { CommandManager } from "../Commands/CommandManager";
import { EditCellCommand } from "../Commands/EditCellCommand";
import { RowModel } from "../Models/Row";
import { ColumnModel } from "../Models/Column";
import { ScrollManager } from "../Scroll/ScrollManager";

export class EditorManager {
    private editor: HTMLTextAreaElement;
    private editingRow = -1;
    private editingColumn = -1;
    private canvas: HTMLCanvasElement;
    private dataStore: DataStore;
    private commandManager: CommandManager;
    private rowModel: RowModel;
    private columnModel: ColumnModel;
    private scrollManager: ScrollManager;
    private onSave: (editedRow: number) => void;

    constructor(
        canvas: HTMLCanvasElement,
        dataStore: DataStore,
        commandManager: CommandManager,
        rowModel: RowModel,
        columnModel: ColumnModel,
        scrollManager: ScrollManager,
        onSave: (editedRow: number) => void
    ) {
        this.canvas = canvas;
        this.dataStore = dataStore;
        this.commandManager = commandManager;
        this.rowModel = rowModel;
        this.columnModel = columnModel;
        this.scrollManager = scrollManager;
        this.onSave = onSave;
        this.editor = document.createElement("textarea");
        this.editor.style.position = "absolute";
        this.editor.style.display = "none";
        document.body.appendChild(this.editor);
        this.editor.addEventListener("keydown", (event) => {
            if (event.key === "Enter" && event.ctrlKey) {
                event.preventDefault();
                const editedRow = this.saveEdit();
                if (editedRow !== null) {
                    this.onSave(editedRow);
                }
            }
        });
    }
    beginEdit(row: number, column: number, initialValue?: string): void {
        const value = initialValue ?? String(this.dataStore.getCell(row, column) ?? "");
        this.editingRow = row;
        this.editingColumn = column;
        this.editor.value = value;
        const rect = this.canvas.getBoundingClientRect();
        const scaleX = rect.width / this.canvas.width;
        const scaleY = rect.height / this.canvas.height;
        const canvasX = this.columnModel.getOffset(column) - this.scrollManager.getX();
        const canvasY = this.rowModel.getOffset(row) - this.scrollManager.getY();
        const x = rect.left + canvasX * scaleX;
        const y = rect.top + canvasY * scaleY;
        const width = this.columnModel.getWidth(column) * scaleX;
        const height = this.rowModel.getHeight(row) * scaleY;
        this.editor.style.left = `${x}px`;
        this.editor.style.top = `${y}px`;
        this.editor.style.width = `${width}px`;
        this.editor.style.height = `${height}px`;
        this.editor.style.display = "block";
        this.editor.focus();
    }
    saveEdit(): number | null {
        if (this.editingRow < 0 || this.editingColumn < 0) {
            return null;
        }
        const oldValue = this.dataStore.getCell(this.editingRow, this.editingColumn);
        const command = new EditCellCommand(
            this.dataStore,
            this.editingRow,
            this.editingColumn,
            oldValue,
            this.editor.value
        );
        this.commandManager.execute(command);
        const editedRow = this.editingRow;
        this.hideEditor();
        return editedRow;
    }
    cancelEdit(): void {
        this.hideEditor();
    }
    isOpen(): boolean {
        return this.editor.style.display === "block";
    }
    private hideEditor(): void {
        this.editor.style.display = "none";
        this.editingRow = -1;
        this.editingColumn = -1;
    }
}
