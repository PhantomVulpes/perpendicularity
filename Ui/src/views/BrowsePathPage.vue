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
            <div class="px-6 py-3 bg-gray-50 dark:bg-gray-700 border-b border-gray-200 dark:border-gray-600">
              <h2 class="text-sm font-semibold text-gray-700 dark:text-gray-300 uppercase tracking-wide">
                Files
              </h2>
            </div>
            <div class="divide-y divide-gray-200 dark:divide-gray-700">
              <div
                v-for="file in contents?.files"
                :key="file"
                class="flex items-center px-6 py-4 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <i class="pi pi-file text-2xl text-gray-500 dark:text-gray-400"></i>
                <span class="ml-4 text-lg text-gray-800 dark:text-gray-200">
                  {{ file }}
                </span>
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
import type { DirectoryContentsResponse } from '@/api/apiclients/PerpendicularityApiClient'

const route = useRoute()

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

const loadContents = async () => {
  loading.value = true
  error.value = null
  
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

onMounted(() => {
  loadContents()
})

// Reload contents when route changes
watch(() => [rootDirectory.value, remainingPath.value], () => {
  loadContents()
})
</script>
