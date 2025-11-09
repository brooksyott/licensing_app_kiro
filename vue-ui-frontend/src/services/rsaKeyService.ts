import api from './api'
import type { RsaKey, CreateRsaKeyDto } from '@/types/rsaKey'

export const rsaKeyService = {
  async getAll(): Promise<RsaKey[]> {
    const response = await api.get('/api/RsaKeys')
    return response.data
  },

  async getById(id: string): Promise<RsaKey> {
    const response = await api.get(`/api/RsaKeys/${id}`)
    return response.data
  },

  async create(rsaKey: CreateRsaKeyDto): Promise<RsaKey> {
    const response = await api.post('/api/RsaKeys', rsaKey)
    return response.data
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/api/RsaKeys/${id}`)
  }
}
