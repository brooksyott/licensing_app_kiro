<script setup lang="ts">
import { useAuth } from '@/composables/useAuth'
import { useRoute } from 'vue-router'

// Use shared authentication state from composable
const { isLoggedIn, toggleAuth } = useAuth()
const route = useRoute()

const navItems = [
  { name: 'Customers', path: '/customers' },
  { name: 'Products', path: '/products' },
  { name: 'SKUs', path: '/skus' },
  { name: 'Licenses', path: '/licenses' },
  { name: 'RSA Keys', path: '/rsa-keys' },
  { name: 'API Keys', path: '/api-keys' }
]
</script>

<template>
  <header class="app-header">
    <div class="header-content">
      <div class="header-left">
        <h1 class="app-title">License Management</h1>
        <nav class="nav-menu">
          <router-link
            v-for="item in navItems"
            :key="item.path"
            :to="item.path"
            class="nav-link"
            :class="{ active: route.path === item.path }"
          >
            {{ item.name }}
          </router-link>
        </nav>
      </div>
      <div class="header-right">
        <span class="auth-status">
          Status: {{ isLoggedIn ? 'Logged In' : 'Logged Out' }}
        </span>
        <button class="auth-button" @click="toggleAuth">
          {{ isLoggedIn ? 'Logout' : 'Login' }}
        </button>
      </div>
    </div>
  </header>
</template>

<style scoped>
.app-header {
  background-color: #2c3e50;
  color: white;
  padding: 1rem 2rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.header-content {
  max-width: 1400px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 2rem;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 2rem;
  flex: 1;
}

.app-title {
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0;
  white-space: nowrap;
}

.nav-menu {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.nav-link {
  color: #ecf0f1;
  text-decoration: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  font-size: 0.9rem;
  font-weight: 500;
  transition: all 0.2s;
  white-space: nowrap;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.1);
  color: white;
}

.nav-link.active {
  background-color: #3498db;
  color: white;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.auth-status {
  font-size: 0.9rem;
  color: #ecf0f1;
  white-space: nowrap;
}

.auth-button {
  background-color: #3498db;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  font-weight: 500;
  transition: background-color 0.2s;
  white-space: nowrap;
}

.auth-button:hover {
  background-color: #2980b9;
}

.auth-button:active {
  transform: translateY(1px);
}

/* Responsive styles for tablet */
@media (max-width: 1024px) {
  .header-content {
    gap: 1rem;
  }

  .header-left {
    gap: 1rem;
  }

  .app-title {
    font-size: 1.25rem;
  }

  .nav-link {
    padding: 0.4rem 0.75rem;
    font-size: 0.85rem;
  }

  .auth-status {
    font-size: 0.85rem;
  }

  .auth-button {
    padding: 0.4rem 0.875rem;
    font-size: 0.85rem;
  }
}

/* Responsive styles for mobile */
@media (max-width: 768px) {
  .app-header {
    padding: 1rem;
  }

  .header-content {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
  }

  .header-left {
    flex-direction: column;
    align-items: stretch;
    gap: 0.75rem;
  }

  .app-title {
    font-size: 1.25rem;
    text-align: center;
  }

  .nav-menu {
    flex-wrap: wrap;
    justify-content: center;
    gap: 0.5rem;
  }

  .nav-link {
    padding: 0.5rem 0.75rem;
    font-size: 0.85rem;
  }

  .header-right {
    justify-content: space-between;
  }

  .auth-status {
    font-size: 0.85rem;
  }

  .auth-button {
    padding: 0.5rem 0.875rem;
    font-size: 0.85rem;
  }
}
</style>
