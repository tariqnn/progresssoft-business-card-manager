import { Directive, ElementRef, HostBinding, HostListener, output } from '@angular/core';

@Directive({
  selector: '[appDragDrop]',
  standalone: true
})
export class DragDropDirective {
  readonly fileDropped = output<File>();

  @HostBinding('class.is-dragging') isDragging = false;

  constructor(private readonly el: ElementRef<HTMLElement>) {}

  @HostListener('dragenter', ['$event'])
  @HostListener('dragover', ['$event'])
  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  @HostListener('dragleave', ['$event'])
  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    if (!this.el.nativeElement.contains(event.relatedTarget as Node)) {
      this.isDragging = false;
    }
  }

  @HostListener('drop', ['$event'])
  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    const file = event.dataTransfer?.files?.[0];
    if (file) {
      this.fileDropped.emit(file);
    }
  }
}
