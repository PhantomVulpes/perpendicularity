<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-6xl mx-auto">
        <!-- External Projects Card -->
        <Card class="mb-6">
          <template #title>
            <div class="flex items-center gap-2 text-2xl">
              <i class="pi pi-link text-primary"></i>
              Manage External Projects
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

              <!-- Add New Project Form -->
              <div class="mb-8 p-4 bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700">
                <h3 class="text-lg font-semibold mb-4 flex items-center gap-2">
                  <i class="pi pi-plus-circle"></i>
                  Add New External Project
                </h3>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div class="flex flex-col gap-2">
                    <label for="projectName" class="font-medium text-sm">Project Name</label>
                    <InputText
                      id="projectName"
                      v-model="newProject.name"
                      placeholder="Enter project name"
                      :disabled="adding"
                    />
                  </div>
                  <div class="flex flex-col gap-2">
                    <label for="projectUri" class="font-medium text-sm">Project URI</label>
                    <InputText
                      id="projectUri"
                      v-model="newProject.uri"
                      placeholder="https://example.com"
                      :disabled="adding"
                    />
                  </div>
                  <div class="flex flex-col gap-2 md:col-span-2">
                    <label for="tooltip" class="font-medium text-sm">Tooltip/Description</label>
                    <Textarea
                      id="tooltip"
                      v-model="newProject.tooltip"
                      placeholder="Brief description of the project"
                      rows="2"
                      :disabled="adding"
                    />
                  </div>
                </div>
                <div class="mt-4 flex justify-end">
                  <Button
                    label="Add Project"
                    icon="pi pi-plus"
                    @click="handleAddProject"
                    :loading="adding"
                    :disabled="!isFormValid"
                  />
                </div>
              </div>

              <!-- Loading State -->
              <div v-if="loading" class="flex justify-center items-center py-8">
                <i class="pi pi-spinner pi-spin text-4xl text-primary"></i>
              </div>

              <!-- Projects Table -->
              <div v-else-if="projects.length > 0" class="overflow-x-auto">
                <DataTable 
                  :value="projects" 
                  stripedRows
                  class="p-datatable-sm"
                >
                  <Column field="projectName" header="Project Name" sortable></Column>
                  <Column field="projectUri" header="URI" sortable>
                    <template #body="slotProps">
                      <a 
                        :href="slotProps.data.projectUri" 
                        target="_blank" 
                        rel="noopener noreferrer"
                        class="text-blue-600 dark:text-blue-400 hover:underline flex items-center gap-1"
                      >
                        {{ slotProps.data.projectUri }}
                        <i class="pi pi-external-link text-xs"></i>
                      </a>
                    </template>
                  </Column>
                  <Column field="tooltip" header="Description">
                    <template #body="slotProps">
                      <span class="text-sm text-gray-600 dark:text-gray-400">
                        {{ slotProps.data.tooltip || '-' }}
                      </span>
                    </template>
                  </Column>
                  <Column header="Actions" style="width: 8rem">
                    <template #body="slotProps">
                      <Button
                        icon="pi pi-trash"
                        @click="confirmDelete(slotProps.data)"
                        :loading="deletingProjectId === slotProps.data.key"
                        size="small"
                        severity="danger"
                        outlined
                        v-tooltip.top="'Delete project'"
                      />
                    </template>
                  </Column>
                </DataTable>
              </div>

              <!-- No Projects Message -->
              <div v-else class="text-center py-8 text-gray-500 dark:text-gray-400">
                <i class="pi pi-link text-4xl mb-4"></i>
                <p>No external projects configured yet.</p>
              </div>

              <!-- Refresh Button -->
              <div class="mt-4 flex justify-end">
                <Button
                  label="Refresh"
                  icon="pi pi-refresh"
                  @click="loadProjects"
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

    <!-- Delete Confirmation Dialog -->
    <Dialog v-model:visible="deleteDialogVisible" header="Confirm Delete" :modal="true" class="w-96">
      <div class="flex items-start gap-3">
        <i class="pi pi-exclamation-triangle text-3xl text-orange-500"></i>
        <div>
          <p class="mb-2">Are you sure you want to delete this project?</p>
          <p class="font-semibold">{{ projectToDelete?.projectName }}</p>
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" @click="deleteDialogVisible = false" severity="secondary" text />
        <Button label="Delete" @click="handleDeleteProject" severity="danger" :loading="deleting" />
      </template>
    </Dialog>
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
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Dialog from 'primevue/dialog'
import { useAuth } from '@/services/auth'
import { UserStatus, ExternalProject } from '@/api/apiclients/Perpendicularity/PerpendicularityApiClient'
import { getAllExternalProjects } from '@/api/ExternalProject/GetAllExternalProjectsQuery'
import { addExternalProject } from '@/api/ExternalProject/AddExternalProjectCommand'
import { deleteExternalProject } from '@/api/ExternalProject/DeleteExternalProjectCommand'

const router = useRouter()
const toast = useToast()
const { user, isAuthenticated } = useAuth()

// Check if user is admin
const isAdmin = computed(() => {
  return isAuthenticated.value && user.value?.status === UserStatus._4
})

// State
const loading = ref(false)
const adding = ref(false)
const deleting = ref(false)
const projects = ref<ExternalProject[]>([])
const successMessage = ref('')
const errorMessage = ref('')
const deletingProjectId = ref<string | null>(null)
const deleteDialogVisible = ref(false)
const projectToDelete = ref<ExternalProject | null>(null)

// New project form
const newProject = ref({
  name: '',
  uri: '',
  tooltip: ''
})

// Form validation
const isFormValid = computed(() => {
  return newProject.value.name.trim() !== '' && 
         newProject.value.uri.trim() !== ''
})

// Load projects on mount
onMounted(() => {
  loadProjects()
})

// Load all projects
const loadProjects = async () => {
  loading.value = true
  errorMessage.value = ''
  
  try {
    projects.value = await getAllExternalProjects()
  } catch (error: any) {
    console.error('Failed to load projects:', error)
    errorMessage.value = error.message || 'Failed to load projects. Please try again.'
  } finally {
    loading.value = false
  }
}

// Add a new project
const handleAddProject = async () => {
  if (!isFormValid.value) return
  
  adding.value = true
  successMessage.value = ''
  errorMessage.value = ''
  
  try {
    await addExternalProject(
      newProject.value.name,
      newProject.value.uri,
      newProject.value.tooltip
    )
    
    successMessage.value = `Successfully added project "${newProject.value.name}"!`
    
    // Reset form
    newProject.value = {
      name: '',
      uri: '',
      tooltip: ''
    }
    
    // Reload projects
    await loadProjects()
  } catch (error: any) {
    console.error('Failed to add project:', error)
    errorMessage.value = error.message || 'Failed to add project. Please try again.'
  } finally {
    adding.value = false
  }
}

// Confirm delete
const confirmDelete = (project: ExternalProject) => {
  projectToDelete.value = project
  deleteDialogVisible.value = true
}

// Delete a project
const handleDeleteProject = async () => {
  if (!projectToDelete.value?.key) return
  
  deletingProjectId.value = projectToDelete.value.key
  deleting.value = true
  successMessage.value = ''
  errorMessage.value = ''
  
  try {
    await deleteExternalProject(projectToDelete.value.key)
    successMessage.value = `Successfully deleted project "${projectToDelete.value.projectName}"!`
    
    // Close dialog
    deleteDialogVisible.value = false
    projectToDelete.value = null
    
    // Reload projects
    await loadProjects()
  } catch (error: any) {
    console.error('Failed to delete project:', error)
    errorMessage.value = error.message || 'Failed to delete project. Please try again.'
  } finally {
    deletingProjectId.value = null
    deleting.value = false
  }
}
</script>
