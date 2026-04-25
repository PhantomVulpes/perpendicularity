<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-4xl mx-auto">
        <!-- Account Card -->
        <Card class="mb-6">
          <template #title>
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-2 text-2xl">
                <i class="pi pi-user text-primary"></i>
                Account Information
              </div>
            </div>
          </template>
          
          <template #content>
            <!-- Loading State -->
            <div v-if="loading" class="flex justify-center items-center py-8">
              <i class="pi pi-spinner pi-spin text-4xl text-primary"></i>
            </div>

            <!-- Error Message -->
            <Message v-else-if="errorMessage" severity="error" :closable="true" @close="errorMessage = ''">
              {{ errorMessage }}
            </Message>

            <!-- User Information -->
            <div v-else-if="userProfile" class="space-y-6">
              <!-- Success Message -->
              <Message v-if="successMessage" severity="success" :closable="true" @close="successMessage = ''">
                {{ successMessage }}
              </Message>

              <!-- Unified View/Edit Mode -->
              <div class="space-y-4">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <!-- First Name -->
                  <div class="flex flex-col gap-2">
                    <label for="firstName" class="text-sm font-semibold text-gray-700 dark:text-gray-300">First Name</label>
                    <InputText
                      v-if="editMode"
                      id="firstName"
                      v-model="editForm.firstName"
                      placeholder="Enter first name"
                    />
                    <div v-else class="text-lg text-gray-900 dark:text-gray-100">{{ userProfile.firstName }}</div>
                  </div>
                  
                  <!-- Last Name -->
                  <div class="flex flex-col gap-2">
                    <label for="lastName" class="text-sm font-semibold text-gray-700 dark:text-gray-300">Last Name</label>
                    <InputText
                      v-if="editMode"
                      id="lastName"
                      v-model="editForm.lastName"
                      placeholder="Enter last name"
                    />
                    <div v-else class="text-lg text-gray-900 dark:text-gray-100">{{ userProfile.lastName }}</div>
                  </div>

                  <!-- Password -->
                  <div class="flex flex-col gap-2">
                    <label for="password" class="text-sm font-semibold text-gray-700 dark:text-gray-300">
                      {{ editMode ? 'Change Password' : 'Password' }}
                    </label>
                    <Password
                      v-if="editMode"
                      id="password"
                      v-model="editForm.password"
                      placeholder="Leave blank to keep current password"
                      toggleMask
                      :feedback="false"
                    />
                    <div v-else class="text-lg text-gray-900 dark:text-gray-100">[Redacted]</div>
                  </div>

                  <!-- Display Name -->
                  <div class="flex flex-col gap-2">
                    <label class="text-sm font-semibold text-gray-700 dark:text-gray-300">Display Name (Calculated value)</label>
                    <div class="text-lg text-gray-900 dark:text-gray-100">{{ userProfile.displayName }}</div>
                  </div>

                  <!-- Status -->
                  <div class="flex flex-col gap-2">
                    <label for="status" class="text-sm font-semibold text-gray-700 dark:text-gray-300">Status</label>
                    <Dropdown
                      v-if="editMode && isAdmin && !isViewingSelf"
                      id="status"
                      v-model="editForm.status"
                      :options="userStatusOptions"
                      optionLabel="label"
                      optionValue="value"
                      placeholder="Select status"
                    />
                    <Tag 
                      v-else
                      :value="getUserStatusLabel(userProfile.status)"
                      :severity="getUserStatusSeverity(userProfile.status)"
                    />
                  </div>

                  <!-- Member Since -->
                  <div class="flex flex-col gap-2">
                    <label class="text-sm font-semibold text-gray-700 dark:text-gray-300">Member Since</label>
                    <div class="text-lg text-gray-900 dark:text-gray-100">{{ formatDate(userProfile.creationDate) }}</div>
                  </div>

                  <!-- Last Login -->
                  <div class="flex flex-col gap-2">
                    <label class="text-sm font-semibold text-gray-700 dark:text-gray-300">Last Login</label>
                    <div class="text-lg text-gray-900 dark:text-gray-100">{{ formatDate(userProfile.lastLoginDate) }}</div>
                  </div>
                </div>

                <!-- Action Buttons -->
                <div v-if="canEdit" class="flex justify-end gap-2 mt-6">
                  <template v-if="editMode">
                    <Button
                      label="Cancel"
                      icon="pi pi-times"
                      @click="cancelEdit"
                      severity="secondary"
                      outlined
                    />
                    <Button
                      label="Save Changes"
                      icon="pi pi-check"
                      @click="saveChanges"
                      :loading="saving"
                      severity="success"
                    />
                  </template>
                  <Button
                    v-else
                    label="Edit Profile or Change Password"
                    icon="pi pi-pencil"
                    @click="enterEditMode"
                    severity="primary"
                  />
                </div>

                <!-- Download Metrics Section -->
                <div class="mt-8 pt-6 border-t border-gray-200 dark:border-gray-700">
                  <div class="flex items-center justify-between mb-4">
                    <h3 class="text-xl font-semibold flex items-center gap-2">
                      <i class="pi pi-download text-primary"></i>
                      Download History
                    </h3>
                    <Button
                      :icon="showDownloads ? 'pi pi-chevron-up' : 'pi pi-chevron-down'"
                      @click="showDownloads = !showDownloads"
                      text
                      rounded
                      severity="secondary"
                      v-tooltip.top="showDownloads ? 'Collapse' : 'Expand'"
                    />
                  </div>

                  <div v-if="showDownloads">
                    <div v-if="userProfile.downloadMetrics && userProfile.downloadMetrics.length > 0">
                      <DataTable 
                        :value="userProfile.downloadMetrics" 
                        class="p-datatable-sm"
                        stripedRows
                        :paginator="userProfile.downloadMetrics.length > 10"
                        :rows="10"
                        paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport"
                        currentPageReportTemplate="Showing {first} to {last} of {totalRecords} downloads"
                      >
                        <Column field="path" header="Path" sortable>
                          <template #body="slotProps">
                            <code class="text-sm">{{ slotProps.data.path }}</code>
                          </template>
                        </Column>
                        <Column field="sizeBytes" header="Size" sortable>
                          <template #body="slotProps">
                            {{ formatBytes(slotProps.data.sizeBytes) }}
                          </template>
                        </Column>
                        <Column field="downloadDate" header="Download Date" sortable>
                          <template #body="slotProps">
                            {{ formatDate(slotProps.data.downloadDate) }}
                          </template>
                        </Column>
                      </DataTable>
                    </div>
                    <div v-else class="text-center py-8 text-gray-500 dark:text-gray-400">
                      <i class="pi pi-inbox text-4xl mb-4"></i>
                      <p>No downloads recorded yet.</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </template>
        </Card>

        <!-- Back Button -->
        <div class="flex gap-2">
          <Button
            label="Back"
            icon="pi pi-arrow-left"
            @click="goBack"
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
import { useRouter, useRoute } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import Card from 'primevue/card'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Dropdown from 'primevue/dropdown'
import Message from 'primevue/message'
import Tag from 'primevue/tag'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import { useAuth } from '@/services/auth'
import { UserStatus, RegisteredUser } from '@/api/apiclients/Perpendicularity/PerpendicularityApiClient'
import { getUserByKey } from '@/api/User/GetUserByKeyQuery'
import { editUser } from '@/api/User/EditUserCommand'

const router = useRouter()
const route = useRoute()
const toast = useToast()
const { user, isAuthenticated } = useAuth()

// State
const loading = ref(false)
const saving = ref(false)
const userProfile = ref<RegisteredUser | null>(null)
const errorMessage = ref('')
const successMessage = ref('')
const editMode = ref(false)
const showDownloads = ref(false)
const editForm = ref({
  firstName: '',
  lastName: '',
  password: '',
  status: null as UserStatus | null
})

// Check if user is admin
const isAdmin = computed(() => {
  return isAuthenticated.value && user.value?.status === UserStatus.Admin
})

// Check if viewing own profile
const isViewingSelf = computed(() => {
  return user.value?.key === userProfile.value?.key
})

// Can edit if viewing self or is admin
const canEdit = computed(() => {
  return isViewingSelf.value || isAdmin.value
})

// User status options for dropdown
const userStatusOptions = [
  { label: 'Unknown', value: UserStatus.Unknown },
  { label: 'Inactive', value: UserStatus.Inactive },
  { label: 'Awaiting Approval', value: UserStatus.Unapproved },
  { label: 'Approved', value: UserStatus.Approved },
  { label: 'Rejected', value: UserStatus.Rejected },
  { label: 'Administrator', value: UserStatus.Admin }
]

// Get user key from route or use current user
const getUserKey = (): string | null => {
  const routeKey = route.params.userKey as string | undefined
  if (routeKey) {
    return routeKey
  }
  return user.value?.key || null
}

// Load user profile
const loadUserProfile = async () => {
  loading.value = true
  errorMessage.value = ''
  
  try {
    const userKey = getUserKey()
    if (!userKey) {
      errorMessage.value = 'No user key provided'
      return
    }

    userProfile.value = await getUserByKey(userKey)
  } catch (error: any) {
    console.error('Failed to load user profile:', error)
    errorMessage.value = error.message || 'Failed to load user profile. Please try again.'
  } finally {
    loading.value = false
  }
}

// Enter edit mode
const enterEditMode = () => {
  if (!userProfile.value) return
  
  editForm.value = {
    firstName: userProfile.value.firstName || '',
    lastName: userProfile.value.lastName || '',
    password: '',
    status: userProfile.value.status || null
  }
  editMode.value = true
}

// Cancel edit mode
const cancelEdit = () => {
  editMode.value = false
  successMessage.value = ''
  errorMessage.value = ''
}

// Save changes
const saveChanges = async () => {
  if (!userProfile.value?.key) return
  
  saving.value = true
  errorMessage.value = ''
  successMessage.value = ''
  
  try {
    await editUser({
      userToEditKey: userProfile.value.key,
      firstName: editForm.value.firstName || undefined,
      lastName: editForm.value.lastName || undefined,
      passwordRaw: editForm.value.password || undefined,
      status: (isAdmin.value && !isViewingSelf.value) ? editForm.value.status || undefined : undefined
    })
    
    successMessage.value = 'Profile updated successfully!'
    editMode.value = false
    
    // Reload profile
    await loadUserProfile()
    
    toast.add({
      severity: 'success',
      summary: 'Success',
      detail: 'Profile updated successfully!',
      life: 3000
    })
  } catch (error: any) {
    console.error('Failed to save changes:', error)
    errorMessage.value = error.message || 'Failed to save changes. Please try again.'
    
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: errorMessage.value,
      life: 5000
    })
  } finally {
    saving.value = false
  }
}

// Format date
const formatDate = (date: Date | undefined): string => {
  if (!date) return 'N/A'
  
  try {
    if (date.getTime() === 0 || date.getFullYear() < 1900) {
      return 'Never'
    }
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  } catch {
    return 'Invalid Date'
  }
}

// Get user status label
const getUserStatusLabel = (status: UserStatus | undefined): string => {
  switch (status) {
    case UserStatus.Unknown: return 'Unknown'
    case UserStatus.Inactive: return 'Inactive'
    case UserStatus.Unapproved: return 'Awaiting Approval'
    case UserStatus.Approved: return 'Approved'
    case UserStatus.Rejected: return 'Rejected'
    case UserStatus.Admin: return 'Administrator'
    default: return 'Unknown'
  }
}

// Get user status severity for Tag component
const getUserStatusSeverity = (status: UserStatus | undefined): 'secondary' | 'info' | 'warn' | 'success' | 'danger' => {
  switch (status) {
    case UserStatus.Unknown: return 'secondary'
    case UserStatus.Inactive: return 'danger'
    case UserStatus.Unapproved: return 'warn'
    case UserStatus.Approved: return 'success'
    case UserStatus.Rejected: return 'danger'
    case UserStatus.Admin: return 'info'
    default: return 'secondary'
  }
}

// Format bytes to human-readable size
const formatBytes = (bytes?: number) => {
  if (!bytes || bytes === 0) return '0 Bytes'
  
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  
  return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i]
}

// Go back to previous page
const goBack = () => {
  router.back()
}

// Load on mount
onMounted(() => {
  loadUserProfile()
})
</script>

<style scoped>
</style>
