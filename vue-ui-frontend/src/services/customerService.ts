import api from './api'
import type { Customer, CreateCustomerDto, UpdateCustomerDto } from '@/types/customer'

export const customerService = {
  async getAll(): Promise<Customer[]> {
    const response = await api.get('/api/Customers')
    return response.data
  },

  async getById(id: string): Promise<Customer> {
    const response = await api.get(`/api/Customers/${id}`)
    return response.data
  },

  async create(customer: CreateCustomerDto): Promise<Customer> {
    const response = await api.post('/api/Customers', customer)
    return response.data
  },

  async update(id: string, customer: UpdateCustomerDto): Promise<Customer> {
    const response = await api.put(`/api/Customers/${id}`, customer)
    return response.data
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/api/Customers/${id}`)
  }
}
