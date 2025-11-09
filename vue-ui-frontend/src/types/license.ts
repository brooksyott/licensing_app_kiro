export interface LicenseSku {
  skuId: string
  skuName: string
  skuCode: string
}

export interface License {
  id: string
  customerId: string
  customerName: string
  productId: string
  productName: string
  skus: LicenseSku[]
  rsaKeyId: string
  rsaKeyName: string
  licenseType: string
  status: string
  expirationDate: string | null
  maxActivations: number
  currentActivations: number
  licenseKey: string
  signedPayload: string
  createdAt: string
  updatedAt: string
}

export interface CreateLicenseDto {
  customerId: string
  productId: string
  skuIds: string[]
  rsaKeyId: string
  licenseType: string
  expirationDate: string | null
  maxActivations: number
}

export interface UpdateLicenseDto {
  customerId: string
  productId: string
  skuIds: string[]
  rsaKeyId: string
  licenseType: string
  expirationDate: string | null
  maxActivations: number
}
