<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-6xl mx-auto">
        <!-- Approve Users Card -->
        <Card class="mb-6">
          <template #title>
            <div class="flex items-center gap-2 text-2xl">
              <i class="pi pi-users text-primary"></i>
              Approve Users
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
              <div v-if="loading" class="flex justify-center items-center py-8">
                <i class="pi pi-spinner pi-spin text-4xl text-primary"></i>
              </div>

              <!-- Users Table -->
              <div v-else-if="users.length > 0" class="overflow-x-auto">
                <DataTable 
                  :value="users" 
                  stripedRows
                  class="p-datatable-sm"
                >
                  <Column header="" style="width: 3rem">
                    <template #body="slotProps">
                      <Button
                        icon="pi pi-key"
                        v-tooltip.top="slotProps.data.key || 'No key'"
                        @click="copyToClipboard(slotProps.data.key)"
                        text
                        rounded
                        size="small"
                        severity="secondary"
                        class="hover:bg-gray-100 dark:hover:bg-gray-700"
                        aria-label="Copy user key"
                      />
                    </template>
                  </Column>
                  <Column field="firstName" header="First Name" sortable></Column>
                  <Column field="lastName" header="Last Name" sortable></Column>
                  <Column field="status" header="Status" sortable>
                    <template #body="slotProps">
                      <Tag 
                        :value="getUserStatusLabel(slotProps.data.status)"
                        :severity="getUserStatusSeverity(slotProps.data.status)"
                      />
                    </template>
                  </Column>
                  <Column field="creationDate" header="Registered" sortable>
                    <template #body="slotProps">
                      {{ formatDate(slotProps.data.creationDate) }}
                    </template>
                  </Column>
                  <Column field="lastLoginDate" header="Last Login" sortable>
                    <template #body="slotProps">
                      {{ formatDate(slotProps.data.lastLoginDate) }}
                    </template>
                  </Column>
                  <Column header="Actions">
                    <template #body="slotProps">
                      <Button
                        v-if="slotProps.data.status === UserStatus._2"
                        label="Approve"
                        icon="pi pi-check"
                        @click="handleApproveUser(slotProps.data)"
                        :loading="approvingUserId === slotProps.data.key"
                        size="small"
                        severity="success"
                        outlined
                      />
                      <span v-else class="text-gray-500 dark:text-gray-400">-</span>
                    </template>
                  </Column>
                </DataTable>
              </div>

              <!-- No Users Message -->
              <div v-else class="text-center py-8 text-gray-500 dark:text-gray-400">
                <i class="pi pi-users text-4xl mb-4"></i>
                <p>No users found in the database.</p>
              </div>

              <!-- Refresh Button -->
              <div class="mt-4 flex justify-end">
                <Button
                  label="Refresh"
                  icon="pi pi-refresh"
                  @click="loadUsers"
                  :loading="loading"
                  severity="secondary"
                  outlined
                />
              </div>
            </div>
          </template>
        </Card>

        <!-- Back Buttons -->
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
import { useToast } from 'primevue/usetoast'
import Card from 'primevue/card'
import Button from 'primevue/button'
import Message from 'primevue/message'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Tag from 'primevue/tag'
import { useAuth } from '@/services/auth'
import { UserStatus, RegisteredUser } from '@/api/apiclients/PerpendicularityApiClient'
import { getAllUsers } from '@/api/User/GetAllUsersQuery'
import { approveUser } from '@/api/Admin/ApproveUserCommand'

const router = useRouter()
const toast = useToast()
const { user, isAuthenticated } = useAuth()

// Check if user is admin
const isAdmin = computed(() => {
  return isAuthenticated.value && user.value?.status === UserStatus._4
})

// State
const loading = ref(false)
const users = ref<RegisteredUser[]>([])
const successMessage = ref('')
const errorMessage = ref('')
const approvingUserId = ref<string | null>(null)

// Load users on mount
onMounted(() => {
  if (isAdmin.value) {
    loadUsers()
  }
})

// Load all users
const loadUsers = async () => {
  loading.value = true
  errorMessage.value = ''
  
  try {
    users.value = await getAllUsers()
  } catch (error: any) {
    console.error('Failed to load users:', error)
    errorMessage.value = error.message || 'Failed to load users. Please try again.'
  } finally {
    loading.value = false
  }
}

// Approve a user
const handleApproveUser = async (userToApprove: RegisteredUser) => {
  if (!userToApprove.key) return
  
  approvingUserId.value = userToApprove.key
  successMessage.value = ''
  errorMessage.value = ''
  
  try {
    await approveUser(userToApprove.key)
    successMessage.value = `Successfully approved ${userToApprove.displayName}!`
    
    // Reload users to reflect the change
    await loadUsers()
  } catch (error: any) {
    console.error('Failed to approve user:', error)
    errorMessage.value = error.message || `Failed to approve ${userToApprove.displayName}. Please try again.`
  } finally {
    approvingUserId.value = null
  }
}

// Copy to clipboard
const copyToClipboard = async (text?: string) => {
  if (!text) {
    toast.add({
      severity: 'warn',
      summary: 'No Key',
      detail: 'This user has no key to copy.',
      life: 3000
    })
    return
  }
  
  try {
    await navigator.clipboard.writeText(text)
    toast.add({
      severity: 'success',
      summary: 'Copied!',
      detail: 'User key copied to clipboard.',
      life: 2000
    })
  } catch (error) {
    console.error('Failed to copy to clipboard:', error)
    toast.add({
      severity: 'error',
      summary: 'Failed to Copy',
      detail: 'Could not copy to clipboard. Please try again.',
      life: 3000
    })
  }
}

// Get user status label
const getUserStatusLabel = (status?: UserStatus) => {
  switch (status) {
    case UserStatus._0: return 'Unknown'
    case UserStatus._1: return 'Inactive'
    case UserStatus._2: return 'Unapproved'
    case UserStatus._3: return 'Approved'
    case UserStatus._4: return 'Admin'
    default: return 'Unknown'
  }
}

// Get user status severity for Tag component
const getUserStatusSeverity = (status?: UserStatus) => {
  switch (status) {
    case UserStatus._0: return 'secondary'
    case UserStatus._1: return 'danger'
    case UserStatus._2: return 'warning'
    case UserStatus._3: return 'success'
    case UserStatus._4: return 'info'
    default: return 'secondary'
  }
}

// Format date
const formatDate = (date?: Date) => {
  if (!date) return 'Never'
  return new Date(date).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>
