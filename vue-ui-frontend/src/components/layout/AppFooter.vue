<template>
  <footer class="app-footer">
    <div class="footer-content">
      <p>&copy; {{ new Date().getFullYear() }} License Management System. All rights reserved.</p>
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
}

.footer-content {
  max-width: 1400px;
  margin: 0 auto;
  text-align: center;
}

.footer-content p {
  margin: 0;
  font-size: 0.9rem;
  color: #ecf0f1;
}
</style>
