import api from './api'
import type { Sku, CreateSkuDto, UpdateSkuDto } from '@/types/sku'

export const skuService = {
  async getAll(): Promise<Sku[]> {
    const response = await api.get('/api/Skus')
    return response.data
  },

  async getById(id: string): Promise<Sku> {
    const response = await api.get(`/api/Skus/${id}`)
    return response.data
  },

  async create(sku: CreateSkuDto): Promise<Sku> {
    const response = await api.post('/api/Skus', sku)
    return response.data
  },

  async update(id: string, sku: UpdateSkuDto): Promise<Sku> {
    const response = await api.put(`/api/Skus/${id}`, sku)
    return response.data
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/api/Skus/${id}`)
  }
}
