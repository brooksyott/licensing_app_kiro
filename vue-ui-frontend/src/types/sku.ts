export interface Sku {
  id: string
  name: string
  skuCode: string
  productId: string
  productName: string
  description: string
  createdAt: string
  updatedAt: string
}

export interface CreateSkuDto {
  name: string
  skuCode: string
  productId: string
  description: string
}

export interface UpdateSkuDto {
  name: string
  skuCode: string
  productId: string
  description: string
}
