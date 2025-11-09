<script setup lang="ts">
import { useNotification } from '@/composables/useNotification'

const { notifications, removeNotification } = useNotification()
</script>

<template>
  <div class="notification-container">
    <transition-group name="notification">
      <div
        v-for="notification in notifications"
        :key="notification.id"
        :class="['notification', `notification-${notification.type}`]"
      >
        <span class="notification-message">{{ notification.message }}</span>
        <button
          class="notification-close"
          @click="removeNotification(notification.id)"
        >
          ×
        </button>
      </div>
    </transition-group>
  </div>
</template>

<style scoped>
.notification-container {
  position: fixed;
  top: 1rem;
  right: 1rem;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  max-width: 400px;
}

.notification {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 1.5rem;
  border-radius: 4px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  animation: slideIn 0.3s ease-out;
}

.notification-success {
  background-color: #d4edda;
  color: #155724;
  border-left: 4px solid #28a745;
}

.notification-error {
  background-color: #f8d7da;
  color: #721c24;
  border-left: 4px solid #dc3545;
}

.notification-info {
  background-color: #d1ecf1;
  color: #0c5460;
  border-left: 4px solid #17a2b8;
}

.notification-message {
  flex: 1;
  margin-right: 1rem;
}

.notification-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: inherit;
  opacity: 0.7;
  transition: opacity 0.2s;
}

.notification-close:hover {
  opacity: 1;
}

.notification-enter-active,
.notification-leave-active {
  transition: all 0.3s ease;
}

.notification-enter-from {
  opacity: 0;
  transform: translateX(100%);
}

.notification-leave-to {
  opacity: 0;
  transform: translateX(100%);
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateX(100%);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

/* Responsive styles for mobile */
@media (max-width: 768px) {
  .notification-container {
    left: 0.5rem;
    right: 0.5rem;
    top: 0.5rem;
    max-width: none;
  }

  .notification {
    padding: 0.875rem 1.25rem;
    font-size: 0.9rem;
  }

  .notification-message {
    margin-right: 0.75rem;
  }

  .notification-close {
    font-size: 1.25rem;
  }
}
</style>
