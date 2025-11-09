export interface License {
  id: string
  customerId: string
  customerName: string
  productId: string
  productName: string
  skuId: string
  skuName: string
  rsaKeyId: string
  rsaKeyName: string
  licenseType: string
  status: string
  expirationDate: string | null
  maxActivations: number
  currentActivations: number
  licenseKey: string
  createdAt: string
  updatedAt: string
}

export interface CreateLicenseDto {
  customerId: string
  productId: string
  skuId: string
  rsaKeyId: string
  licenseType: string
  expirationDate: string | null
  maxActivations: number
}

export interface UpdateLicenseDto {
  customerId: string
  productId: string
  skuId: string
  rsaKeyId: string
  licenseType: string
  expirationDate: string | null
  maxActivations: number
}
