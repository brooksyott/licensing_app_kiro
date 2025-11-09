import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import AppLayout from '@/components/layout/AppLayout.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: AppLayout,
    children: [
      {
        path: '',
        redirect: '/customers'
      },
      {
        path: '/customers',
        name: 'customers',
        component: () => import('@/views/CustomersView.vue')
      },
      {
        path: '/products',
        name: 'products',
        component: () => import('@/views/ProductsView.vue')
      },
      {
        path: '/skus',
        name: 'skus',
        component: () => import('@/views/SkusView.vue')
      },
      {
        path: '/licenses',
        name: 'licenses',
        component: () => import('@/views/LicensesView.vue')
      },
      {
        path: '/rsa-keys',
        name: 'rsa-keys',
        component: () => import('@/views/RsaKeysView.vue')
      },
      {
        path: '/api-keys',
        name: 'api-keys',
        component: () => import('@/views/ApiKeysView.vue')
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
