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
    path: '/account/:userKey?',
    name: 'Account',
    component: () => import('../views/AccountPage.vue')
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
    path: '/admin/users',
    name: 'ApproveUsers',
    component: () => import('../views/ApproveUsersPage.vue')
  },
  {
    path: '/admin/external-projects',
    name: 'ExternalProjects',
    component: () => import('../views/ExternalProjectsPage.vue')
  },
  {
    path: '/roadmap',
    name: 'Roadmap',
    component: () => import('../views/RoadmapPage.vue')
  },
  {
    path: '/browse',
    name: 'Browse',
    component: () => import('../views/BrowsePage.vue')
  },
  {
    path: '/browse/:rootDirectory/:pathMatch(.*)*',
    name: 'BrowsePath',
    component: () => import('../views/BrowsePathPage.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
