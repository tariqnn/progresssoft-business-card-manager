import { Injectable, signal } from '@angular/core';

export type NoticeKind = 'success' | 'error' | 'info';

export interface Notice {
  kind: NoticeKind;
  message: string;
  id: number;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly autoDismissMs = 4500;
  private nextId = 1;

  readonly notice = signal<Notice | null>(null);

  show(kind: NoticeKind, message: string): void {
    const id = this.nextId++;
    this.notice.set({ kind, message, id });

    window.setTimeout(() => {
      if (this.notice()?.id === id) {
        this.notice.set(null);
      }
    }, this.autoDismissMs);
  }

  success(message: string): void {
    this.show('success', message);
  }

  error(message: string): void {
    this.show('error', message);
  }

  info(message: string): void {
    this.show('info', message);
  }

  dismiss(): void {
    this.notice.set(null);
  }

  fromHttpError(error: unknown, fallback: string): string {
    if (typeof error === 'object' && error !== null && 'error' in error) {
      const body = (error as { error?: { message?: string; errors?: string[] } }).error;
      if (body?.errors?.length) {
        return `${body.message ?? fallback} ${body.errors.join(' ')}`;
      }
      return body?.message ?? fallback;
    }
    return fallback;
  }
}
