export interface ApiKey {
  id: string
  name: string
  role: string
  isActive: boolean
  createdBy: string
  createdAt: string
  lastUsedAt: string | null
}

export interface CreateApiKeyDto {
  name: string
  role: string
  createdBy: string
}

export interface UpdateApiKeyDto {
  name: string
  role: string
  isActive: boolean
}

export interface ApiKeyCreationResponse {
  id: string
  name: string
  role: string
  isActive: boolean
  createdBy: string
  createdAt: string
  key: string // Only available on creation
}
