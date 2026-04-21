<template>
  <Dialog 
    v-model:visible="localVisible" 
    modal 
    header="Request Assistance" 
    :style="{ width: '30rem' }"
    @hide="handleClose"
  >
    <div class="flex flex-col gap-4 pt-2">
      <!-- Success Message -->
      <Message v-if="success" severity="success" :closable="false">
        Your request has been submitted successfully!
      </Message>

      <!-- Error Message -->
      <Message v-if="error" severity="error" :closable="true" @close="error = ''">
        {{ error }}
      </Message>

      <!-- Form (hidden after success) -->
      <div v-if="!success" class="flex flex-col gap-4">
        <!-- Title Field -->
        <div class="flex flex-col gap-2">
          <label for="title" class="font-semibold text-gray-700 dark:text-gray-300">
            Title
          </label>
          <InputText
            id="title"
            v-model="title"
            placeholder="Brief description of your issue"
            :disabled="loading"
            class="w-full"
            required
          />
        </div>

        <!-- Description Field -->
        <div class="flex flex-col gap-2">
          <label for="description" class="font-semibold text-gray-700 dark:text-gray-300">
            Description
          </label>
          <Textarea
            id="description"
            v-model="description"
            placeholder="Provide details about the assistance you need"
            :disabled="loading"
            rows="5"
            class="w-full"
            required
          />
        </div>

        <!-- Info Message -->
        <Message severity="info" :closable="false">
          This will create a ticket in the PERP project for support team review.
        </Message>
      </div>
    </div>

    <template #footer>
      <div v-if="!success" class="flex gap-2 justify-end">
        <Button 
          label="Cancel" 
          icon="pi pi-times" 
          @click="handleClose" 
          severity="secondary" 
          outlined
          :disabled="loading"
        />
        <Button 
          label="Submit Request" 
          icon="pi pi-send" 
          @click="handleSubmit" 
          severity="primary"
          :loading="loading"
          :disabled="!title || !description"
        />
      </div>
      <div v-else class="flex justify-end">
        <Button 
          label="Close" 
          icon="pi pi-check" 
          @click="handleClose" 
          severity="success"
        />
      </div>
    </template>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Message from 'primevue/message'
import { useAuth } from '@/services/auth'
import { createTicket } from '@/api/Zinc/CreateTicketCommand'
import { getAssistanceProject } from '@/api/zincClient'

const props = defineProps<{
  visible: boolean
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

const { user } = useAuth()

// Local state
const localVisible = ref(props.visible)
const title = ref('')
const description = ref('')
const loading = ref(false)
const error = ref('')
const success = ref(false)

// Watch for prop changes
watch(() => props.visible, (newVal) => {
  localVisible.value = newVal
  if (newVal) {
    // Reset form when dialog opens
    resetForm()
  }
})

// Watch for local changes and emit
watch(localVisible, (newVal) => {
  emit('update:visible', newVal)
})

const resetForm = () => {
  title.value = ''
  description.value = ''
  error.value = ''
  success.value = false
  loading.value = false
}

const handleClose = () => {
  localVisible.value = false
  // Delay reset to avoid visual glitch
  setTimeout(resetForm, 300)
}

const handleSubmit = async () => {
  if (!title.value || !description.value) {
    error.value = 'Please fill in all fields'
    return
  }

  if (!user.value) {
    error.value = 'You must be logged in to request assistance'
    return
  }

  loading.value = true
  error.value = ''

  try {
    // Create labels with username and "Request Assistance"
    const userName = `${user.value.firstName} ${user.value.lastName}`
    const labels = [userName, 'Request Assistance']
    const ticketProject = await getAssistanceProject()

    // Submit ticket to runtime-configured project
    await createTicket(ticketProject, title.value, description.value, labels)

    success.value = true
  } catch (err: any) {
    console.error('Failed to create ticket:', err)
    error.value = err.message || 'Failed to submit request. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>
