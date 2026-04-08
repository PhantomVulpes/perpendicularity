<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-4xl mx-auto">
        <!-- Header -->
        <div class="mb-8">
          <h1 class="text-4xl font-bold text-gray-800 dark:text-white mb-2">Browse Files</h1>
          <p class="text-gray-600 dark:text-gray-400">Select a folder to browse</p>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-8">
          <div class="flex items-center justify-center">
            <i class="pi pi-spinner pi-spin text-4xl text-blue-600"></i>
            <span class="ml-3 text-gray-600 dark:text-gray-300">Loading folders...</span>
          </div>
        </div>

        <!-- Error State -->
        <div v-else-if="error" class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-8">
          <div class="flex items-center text-red-600">
            <i class="pi pi-exclamation-circle text-3xl"></i>
            <div class="ml-3">
              <div class="font-semibold">Error loading folders</div>
              <div class="text-sm">{{ error }}</div>
            </div>
          </div>
        </div>

        <!-- Folders List -->
        <div v-else-if="aliases && aliases.length > 0" class="bg-white dark:bg-gray-800 rounded-lg shadow-lg overflow-hidden">
          <div class="divide-y divide-gray-200 dark:divide-gray-700">
            <RouterLink
              v-for="alias in aliases"
              :key="alias"
              :to="`/browse/${alias}`"
              class="flex items-center px-6 py-4 hover:bg-blue-50 dark:hover:bg-gray-700 transition-colors cursor-pointer group"
            >
              <i class="pi pi-folder text-2xl text-blue-600 dark:text-blue-400 group-hover:text-blue-700 dark:group-hover:text-blue-300"></i>
              <span class="ml-4 text-lg font-medium text-gray-800 dark:text-gray-200 group-hover:text-blue-700 dark:group-hover:text-blue-300">
                {{ alias }}
              </span>
              <i class="pi pi-chevron-right ml-auto text-gray-400 group-hover:text-blue-600 dark:group-hover:text-blue-400"></i>
            </RouterLink>
          </div>
        </div>

        <!-- Empty State -->
        <div v-else class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-8">
          <div class="text-center text-gray-500 dark:text-gray-400">
            <i class="pi pi-folder-open text-6xl mb-4"></i>
            <p class="text-lg">No folders available</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { createAuthenticatedClient } from '@/api/apiClient'

const aliases = ref<string[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

onMounted(async () => {
  try {
    const client = createAuthenticatedClient()
    aliases.value = await client.rootDirectory()
  } catch (e: any) {
    error.value = e.message || 'Failed to load folders'
    console.error('Error loading aliases:', e)
  } finally {
    loading.value = false
  }
})
</script>
