import { useState, useRef, type ChangeEvent, type DragEvent } from 'react'
import { useMutation } from '@tanstack/react-query'
import { importsApi } from '@/api/imports'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { Loader2, Upload, FileSpreadsheet, FileArchive, X, Download, AlertCircle, CheckCircle2, ChevronDown, ChevronUp } from 'lucide-react'
import { toast } from 'sonner'
import type { ImportApiResponse } from '@/types/import'

function ImportForm({
  type,
  isPending,
  onUpload,
  onResult,
  handleDownloadTemplate,
}: {
  type: 'products' | 'categories'
  isPending: boolean
  onUpload: (formData: FormData, onSuccess: (data: ImportApiResponse) => void) => void
  onResult: (data: ImportApiResponse | null) => void
  handleDownloadTemplate: () => Promise<void>
}) {
  const [excelFile, setExcelFile] = useState<File | null>(null)
  const [zipFile, setZipFile] = useState<File | null>(null)
  const [duplicateAction, setDuplicateAction] = useState<'skip' | 'update'>('skip')
  const [result, setResult] = useState<ImportApiResponse | null>(null)
  const [showErrors, setShowErrors] = useState(true)
  const [dragOver, setDragOver] = useState(false)
  const excelRef = useRef<HTMLInputElement>(null)
  const zipRef = useRef<HTMLInputElement>(null)

  const handleExcelSelect = (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (file) {
      const ext = file.name.split('.').pop()?.toLowerCase()
      if (!ext || !['xlsx', 'xls'].includes(ext)) {
        toast.error('Vui lòng chọn file Excel (.xlsx hoặc .xls)')
        return
      }
      setExcelFile(file)
      setResult(null)
    }
  }

  const handleZipSelect = (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (file) {
      const ext = file.name.split('.').pop()?.toLowerCase()
      if (ext !== 'zip') {
        toast.error('Vui lòng chọn file .zip')
        return
      }
      setZipFile(file)
    }
  }

  const handleDrop = (e: DragEvent<HTMLDivElement>) => {
    e.preventDefault()
    setDragOver(false)
    const file = e.dataTransfer.files[0]
    if (!file) return
    const ext = file.name.split('.').pop()?.toLowerCase()
    if (ext === 'xlsx' || ext === 'xls') {
      setExcelFile(file)
      setResult(null)
    } else if (ext === 'zip') {
      setZipFile(file)
    } else {
      toast.error('Định dạng file không hỗ trợ')
    }
  }

  const handleDragOver = (e: DragEvent<HTMLDivElement>) => {
    e.preventDefault()
    setDragOver(true)
  }

  const handleDragLeave = () => setDragOver(false)

  const handleSubmit = () => {
    if (!excelFile) {
      toast.error('Vui lòng chọn file Excel')
      return
    }

    const formData = new FormData()
    formData.append('excelFile', excelFile)
    if (zipFile) formData.append('zipFile', zipFile)
    formData.append('onDuplicate', duplicateAction)

    onUpload(formData, (data) => {
      setResult(data)
    })
  }

  const resetForm = () => {
    setExcelFile(null)
    setZipFile(null)
    setResult(null)
    setDuplicateAction('skip')
  }

  return (
    <div className="space-y-4">
      <Card>
        <CardHeader>
          <CardTitle className="text-base">Tải lên file Excel</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div
            onDrop={handleDrop}
            onDragOver={handleDragOver}
            onDragLeave={handleDragLeave}
            className={`flex flex-col items-center justify-center rounded-lg border-2 border-dashed p-8 transition-colors ${
              dragOver ? 'border-primary bg-primary/5' : 'border-border bg-muted/30'
            }`}
          >
            <Upload className="mb-3 size-8 text-muted-foreground" />
            <p className="mb-1 text-sm font-medium">Kéo thả file Excel vào đây</p>
            <p className="mb-3 text-xs text-muted-foreground">hoặc</p>
            <div className="flex gap-2">
              <Button variant="secondary" size="sm" onClick={() => excelRef.current?.click()}>
                <FileSpreadsheet className="mr-1 size-4" />
                Chọn file Excel
              </Button>
              {type === 'products' && (
                <Button variant="secondary" size="sm" onClick={() => zipRef.current?.click()}>
                  <FileArchive className="mr-1 size-4" />
                  Chọn file ảnh (.zip)
                </Button>
              )}
            </div>
            <input ref={excelRef} type="file" accept=".xlsx,.xls" className="hidden" onChange={handleExcelSelect} />
            {type === 'products' && <input ref={zipRef} type="file" accept=".zip" className="hidden" onChange={handleZipSelect} />}
          </div>

          {excelFile && (
            <div className="flex items-center justify-between rounded-lg border bg-surface px-3 py-2">
              <div className="flex items-center gap-2 text-sm">
                <FileSpreadsheet className="size-4 text-emerald-600" />
                <span className="font-medium">{excelFile.name}</span>
                <span className="text-muted-foreground">({(excelFile.size / 1024).toFixed(1)} KB)</span>
              </div>
              <Button variant="ghost" size="icon" className="size-6" onClick={() => setExcelFile(null)}>
                <X className="size-4" />
              </Button>
            </div>
          )}

          {type === 'products' && zipFile && (
            <div className="flex items-center justify-between rounded-lg border bg-surface px-3 py-2">
              <div className="flex items-center gap-2 text-sm">
                <FileArchive className="size-4 text-amber-600" />
                <span className="font-medium">{zipFile.name}</span>
                <span className="text-muted-foreground">({(zipFile.size / 1024).toFixed(1)} KB)</span>
              </div>
              <Button variant="ghost" size="icon" className="size-6" onClick={() => setZipFile(null)}>
                <X className="size-4" />
              </Button>
            </div>
          )}

          <div>
            <label className="text-sm font-medium">Khi trùng {type === 'products' ? 'SKU' : 'tên'}</label>
            <div className="mt-1 flex gap-4">
              <label className="flex items-center gap-2 text-sm">
                <input type="radio" name={`duplicate-${type}`} value="skip" checked={duplicateAction === 'skip'} onChange={() => setDuplicateAction('skip')} className="size-4" />
                Bỏ qua
              </label>
              <label className="flex items-center gap-2 text-sm">
                <input type="radio" name={`duplicate-${type}`} value="update" checked={duplicateAction === 'update'} onChange={() => setDuplicateAction('update')} className="size-4" />
                Cập nhật
              </label>
            </div>
          </div>

          <div className="flex justify-end gap-2">
            {result && (
              <Button variant="outline" onClick={resetForm}>
                <X className="mr-1 size-4" />
                Nhập lại
              </Button>
            )}
            <Button onClick={handleSubmit} disabled={!excelFile || isPending}>
              {isPending ? (
                <>
                  <Loader2 className="mr-2 size-4 animate-spin" />
                  Đang xử lý…
                </>
              ) : (
                <>
                  <Upload className="mr-2 size-4" />
                  Tiến hành Import
                </>
              )}
            </Button>
          </div>
        </CardContent>
      </Card>

      {isPending && (
        <Card>
          <CardContent className="flex items-center justify-center py-12">
            <div className="flex flex-col items-center gap-3 text-muted-foreground">
              <Loader2 className="size-8 animate-spin" />
              <p className="text-sm">Đang xử lý file Excel...</p>
            </div>
          </CardContent>
        </Card>
      )}

      {result && (
        <Card>
          <CardHeader>
            <CardTitle className="text-base">Kết quả import</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-3 gap-4">
              <div className="rounded-lg bg-muted p-4 text-center">
                <p className="text-2xl font-bold">{result.totalRows}</p>
                <p className="text-xs text-muted-foreground">Tổng số dòng</p>
              </div>
              <div className="rounded-lg bg-emerald-50 p-4 text-center dark:bg-emerald-950/20">
                <p className="text-2xl font-bold text-emerald-600">{result.successCount}</p>
                <p className="text-xs text-emerald-600">Thành công</p>
              </div>
              <div className="rounded-lg bg-red-50 p-4 text-center dark:bg-red-950/20">
                <p className="text-2xl font-bold text-red-600">{result.failureCount}</p>
                <p className="text-xs text-red-600">Thất bại</p>
              </div>
            </div>

            {result.errors.length > 0 && (
              <div className="space-y-2">
                <button
                  onClick={() => setShowErrors(!showErrors)}
                  className="flex items-center gap-1 text-sm font-medium text-muted-foreground hover:text-foreground"
                >
                  {showErrors ? <ChevronUp className="size-4" /> : <ChevronDown className="size-4" />}
                  Chi tiết lỗi ({result.errors.length})
                </button>
                {showErrors && (
                  <div className="overflow-x-auto rounded-lg border">
                    <table className="w-full text-sm">
                      <thead className="bg-muted/50">
                        <tr>
                          <th className="px-3 py-2 text-left font-medium">Dòng</th>
                          <th className="px-3 py-2 text-left font-medium">Tên</th>
                          <th className="px-3 py-2 text-left font-medium">Lỗi</th>
                        </tr>
                      </thead>
                      <tbody>
                        {result.errors.map((err, idx) => (
                          <tr key={idx} className="border-t">
                            <td className="px-3 py-2 text-muted-foreground">{err.rowIndex}</td>
                            <td className="px-3 py-2">{err.productName || '—'}</td>
                            <td className="px-3 py-2 text-red-600">{err.errorMessage}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
              </div>
            )}
          </CardContent>
        </Card>
      )}
    </div>
  )
}

export function ImportPage() {
  const productMutation = useMutation({
    mutationFn: (formData: FormData) => importsApi.upload(formData).then((r) => r.data),
  })

  const categoryMutation = useMutation({
    mutationFn: (formData: FormData) => importsApi.uploadCategories(formData).then((r) => r.data),
  })

  const handleDownloadProductTemplate = async () => {
    try {
      const response = await importsApi.downloadTemplate()
      const url = URL.createObjectURL(new Blob([response.data]))
      const link = document.createElement('a')
      link.href = url
      link.download = 'product_import_template.xlsx'
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      URL.revokeObjectURL(url)
      toast.success('Đã tải file mẫu')
    } catch {
      toast.error('Không thể tải file mẫu')
    }
  }

  const handleDownloadCategoryTemplate = async () => {
    try {
      const response = await importsApi.downloadCategoryTemplate()
      const url = URL.createObjectURL(new Blob([response.data]))
      const link = document.createElement('a')
      link.href = url
      link.download = 'category_import_template.xlsx'
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      URL.revokeObjectURL(url)
      toast.success('Đã tải file mẫu')
    } catch {
      toast.error('Không thể tải file mẫu')
    }
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-semibold">Import dữ liệu</h1>

      <Tabs defaultValue="products">
        <TabsList>
          <TabsTrigger value="products">Sản phẩm</TabsTrigger>
          <TabsTrigger value="categories">Danh mục</TabsTrigger>
        </TabsList>

        <TabsContent value="products" className="mt-4">
          <div className="mb-4 flex justify-end">
            <Button variant="outline" size="sm" onClick={handleDownloadProductTemplate}>
              <Download className="mr-1 size-4" />
              Tải file Excel mẫu
            </Button>
          </div>
          <ImportForm
            type="products"
            isPending={productMutation.isPending}
            onUpload={(formData: FormData, onSuccess: (data: ImportApiResponse) => void) =>
              productMutation.mutate(formData, {
                onSuccess: (data: ImportApiResponse) => {
                  onSuccess(data)
                  if (data.successCount > 0) toast.success(`Import thành công ${data.successCount} sản phẩm`)
                  if (data.failureCount > 0) toast.error(`${data.failureCount} dòng bị lỗi`)
                },
                onError: () => toast.error('Import thất bại. Vui lòng thử lại.'),
              })
            }
            onResult={() => {}}
            handleDownloadTemplate={handleDownloadProductTemplate}
          />
        </TabsContent>

        <TabsContent value="categories" className="mt-4">
          <div className="mb-4 flex justify-end">
            <Button variant="outline" size="sm" onClick={handleDownloadCategoryTemplate}>
              <Download className="mr-1 size-4" />
              Tải file Excel mẫu
            </Button>
          </div>
          <ImportForm
            type="categories"
            isPending={categoryMutation.isPending}
            onUpload={(formData: FormData, onSuccess: (data: ImportApiResponse) => void) =>
              categoryMutation.mutate(formData, {
                onSuccess: (data: ImportApiResponse) => {
                  onSuccess(data)
                  if (data.successCount > 0) toast.success(`Import thành công ${data.successCount} danh mục`)
                  if (data.failureCount > 0) toast.error(`${data.failureCount} dòng bị lỗi`)
                },
                onError: () => toast.error('Import thất bại. Vui lòng thử lại.'),
              })
            }
            onResult={() => {}}
            handleDownloadTemplate={handleDownloadCategoryTemplate}
          />
        </TabsContent>
      </Tabs>
    </div>
  )
}
