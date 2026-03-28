<template>
  <div v-if="isAuthenticated" class="bg-primary text-white shadow-md">
    <div class="container mx-auto px-4 py-3 flex items-center justify-between">
      <div class="flex items-center gap-3">
        <i class="pi pi-user text-xl"></i>
        <div>
          <div class="font-semibold">{{ user?.displayName || user?.firstName }}</div>
          <div class="text-xs opacity-90">{{ getUserStatus() }}</div>
        </div>
      </div>
      
      <Button
        label="Sign Out"
        icon="pi pi-sign-out"
        @click="handleSignOut"
        severity="secondary"
        outlined
        size="small"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuth } from '@/services/auth'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import { UserStatus } from '@/api/apiclients/PerpendicularityApiClient'

const { user, isAuthenticated, signOut } = useAuth()
const router = useRouter()

const getUserStatus = () => {
  if (!user.value) return ''
  
  switch (user.value.status) {
    case UserStatus._0: return 'Unknown Status'
    case UserStatus._1: return 'Inactive'
    case UserStatus._2: return 'Awaiting Approval'
    case UserStatus._3: return 'Approved'
    case UserStatus._4: return 'Admin'
    default: return ''
  }
}

const handleSignOut = () => {
  signOut()
  router.push('/')
}
</script>
