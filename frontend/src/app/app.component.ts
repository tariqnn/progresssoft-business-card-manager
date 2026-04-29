import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, ViewChild, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, debounceTime, switchMap } from 'rxjs';
import {
  BusinessCard,
  BusinessCardDraft,
  BusinessCardFilters,
  EMPTY_FILTERS,
  ExportFormat,
  ImportPreview
} from './core/models/business-card.model';
import { BusinessCardService } from './core/services/business-card.service';
import { FileDownloadService } from './core/services/file-download.service';
import { NotificationService } from './core/services/notification.service';
import { CardFiltersComponent } from './components/card-filters/card-filters.component';
import { CardFormComponent } from './components/card-form/card-form.component';
import { CardListComponent } from './components/card-list/card-list.component';
import { ImportPanelComponent } from './components/import-panel/import-panel.component';
import { NoticeComponent } from './components/notice/notice.component';

@Component({
  selector: 'app-root',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    NoticeComponent,
    CardFormComponent,
    ImportPanelComponent,
    CardFiltersComponent,
    CardListComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  private readonly cardService = inject(BusinessCardService);
  private readonly downloads = inject(FileDownloadService);
  private readonly notifications = inject(NotificationService);
  private readonly destroyRef = inject(DestroyRef);

  @ViewChild(CardFormComponent) cardForm?: CardFormComponent;
  @ViewChild(ImportPanelComponent) importPanel?: ImportPanelComponent;

  readonly cards = signal<BusinessCard[]>([]);
  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly isImporting = signal(false);
  readonly isExporting = signal(false);

  private filters: BusinessCardFilters = { ...EMPTY_FILTERS };
  private readonly reload$ = new Subject<BusinessCardFilters>();

  ngOnInit(): void {
    this.reload$
      .pipe(
        debounceTime(0),
        switchMap(filters => {
          this.isLoading.set(true);
          return this.cardService.list(filters);
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: cards => {
          this.cards.set(cards);
          this.isLoading.set(false);
        },
        error: error => {
          this.isLoading.set(false);
          this.notifications.error(this.notifications.fromHttpError(error, 'Unable to load business cards.'));
        }
      });

    this.reload();
  }

  onFiltersChanged(filters: BusinessCardFilters): void {
    this.filters = filters;
    this.reload();
  }

  onCardSubmit(draft: BusinessCardDraft): void {
    this.isSaving.set(true);
    this.cardService.create(draft)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.isSaving.set(false);
          this.cardForm?.reset();
          this.notifications.success('Business card saved.');
          this.reload();
        },
        error: error => {
          this.isSaving.set(false);
          this.notifications.error(this.notifications.fromHttpError(error, 'Unable to save business card.'));
        }
      });
  }

  onImportConfirmed(preview: ImportPreview): void {
    this.isImporting.set(true);
    this.cardService.importFile(preview.mode, preview.file)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: result => {
          this.isImporting.set(false);
          this.importPanel?.clearAfterImport();
          this.notifications.success(`${result.importedCount} card(s) imported successfully.`);
          this.reload();
        },
        error: error => {
          this.isImporting.set(false);
          this.notifications.error(this.notifications.fromHttpError(error, 'Unable to import file.'));
        }
      });
  }

  onCardDeleted(card: BusinessCard): void {
    this.cardService.delete(card.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.notifications.success('Business card deleted.');
          this.reload();
        },
        error: error => {
          this.notifications.error(this.notifications.fromHttpError(error, 'Unable to delete business card.'));
        }
      });
  }

  onCardExported(payload: { card: BusinessCard; format: ExportFormat }): void {
    this.isExporting.set(true);
    this.cardService.exportOne(payload.card.id, payload.format)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: response => {
          this.isExporting.set(false);
          this.downloads.saveResponse(response, payload.format);
        },
        error: error => {
          this.isExporting.set(false);
          this.notifications.error(this.notifications.fromHttpError(error, `Unable to export ${payload.card.name}.`));
        }
      });
  }

  exportAll(format: ExportFormat): void {
    this.isExporting.set(true);
    this.cardService.exportAll(format, this.filters)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: response => {
          this.isExporting.set(false);
          this.downloads.saveResponse(response, format);
        },
        error: error => {
          this.isExporting.set(false);
          this.notifications.error(this.notifications.fromHttpError(error, `Unable to export ${format.toUpperCase()}.`));
        }
      });
  }

  private reload(): void {
    this.reload$.next(this.filters);
  }
}
