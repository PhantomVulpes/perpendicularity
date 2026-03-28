<template>
  <footer class="bg-gray-800 text-gray-300 py-4 mt-auto">
    <div class="container mx-auto px-4">
      <div class="flex items-center justify-center gap-4 text-sm">
        <span>© 2026</span>
        <span class="text-gray-500">|</span>
        <span>API Version: {{ apiVersion }}</span>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
import { ref } from 'vue'

// TODO: Replace with TanStack Query to fetch from API
// API should expose ApplicationConfiguration.Version (currently "Alpha 0.1")
const apiVersion = ref('Alpha 0.1')
</script>
