<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-6xl mx-auto">
        <!-- Admin Dashboard Card -->
        <Card class="mb-6">
          <template #title>
            <div class="flex items-center gap-2 text-2xl">
              <i class="pi pi-shield text-primary"></i>
              Admin Dashboard
            </div>
          </template>
          
          <template #content>
            <!-- Access Denied Message for Non-Admins -->
            <Message v-if="!isAdmin" severity="warn" :closable="false">
              <div class="flex items-center gap-2">
                <i class="pi pi-exclamation-triangle"></i>
                <span class="font-semibold">Access Denied</span>
              </div>
              <p class="mt-2">
                You must be an administrator to access this page.
              </p>
            </Message>

            <!-- Admin Content -->
            <div v-else>
              <p class="mb-4 text-gray-700 dark:text-gray-300">
                Logged in as {{ fullName }}
              </p>

              <ul class="space-y-3">
                <li>
                  <a @click="router.push('/admin/settings')" class="text-blue-600 dark:text-blue-400 hover:underline cursor-pointer">
                    Application Settings
                  </a>
                  <span class="text-gray-600 dark:text-gray-400"> - Initialize and manage global application settings</span>
                </li>
                <li>
                  <a @click="router.push('/admin/approve-users')" class="text-blue-600 dark:text-blue-400 hover:underline cursor-pointer">
                    Approve Users
                  </a>
                  <span class="text-gray-600 dark:text-gray-400"> - Review and approve user registrations</span>
                </li>
              </ul>
            </div>
          </template>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import Card from 'primevue/card'
import Message from 'primevue/message'
import { useAuth } from '@/services/auth'
import { UserStatus } from '@/api/apiclients/PerpendicularityApiClient'

const router = useRouter()
const { user, isAuthenticated } = useAuth()

// Check if user is admin
const isAdmin = computed(() => {
  return isAuthenticated.value && user.value?.status === UserStatus._4
})

const fullName = computed(() => {
  if (!user.value) return ''
  return `${user.value.firstName} ${user.value.lastName}`
})
</script>
