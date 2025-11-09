<script setup lang="ts">
import Logo from '@/assets/logo.svg'
</script>

<template>
  <footer class="app-footer">
    <div class="footer-content">
      <div class="footer-left">
        <img :src="Logo" alt="Logo" class="footer-logo" />
      </div>
      <div class="footer-center">
        <p>&copy; {{ new Date().getFullYear() }} License Management System. All rights reserved.</p>
      </div>
      <div class="footer-right"></div>
    </div>
  </footer>
</template>

<style scoped>
.app-footer {
  background-color: #2c3e50;
  color: white;
  height: 100px;
  margin-top: auto;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  padding: 0 2rem;
}

.footer-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  max-width: 1400px;
}

.footer-left {
  flex: 1;
  display: flex;
  justify-content: flex-start;
}

.footer-logo {
  height: 40px;
  width: auto;
}

.footer-center {
  flex: 1;
  text-align: center;
}

.footer-center p {
  margin: 0;
  font-size: 0.9rem;
  color: #ecf0f1;
}

.footer-right {
  flex: 1;
}

/* Responsive styles for mobile */
@media (max-width: 768px) {
  .app-footer {
    padding: 0 1rem;
    height: auto;
    min-height: 100px;
  }

  .footer-content {
    flex-direction: column;
    gap: 1rem;
    padding: 1rem 0;
  }

  .footer-left,
  .footer-center,
  .footer-right {
    flex: none;
    width: 100%;
  }

  .footer-left {
    justify-content: center;
  }

  .footer-logo {
    height: 32px;
  }

  .footer-center p {
    font-size: 0.85rem;
  }
}
</style>
