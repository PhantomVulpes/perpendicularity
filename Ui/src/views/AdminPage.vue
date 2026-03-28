<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-4xl mx-auto">
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
              <!-- Success Message -->
              <Message v-if="successMessage" severity="success" :closable="true" @close="successMessage = ''">
                {{ successMessage }}
              </Message>

              <!-- Error Message -->
              <Message v-if="errorMessage" severity="error" :closable="true" @close="errorMessage = ''">
                {{ errorMessage }}
              </Message>

              <!-- User Info -->
              <div class="mb-6 p-4 bg-blue-50 dark:bg-gray-700 rounded-lg">
                <div class="flex items-center gap-2 text-lg font-semibold mb-2">
                  <i class="pi pi-user text-blue-600 dark:text-blue-400"></i>
                  <span>Logged in as Administrator</span>
                </div>
                <p class="text-gray-700 dark:text-gray-300">
                  {{ fullName }}
                </p>
              </div>

              <!-- Application Settings Section -->
              <div class="border border-gray-200 dark:border-gray-700 rounded-lg p-6">
                <h3 class="text-xl font-semibold mb-3 flex items-center gap-2">
                  <i class="pi pi-cog"></i>
                  Application Settings
                </h3>
                
                <p class="text-gray-600 dark:text-gray-400 mb-4">
                  Initialize the global application settings. This action will create default settings for the application.
                </p>

                <div class="bg-amber-50 dark:bg-amber-900/20 border border-amber-200 dark:border-amber-800 rounded-lg p-4 mb-4">
                  <div class="flex items-start gap-2">
                    <i class="pi pi-exclamation-circle text-amber-600 dark:text-amber-400 mt-0.5"></i>
                    <div class="text-sm text-amber-800 dark:text-amber-300">
                      <span class="font-semibold">Warning:</span> This operation should only be performed once during initial setup.
                    </div>
                  </div>
                </div>

                <Button
                  label="Initialize Application Settings"
                  icon="pi pi-bolt"
                  @click="handleInitializeSettings"
                  :loading="loading"
                  severity="primary"
                  class="w-full sm:w-auto"
                />
              </div>
            </div>
          </template>
        </Card>

        <!-- Back to Home Button -->
        <Button
          label="Back to Home"
          icon="pi pi-arrow-left"
          @click="router.push('/')"
          severity="secondary"
          text
          class="w-full sm:w-auto"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import Card from 'primevue/card'
import Button from 'primevue/button'
import Message from 'primevue/message'
import { useAuth } from '@/services/auth'
import { UserStatus } from '@/api/apiclients/PerpendicularityApiClient'
import { initializeApplicationSettings } from '@/api/Admin/InitializeSettingsCommand'

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

// UI state
const loading = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

const handleInitializeSettings = async () => {
  loading.value = true
  successMessage.value = ''
  errorMessage.value = ''

  try {
    await initializeApplicationSettings()
    successMessage.value = 'Application settings initialized successfully!'
  } catch (error: any) {
    console.error('Failed to initialize settings:', error)
    
    // Extract error message from API response if available
    if (error.response) {
      const errorText = await error.response.text()
      errorMessage.value = errorText || 'Failed to initialize application settings. Please try again.'
    } else {
      errorMessage.value = error.message || 'Failed to initialize application settings. Please try again.'
    }
  } finally {
    loading.value = false
  }
}
</script>
