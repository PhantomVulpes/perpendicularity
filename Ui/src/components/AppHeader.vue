<template>
  <div class="bg-white border-b border-gray-200 shadow-sm">
    <div class="container mx-auto px-4 py-3">
      <div class="flex items-center justify-between">
        <!-- Left: App Title -->
        <RouterLink to="/" class="text-lg font-semibold text-gray-800 hover:text-blue-600 transition-colors cursor-pointer">
          Shadesmar's Perpendicularity
        </RouterLink>
        
        <!-- Right: Auth Actions or User Info -->
        <div v-if="isAuthenticated" class="flex items-center gap-3">
          <div class="text-right">
            <div class="font-semibold text-gray-800">{{ fullName }}</div>
            <div class="flex items-center justify-end gap-1 text-xs font-medium" :class="statusClasses">
              <i :class="statusIcon"></i>
              <span>{{ statusText }}</span>
            </div>
          </div>
          
          <!-- Admin Button (only shown for administrators) -->
          <Button
            v-if="isAdmin"
            label="Admin"
            icon="pi pi-shield"
            @click="router.push('/admin')"
            severity="info"
            outlined
            size="small"
            aria-label="Admin Dashboard"
          />
          
          <Button
            icon="pi pi-sign-out"
            @click="handleSignOut"
            severity="secondary"
            outlined
            size="small"
            aria-label="Sign Out"
          />
        </div>
        
        <div v-else class="flex gap-2">
          <Button
            label="Sign In"
            icon="pi pi-sign-in"
            @click="router.push('/login')"
            severity="primary"
            size="small"
          />
          <Button
            label="Register"
            icon="pi pi-user-plus"
            @click="router.push('/register')"
            severity="secondary"
            outlined
            size="small"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAuth } from '@/services/auth'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import { UserStatus } from '@/api/apiclients/PerpendicularityApiClient'

const { user, isAuthenticated, signOut } = useAuth()
const router = useRouter()

const isAdmin = computed(() => {
  return isAuthenticated.value && user.value?.status === UserStatus._4
})

const fullName = computed(() => {
  if (!user.value) return ''
  return `${user.value.firstName} ${user.value.lastName}`
})

const statusText = computed(() => {
  if (!user.value) return ''
  
  switch (user.value.status) {
    case UserStatus._0: return 'Unknown Status'
    case UserStatus._1: return 'Inactive'
    case UserStatus._2: return 'Awaiting Approval'
    case UserStatus._3: return 'Approved'
    case UserStatus._4: return 'Administrator'
    default: return ''
  }
})

const statusIcon = computed(() => {
  if (!user.value) return ''
  
  switch (user.value.status) {
    case UserStatus._0: return 'pi pi-question-circle'
    case UserStatus._1: return 'pi pi-times-circle'
    case UserStatus._2: return 'pi pi-clock'
    case UserStatus._3: return 'pi pi-check-circle'
    case UserStatus._4: return 'pi pi-shield'
    default: return ''
  }
})

const statusClasses = computed(() => {
  if (!user.value) return ''
  
  switch (user.value.status) {
    case UserStatus._0: return 'text-gray-500'
    case UserStatus._1: return 'text-red-600'
    case UserStatus._2: return 'text-amber-600'
    case UserStatus._3: return 'text-green-600'
    case UserStatus._4: return 'text-blue-600'
    default: return ''
  }
})

const handleSignOut = () => {
  signOut()
  router.push('/')
}
</script>
