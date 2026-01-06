<script setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '../stores/auth'
import { useCourseStore } from '../stores/courses'

const authStore = useAuthStore()
const courseStore = useCourseStore()

const metrics = ref({
  totalCourses: 0,
  publishedCourses: 0,
  totalLessons: 0
})

const loading = ref(true)

onMounted(async () => {
  await loadMetrics()
})

const loadMetrics = async () => {
  loading.value = true
  try {
    const data = await courseStore.fetchMetrics()
    metrics.value = data
  } catch (e) {
    console.error('Failed to load metrics:', e)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-gray-100">
    <nav class="bg-white shadow">
      <div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div class="flex h-16 justify-between">
          <div class="flex">
            <div class="flex flex-shrink-0 items-center">
              <h1 class="text-xl font-bold text-gray-800">Metrics Dashboard</h1>
            </div>
          </div>
          <div class="flex items-center gap-4">
            <router-link to="/" class="text-indigo-600 hover:text-indigo-800 font-medium">
              ← Back to Courses
            </router-link>
            <span class="text-gray-600">{{ authStore.user?.email }}</span>
            <button @click="authStore.logout" class="rounded bg-red-500 px-3 py-2 text-sm font-medium text-white hover:bg-red-700">
              Logout
            </button>
          </div>
        </div>
      </div>
    </nav>

    <main>
      <div class="mx-auto max-w-7xl py-12 sm:px-6 lg:px-8">
        
        <!-- Loading State -->
        <div v-if="loading" class="flex justify-center items-center h-64">
          <div class="text-gray-500 text-lg">Loading metrics...</div>
        </div>

        <!-- Metrics Cards -->
        <div v-else class="grid gap-8 md:grid-cols-3">
          
          <!-- Total Courses Card -->
          <div class="rounded-xl bg-white p-8 shadow-lg transition hover:shadow-xl">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-gray-600 uppercase tracking-wide">Total Courses</p>
                <p class="mt-3 text-5xl font-bold text-indigo-600">{{ metrics.totalCourses }}</p>
              </div>
              <div class="rounded-full bg-indigo-100 p-4">
                <svg class="h-10 w-10 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
                </svg>
              </div>
            </div>
            <p class="mt-4 text-sm text-gray-500">All courses in the system</p>
          </div>

          <!-- Published Courses Card -->
          <div class="rounded-xl bg-white p-8 shadow-lg transition hover:shadow-xl">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-gray-600 uppercase tracking-wide">Published</p>
                <p class="mt-3 text-5xl font-bold text-green-600">{{ metrics.publishedCourses }}</p>
              </div>
              <div class="rounded-full bg-green-100 p-4">
                <svg class="h-10 w-10 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
            </div>
            <p class="mt-4 text-sm text-gray-500">Courses available to students</p>
          </div>

          <!-- Total Lessons Card -->
          <div class="rounded-xl bg-white p-8 shadow-lg transition hover:shadow-xl">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-gray-600 uppercase tracking-wide">Total Lessons</p>
                <p class="mt-3 text-5xl font-bold text-slate-600">{{ metrics.totalLessons }}</p>
              </div>
              <div class="rounded-full bg-slate-100 p-4">
                <svg class="h-10 w-10 text-slate-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01" />
                </svg>
              </div>
            </div>
            <p class="mt-4 text-sm text-gray-500">Content across all courses</p>
          </div>

        </div>

        <!-- Additional Info -->
        <div class="mt-12 rounded-xl bg-white p-8 shadow-lg">
          <h2 class="text-2xl font-bold text-gray-800 mb-4">System Overview</h2>
          <div class="grid gap-4 md:grid-cols-2">
            <div class="flex items-center gap-3 p-4 rounded-lg bg-gray-50">
              <div class="text-3xl">📊</div>
              <div>
                <p class="font-semibold text-gray-800">Average Lessons per Course</p>
                <p class="text-2xl font-bold text-indigo-600">
                  {{ metrics.totalCourses > 0 ? (metrics.totalLessons / metrics.totalCourses).toFixed(1) : '0' }}
                </p>
              </div>
            </div>
            <div class="flex items-center gap-3 p-4 rounded-lg bg-gray-50">
              <div class="text-3xl">✅</div>
              <div>
                <p class="font-semibold text-gray-800">Publication Rate</p>
                <p class="text-2xl font-bold text-green-600">
                  {{ metrics.totalCourses > 0 ? ((metrics.publishedCourses / metrics.totalCourses) * 100).toFixed(0) : '0' }}%
                </p>
              </div>
            </div>
          </div>
        </div>

      </div>
    </main>
  </div>
</template>
