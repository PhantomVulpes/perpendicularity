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

              <!-- View Mode -->
              <div v-if="!editMode" class="space-y-4">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <label class="block text-sm font-semibold text-gray-700 dark:text-gray-300 mb-2">First Name</label>
                    <div class="text-lg text-gray-900 dark:text-gray-100">{{ userProfile.firstName }}</div>
                  </div>
                  
                  <div>
                    <label class="block text-sm font-semibold text-gray-700 dark:text-gray-300 mb-2">Last Name</label>
                    <div class="text-lg text-gray-900 dark:text-gray-100">{{ userProfile.lastName }}</div>
                  </div>

                  <div>
                    <label class="block text-sm font-semibold text-gray-700 dark:text-gray-300 mb-2">Display Name</label>
                    <div class="text-lg text-gray-900 dark:text-gray-100">{{ userProfile.displayName }}</div>
                  </div>

                  <div>
                    <label class="block text-sm font-semibold text-gray-700 dark:text-gray-300 mb-2">Status</label>
                    <Tag 
                      :value="getUserStatusLabel(userProfile.status)"
                      :severity="getUserStatusSeverity(userProfile.status)"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-semibold text-gray-700 dark:text-gray-300 mb-2">Member Since</label>
                    <div class="text-lg text-gray-900 dark:text-gray-100">{{ formatDate(userProfile.creationDate) }}</div>
                  </div>

                  <div>
                    <label class="block text-sm font-semibold text-gray-700 dark:text-gray-300 mb-2">Last Login</label>
                    <div class="text-lg text-gray-900 dark:text-gray-100">{{ formatDate(userProfile.lastLoginDate) }}</div>
                  </div>
                </div>

                <!-- Edit Button -->
                <div v-if="canEdit" class="flex justify-end mt-6">
                  <Button
                    label="Edit Profile"
                    icon="pi pi-pencil"
                    @click="enterEditMode"
                    severity="primary"
                  />
                </div>
              </div>

              <!-- Edit Mode -->
              <div v-else class="space-y-4">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div class="flex flex-col gap-2">
                    <label for="firstName" class="font-semibold text-gray-700 dark:text-gray-300">First Name</label>
                    <InputText
                      id="firstName"
                      v-model="editForm.firstName"
                      placeholder="Enter first name"
                    />
                  </div>
                  
                  <div class="flex flex-col gap-2">
                    <label for="lastName" class="font-semibold text-gray-700 dark:text-gray-300">Last Name</label>
                    <InputText
                      id="lastName"
                      v-model="editForm.lastName"
                      placeholder="Enter last name"
                    />
                  </div>

                  <div class="flex flex-col gap-2 md:col-span-2">
                    <label for="password" class="font-semibold text-gray-700 dark:text-gray-300">New Password (optional)</label>
                    <Password
                      id="password"
                      v-model="editForm.password"
                      placeholder="Leave blank to keep current password"
                      toggleMask
                      :feedback="false"
                    />
                  </div>

                  <!-- Admin-only status change -->
                  <div v-if="isAdmin && !isViewingSelf" class="flex flex-col gap-2">
                    <label for="status" class="font-semibold text-gray-700 dark:text-gray-300">User Status</label>
                    <Dropdown
                      id="status"
                      v-model="editForm.status"
                      :options="userStatusOptions"
                      optionLabel="label"
                      optionValue="value"
                      placeholder="Select status"
                    />
                  </div>
                </div>

                <!-- Action Buttons -->
                <div class="flex justify-end gap-2 mt-6">
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
const editForm = ref({
  firstName: '',
  lastName: '',
  password: '',
  status: null as UserStatus | null
})

// Check if user is admin
const isAdmin = computed(() => {
  return isAuthenticated.value && user.value?.status === UserStatus._4
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
  { label: 'Unknown', value: UserStatus._0 },
  { label: 'Inactive', value: UserStatus._1 },
  { label: 'Awaiting Approval', value: UserStatus._2 },
  { label: 'Approved', value: UserStatus._3 },
  { label: 'Administrator', value: UserStatus._4 }
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
    case UserStatus._0: return 'Unknown'
    case UserStatus._1: return 'Inactive'
    case UserStatus._2: return 'Awaiting Approval'
    case UserStatus._3: return 'Approved'
    case UserStatus._4: return 'Administrator'
    default: return 'Unknown'
  }
}

// Get user status severity for Tag component
const getUserStatusSeverity = (status: UserStatus | undefined): 'secondary' | 'info' | 'warn' | 'success' | 'danger' => {
  switch (status) {
    case UserStatus._0: return 'secondary'
    case UserStatus._1: return 'danger'
    case UserStatus._2: return 'warn'
    case UserStatus._3: return 'success'
    case UserStatus._4: return 'info'
    default: return 'secondary'
  }
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
