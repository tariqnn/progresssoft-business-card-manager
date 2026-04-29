import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { BusinessCardFilters, EMPTY_FILTERS } from '../../core/models/business-card.model';

@Component({
  selector: 'app-card-filters',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './card-filters.component.html',
  styleUrl: './card-filters.component.css'
})
export class CardFiltersComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);

  readonly filtersChanged = output<BusinessCardFilters>();

  readonly form = this.fb.nonNullable.group({ ...EMPTY_FILTERS });

  ngOnInit(): void {
    this.form.valueChanges
      .pipe(
        debounceTime(280),
        distinctUntilChanged((a, b) => JSON.stringify(a) === JSON.stringify(b)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => this.emit());
  }

  clear(): void {
    this.form.reset({ ...EMPTY_FILTERS });
  }

  hasActiveFilters(): boolean {
    return Object.values(this.form.getRawValue()).some(v => v.trim() !== '');
  }

  private emit(): void {
    this.filtersChanged.emit(this.form.getRawValue());
  }
}
