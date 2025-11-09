import api from './api'
import type { ApiKey, CreateApiKeyDto, UpdateApiKeyDto, ApiKeyCreationResponse } from '@/types/apiKey'

export const apiKeyService = {
  async getAll(): Promise<ApiKey[]> {
    const response = await api.get('/api/ApiKeys')
    return response.data
  },

  async getById(id: string): Promise<ApiKey> {
    const response = await api.get(`/api/ApiKeys/${id}`)
    return response.data
  },

  async create(apiKey: CreateApiKeyDto): Promise<ApiKeyCreationResponse> {
    const response = await api.post('/api/ApiKeys', apiKey)
    return response.data
  },

  async update(id: string, apiKey: UpdateApiKeyDto): Promise<ApiKey> {
    const response = await api.put(`/api/ApiKeys/${id}`, apiKey)
    return response.data
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/api/ApiKeys/${id}`)
  }
}
