import { Injectable } from '@angular/core';
import { HttpResponse } from '@angular/common/http';
import { ExportFormat } from '../models/business-card.model';

@Injectable({ providedIn: 'root' })
export class FileDownloadService {
  saveResponse(response: HttpResponse<Blob>, format: ExportFormat): void {
    if (!response.body) return;
    const fileName = this.parseFileName(response.headers.get('content-disposition'), format);
    this.download(response.body, fileName);
  }

  private download(blob: Blob, fileName: string): void {
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = fileName;
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);
    URL.revokeObjectURL(url);
  }

  private parseFileName(contentDisposition: string | null, format: ExportFormat): string {
    const match = contentDisposition?.match(/filename="?([^"]+)"?/i);
    return match?.[1] ?? `business-cards.${format}`;
  }
}
