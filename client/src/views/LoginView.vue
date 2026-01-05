<script setup>
import { ref } from 'vue'
import { useAuthStore } from '../stores/auth'

const email = ref('')
const password = ref('')
const authStore = useAuthStore()
const errorMessage = ref('')

const handleSubmit = async () => {
  errorMessage.value = ''
  try {
    await authStore.login(email.value, password.value)
  } catch (e) {
    errorMessage.value = e.response?.data?.message || 'Login failed'
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-gray-100">
    <div class="w-full max-w-md rounded-lg bg-white p-8 shadow-md">
      <h2 class="mb-6 text-center text-2xl font-bold text-gray-800">Login</h2>
      
      <div v-if="errorMessage" class="mb-4 rounded bg-red-100 border border-red-400 text-red-700 px-4 py-3 relative" role="alert">
        <strong class="font-bold">Error: </strong>
        <span class="block sm:inline">{{ errorMessage }}</span>
      </div>

      <form @submit.prevent="handleSubmit">
        <div class="mb-4">
          <label class="mb-2 block text-sm font-bold text-gray-700" for="email">Email</label>
          <input v-model="email" class="w-full appearance-none rounded border px-3 py-2 leading-tight text-gray-700 shadow focus:outline-none focus:shadow-outline" id="email" type="email" placeholder="Email" required>
        </div>
        <div class="mb-6">
          <label class="mb-2 block text-sm font-bold text-gray-700" for="password">Password</label>
          <input v-model="password" class="w-full appearance-none rounded border px-3 py-2 leading-tight text-gray-700 shadow focus:outline-none focus:shadow-outline" id="password" type="password" placeholder="Password" required>
        </div>
        <div class="flex items-center justify-between">
          <button class="rounded bg-blue-500 px-4 py-2 font-bold text-white hover:bg-blue-700 focus:outline-none focus:shadow-outline" type="submit">
            Sign In
          </button>
          <router-link to="/register" class="inline-block align-baseline text-sm font-bold text-blue-500 hover:text-blue-800">
            Register
          </router-link>
        </div>
      </form>
    </div>
  </div>
</template>
