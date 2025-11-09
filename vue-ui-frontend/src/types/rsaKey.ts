export interface RsaKey {
  id: string
  name: string
  publicKey: string
  keySize: number
  createdBy: string
  createdAt: string
  updatedAt: string
}

export interface CreateRsaKeyDto {
  name: string
  keySize: number
  createdBy: string
}
