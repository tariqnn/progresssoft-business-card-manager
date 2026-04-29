import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, input, output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BusinessCardDraft, EMPTY_DRAFT, MAX_PHOTO_BYTES } from '../../core/models/business-card.model';
import { NotificationService } from '../../core/services/notification.service';
import { CardPreviewComponent } from '../card-preview/card-preview.component';

@Component({
  selector: 'app-card-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CardPreviewComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './card-form.component.html',
  styleUrl: './card-form.component.css'
})
export class CardFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly notifications = inject(NotificationService);
  private readonly destroyRef = inject(DestroyRef);

  readonly cardSubmit = output<BusinessCardDraft>();
  readonly isSaving = input(false);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    gender: ['', [Validators.required]],
    dateOfBirth: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(254)]],
    phone: ['', [Validators.required, Validators.maxLength(30)]],
    photoBase64: [null as string | null],
    address: ['', [Validators.required, Validators.maxLength(500)]]
  });

  livePreview: BusinessCardDraft = { ...EMPTY_DRAFT };

  ngOnInit(): void {
    this.form.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(value => {
        this.livePreview = {
          name: value.name ?? '',
          gender: value.gender ?? '',
          dateOfBirth: value.dateOfBirth ?? '',
          email: value.email ?? '',
          phone: value.phone ?? '',
          photoBase64: value.photoBase64 ?? null,
          address: value.address ?? ''
        };
      });
  }

  async onPhotoSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    if (file.size > MAX_PHOTO_BYTES) {
      this.notifications.error('Photo must not exceed 1MB.');
      input.value = '';
      return;
    }

    const dataUrl = await this.readAsDataUrl(file);
    this.form.patchValue({ photoBase64: dataUrl });
  }

  removePhoto(): void {
    this.form.patchValue({ photoBase64: null });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.cardSubmit.emit(this.form.getRawValue() as BusinessCardDraft);
  }

  reset(): void {
    this.form.reset({ ...EMPTY_DRAFT });
  }

  hasError(field: string, error: string): boolean {
    const control = this.form.get(field);
    return !!(control?.touched && control.hasError(error));
  }

  private readAsDataUrl(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(String(reader.result));
      reader.onerror = () => reject(new Error('Unable to read selected file.'));
      reader.readAsDataURL(file);
    });
  }
}
