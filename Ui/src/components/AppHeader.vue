<template>
  <div class="bg-white border-b border-gray-200 shadow-sm">
    <div class="container mx-auto px-4 py-3">
      <div class="flex items-center justify-between">
        <!-- Left: App Title & Navigation -->
        <div class="flex items-center gap-6">
          <RouterLink to="/" class="text-lg font-semibold text-gray-800 hover:text-blue-600 transition-colors cursor-pointer">
            Shadesmar's Perpendicularity
          </RouterLink>
          <RouterLink 
            to="/browse" 
            class="text-sm font-medium text-gray-600 hover:text-blue-600 transition-colors flex items-center gap-1"
          >
            <i class="pi pi-folder-open text-xs"></i>
            Browse
          </RouterLink>
          <RouterLink 
            to="/roadmap" 
            class="text-sm font-medium text-gray-600 hover:text-blue-600 transition-colors flex items-center gap-1"
          >
            <i class="pi pi-map text-xs"></i>
            Roadmap
          </RouterLink>
          <Button
            label="Other Projects"
            icon="pi pi-globe"
            @click="toggleExternalProjectsMenu"
            severity="secondary"
            text
            size="small"
            class="text-sm font-medium"
            aria-label="Other Projects"
          />
          <Menu ref="externalProjectsMenu" :model="externalProjectMenuItems" :popup="true">
            <template #item="{ item, props }">
              <a 
                v-bind="props.action"
                class="flex items-center gap-2 px-3 py-2"
                v-tooltip.right="item.data?.tooltip"
              >
                <i :class="item.icon" class="text-xs"></i>
                <span>{{ item.label }}</span>
              </a>
            </template>
          </Menu>
        </div>
        
        <!-- Right: Auth Actions or User Info -->
        <div v-if="isAuthenticated" class="flex items-center gap-3">
          <!-- Request Assistance Button (shown for all authenticated users) -->
          <Button
            label="Request Assistance"
            icon="pi pi-question-circle"
            @click="showAssistanceModal = true"
            severity="help"
            outlined
            size="small"
            aria-label="Request Assistance"
            class="pr-4"
          />

          <!-- Admin Button (only shown for administrators) -->
          <Button
            v-if="isAdmin"
            label="Admin Stuff"
            icon="pi pi-shield"
            @click="router.push('/admin')"
            severity="info"
            outlined
            size="small"
            aria-label="Admin Dashboard"
            class="pr-4"
          />

          <div class="text-right">
            <div class="font-semibold text-gray-800">{{ fullName }}</div>
            <div class="flex items-center justify-end gap-1 text-xs font-medium" :class="statusClasses">
              <i :class="statusIcon"></i>
              <span>{{ statusText }}</span>
            </div>
          </div>
          
          <Button
            icon="pi pi-sign-out"
            @click="handleSignOut"
            severity="secondary"
            outlined
            size="small"
            aria-label="Sign Out"
          />
        </div>
        
        <div v-else class="flex gap-2">
          <Button
            label="Sign In"
            icon="pi pi-sign-in"
            @click="router.push('/login')"
            severity="primary"
            size="small"
          />
          <Button
            label="Register"
            icon="pi pi-user-plus"
            @click="router.push('/register')"
            severity="secondary"
            outlined
            size="small"
          />
        </div>
      </div>
    </div>
  </div>

  <!-- Request Assistance Modal -->
  <RequestAssistanceModal v-model:visible="showAssistanceModal" />
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAuth } from '@/services/auth'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import Menu from 'primevue/menu'
import type { MenuItem } from 'primevue/menuitem'
import { UserStatus, ExternalProject } from '@/api/apiclients/Perpendicularity/PerpendicularityApiClient'
import { getAllExternalProjects } from '@/api/ExternalProject/GetAllExternalProjectsQuery'
import RequestAssistanceModal from './RequestAssistanceModal.vue'

const { user, isAuthenticated, signOut } = useAuth()
const router = useRouter()

// External Projects
const externalProjects = ref<ExternalProject[]>([])
const externalProjectsMenu = ref()

const toggleExternalProjectsMenu = (event: Event) => {
  externalProjectsMenu.value.toggle(event)
}

const fetchExternalProjects = async () => {
  try {
    const projects = await getAllExternalProjects()
    externalProjects.value = projects
  } catch (error) {
    console.error('Failed to fetch external projects:', error)
  }
}

const externalProjectMenuItems = computed<MenuItem[]>(() => {
  return externalProjects.value.map(project => ({
    label: project.projectName || '',
    icon: 'pi pi-external-link',
    command: (_event) => {
      if (project.projectUri) {
        window.open(project.projectUri, '_blank')
      }
    },
    data: {
      tooltip: project.tooltip || ''
    }
  }))
})

onMounted(() => {
  fetchExternalProjects()
})

// Modal visibility
const showAssistanceModal = ref(false)

const isAdmin = computed(() => {
  return isAuthenticated.value && user.value?.status === UserStatus._4
})

const fullName = computed(() => {
  if (!user.value) return ''
  return `${user.value.firstName} ${user.value.lastName}`
})

const statusText = computed(() => {
  if (!user.value) return ''
  
  switch (user.value.status) {
    case UserStatus._0: return 'Unknown Status'
    case UserStatus._1: return 'Inactive'
    case UserStatus._2: return 'Awaiting Approval'
    case UserStatus._3: return 'Approved'
    case UserStatus._4: return 'Administrator'
    default: return ''
  }
})

const statusIcon = computed(() => {
  if (!user.value) return ''
  
  switch (user.value.status) {
    case UserStatus._0: return 'pi pi-question-circle'
    case UserStatus._1: return 'pi pi-times-circle'
    case UserStatus._2: return 'pi pi-clock'
    case UserStatus._3: return 'pi pi-check-circle'
    case UserStatus._4: return 'pi pi-shield'
    default: return ''
  }
})

const statusClasses = computed(() => {
  if (!user.value) return ''
  
  switch (user.value.status) {
    case UserStatus._0: return 'text-gray-500'
    case UserStatus._1: return 'text-red-600'
    case UserStatus._2: return 'text-amber-600'
    case UserStatus._3: return 'text-green-600'
    case UserStatus._4: return 'text-blue-600'
    default: return ''
  }
})

const handleSignOut = () => {
  signOut()
  router.push('/')
}
</script>
