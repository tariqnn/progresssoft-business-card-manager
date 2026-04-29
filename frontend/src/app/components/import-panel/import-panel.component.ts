import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BusinessCardDraft, ImportMode, ImportPreview } from '../../core/models/business-card.model';
import { FileParserService } from '../../core/services/file-parser.service';
import { NotificationService } from '../../core/services/notification.service';
import { DragDropDirective } from '../../shared/directives/drag-drop.directive';

@Component({
  selector: 'app-import-panel',
  standalone: true,
  imports: [CommonModule, FormsModule, DragDropDirective],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './import-panel.component.html',
  styleUrl: './import-panel.component.css'
})
export class ImportPanelComponent {
  private readonly parser = inject(FileParserService);
  private readonly notifications = inject(NotificationService);

  readonly isImporting = input(false);
  readonly importConfirmed = output<ImportPreview>();

  readonly mode = signal<ImportMode>('csv');
  readonly preview = signal<ImportPreview | null>(null);

  setMode(mode: ImportMode): void {
    this.mode.set(mode);
    this.preview.set(null);
  }

  acceptAttribute(): string {
    switch (this.mode()) {
      case 'csv': return '.csv,text/csv';
      case 'xml': return '.xml,application/xml,text/xml';
      case 'qr': return 'image/*';
    }
  }

  async onFileSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (file) await this.handleFile(file);
  }

  async onFileDropped(file: File): Promise<void> {
    await this.handleFile(file);
  }

  confirmImport(): void {
    const current = this.preview();
    if (current) this.importConfirmed.emit(current);
  }

  cancelPreview(): void {
    this.preview.set(null);
  }

  trackByIndex(index: number, _item: BusinessCardDraft): number {
    return index;
  }

  private async handleFile(file: File): Promise<void> {
    try {
      const cards = await this.parser.parse(file, this.mode());
      this.preview.set({ mode: this.mode(), file, cards });
      this.notifications.info(`${cards.length} card${cards.length === 1 ? '' : 's'} previewed from ${file.name}.`);
    } catch (error) {
      this.preview.set(null);
      this.notifications.error(error instanceof Error ? error.message : 'Unable to preview import file.');
    }
  }

  clearAfterImport(): void {
    this.preview.set(null);
  }
}
