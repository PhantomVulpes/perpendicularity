<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-4xl mx-auto">
        <!-- Header -->
        <div class="text-center mb-8">
          <h1 class="text-4xl font-bold text-gray-800 dark:text-white mb-2">
            Roadmap
          </h1>
          <p class="text-gray-600 dark:text-gray-400">
            Planned features and improvements for Perpendicularity
          </p>
        </div>

        <!-- Roadmap Card -->
        <Card class="mb-6">
          <template #title>
            <div class="flex items-center gap-2 text-2xl">
              <i class="pi pi-map text-primary"></i>
              Feature Roadmap
            </div>
          </template>
          
          <template #content>
            <!-- Completed Features Section -->
            <div class="mb-8">
              <h2 class="text-xl font-semibold text-gray-800 dark:text-white mb-4 flex items-center gap-2">
                <i class="pi pi-check-circle text-green-500"></i>
                Completed Features
              </h2>
              <div class="space-y-3">
                <div
                  v-for="feature in completedFeatures"
                  :key="feature.id"
                  class="flex items-start gap-3 p-3 rounded-lg bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800"
                >
                  <Checkbox
                    v-model="feature.completed"
                    :binary="true"
                    :disabled="true"
                    class="mt-1"
                  />
                  <div class="flex-1">
                    <div class="font-medium text-gray-800 dark:text-white line-through">
                      {{ feature.title }}
                    </div>
                    <div class="text-sm text-gray-600 dark:text-gray-400 mt-1">
                      {{ feature.description }}
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Upcoming Features Section -->
            <div>
              <h2 class="text-xl font-semibold text-gray-800 dark:text-white mb-4 flex items-center gap-2">
                <i class="pi pi-star text-purple-500"></i>
                Upcoming Features
              </h2>
              <div class="space-y-3">
                <div
                  v-for="feature in upcomingFeatures"
                  :key="feature.id"
                  class="flex items-start gap-3 p-3 rounded-lg bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 hover:shadow-md transition-shadow"
                >
                  <Checkbox
                    v-model="feature.completed"
                    :binary="true"
                    :disabled="true"
                    class="mt-1"
                  />
                  <div class="flex-1">
                    <div class="font-medium text-gray-800 dark:text-white">
                      {{ feature.title }}
                    </div>
                    <div class="text-sm text-gray-600 dark:text-gray-400 mt-1">
                      {{ feature.description }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </template>
        </Card>

        <!-- Tech Stack Footer -->
        <div class="text-center text-sm text-gray-500 dark:text-gray-400">
          Vue3, PrimeVue, Tailwind
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import Card from 'primevue/card'
import Checkbox from 'primevue/checkbox'

interface RoadmapFeature {
  id: number
  title: string
  description: string
  completed: boolean
}

// Completed features
const completedFeatures = ref<RoadmapFeature[]>([
  {
    id: 1,
    title: 'User Authentication',
    description: 'JWT-based user authentication with login and registration',
    completed: true
  },
  {
    id: 2,
    title: 'Admin Dashboard',
    description: 'Administrative interface for managing users and settings',
    completed: true
  },
  {
    id: 3,
    title: 'User Approval System',
    description: 'Workflow for approving new user registrations',
    completed: true
  },
  {
    id: 4,
    title: 'File Browsing',
    description: 'Allow approved users to browse folders and files in the server',
    completed: true
  },
])

// Upcoming features
const upcomingFeatures = ref<RoadmapFeature[]>([
  {
    id: 5,
    title: 'File Download',
    description: 'Allow approved users to download content directly',
    completed: false
  },
  {
    id: 6,
    title: 'File Upload',
    description: 'Allow approved users to upload content directly',
    completed: false
  },
  {
    id: 7,
    title: 'Personal File Storage',
    description: 'Allow approved users to create their own private folders and store their own files. Save data, music, backups.',
    completed: false
  },
  {
    id: 8,
    title: 'New Content Dashboard',
    description: 'Get notified on the home page when new content is added',
    completed: false
  },
  {
    id: 9,
    title: 'Storage Analytics',
    description: 'Dashboard showing storage usage, popular content, and statistics',
    completed: false
  },
  {
    id: 10,
    title: 'Content Request System',
    description: 'Users can request new content to be added to the server',
    completed: false
  },
  {
    id: 11,
    title: 'User Tagging System',
    description: 'Browse through user-tagged files with custom tags and categories',
    completed: false
  },
  {
    id: 12,
    title: 'Advanced Search',
    description: 'Enhanced search functionality with filters and categories',
    completed: false
  },
])
</script>
