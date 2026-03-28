import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import RegisterPage from '../views/RegisterPage.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Home',
    component: () => import('../views/HomePage.vue')
  },
  {
    path: '/register',
    name: 'Register',
    component: RegisterPage
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
