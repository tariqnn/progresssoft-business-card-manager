import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { BusinessCard, ExportFormat } from '../../core/models/business-card.model';

@Component({
  selector: 'app-card-list',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './card-list.component.html',
  styleUrl: './card-list.component.css'
})
export class CardListComponent {
  readonly cards = input.required<BusinessCard[]>();
  readonly isLoading = input(false);
  readonly isExporting = input(false);

  readonly cardDeleted = output<BusinessCard>();
  readonly cardExported = output<{ card: BusinessCard; format: ExportFormat }>();

  initials(name: string): string {
    const parts = name.trim().split(/\s+/);
    if (parts.length >= 2) return (parts[0][0] + parts[1][0]).toUpperCase();
    return name.slice(0, 2).toUpperCase();
  }

  confirmDelete(card: BusinessCard): void {
    if (window.confirm(`Delete ${card.name}? This cannot be undone.`)) {
      this.cardDeleted.emit(card);
    }
  }

  exportCard(card: BusinessCard, format: ExportFormat): void {
    this.cardExported.emit({ card, format });
  }

  trackById(_index: number, card: BusinessCard): number {
    return card.id;
  }
}
