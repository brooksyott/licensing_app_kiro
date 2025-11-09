import api from './api'
import type { Product, CreateProductDto, UpdateProductDto } from '@/types/product'

export const productService = {
  async getAll(): Promise<Product[]> {
    const response = await api.get('/api/Products')
    return response.data
  },

  async getById(id: string): Promise<Product> {
    const response = await api.get(`/api/Products/${id}`)
    return response.data
  },

  async create(product: CreateProductDto): Promise<Product> {
    const response = await api.post('/api/Products', product)
    return response.data
  },

  async update(id: string, product: UpdateProductDto): Promise<Product> {
    const response = await api.put(`/api/Products/${id}`, product)
    return response.data
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/api/Products/${id}`)
  }
}
