export interface Product {
  id: string
  name: string
  productCode: string
  version: string
  description: string
  createdAt: string
  updatedAt: string
}

export interface CreateProductDto {
  name: string
  productCode: string
  version: string
  description: string
}

export interface UpdateProductDto {
  name: string
  productCode: string
  version: string
  description: string
}
