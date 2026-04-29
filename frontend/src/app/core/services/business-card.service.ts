import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  BusinessCard,
  BusinessCardDraft,
  BusinessCardFilters,
  ExportFormat,
  ImportMode,
  ImportResult
} from '../models/business-card.model';

@Injectable({ providedIn: 'root' })
export class BusinessCardService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/BusinessCards`;

  list(filters: BusinessCardFilters): Observable<BusinessCard[]> {
    return this.http.get<BusinessCard[]>(this.baseUrl, { params: this.toParams(filters) });
  }

  create(draft: BusinessCardDraft): Observable<BusinessCard> {
    return this.http.post<BusinessCard>(this.baseUrl, draft);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  importFile(mode: ImportMode, file: File): Observable<ImportResult> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ImportResult>(`${this.baseUrl}/import/${mode}`, formData);
  }

  exportAll(format: ExportFormat, filters: BusinessCardFilters): Observable<HttpResponse<Blob>> {
    return this.http.get(`${this.baseUrl}/export/${format}`, {
      observe: 'response',
      params: this.toParams(filters),
      responseType: 'blob'
    });
  }

  exportOne(id: number, format: ExportFormat): Observable<HttpResponse<Blob>> {
    return this.http.get(`${this.baseUrl}/${id}/export/${format}`, {
      observe: 'response',
      responseType: 'blob'
    });
  }

  private toParams(filters: BusinessCardFilters): HttpParams {
    let params = new HttpParams();
    (Object.entries(filters) as [keyof BusinessCardFilters, string][]).forEach(([key, value]) => {
      if (value?.trim()) {
        params = params.set(key, value.trim());
      }
    });
    return params;
  }
}
