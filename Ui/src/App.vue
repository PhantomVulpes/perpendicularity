<script setup lang="ts">
import AppHeader from './components/AppHeader.vue'
import AppFooter from './components/AppFooter.vue'
import Toast from 'primevue/toast'
</script>

<template>
  <Toast />
  <div class="flex flex-col min-h-screen">
    <AppHeader />
    <main class="flex-1">
      <router-view />
    </main>
    <AppFooter />
  </div>
</template>

<style scoped>
</style>
