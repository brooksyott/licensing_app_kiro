import { ref } from 'vue'

export interface Notification {
  id: number
  type: 'success' | 'error' | 'info'
  message: string
  duration?: number
}

const notifications = ref<Notification[]>([])
let notificationId = 0

export function useNotification() {
  const addNotification = (type: Notification['type'], message: string, duration: number = 3000) => {
    const id = ++notificationId
    const notification: Notification = { id, type, message, duration }
    
    notifications.value.push(notification)

    if (type === 'success' && duration > 0) {
      setTimeout(() => {
        removeNotification(id)
      }, duration)
    }
  }

  const removeNotification = (id: number) => {
    const index = notifications.value.findIndex(n => n.id === id)
    if (index > -1) {
      notifications.value.splice(index, 1)
    }
  }

  const success = (message: string) => {
    addNotification('success', message, 3000)
  }

  const error = (message: string) => {
    addNotification('error', message, 0) // Errors don't auto-dismiss
  }

  const info = (message: string) => {
    addNotification('info', message, 3000)
  }

  return {
    notifications,
    success,
    error,
    info,
    removeNotification
  }
}
