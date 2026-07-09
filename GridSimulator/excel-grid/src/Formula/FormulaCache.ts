export class FormulaCache {
    private values = new Map<string, string | number>();
    get(key: string): string | number | undefined {
        return this.values.get(key);
    }
    set(key: string, value: string | number): void {
        this.values.set(key, value);
    }
    clear(): void {
        this.values.clear();
    }
    delete(key: string): void {
        this.values.delete(key);
    }
}