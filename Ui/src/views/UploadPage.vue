<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-2xl mx-auto">
        <!-- Upload Card -->
        <Card>
          <template #title>
            <div class="flex items-center gap-2 text-2xl">
              <i class="pi pi-upload text-primary"></i>
              Upload File
            </div>
          </template>
          
          <template #content>
            <!-- Success Message -->
            <Message v-if="successMessage" severity="success" :closable="true" @close="successMessage = ''">
              {{ successMessage }}
            </Message>

            <!-- Error Message -->
            <Message v-if="errorMessage" severity="error" :closable="true" @close="errorMessage = ''">
              {{ errorMessage }}
            </Message>

            <!-- Info Message -->
            <Message severity="info" :closable="false" class="mb-6">
              <div class="flex items-start gap-2">
                <i class="pi pi-info-circle mt-1"></i>
                <div>
                  <p class="text-sm">Upload a file to the server to be triaged by an admin. Your upload will not appear immediately as it must be sorted on the server. Select a destination folder and choose a file to upload. For multiple files, please use a .zip file.</p>
                </div>
              </div>
            </Message>

            <!-- Loading State -->
            <div v-if="loadingAliases" class="text-center py-8">
              <i class="pi pi-spin pi-spinner text-4xl text-primary"></i>
              <p class="mt-4 text-gray-600 dark:text-gray-400">Loading upload destinations...</p>
            </div>

            <!-- Upload Form -->
            <div v-else class="flex flex-col gap-6">
              <!-- Destination Dropdown -->
              <div class="flex flex-col gap-2">
                <label for="destination" class="font-semibold text-gray-700 dark:text-gray-300">
                  Upload Destination
                </label>
                <Dropdown
                  id="destination"
                  v-model="selectedAlias"
                  :options="aliases"
                  placeholder="Select a destination folder"
                  :disabled="uploading"
                  class="w-full"
                  :class="{ 'border-red-500': aliasError }"
                />
                <small v-if="aliasError" class="text-red-600">{{ aliasError }}</small>
              </div>

              <!-- File Input -->
              <div class="flex flex-col gap-2">
                <label for="file-upload" class="font-semibold text-gray-700 dark:text-gray-300">
                  Select File
                </label>
                <FileUpload
                  mode="basic"
                  name="file"
                  :auto="false"
                  :maxFileSize="1073741824"
                  :disabled="uploading"
                  @select="onFileSelect"
                  chooseLabel="Choose File"
                  class="w-full"
                >
                  <template #empty>
                    <p>Drag and drop file here to upload.</p>
                  </template>
                </FileUpload>
                <small v-if="selectedFile" class="text-gray-600 dark:text-gray-400">
                  Selected: {{ selectedFile.name }} ({{ formatFileSize(selectedFile.size) }})
                </small>
                <small v-if="fileError" class="text-red-600">{{ fileError }}</small>
              </div>

              <!-- Upload Button -->
              <div class="flex justify-end gap-2 pt-4">
                <Button
                  label="Cancel"
                  icon="pi pi-times"
                  @click="handleCancel"
                  severity="secondary"
                  outlined
                  :disabled="uploading"
                />
                <Button
                  label="Upload File"
                  icon="pi pi-upload"
                  @click="handleUpload"
                  severity="primary"
                  :loading="uploading"
                  :disabled="!selectedFile || !selectedAlias"
                />
              </div>
            </div>
          </template>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Card from 'primevue/card'
import Button from 'primevue/button'
import Message from 'primevue/message'
import Dropdown from 'primevue/dropdown'
import FileUpload from 'primevue/fileupload'
import type { FileUploadSelectEvent } from 'primevue/fileupload'
import { useAuth } from '@/services/auth'
import { createAuthenticatedClient } from '@/api/apiClient'
import { FileParameter } from '@/api/apiclients/Perpendicularity/PerpendicularityApiClient'
import { createTicket } from '@/api/Zinc/CreateTicketCommand'
import { getAssistanceProject } from '@/api/zincClient'

const router = useRouter()
const { user } = useAuth()

// State
const aliases = ref<string[]>([])
const selectedAlias = ref<string>('')
const selectedFile = ref<File | null>(null)
const loadingAliases = ref(true)
const uploading = ref(false)
const successMessage = ref('')
const errorMessage = ref('')
const aliasError = ref('')
const fileError = ref('')

// Load upload destinations on mount
onMounted(async () => {
  try {
    const client = createAuthenticatedClient()
    aliases.value = await client.uploadDirectory()
  } catch (e: any) {
    errorMessage.value = e.message || 'Failed to load upload destinations'
    console.error('Error loading upload aliases:', e)
  } finally {
    loadingAliases.value = false
  }
})

const onFileSelect = (event: FileUploadSelectEvent) => {
  if (event.files && event.files.length > 0) {
    selectedFile.value = event.files[0]
    fileError.value = ''
  }
}

const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i]
}

const handleCancel = () => {
  router.push('/browse')
}

const handleUpload = async () => {
  // Validation
  aliasError.value = ''
  fileError.value = ''
  errorMessage.value = ''

  if (!selectedAlias.value) {
    aliasError.value = 'Please select an upload destination'
    return
  }

  if (!selectedFile.value) {
    fileError.value = 'Please select a file to upload'
    return
  }

  if (!user.value) {
    errorMessage.value = 'You must be logged in to upload files'
    return
  }

  uploading.value = true

  try {
    const client = createAuthenticatedClient()
    
    // Create FileParameter for the upload
    const fileParam: FileParameter = {
      data: selectedFile.value,
      fileName: selectedFile.value.name
    }

    // Upload the file
    await client.upload(selectedAlias.value, fileParam)

    // Create Zinc ticket
    const userName = `${user.value.firstName} ${user.value.lastName}`
    const fileCount = selectedFile.value.name.toLowerCase().endsWith('.zip') ? 'file(s)' : 'file'
    const title = `${userName} uploaded ${selectedAlias.value} ${fileCount}`
    const description = `File uploaded: ${selectedFile.value.name}\nDestination: ${selectedAlias.value}\nSize: ${formatFileSize(selectedFile.value.size)}`
    const labels = [userName, 'Upload']
    const ticketProject = await getAssistanceProject()

    await createTicket(ticketProject, title, description, labels)

    // Success
    successMessage.value = `File "${selectedFile.value.name}" uploaded successfully to ${selectedAlias.value}!`
    
    // Reset form
    selectedFile.value = null
    selectedAlias.value = ''
    
    // Scroll to top to show success message
    window.scrollTo({ top: 0, behavior: 'smooth' })
  } catch (err: any) {
    console.error('Failed to upload file:', err)
    errorMessage.value = err.message || 'Failed to upload file. Please try again.'
  } finally {
    uploading.value = false
  }
}
</script>
