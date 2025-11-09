export interface Customer {
  id: string
  name: string
  email: string
  organization: string
  isVisible: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateCustomerDto {
  name: string
  email: string
  organization: string
  isVisible: boolean
}

export interface UpdateCustomerDto {
  name: string
  email: string
  organization: string
  isVisible: boolean
}
