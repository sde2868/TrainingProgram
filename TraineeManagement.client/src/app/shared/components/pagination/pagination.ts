import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  imports: [],
  templateUrl: './pagination.html',
  styleUrl: './pagination.css'
})
export class Pagination {
  @Input() pageNumber = 1;
  @Input() pageSize = 10;
  @Input() totalRecords = 0;

  @Output() pageChanged = new EventEmitter<number>();

  get totalPages(): number {
    return Math.ceil(this.totalRecords / this.pageSize) || 1;
  }

  previous(): void {
    if (this.pageNumber <= 1) {
      return;
    }
    this.pageChanged.emit(this.pageNumber - 1);
  }

  next(): void {
    if (this.pageNumber >= this.totalPages) {
      return;
    }
    this.pageChanged.emit(this.pageNumber + 1);
  }
}