<script setup>
import { ref } from 'vue'
import { useAuthStore } from '../stores/auth'

const email = ref('')
const password = ref('')
const authStore = useAuthStore()

const handleSubmit = async () => {
  try {
    await authStore.register(email.value, password.value)
  } catch (e) {
    alert('Registration failed')
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-gray-100">
    <div class="w-full max-w-md rounded-lg bg-white p-8 shadow-md">
      <h2 class="mb-6 text-center text-2xl font-bold text-gray-800">Register</h2>
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
          <button class="rounded bg-green-500 px-4 py-2 font-bold text-white hover:bg-green-700 focus:outline-none focus:shadow-outline" type="submit">
            Register
          </button>
          <router-link to="/login" class="inline-block align-baseline text-sm font-bold text-blue-500 hover:text-blue-800">
            Login
          </router-link>
        </div>
      </form>
    </div>
  </div>
</template>
