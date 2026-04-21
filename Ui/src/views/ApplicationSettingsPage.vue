<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-4xl mx-auto">
        <!-- Application Settings Card -->
        <Card class="mb-6">
          <template #title>
            <div class="flex items-center justify-between text-2xl">
              <div class="flex items-center gap-2">
                <i class="pi pi-cog text-primary"></i>
                Application Settings
              </div>
              <Button
                v-if="isAdmin"
                label="Initialize"
                icon="pi pi-bolt"
                @click="handleInitializeSettings"
                :loading="initLoading"
                severity="secondary"
                size="small"
                outlined
                class="text-sm"
              />
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

              <!-- Loading State -->
              <div v-if="loading" class="text-center py-8">
                <i class="pi pi-spin pi-spinner text-4xl text-primary"></i>
                <p class="mt-4 text-gray-600 dark:text-gray-400">Loading settings...</p>
              </div>

              <!-- No Settings State -->
              <div v-else-if="!currentSettings" class="text-center py-8 bg-gray-50 dark:bg-gray-800 rounded-lg">
                <i class="pi pi-inbox text-4xl mb-2 text-gray-400"></i>
                <p class="text-gray-600 dark:text-gray-400 mb-4">
                  No application settings found. Please initialize the settings first.
                </p>
              </div>

              <!-- Settings Display/Edit -->
              <div v-else>
                <!-- Settings Info -->
                <div class="mb-6 p-4 bg-gray-50 dark:bg-gray-800 rounded-lg">
                  <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <div>
                      <label class="block text-sm font-medium text-gray-600 dark:text-gray-400 mb-1">
                        Settings Key
                      </label>
                      <p class="font-mono text-sm">{{ currentSettings.key }}</p>
                    </div>
                    <div v-if="currentSettings.editingToken">
                      <label class="block text-sm font-medium text-gray-600 dark:text-gray-400 mb-1">
                        Editing Token
                      </label>
                      <p class="font-mono text-sm">{{ currentSettings.editingToken }}</p>
                    </div>
                  </div>
                </div>

                <!-- Directory Configurations Section -->
                <div class="border border-gray-200 dark:border-gray-700 rounded-lg p-6">
                  <div class="flex items-center justify-between mb-4">
                    <h3 class="text-xl font-semibold flex items-center gap-2">
                      <i class="pi pi-folder-open"></i>
                      Download Paths
                    </h3>
                    <Button
                      v-if="!isEditMode"
                      label="Edit"
                      icon="pi pi-pencil"
                      @click="enterEditMode"
                      severity="primary"
                      outlined
                    />
                    <div v-else class="flex gap-2">
                      <Button
                        label="Cancel"
                        icon="pi pi-times"
                        @click="cancelEdit"
                        severity="secondary"
                        outlined
                      />
                      <Button
                        label="Save"
                        icon="pi pi-save"
                        @click="handleSaveSettings"
                        :loading="saveLoading"
                        severity="success"
                      />
                    </div>
                  </div>

                  <!-- Read-only View -->
                  <div v-if="!isEditMode">
                    <div v-if="currentSettings.downloadPaths && currentSettings.downloadPaths.length > 0" class="space-y-3">
                      <div 
                        v-for="(config, index) in currentSettings.downloadPaths" 
                        :key="index"
                        class="p-4 bg-gray-50 dark:bg-gray-800 rounded-lg"
                      >
                        <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
                          <div>
                            <label class="block text-xs font-medium text-gray-600 dark:text-gray-400 mb-1">
                              Path
                            </label>
                            <p class="font-mono text-sm">{{ config.path }}</p>
                          </div>
                          <div>
                            <label class="block text-xs font-medium text-gray-600 dark:text-gray-400 mb-1">
                              Alias
                            </label>
                            <p class="text-sm">{{ config.alias }}</p>
                          </div>
                        </div>
                      </div>
                    </div>
                    <div v-else class="text-center py-8 text-gray-500 dark:text-gray-400 bg-gray-50 dark:bg-gray-800 rounded-lg">
                      <i class="pi pi-inbox text-4xl mb-2"></i>
                      <p>No download paths configured.</p>
                    </div>
                  </div>

                  <!-- Edit Mode -->
                  <div v-else>
                    <div class="space-y-4 mb-4">
                      <div 
                        v-for="(config, index) in editableConfigurations" 
                        :key="index" 
                        class="flex flex-col sm:flex-row gap-3 p-4 bg-gray-50 dark:bg-gray-800 rounded-lg"
                      >
                        <div class="flex-1">
                          <label :for="`edit-path-${index}`" class="block text-sm font-medium mb-1">
                            Path
                          </label>
                          <InputText
                            :id="`edit-path-${index}`"
                            v-model="config.path"
                            placeholder="/path/to/directory"
                            class="w-full"
                          />
                        </div>
                        <div class="flex-1">
                          <label :for="`edit-alias-${index}`" class="block text-sm font-medium mb-1">
                            Alias
                          </label>
                          <InputText
                            :id="`edit-alias-${index}`"
                            v-model="config.alias"
                            placeholder="My Downloads"
                            class="w-full"
                          />
                        </div>
                        <div class="flex items-end">
                          <Button
                            icon="pi pi-trash"
                            @click="removeDirectoryConfig(index)"
                            severity="danger"
                            outlined
                            class="w-full sm:w-auto"
                          />
                        </div>
                      </div>

                      <!-- Empty State in Edit Mode -->
                      <div 
                        v-if="editableConfigurations.length === 0"
                        class="text-center py-8 text-gray-500 dark:text-gray-400 bg-gray-50 dark:bg-gray-800 rounded-lg"
                      >
                        <i class="pi pi-inbox text-4xl mb-2"></i>
                        <p>No directory configurations. Click "Add Directory" to get started.</p>
                      </div>
                    </div>

                    <!-- Add Directory Button -->
                    <Button
                      label="Add Directory"
                      icon="pi pi-plus"
                      @click="addDirectoryConfig"
                      severity="secondary"
                      outlined
                    />
                  </div>
                </div>
              </div>
            </div>
          </template>
        </Card>

        <!-- Back Button -->
        <div class="flex gap-2">
          <Button
            label="Back to Admin Dashboard"
            icon="pi pi-arrow-left"
            @click="router.push('/admin')"
            severity="secondary"
            text
            class="w-full sm:w-auto"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Card from 'primevue/card'
import Button from 'primevue/button'
import Message from 'primevue/message'
import InputText from 'primevue/inputtext'
import { useAuth } from '@/services/auth'
import { UserStatus, ApplicationSettings, DirectoryConfiguration } from '@/api/apiclients/Perpendicularity/PerpendicularityApiClient'
import { initializeApplicationSettings } from '@/api/Admin/InitializeSettingsCommand'
import { editApplicationSettings } from '@/api/Admin/EditApplicationSettingsCommand'
import { getApplicationSettings } from '@/api/Admin/GetApplicationSettingsQuery'

const router = useRouter()
const { user, isAuthenticated } = useAuth()

// Check if user is admin
const isAdmin = computed(() => {
  return isAuthenticated.value && user.value?.status === UserStatus._4
})

// UI state
const loading = ref(false)
const initLoading = ref(false)
const saveLoading = ref(false)
const successMessage = ref('')
const errorMessage = ref('')
const isEditMode = ref(false)

// Settings data
const currentSettings = ref<ApplicationSettings | null>(null)
const editableConfigurations = ref<Array<{ path: string; alias: string }>>([])

// Load settings on mount
onMounted(async () => {
  if (isAdmin.value) {
    await loadSettings()
  }
})

const loadSettings = async () => {
  loading.value = true
  errorMessage.value = ''

  try {
    currentSettings.value = await getApplicationSettings()
  } catch (error: any) {
    console.error('Failed to load settings:', error)
    
    // If 404 or similar, settings don't exist yet
    if (error.status === 404) {
      currentSettings.value = null
    } else {
      if (error.response) {
        const errorText = await error.response.text()
        errorMessage.value = errorText || 'Failed to load application settings.'
      } else {
        errorMessage.value = error.message || 'Failed to load application settings.'
      }
    }
  } finally {
    loading.value = false
  }
}

const enterEditMode = () => {
  isEditMode.value = true
  // Create a copy of current download paths for editing
  if (currentSettings.value?.downloadPaths) {
    editableConfigurations.value = currentSettings.value.downloadPaths.map(config => ({
      path: config.path || '',
      alias: config.alias || ''
    }))
  } else {
    editableConfigurations.value = []
  }
}

const cancelEdit = () => {
  isEditMode.value = false
  editableConfigurations.value = []
  successMessage.value = ''
  errorMessage.value = ''
}

const addDirectoryConfig = () => {
  editableConfigurations.value.push({ path: '', alias: '' })
}

const removeDirectoryConfig = (index: number) => {
  editableConfigurations.value.splice(index, 1)
}

const handleInitializeSettings = async () => {
  initLoading.value = true
  successMessage.value = ''
  errorMessage.value = ''

  try {
    await initializeApplicationSettings()
    successMessage.value = 'Application settings initialized successfully!'
    // Reload settings after initialization
    await loadSettings()
  } catch (error: any) {
    console.error('Failed to initialize settings:', error)
    
    if (error.response) {
      const errorText = await error.response.text()
      errorMessage.value = errorText || 'Failed to initialize application settings. Please try again.'
    } else {
      errorMessage.value = error.message || 'Failed to initialize application settings. Please try again.'
    }
  } finally {
    initLoading.value = false
  }
}

const handleSaveSettings = async () => {
  saveLoading.value = true
  successMessage.value = ''
  errorMessage.value = ''

  try {
    // Validate configurations
    const hasEmptyFields = editableConfigurations.value.some(
      config => !config.path.trim() || !config.alias.trim()
    )
    
    if (hasEmptyFields) {
      errorMessage.value = 'All directory configurations must have both path and alias filled in.'
      saveLoading.value = false
      return
    }

    // Convert to DirectoryConfiguration objects
    const configs = editableConfigurations.value.map(
      config => new DirectoryConfiguration({ 
        path: config.path.trim(), 
        alias: config.alias.trim() 
      })
    )

    await editApplicationSettings(configs)
    successMessage.value = 'Application settings saved successfully!'
    
    // Exit edit mode and reload settings
    isEditMode.value = false
    await loadSettings()
  } catch (error: any) {
    console.error('Failed to save settings:', error)
    
    if (error.response) {
      const errorText = await error.response.text()
      errorMessage.value = errorText || 'Failed to save application settings. Please try again.'
    } else {
      errorMessage.value = error.message || 'Failed to save application settings. Please try again.'
    }
  } finally {
    saveLoading.value = false
  }
}
</script>
