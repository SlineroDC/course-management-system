<script setup>
import { useAuthStore } from '../stores/auth'

const authStore = useAuthStore()
</script>

<template>
  <div class="min-h-screen bg-gray-100">
    <nav class="bg-white shadow">
      <div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div class="flex h-16 justify-between">
          <div class="flex">
            <div class="flex flex-shrink-0 items-center">
              <h1 class="text-xl font-bold text-gray-800">Course Manager</h1>
            </div>
          </div>
          <div class="flex items-center">
            <button @click="authStore.logout" class="rounded bg-red-500 px-3 py-2 text-sm font-medium text-white hover:bg-red-700">
              Logout
            </button>
          </div>
        </div>
      </div>
    </nav>

    <main>
      <div class="mx-auto max-w-7xl py-6 sm:px-6 lg:px-8">
        <div class="px-4 py-6 sm:px-0">
          <div class="h-96 rounded-lg border-4 border-dashed border-gray-200 p-4 text-center text-gray-500">
            Dashboard Content (Courses will be listed here)
          </div>
        </div>
      </div>
    </main>
  </div>
</template>
