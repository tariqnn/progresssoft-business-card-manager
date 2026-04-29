import { Injectable } from '@angular/core';
import { BrowserQRCodeReader } from '@zxing/browser';
import { BusinessCardDraft, ImportMode } from '../models/business-card.model';

@Injectable({ providedIn: 'root' })
export class FileParserService {
  private readonly qrReader = new BrowserQRCodeReader();

  async parse(file: File, mode: ImportMode): Promise<BusinessCardDraft[]> {
    if (mode === 'qr') {
      return [await this.parseQrFile(file)];
    }
    const text = await file.text();
    return mode === 'csv' ? this.parseCsv(text) : this.parseXml(text);
  }

  private parseCsv(text: string): BusinessCardDraft[] {
    const rows = this.parseCsvRows(text).filter(row => row.some(cell => cell.trim()));
    if (rows.length < 2) {
      throw new Error('CSV must contain a header row and at least one business card.');
    }
    const headers = rows[0].map(h => h.trim().toLowerCase());
    return rows.slice(1).map(row => this.normalize({
      name: this.byHeader(headers, row, 'name'),
      gender: this.byHeader(headers, row, 'gender'),
      dateOfBirth: this.byHeader(headers, row, 'dateofbirth'),
      email: this.byHeader(headers, row, 'email'),
      phone: this.byHeader(headers, row, 'phone'),
      photoBase64: this.byHeader(headers, row, 'photobase64') || this.byHeader(headers, row, 'photo') || null,
      address: this.byHeader(headers, row, 'address')
    }));
  }

  private parseCsvRows(text: string): string[][] {
    const rows: string[][] = [];
    let row: string[] = [];
    let cell = '';
    let inQuotes = false;

    for (let i = 0; i < text.length; i++) {
      const ch = text[i];
      const next = text[i + 1];

      if (ch === '"' && inQuotes && next === '"') {
        cell += '"';
        i++;
      } else if (ch === '"') {
        inQuotes = !inQuotes;
      } else if (ch === ',' && !inQuotes) {
        row.push(cell);
        cell = '';
      } else if ((ch === '\n' || ch === '\r') && !inQuotes) {
        if (ch === '\r' && next === '\n') i++;
        row.push(cell);
        rows.push(row);
        row = [];
        cell = '';
      } else {
        cell += ch;
      }
    }

    if (cell.length > 0 || row.length > 0) {
      row.push(cell);
      rows.push(row);
    }
    return rows;
  }

  private parseXml(text: string): BusinessCardDraft[] {
    const doc = new DOMParser().parseFromString(text, 'application/xml');
    if (doc.querySelector('parsererror')) {
      throw new Error('XML file is not valid.');
    }
    const root = doc.documentElement;
    const elements = root.tagName.toLowerCase() === 'businesscard'
      ? [root]
      : Array.from(doc.getElementsByTagName('BusinessCard'));

    if (elements.length === 0) {
      throw new Error('XML must contain BusinessCard elements.');
    }

    return elements.map(el => this.normalize({
      name: this.xmlValue(el, 'Name'),
      gender: this.xmlValue(el, 'Gender'),
      dateOfBirth: this.xmlValue(el, 'DateOfBirth'),
      email: this.xmlValue(el, 'Email'),
      phone: this.xmlValue(el, 'Phone'),
      photoBase64: this.xmlValue(el, 'PhotoBase64') || this.xmlValue(el, 'Photo') || null,
      address: this.xmlValue(el, 'Address')
    }));
  }

  private async parseQrFile(file: File): Promise<BusinessCardDraft> {
    const url = URL.createObjectURL(file);
    try {
      const result = await this.qrReader.decodeFromImageUrl(url);
      return this.parseQrPayload(result.getText());
    } finally {
      URL.revokeObjectURL(url);
    }
  }

  private parseQrPayload(payload: string): BusinessCardDraft {
    const trimmed = payload.trim();

    if (trimmed.startsWith('{')) {
      return this.normalize(JSON.parse(trimmed) as Partial<BusinessCardDraft>);
    }
    if (trimmed.startsWith('<')) {
      return this.parseXml(trimmed)[0];
    }

    const values = new Map<string, string>();
    trimmed
      .split(/\r?\n|;/)
      .map(line => line.trim())
      .filter(Boolean)
      .forEach(line => {
        const sep = line.search(/[:=]/);
        if (sep > 0) {
          values.set(
            line.slice(0, sep).split(';')[0].trim().toLowerCase(),
            line.slice(sep + 1).trim()
          );
        }
      });

    if (values.size === 0) {
      throw new Error('QR payload must contain JSON, XML, or key-value business card data.');
    }

    return this.normalize({
      name: values.get('name') ?? values.get('fn') ?? '',
      gender: values.get('gender') ?? values.get('x-gender') ?? '',
      dateOfBirth: values.get('dateofbirth') ?? values.get('dob') ?? values.get('bday') ?? '',
      email: values.get('email') ?? '',
      phone: values.get('phone') ?? values.get('tel') ?? '',
      photoBase64: null,
      address: values.get('address') ?? values.get('adr') ?? ''
    });
  }

  normalize(card: Partial<BusinessCardDraft>): BusinessCardDraft {
    return {
      name: card.name?.trim() ?? '',
      gender: card.gender?.trim() ?? '',
      dateOfBirth: card.dateOfBirth?.trim() ?? '',
      email: card.email?.trim() ?? '',
      phone: card.phone?.trim() ?? '',
      photoBase64: card.photoBase64 && card.photoBase64.trim() ? card.photoBase64.trim() : null,
      address: card.address?.trim() ?? ''
    };
  }

  private byHeader(headers: string[], row: string[], header: string): string {
    const i = headers.indexOf(header);
    return i >= 0 ? (row[i] ?? '').trim() : '';
  }

  private xmlValue(element: Element, name: string): string {
    const child = Array.from(element.children).find(c => c.tagName.toLowerCase() === name.toLowerCase());
    return child?.textContent?.trim() ?? '';
  }
}
