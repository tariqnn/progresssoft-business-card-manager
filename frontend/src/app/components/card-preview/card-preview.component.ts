import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { BusinessCardDraft } from '../../core/models/business-card.model';

@Component({
  selector: 'app-card-preview',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './card-preview.component.html',
  styleUrl: './card-preview.component.css'
})
export class CardPreviewComponent {
  readonly card = input.required<BusinessCardDraft>();

  initials(): string {
    const name = this.card().name.trim() || 'BC';
    const parts = name.split(/\s+/);
    if (parts.length >= 2) {
      return (parts[0][0] + parts[1][0]).toUpperCase();
    }
    return name.slice(0, 2).toUpperCase();
  }
}
