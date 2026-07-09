export class ScrollManager {
    private scrollX = 0;
    private scrollY = 0;
    private autoScrollX = 0;
    private autoScrollY = 0;
    private autoScrollFrame = 0;
    private readonly AUTO_SCROLL_MARGIN = 30;
    private readonly AUTO_SCROLL_SPEED = 20;
    private tickCallback: (() => void) | null = null;
    getX(): number {
        return this.scrollX;
    }
    getY(): number {
        return this.scrollY;
    }
    setPosition(x: number, y: number): void {
        this.scrollX = x;
        this.scrollY = y;
    }
    scrollBy(deltaX: number, deltaY: number): void {
        this.scrollX += deltaX;
        this.scrollY += deltaY;
    }
    clamp(maxScrollX: number, maxScrollY: number): void {
        this.scrollX = Math.max(0, Math.min(this.scrollX, maxScrollX));
        this.scrollY = Math.max(0, Math.min(this.scrollY, maxScrollY));
    }
    updateAutoScroll(mouseX: number, mouseY: number, width: number, height: number, rowHeaderWidth: number, headerRowHeight: number): void {
        this.autoScrollX = 0;
        this.autoScrollY = 0;
        if (mouseX > width - this.AUTO_SCROLL_MARGIN) {
            this.autoScrollX = this.AUTO_SCROLL_SPEED;
        } else if (mouseX < rowHeaderWidth + this.AUTO_SCROLL_MARGIN) {
            this.autoScrollX = -this.AUTO_SCROLL_SPEED;
        }
        if (mouseY > height - this.AUTO_SCROLL_MARGIN) {
            this.autoScrollY = this.AUTO_SCROLL_SPEED;
        } else if (mouseY < headerRowHeight + this.AUTO_SCROLL_MARGIN) {
            this.autoScrollY = -this.AUTO_SCROLL_SPEED;
        }
    }
    hasAutoScroll(): boolean {
        return this.autoScrollX !== 0 || this.autoScrollY !== 0;
    }
    startAutoScroll(callback: () => void): void {
        this.tickCallback = callback;
        if (this.autoScrollFrame) {
            return;
        }
        const tick = () => {
            if (!this.tickCallback) {
                this.stopAutoScroll();
                return;
            }
            if (this.autoScrollX !== 0 || this.autoScrollY !== 0) {
                this.scrollX += this.autoScrollX;
                this.scrollY += this.autoScrollY;
                this.tickCallback();
            }
            this.autoScrollFrame = requestAnimationFrame(tick);
        };
        this.autoScrollFrame = requestAnimationFrame(tick);
    }
    stopAutoScroll(): void {
        if (!this.autoScrollFrame) {
            return;
        }
        cancelAnimationFrame(this.autoScrollFrame);
        this.autoScrollFrame = 0;
    }
    ensureCellVisible(
        cellLeft: number,
        cellTop: number,
        cellRight: number,
        cellBottom: number,
        viewportWidth: number,
        viewportHeight: number,
        rowHeaderWidth: number,
        headerRowHeight: number
    ): void {
        const viewportLeft = this.scrollX + rowHeaderWidth;
        const viewportRight = this.scrollX + viewportWidth;
        const viewportTop = this.scrollY + headerRowHeight;
        const viewportBottom = this.scrollY + viewportHeight;
        if (cellLeft < viewportLeft) {
            this.scrollX = cellLeft - rowHeaderWidth;
        } else if (cellRight > viewportRight) {
            this.scrollX = cellRight - viewportWidth;
        }
        if (cellTop < viewportTop) {
            this.scrollY = cellTop - headerRowHeight;
        } else if (cellBottom > viewportBottom) {
            this.scrollY = cellBottom - viewportHeight;
        }
    }
}