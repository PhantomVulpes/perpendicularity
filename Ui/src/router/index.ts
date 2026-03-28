import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import RegisterPage from '../views/RegisterPage.vue'
import LoginPage from '../views/LoginPage.vue'

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
  },
  {
    path: '/login',
    name: 'Login',
    component: LoginPage
  },
  {
    path: '/admin',
    name: 'Admin',
    component: () => import('../views/AdminPage.vue')
  },
  {
    path: '/admin/settings',
    name: 'ApplicationSettings',
    component: () => import('../views/ApplicationSettingsPage.vue')
  },
  {
    path: '/admin/approve-users',
    name: 'ApproveUsers',
    component: () => import('../views/ApproveUsersPage.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
