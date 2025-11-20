// src/router/index.js

import { createRouter, createWebHistory } from 'vue-router'

import LoginView from '@/views/LoginView.vue'
import RegisterView from '@/views/RegisterView.vue'
import DashboardView from '@/views/DashboardView.vue'

//定義路由規則
const routes = [
  {
    path: '/',
    name: 'Dashboard',
    component: DashboardView,
    meta: { requiresAuth: true }, // <-- 標記：這頁需要登入才能看
  },
  {
    path: '/login',
    name: 'Login',
    component: LoginView,
    meta: { guestOnly: true }, // <-- 標記：這頁只有「訪客」能看 (已登入的不用看)
  },
  {
    path: '/register',
    name: 'Register',
    component: RegisterView,
    meta: { guestOnly: true },
  },
]

//建立並匯出 Router
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: routes,
})

// --- 關鍵：路由守衛 (保全) ---
router.beforeEach((to, from, next) => {
  // 1. 檢查有沒有 Token
  const token = localStorage.getItem('jwt_token')
  const isAuthenticated = !!token // 轉成布林值 (true/false)

  // 2. 規則判斷
  if (to.meta.requiresAuth && !isAuthenticated) {
    // 如果要去「需要登入」的頁面，但「沒登入」 -> 踢去登入頁
    next('/login')
  } else if (to.meta.guestOnly && isAuthenticated) {
    // 如果要去「訪客頁 (登入/註冊)」，但「已經登入」 -> 踢回首頁
    next('/')
  } else {
    // 其他情況 -> 放行
    next()
  }
})
export default router
