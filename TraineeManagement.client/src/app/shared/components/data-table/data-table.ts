import { Component, Input } from '@angular/core';

export interface DataTableColumn {
  key: string;
  label: string;
}

@Component({
  selector: 'app-data-table',
  imports: [],
  templateUrl: './data-table.html',
  styleUrl: './data-table.css'
})
export class DataTable {
  @Input() columns: DataTableColumn[] = [];
  @Input() rows: Record<string, unknown>[] = [];
}