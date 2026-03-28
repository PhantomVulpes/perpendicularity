<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800 flex items-center justify-center p-4">
    <Card class="w-full max-w-md">
      <template #title>
        <div class="flex items-center gap-2 text-2xl">
          <i class="pi pi-sign-in text-primary"></i>
          Sign In
        </div>
      </template>
      
      <template #content>
        <form @submit.prevent="handleLogin" class="flex flex-col gap-4">
          <!-- Success Message -->
          <Message v-if="success" severity="success" :closable="false">
            Welcome back, {{ userDisplayName }}!
          </Message>

          <!-- Error Message -->
          <Message v-if="error" severity="error" :closable="true" @close="error = ''">
            {{ error }}
          </Message>

          <!-- First Name -->
          <div class="flex flex-col gap-2">
            <label for="firstName" class="font-semibold text-gray-700 dark:text-gray-300">
              First Name
            </label>
            <InputText
              id="firstName"
              v-model="firstName"
              placeholder="Enter your first name"
              :disabled="loading"
              class="w-full"
            />
          </div>

          <!-- Last Name -->
          <div class="flex flex-col gap-2">
            <label for="lastName" class="font-semibold text-gray-700 dark:text-gray-300">
              Last Name
            </label>
            <InputText
              id="lastName"
              v-model="lastName"
              placeholder="Enter your last name"
              :disabled="loading"
              class="w-full"
            />
          </div>

          <!-- Password -->
          <div class="flex flex-col gap-2">
            <label for="password" class="font-semibold text-gray-700 dark:text-gray-300">
              Password
            </label>
            <Password
              id="password"
              v-model="password"
              placeholder="Enter your password"
              :disabled="loading"
              :feedback="false"
              toggleMask
              class="w-full"
            />
          </div>

          <!-- Submit Button -->
          <Button
            type="submit"
            label="Sign In"
            icon="pi pi-sign-in"
            :loading="loading"
            severity="primary"
            class="w-full mt-2"
          />

          <!-- Register Link -->
          <Button
            type="button"
            label="Don't have an account? Register"
            icon="pi pi-user-plus"
            severity="secondary"
            text
            class="w-full"
            @click="router.push('/register')"
            :disabled="loading"
          />

          <!-- Back to Home Link -->
          <Button
            type="button"
            label="Back to Home"
            icon="pi pi-arrow-left"
            severity="secondary"
            text
            class="w-full"
            @click="router.push('/')"
            :disabled="loading"
          />
        </form>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import Card from 'primevue/card'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Button from 'primevue/button'
import Message from 'primevue/message'
import { LoginUser } from '@/api/User/LoginUserQuery'
import { useAuth } from '@/services/auth'

const router = useRouter()
const { signIn } = useAuth()

// Form fields
const firstName = ref('')
const lastName = ref('')
const password = ref('')

// UI state
const loading = ref(false)
const error = ref('')
const success = ref(false)
const userDisplayName = ref('')

const handleLogin = async () => {
  error.value = ''
  success.value = false
  
  // Basic validation
  if (!firstName.value || !lastName.value || !password.value) {
    error.value = 'All fields are required'
    return
  }

  loading.value = true

  try {
    const user = await LoginUser(firstName.value, lastName.value, password.value);
    
    // Store user session
    signIn(user)
    
    userDisplayName.value = user.displayName || `${user.firstName} ${user.lastName}`
    success.value = true
    
    // Reset form after a delay and redirect
    setTimeout(() => {
      router.push('/')
    }, 1500)
    
  } catch (err: any) {
    console.error('Login error:', err)
    error.value = err.message || 'Login failed. Please check your credentials and try again.'
  } finally {
    loading.value = false
  }
}
</script>
