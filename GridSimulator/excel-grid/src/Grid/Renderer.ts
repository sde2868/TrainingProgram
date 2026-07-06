export class Renderer {
    private ctx: CanvasRenderingContext2D;
    constructor(ctx: CanvasRenderingContext2D) {
        this.ctx = ctx;
        this.ctx.font = "13px Arial";
        this.ctx.strokeStyle = "#d1d5db";
        this.ctx.lineWidth = 1;
    }
    drawHeader(x: number, y: number, width: number, height: number, value: string, isSelected: boolean): void {
        this.ctx.save();
        this.ctx.fillStyle = isSelected ? "#dbeafe" : "#f3f4f6";
        this.ctx.fillRect(x, y, width, height);
        this.ctx.strokeStyle = "#d1d5db";
        this.ctx.strokeRect(x, y, width, height);
        this.ctx.fillStyle = "#374151";
        this.ctx.textAlign = "center";
        this.ctx.textBaseline = "middle";
        this.ctx.fillText(value, x + width / 2, y + height / 2);
        this.ctx.restore();
    }
    drawCell(x: number, y: number, width: number, height: number, value: string, isSelected: boolean): number {
        this.ctx.strokeRect(x, y, width, height);
        // this.ctx.fillText(value, x + 5, y + 20);
        this.ctx.save();
        if (isSelected) {
            this.ctx.fillStyle = "#dbeafe";
            this.ctx.fillRect(x, y, width, height);
        }
        // this.ctx.strokeRect(x, y, width, height);
        this.ctx.beginPath();
        this.ctx.rect(x + 1, y + 1, width - 2, height - 2);
        this.ctx.clip();

        this.ctx.fillStyle = "#000000";
        // this.ctx.fillText(value, x + 5, y + 20);
        const lines: string[] = [];
        for (const rawLine of value.split("\n")) {
            let currentLine = "";
            for (const word of rawLine.split(" ")) {
                const testLine = currentLine ? `${currentLine} ${word}` : word;
                const testWidth = this.ctx.measureText(testLine).width;
                if (testWidth > width - 10 && currentLine) {
                    lines.push(currentLine);
                    currentLine = word;
                }
                else {
                    currentLine = testLine;
                }
            }
            lines.push(currentLine);
        }
        const lineHeight = 16;
        const requiredHeight = (lines.length * lineHeight) + 8;
        // return requiredHeight;
        if (requiredHeight > height) {
            this.ctx.fillStyle = "#ff0000";
            this.ctx.fillRect(x + width - 5, y, 5, height);
            this.ctx.fillStyle = "#000000";
        }
        for (let i = 0; i < lines.length; i++) {
            this.ctx.fillText(lines[i], x + 5, y + 18 + (i * lineHeight));
        }
        this.ctx.restore();
        return requiredHeight;
    }
    drawActiveCell(x: number, y: number, width: number, height: number): void {
        this.ctx.save();
        this.ctx.strokeStyle = "#2563eb";
        this.ctx.lineWidth = 2;
        this.ctx.strokeRect(x + 1, y + 1, width - 2, height - 2);
        this.ctx.restore();
    }
    clear(width: number, height: number): void {
        this.ctx.clearRect(0, 0, width, height);
    }
}