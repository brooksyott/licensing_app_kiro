import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from 'node:url'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue({
      script: {
        defineModel: true,
        propsDestructure: true
      }
    })
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  build: {
    // Optimize chunk splitting for better caching
    rollupOptions: {
      output: {
        manualChunks: {
          // Separate vendor chunks for better caching
          'vue-vendor': ['vue', 'vue-router'],
          'axios-vendor': ['axios']
        }
      }
    },
    // Increase chunk size warning limit (default is 500kb)
    chunkSizeWarningLimit: 600,
    // Enable source maps for production debugging (optional, can be disabled for smaller builds)
    sourcemap: false
  },
  // Optimize dependencies
  optimizeDeps: {
    include: ['vue', 'vue-router', 'axios']
  }
})
