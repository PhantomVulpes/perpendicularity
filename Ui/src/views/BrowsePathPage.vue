<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-5xl mx-auto">
        <!-- Header with Breadcrumbs -->
        <div class="mb-8">
          <div class="flex items-center gap-2 mb-4 text-sm text-gray-600 dark:text-gray-400">
            <RouterLink to="/browse" class="hover:text-blue-600 dark:hover:text-blue-400 transition-colors">
              Browse
            </RouterLink>
            <i class="pi pi-chevron-right text-xs"></i>
            <RouterLink :to="`/browse/${rootDirectory}`" class="hover:text-blue-600 dark:hover:text-blue-400 transition-colors">
              {{ rootDirectory }}
            </RouterLink>
            <template v-if="pathSegments.length > 0">
              <template v-for="(segment, index) in pathSegments" :key="index">
                <i class="pi pi-chevron-right text-xs"></i>
                <RouterLink 
                  :to="getPathUpTo(index)"
                  class="hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                >
                  {{ segment }}
                </RouterLink>
              </template>
            </template>
          </div>
          <h1 class="text-4xl font-bold text-gray-800 dark:text-white">
            {{ currentFolderName }}
          </h1>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-8">
          <div class="flex items-center justify-center">
            <i class="pi pi-spinner pi-spin text-4xl text-blue-600"></i>
            <span class="ml-3 text-gray-600 dark:text-gray-300">Loading contents...</span>
          </div>
        </div>

        <!-- Error State -->
        <div v-else-if="error" class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-8">
          <div class="flex items-center text-red-600">
            <i class="pi pi-exclamation-circle text-3xl"></i>
            <div class="ml-3">
              <div class="font-semibold">Error loading contents</div>
              <div class="text-sm">{{ error }}</div>
            </div>
          </div>
        </div>

        <!-- Contents -->
        <div v-else class="space-y-6">
          <!-- Directories Section -->
          <div v-if="(contents?.directories?.length ?? 0) > 0" class="bg-white dark:bg-gray-800 rounded-lg shadow-lg overflow-hidden">
            <div class="px-6 py-3 bg-gray-50 dark:bg-gray-700 border-b border-gray-200 dark:border-gray-600">
              <h2 class="text-sm font-semibold text-gray-700 dark:text-gray-300 uppercase tracking-wide">
                Folders
              </h2>
            </div>
            <div class="divide-y divide-gray-200 dark:divide-gray-700">
              <RouterLink
                v-for="directory in contents?.directories"
                :key="directory"
                :to="getDirectoryPath(directory)"
                class="flex items-center px-6 py-4 hover:bg-blue-50 dark:hover:bg-gray-700 transition-colors cursor-pointer group"
              >
                <i class="pi pi-folder text-2xl text-blue-600 dark:text-blue-400 group-hover:text-blue-700 dark:group-hover:text-blue-300"></i>
                <span class="ml-4 text-lg font-medium text-gray-800 dark:text-gray-200 group-hover:text-blue-700 dark:group-hover:text-blue-300">
                  {{ directory }}
                </span>
                <i class="pi pi-chevron-right ml-auto text-gray-400 group-hover:text-blue-600 dark:group-hover:text-blue-400"></i>
              </RouterLink>
            </div>
          </div>

          <!-- Files Section -->
          <div v-if="(contents?.files?.length ?? 0) > 0" class="bg-white dark:bg-gray-800 rounded-lg shadow-lg overflow-hidden">
            <div class="px-6 py-3 bg-gray-50 dark:bg-gray-700 border-b border-gray-200 dark:border-gray-600 flex items-center justify-between">
              <h2 class="text-sm font-semibold text-gray-700 dark:text-gray-300 uppercase tracking-wide">
                Files
              </h2>
              <div v-if="selectedFiles.size > 0" class="flex items-center gap-3">
                <span class="text-sm text-gray-600 dark:text-gray-400">
                  {{ selectedFiles.size }} selected
                </span>
                <button
                  @click="downloadSelected"
                  :disabled="downloading"
                  class="px-4 py-2 bg-blue-600 hover:bg-blue-700 disabled:bg-gray-400 text-white text-sm font-medium rounded-lg transition-colors flex items-center gap-2"
                >
                  <i :class="downloading ? 'pi pi-spinner pi-spin' : 'pi pi-download'"></i>
                  <span>{{ downloading ? 'Downloading...' : 'Download Selected' }}</span>
                </button>
                <button
                  @click="clearSelection"
                  class="px-3 py-2 text-gray-600 dark:text-gray-400 hover:text-gray-800 dark:hover:text-gray-200 text-sm font-medium transition-colors"
                >
                  Clear
                </button>
              </div>
            </div>
            <div class="divide-y divide-gray-200 dark:divide-gray-700">
              <div
                v-for="file in contents?.files"
                :key="file"
                class="flex items-center px-6 py-4 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors group"
              >
                <input
                  type="checkbox"
                  :checked="selectedFiles.has(file)"
                  @change="toggleFileSelection(file)"
                  class="w-5 h-5 rounded border-gray-300 text-blue-600 focus:ring-blue-500 cursor-pointer"
                />
                <i class="pi pi-file text-2xl text-gray-500 dark:text-gray-400 ml-4"></i>
                <span class="ml-4 text-lg text-gray-800 dark:text-gray-200 flex-1">
                  {{ file }}
                </span>
                <button
                  @click="downloadSingleFile(file)"
                  class="px-3 py-1.5 text-blue-600 hover:bg-blue-50 dark:hover:bg-gray-600 rounded transition-colors opacity-0 group-hover:opacity-100"
                  title="Download file"
                >
                  <i class="pi pi-download"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div v-if="(contents?.directories?.length ?? 0) === 0 && (contents?.files?.length ?? 0) === 0" class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-8">
            <div class="text-center text-gray-500 dark:text-gray-400">
              <i class="pi pi-inbox text-6xl mb-4"></i>
              <p class="text-lg">This folder is empty</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { createAuthenticatedClient } from '@/api/apiClient'
import { useAuth } from '@/services/auth'
import type { DirectoryContentsResponse } from '@/api/apiclients/Perpendicularity/PerpendicularityApiClient'
import { DownloadFilesAsZipRequest } from '@/api/apiclients/Perpendicularity/PerpendicularityApiClient'

const route = useRoute()
const { token } = useAuth()

const rootDirectory = computed(() => route.params.rootDirectory as string)
const remainingPath = computed(() => {
  const path = route.params.pathMatch as string | string[]
  if (Array.isArray(path)) {
    return path.join('/')
  }
  return path || ''
})

const pathSegments = computed(() => {
  const path = remainingPath.value
  return path ? path.split('/').filter(s => s.length > 0) : []
})

const currentFolderName = computed(() => {
  if (pathSegments.value.length > 0) {
    return pathSegments.value[pathSegments.value.length - 1]
  }
  return rootDirectory.value
})

const contents = ref<DirectoryContentsResponse | null>(null)
const loading = ref(true)
const error = ref<string | null>(null)
const selectedFiles = ref<Set<string>>(new Set())
const downloading = ref(false)

const loadContents = async () => {
  loading.value = true
  error.value = null
  selectedFiles.value.clear()
  
  try {
    const client = createAuthenticatedClient()
    const path = remainingPath.value
    
    if (path) {
      contents.value = await client.file2(rootDirectory.value, path)
    } else {
      contents.value = await client.file(rootDirectory.value)
    }
  } catch (e: any) {
    error.value = e.message || 'Failed to load directory contents'
    console.error('Error loading directory contents:', e)
  } finally {
    loading.value = false
  }
}

const getPathUpTo = (index: number): string => {
  const segments = pathSegments.value.slice(0, index + 1)
  return `/browse/${rootDirectory.value}/${segments.join('/')}`
}

const getDirectoryPath = (directory: string): string => {
  const currentPath = remainingPath.value
  const newPath = currentPath ? `${currentPath}/${directory}` : directory
  return `/browse/${rootDirectory.value}/${newPath}`
}

const toggleFileSelection = (file: string) => {
  if (selectedFiles.value.has(file)) {
    selectedFiles.value.delete(file)
  } else {
    selectedFiles.value.add(file)
  }
}

const clearSelection = () => {
  selectedFiles.value.clear()
}

const downloadFile = async (url: string, filename: string) => {
  try {
    const headers: HeadersInit = {}
    if (token.value) {
      headers['Authorization'] = `Bearer ${token.value}`
    }
    
    const response = await fetch(url, { headers })
    
    if (!response.ok) {
      throw new Error('Download failed')
    }
    
    const blob = await response.blob()
    const downloadUrl = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = downloadUrl
    link.download = filename
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(downloadUrl)
  } catch (err: any) {
    console.error('Download error:', err)
    alert('Failed to download file: ' + (err.message || 'Unknown error'))
  }
}

const downloadSingleFile = async (file: string) => {
  const currentPath = remainingPath.value
  const filePath = currentPath ? `${currentPath}/${file}` : file
  const url = `/api/file/download/${encodeURIComponent(rootDirectory.value)}/${encodeURIComponent(filePath)}`
  await downloadFile(url, file)
}

const downloadSelected = async () => {
  if (selectedFiles.value.size === 0) return
  
  downloading.value = true
  try {
    const currentPath = remainingPath.value
    const filePaths = Array.from(selectedFiles.value).map(file => 
      currentPath ? `${currentPath}/${file}` : file
    )
    
    if (filePaths.length === 1) {
      // Download single file directly
      await downloadSingleFile(Array.from(selectedFiles.value)[0])
    } else {
      // Download multiple files as ZIP
      const request = new DownloadFilesAsZipRequest({
        rootDirectory: rootDirectory.value,
        filePaths: filePaths
      })
      
      const headers: HeadersInit = {
        'Content-Type': 'application/json'
      }
      if (token.value) {
        headers['Authorization'] = `Bearer ${token.value}`
      }
      
      const url = '/api/file/download-zip'
      const response = await fetch(url, {
        method: 'POST',
        headers,
        body: JSON.stringify(request)
      })
      
      if (!response.ok) {
        throw new Error('Download failed')
      }
      
      const blob = await response.blob()
      const downloadUrl = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = downloadUrl
      
      // Get filename from Content-Disposition header or use default
      const contentDisposition = response.headers.get('Content-Disposition')
      let filename = 'download.zip'
      if (contentDisposition) {
        const filenameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/)
        if (filenameMatch && filenameMatch[1]) {
          filename = filenameMatch[1].replace(/['"]/g, '')
        }
      }
      
      link.download = filename
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      window.URL.revokeObjectURL(downloadUrl)
    }
    
    clearSelection()
  } catch (err: any) {
    console.error('Download error:', err)
    alert('Failed to download files: ' + (err.message || 'Unknown error'))
  } finally {
    downloading.value = false
  }
}

onMounted(() => {
  loadContents()
})

// Reload contents when route changes
watch(() => [rootDirectory.value, remainingPath.value], () => {
  loadContents()
})
</script>
