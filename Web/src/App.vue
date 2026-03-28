<script setup lang="ts">
import { ref } from 'vue'
import Button from 'primevue/button'
import Card from 'primevue/card'

const count = ref(0)
const message = ref('Welcome to Perpendicularity')

const increment = () => {
  count.value++
}
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <div class="container mx-auto px-4 py-8">
      <div class="text-center mb-8">
        <h1 class="text-4xl font-bold text-gray-800 dark:text-white mb-2">
          {{ message }}
        </h1>
        <p class="text-gray-600 dark:text-gray-300">
          Vue 3 + TypeScript + Tailwind CSS + PrimeVue
        </p>
      </div>

      <div class="flex justify-center">
        <Card class="w-full max-w-md">
          <template #title>
            <div class="flex items-center gap-2">
              <i class="pi pi-sparkles text-primary"></i>
              Getting Started
            </div>
          </template>
          <template #content>
            <div class="flex flex-col items-center gap-4">
              <p class="text-center text-gray-600 dark:text-gray-400">
                Click the button to test the counter
              </p>
              <div class="text-2xl font-bold text-primary">
                Count: {{ count }}
              </div>
              <Button 
                @click="increment" 
                label="Increment" 
                icon="pi pi-plus" 
                severity="primary"
                class="w-full"
              />
            </div>
          </template>
        </Card>
      </div>

      <div class="mt-8 text-center">
        <Card class="max-w-2xl mx-auto">
          <template #title>
            Next Steps
          </template>
          <template #content>
            <div class="grid md:grid-cols-2 gap-4 text-left">
              <div class="p-4 bg-blue-50 dark:bg-gray-700 rounded-lg">
                <i class="pi pi-code text-2xl text-blue-500 mb-2"></i>
                <h3 class="font-semibold mb-1">Edit Components</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">
                  Start editing src/App.vue to build your application
                </p>
              </div>
              <div class="p-4 bg-green-50 dark:bg-gray-700 rounded-lg">
                <i class="pi pi-palette text-2xl text-green-500 mb-2"></i>
                <h3 class="font-semibold mb-1">Customize Styles</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">
                  Use Tailwind CSS utilities and PrimeVue components
                </p>
              </div>
              <div class="p-4 bg-purple-50 dark:bg-gray-700 rounded-lg">
                <i class="pi pi-database text-2xl text-purple-500 mb-2"></i>
                <h3 class="font-semibold mb-1">Connect to API</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">
                  API proxy is configured at /api endpoint
                </p>
              </div>
              <div class="p-4 bg-orange-50 dark:bg-gray-700 rounded-lg">
                <i class="pi pi-book text-2xl text-orange-500 mb-2"></i>
                <h3 class="font-semibold mb-1">Documentation</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">
                  Check out Vue, Vite, and PrimeVue docs
                </p>
              </div>
            </div>
          </template>
        </Card>
      </div>
    </div>
  </div>
</template>

<style scoped>
</style>
