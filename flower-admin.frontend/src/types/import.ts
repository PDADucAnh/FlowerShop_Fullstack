export interface ImportError {
  rowIndex: number
  productCode?: string
  productName?: string
  errorMessage: string
}

export interface ImportApiResponse {
  totalRows: number
  successCount: number
  failureCount: number
  errors: ImportError[]
  skippedSkus: string[]
}
