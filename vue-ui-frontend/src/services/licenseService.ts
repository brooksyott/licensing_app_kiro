import api from './api'
import type { License, CreateLicenseDto, UpdateLicenseDto } from '@/types/license'

export const licenseService = {
  async getAll(): Promise<License[]> {
    const response = await api.get('/api/Licenses')
    return response.data
  },

  async getById(id: string): Promise<License> {
    const response = await api.get(`/api/Licenses/${id}`)
    return response.data
  },

  async create(license: CreateLicenseDto): Promise<License> {
    const response = await api.post('/api/Licenses', license)
    return response.data
  },

  async update(id: string, license: UpdateLicenseDto): Promise<License> {
    const response = await api.put(`/api/Licenses/${id}`, license)
    return response.data
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/api/Licenses/${id}`)
  }
}
