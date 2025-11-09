<script setup lang="ts">
import { ref } from 'vue'
import { useAuth } from '@/composables/useAuth'
import { useRoute } from 'vue-router'
import IrisLogo from '@/assets/iris.svg'

// Use shared authentication state from composable
const { isLoggedIn, toggleAuth } = useAuth()
const route = useRoute()

const showSettingsDropdown = ref(false)
let closeTimeout: number | null = null

const navItems = [
  { name: 'Customers', path: '/customers' },
  { name: 'Products', path: '/products' },
  { name: 'SKUs', path: '/skus' },
  { name: 'Licenses', path: '/licenses' }
]

const settingsItems = [
  { name: 'RSA Keys', path: '/rsa-keys' },
  { name: 'API Keys', path: '/api-keys' }
]

const isSettingsActive = () => {
  return settingsItems.some(item => route.path === item.path)
}

const toggleSettings = () => {
  if (closeTimeout) {
    clearTimeout(closeTimeout)
    closeTimeout = null
  }
  showSettingsDropdown.value = !showSettingsDropdown.value
}

const openSettings = () => {
  if (closeTimeout) {
    clearTimeout(closeTimeout)
    closeTimeout = null
  }
  showSettingsDropdown.value = true
}

const closeSettings = () => {
  closeTimeout = window.setTimeout(() => {
    showSettingsDropdown.value = false
  }, 200)
}

const cancelClose = () => {
  if (closeTimeout) {
    clearTimeout(closeTimeout)
    closeTimeout = null
  }
}
</script>

<template>
  <header class="app-header">
    <div class="header-content">
      <div class="header-left">
        <router-link to="/" class="logo-link">
          <img :src="IrisLogo" alt="Iris Logo" class="app-logo" />
        </router-link>
        <h1 class="app-title">License Management</h1>
      </div>
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
        
        <div class="dropdown" @mouseenter="openSettings" @mouseleave="closeSettings">
          <button 
            class="nav-link dropdown-toggle" 
            :class="{ active: isSettingsActive() }"
            @click="toggleSettings"
          >
            Settings ▾
          </button>
          <div v-if="showSettingsDropdown" class="dropdown-menu" @mouseenter="cancelClose">
            <router-link
              v-for="item in settingsItems"
              :key="item.path"
              :to="item.path"
              class="dropdown-item"
              :class="{ active: route.path === item.path }"
            >
              {{ item.name }}
            </router-link>
          </div>
        </div>
      </nav>
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
  background-image: url('@/assets/Header-Background2.jpg');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  color: white;
  padding: 2.4rem 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  width: 100%;
  position: relative;
}

.app-header::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(44, 62, 80, 0.7);
  z-index: 0;
}

.header-content {
  position: relative;
  z-index: 1;
}

.header-content {
  position: relative;
  z-index: 1;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 2rem;
  padding: 0 2rem;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 1rem;
  min-width: 200px;
}

.logo-link {
  display: flex;
  align-items: center;
  cursor: pointer;
  transition: opacity 0.2s;
}

.logo-link:hover {
  opacity: 0.8;
}

.app-logo {
  height: 48px;
  width: auto;
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
  justify-content: center;
  flex: 1;
}

.nav-link {
  color: #ecf0f1;
  text-decoration: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  font-size: 1.1rem;
  font-weight: 500;
  transition: all 0.2s;
  white-space: nowrap;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.1);
  color: white;
}

.nav-link.active {
  background-color: #00A3AD;
  color: white;
}

.dropdown {
  position: relative;
  display: inline-block;
}

.dropdown-toggle {
  background: none;
  border: none;
  cursor: pointer;
  font-family: inherit;
}

.dropdown-menu {
  position: absolute;
  top: 100%;
  left: 0;
  background-color: #2c3e50;
  min-width: 160px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.3);
  border-radius: 4px;
  margin-top: 0.5rem;
  z-index: 1000;
  overflow: hidden;
}

.dropdown-item {
  display: block;
  color: #ecf0f1;
  text-decoration: none;
  padding: 0.75rem 1rem;
  font-size: 1rem;
  font-weight: 500;
  transition: all 0.2s;
  white-space: nowrap;
}

.dropdown-item:hover {
  background-color: rgba(255, 255, 255, 0.1);
  color: white;
}

.dropdown-item.active {
  background-color: #00A3AD;
  color: white;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 1rem;
  min-width: 200px;
  justify-content: flex-end;
}

.auth-status {
  font-size: 0.9rem;
  color: #ecf0f1;
  white-space: nowrap;
}

.auth-button {
  background-color: #00A3AD;
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
  background-color: #008A93;
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
    min-width: 150px;
  }

  .header-right {
    min-width: 150px;
  }

  .app-title {
    font-size: 1.25rem;
  }

  .nav-link {
    padding: 0.4rem 0.75rem;
    font-size: 1rem;
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
    padding: 1.5rem 0;
  }

  .header-content {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
    padding: 0 1rem;
  }

  .header-left {
    min-width: auto;
    justify-content: center;
  }

  .header-right {
    min-width: auto;
    justify-content: space-between;
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

  .auth-status {
    font-size: 0.85rem;
  }

  .auth-button {
    padding: 0.5rem 0.875rem;
    font-size: 0.85rem;
  }
}
</style>
