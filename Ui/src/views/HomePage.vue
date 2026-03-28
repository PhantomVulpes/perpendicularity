<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import Card from 'primevue/card'

const router = useRouter()
const count = ref(0)
const message = ref('Welcome to Perpendicularity')

const increment = () => {
  count.value++
}

const goToRegister = () => {
  router.push('/register')
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

      <div class="flex justify-center gap-4">
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
              <Button 
                @click="goToRegister" 
                label="Register New User" 
                icon="pi pi-user-plus" 
                severity="secondary"
                class="w-full"
              />
            </div>
          </template>
        </Card>
      </div>
    </div>
  </div>
</template>
