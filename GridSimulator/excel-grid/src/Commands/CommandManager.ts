import type { Command } from "./Command";

export class CommandManager {
    private undoStack: Command[] = [];
    private redoStack: Command[] = [];

    execute(command: Command): void {
        command.execute();
        this.undoStack.push(command);
        this.redoStack = [];
    }
    undo(): Command | undefined {
        const command = this.undoStack.pop();
        if (!command) {
            return;
        }
        command.undo();
        this.redoStack.push(command);
        return command;
    }
    redo(): Command | undefined {
        const command = this.redoStack.pop();
        if (!command) {
            return;
        }
        command.execute();
        this.undoStack.push(command);
        return command;
    }
}