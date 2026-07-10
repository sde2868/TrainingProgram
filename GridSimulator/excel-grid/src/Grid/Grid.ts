import { Renderer } from "./Renderer";
import { ViewportRenderer } from "./ViewportRenderer";
import { DataStore } from "../Data/DataStore";
import { SelectionManager } from "../Selection/SelectionManager";
import { StatisticsManager } from "../Stats/StatisticsManager";
import { CommandManager } from "../Commands/CommandManager";
import { FormulaEngine } from "../Formula/FormulaEngine";
import { RowModel } from "../Models/Row";
import { ColumnModel } from "../Models/Column";
import { ScrollManager } from "../Scroll/ScrollManager";
import { ClipboardManager } from "../Clipboard/ClipboardManager";
import { EditorManager } from "../Editor/EditorManager";
import { ResizeManager } from "./ResizeManager";
import { ShortcutHandler } from "./ShortcutHandler";
import { InputManager } from "./InputManager";
import { StatusBarManager } from "../Stats/StatusBarManager";
import { RowSizingManager } from "./RowSizingManager";

export class Grid {
    private canvas: HTMLCanvasElement;
    private renderer: Renderer;
    private viewportRenderer: ViewportRenderer;
    private dataStore: DataStore;
    private selectionManager: SelectionManager;
    private commandManager: CommandManager;
    private rowModel = new RowModel();
    private columnModel = new ColumnModel(this.rowModel.getRowHeaderWidth());
    private scrollManager = new ScrollManager();
    private clipboardManager = new ClipboardManager();
    private editorManager!: EditorManager;
    private resizeManager!: ResizeManager;
    private shortcutHandler!: ShortcutHandler;
    private inputManager!: InputManager;
    private statusBarManager!: StatusBarManager;
    private rowSizingManager!: RowSizingManager;

    private renderScheduled = false;

    private activeRow = 0;
    private activeColumn = 0;

    private formulaEngine: FormulaEngine;

    constructor(
        canvas: HTMLCanvasElement,
        ctx: CanvasRenderingContext2D,
        dataStore: DataStore,
        selectionManager: SelectionManager,
        statisticsManager: StatisticsManager,
        commandManager: CommandManager,
        statusCount: HTMLSpanElement,
        statusAverage: HTMLSpanElement,
        statusSum: HTMLSpanElement,
        statusMin: HTMLSpanElement,
        statusMax: HTMLSpanElement,
        formulaEngine: FormulaEngine
    ) {
        this.canvas = canvas;
        this.dataStore = dataStore;
        this.selectionManager = selectionManager;
        this.commandManager = commandManager;
        this.formulaEngine = formulaEngine;
        this.renderer = new Renderer(ctx);
        this.viewportRenderer = new ViewportRenderer(this.canvas, this.renderer, this.rowModel, this.columnModel, this.scrollManager, this.selectionManager, this.dataStore, this.formulaEngine);
        this.editorManager = new EditorManager(this.canvas, this.dataStore, this.commandManager, this.rowModel, this.columnModel, this.scrollManager,
            (editedRow) => {
                this.autoFitRowHeight(editedRow);
                this.rowModel.rebuildOffsets();
                this.requestRender();
            }
        );
        this.resizeManager = new ResizeManager(this.rowModel, this.columnModel);
        this.statusBarManager = new StatusBarManager(statusCount, statusAverage, statusSum, statusMin, statusMax, statisticsManager, dataStore, selectionManager, this.rowModel, this.columnModel);
        this.rowSizingManager = new RowSizingManager(this.rowModel, this.dataStore, this.columnModel, ctx);
        this.inputManager = new InputManager(this.canvas, this.rowModel, this.columnModel, this.scrollManager, this.resizeManager,
            {
                saveEditor: () => {
                    if (this.editorManager.isOpen()) {
                        this.editorManager.saveEdit();
                    }
                },
                beginEdit: (row: number, column: number) => this.beginEdit(row, column),
                selectAll: () => this.selectAll(),
                selectRow: (row: number) => {
                    this.selectionManager.selectRow(row);
                    this.updateStatistics();
                    this.requestRender();
                },
                selectColumn: (column: number) => {
                    this.selectionManager.selectColumn(column);
                    this.updateStatistics();
                    this.requestRender();
                },
                startSelection: (row: number, column: number) => {
                    this.selectionManager.startSelection(row, column);
                    this.activeRow = row;
                    this.activeColumn = column;
                },
                updateSelection: (row: number, column: number) => this.selectionManager.updateSelection(row, column),
                updateStatistics: () => this.updateStatistics(),
                requestRender: () => this.requestRender(),
                setActiveCell: (row: number, column: number) => {
                    this.activeRow = row;
                    this.activeColumn = column;
                },
                finishColumnResize: () => {
                    const command = this.resizeManager.finishColumnResize();
                    if (command) {
                        this.commandManager.execute(command);
                    }
                    this.columnModel.rebuildOffsets(this.rowModel.getRowHeaderWidth());
                    this.rowSizingManager.reAutoFitRows();
                    this.rowModel.rebuildOffsets();
                    this.updateStatistics();
                    this.requestRender();
                },
                finishRowResize: () => {
                    const command = this.resizeManager.finishRowResize();
                    if (command) {
                        this.commandManager.execute(command);
                    }
                    this.rowSizingManager.clearAutoFitRow(this.resizeManager.getResizingRow());
                    this.rowModel.rebuildOffsets();
                    this.requestRender();
                },
                finishSelection: () => {
                    this.updateStatistics();
                    this.requestRender();
                },
                clampScroll: () => this.clampScroll(),
                onAutoScrollSelection: (row: number, column: number) => {
                    this.selectionManager.updateSelection(row, column);
                    this.updateStatistics();
                    this.clampScroll();
                    this.requestRender();
                }
            }
        );
        this.shortcutHandler = new ShortcutHandler(this.selectionManager, this.dataStore, this.commandManager, this.clipboardManager, this.rowModel, this.columnModel,
            {
                autoFitRow: (r: number) => this.autoFitRowHeight(r),
                rebuildOffsets: () => { this.columnModel.rebuildOffsets(this.rowModel.getRowHeaderWidth()); this.rowModel.rebuildOffsets(); },
                requestRender: () => this.requestRender(),
                updateStatistics: () => this.updateStatistics(),
                moveActiveCell: (row: number, column: number, extendSelection: boolean) => {
                    this.activeRow = Math.max(0, Math.min(row, this.rowModel.getTotalRows() - 1));
                    this.activeColumn = Math.max(0, Math.min(column, this.columnModel.getTotalColumns() - 1));
                    if (extendSelection) {
                        this.extendSelectionToActiveCell();
                    } else {
                        this.moveActiveCell();
                    }
                },
                beginEdit: (row: number, column: number, initialValue?: string) => this.beginEdit(row, column, initialValue),
                cancelEdit: () => {
                    this.editorManager.cancelEdit();
                    this.requestRender();
                },
                isEditorOpen: () => this.editorManager.isOpen(),
                selectAll: () => this.selectAll()
            }
        );
        this.canvas.addEventListener("wheel", this.handleWheel.bind(this), { passive: false });
        this.canvas.addEventListener("mousedown", this.handleMouseDown.bind(this));
        this.canvas.addEventListener("mousemove", this.handleMouseMove.bind(this));
        this.canvas.addEventListener("mouseup", this.handleMouseUp.bind(this));
        this.canvas.addEventListener("dblclick", this.handleDoubleClick.bind(this));
        window.addEventListener("keydown", this.handleKeyDown.bind(this));
        this.updateStatistics();
    }
    public render(): void {
        this.viewportRenderer.render(this.activeRow, this.activeColumn);
    }
    private requestRender(): void {
        if (this.renderScheduled) {
            return;
        }
        this.renderScheduled = true;
        requestAnimationFrame(() => {
            this.renderScheduled = false;
            this.render();
        });
    }
    private updateStatistics(): void {
        this.statusBarManager.update();
    }
    private clampScroll(): void {
        const maxScrollX = Math.max(0, this.columnModel.getTotalWidth() - this.canvas.width);
        const maxScrollY = Math.max(0, this.rowModel.getTotalHeight() - this.canvas.height);
        this.scrollManager.clamp(maxScrollX, maxScrollY);
    }
    private handleWheel(event: WheelEvent): void {
        event.preventDefault();
        this.scrollManager.scrollBy(event.deltaX, event.deltaY);
        this.clampScroll();
        this.requestRender();
    }
    private handleMouseDown(event: MouseEvent): void {
        this.inputManager.handleMouseDown(event);
    }
    private handleMouseMove(event: MouseEvent): void {
        this.inputManager.handleMouseMove(event);
    }
    private handleMouseUp(): void {
        this.inputManager.handleMouseUp();
    }
    private handleDoubleClick(event: MouseEvent): void {
        this.inputManager.handleDoubleClick(event);
    }
    private autoFitRowHeight(row: number): void {
        this.rowSizingManager.autoFitRowHeight(row);
    }
    private ensureActiveCellVisible(): void {
        const cellLeft = this.columnModel.getOffset(this.activeColumn);
        const cellRight = cellLeft + this.columnModel.getWidth(this.activeColumn);
        const cellTop = this.rowModel.getOffset(this.activeRow);
        const cellBottom = cellTop + this.rowModel.getHeight(this.activeRow);
        this.scrollManager.ensureCellVisible(cellLeft, cellTop, cellRight, cellBottom, this.canvas.width, this.canvas.height, this.rowModel.getRowHeaderWidth(), this.rowModel.getHeaderRowHeight());
        this.clampScroll();
    }
    private selectAll(): void {
        this.activeRow = 0;
        this.activeColumn = 0;
        this.selectionManager.selectAll();
        this.updateStatistics();
        this.requestRender();
    }
    private moveActiveCell(): void {
        this.selectionManager.startSelection(this.activeRow, this.activeColumn);
        this.ensureActiveCellVisible();
        this.updateStatistics();
        this.requestRender();
    }
    private extendSelectionToActiveCell(): void {
        this.selectionManager.updateSelection(this.activeRow, this.activeColumn);
        this.ensureActiveCellVisible();
        this.updateStatistics();
        this.requestRender();
    }
    private handleKeyDown(event: KeyboardEvent): void {
        this.shortcutHandler.handleKeyDown(event, this.activeRow, this.activeColumn);
    }
    private beginEdit(row: number, column: number, initialValue?: string): void {
        this.editorManager.beginEdit(row, column, initialValue);
    }
}