export type ImportMode = 'csv' | 'xml' | 'qr';
export type ExportFormat = 'csv' | 'xml';
export type Gender = 'Male' | 'Female';

export interface BusinessCard {
  id: number;
  name: string;
  gender: string;
  dateOfBirth: string;
  email: string;
  phone: string;
  photoBase64: string | null;
  address: string;
  createdAtUtc: string;
}

export interface BusinessCardDraft {
  name: string;
  gender: string;
  dateOfBirth: string;
  email: string;
  phone: string;
  photoBase64: string | null;
  address: string;
}

export interface BusinessCardFilters {
  name: string;
  gender: string;
  dateOfBirth: string;
  email: string;
  phone: string;
}

export interface ImportPreview {
  mode: ImportMode;
  file: File;
  cards: BusinessCardDraft[];
}

export interface ImportResult {
  importedCount: number;
  cards: BusinessCard[];
}

export const EMPTY_DRAFT: BusinessCardDraft = {
  name: '',
  gender: '',
  dateOfBirth: '',
  email: '',
  phone: '',
  photoBase64: null,
  address: ''
};

export const EMPTY_FILTERS: BusinessCardFilters = {
  name: '',
  gender: '',
  dateOfBirth: '',
  email: '',
  phone: ''
};

export const MAX_PHOTO_BYTES = 1024 * 1024;
