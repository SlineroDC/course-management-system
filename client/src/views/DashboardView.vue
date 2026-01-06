<script setup>
import { ref, onMounted, watch } from 'vue'
import { useAuthStore } from '../stores/auth'
import { useCourseStore } from '../stores/courses'

const authStore = useAuthStore()
const courseStore = useCourseStore()

const newCourseTitle = ref('')
const selectedStatus = ref('All')
const actionError = ref('')

// Edit Course Modal State
const showEditCourseModal = ref(false)
const editingCourseId = ref(null)
const editingCourseTitle = ref('')

// Lesson Modal State
const showLessonModal = ref(false)
const selectedCourse = ref(null)
const newLessonTitle = ref('')
const newLessonOrder = ref(1)

// Edit Lesson State
const editingLessonId = ref(null)
const editingLessonTitle = ref('')
const editingLessonOrder = ref(null)
const lessonError = ref('')

onMounted(() => {
  loadCourses()
})

watch(selectedStatus, () => {
  loadCourses(1)
})

const loadCourses = (page = 1) => {
  courseStore.fetchCourses(page, selectedStatus.value)
}

const handleCreateCourse = async () => {
  if (!newCourseTitle.value) return
  actionError.value = ''
  try {
    await courseStore.createCourse(newCourseTitle.value)
    newCourseTitle.value = ''
  } catch (e) {
    actionError.value = e.response?.data?.message || 'Failed to create course'
    setTimeout(() => actionError.value = '', 5000)
  }
}

const openEditCourseModal = (course) => {
  editingCourseId.value = course.id
  editingCourseTitle.value = course.title
  showEditCourseModal.value = true
}

const handleUpdateCourse = async () => {
  actionError.value = ''
  try {
    await courseStore.updateCourse(editingCourseId.value, editingCourseTitle.value)
    showEditCourseModal.value = false
  } catch (e) {
    actionError.value = e.response?.data?.message || 'Failed to update course'
    setTimeout(() => actionError.value = '', 5000)
  }
}

const handleDelete = async (id, hardDelete = false) => {
  const msg = hardDelete 
    ? 'Are you sure you want to PERMANENTLY delete this course? This cannot be undone.' 
    : 'Are you sure you want to delete this course?'
    
  if (confirm(msg)) {
    try {
      await courseStore.deleteCourse(id, hardDelete)
    } catch (e) {
      alert(e.response?.data?.message || 'Failed to delete course')
    }
  }
}

const handlePublish = async (id) => {
  actionError.value = ''
  try {
    await courseStore.publishCourse(id)
  } catch (e) {
    actionError.value = e.response?.data?.message || 'Failed to publish course'
    setTimeout(() => actionError.value = '', 5000)
  }
}

const handleUnpublish = async (id) => {
  actionError.value = ''
  try {
    await courseStore.unpublishCourse(id)
  } catch (e) {
    actionError.value = e.response?.data?.message || 'Failed to unpublish course'
    setTimeout(() => actionError.value = '', 5000)
  }
}

// Lesson Management
const openLessonModal = (course) => {
  selectedCourse.value = course
  // Calculate next order
  const maxOrder = course.lessons && course.lessons.length > 0 
    ? Math.max(...course.lessons.map(l => l.order)) 
    : 0
  newLessonOrder.value = maxOrder + 1
  showLessonModal.value = true
  editingLessonId.value = null // Reset edit state
}

const handleAddLesson = async () => {
  lessonError.value = ''
  try {
    if (newLessonOrder.value <= 0) {
      lessonError.value = 'Lesson order must be greater than 0'
      return
    }
    await courseStore.createLesson(selectedCourse.value.id, newLessonTitle.value, newLessonOrder.value)
    await loadCourses(courseStore.currentPage)
    
    // Update selectedCourse ref from the refreshed list
    const updatedCourse = courseStore.courses.find(c => c.id === selectedCourse.value.id)
    if (updatedCourse) selectedCourse.value = updatedCourse
    
    newLessonTitle.value = ''
    newLessonOrder.value = updatedCourse ? (Math.max(...updatedCourse.lessons.map(l => l.order)) + 1) : 1
  } catch (e) {
    lessonError.value = e.response?.data?.message || 'Failed to add lesson'
  }
}

const startEditLesson = (lesson) => {
  editingLessonId.value = lesson.id
  editingLessonTitle.value = lesson.title
  editingLessonOrder.value = lesson.order
  lessonError.value = ''
}

const saveEditLesson = async (lesson) => {
  lessonError.value = ''
  try {
    if (editingLessonOrder.value <= 0) {
      lessonError.value = 'Lesson order must be greater than 0'
      return
    }
    await courseStore.updateLesson(lesson.id, lesson.courseId, editingLessonTitle.value, editingLessonOrder.value)
    editingLessonId.value = null
    await loadCourses(courseStore.currentPage)
    const updatedCourse = courseStore.courses.find(c => c.id === selectedCourse.value.id)
    if (updatedCourse) selectedCourse.value = updatedCourse
  } catch (e) {
    lessonError.value = e.response?.data?.message || 'Failed to update lesson'
  }
}

const cancelEditLesson = () => {
  editingLessonId.value = null
  editingLessonTitle.value = ''
  editingLessonOrder.value = null
  lessonError.value = ''
}

const handleDeleteLesson = async (lessonId) => {
  if(!confirm('Delete lesson?')) return
  lessonError.value = ''
  try {
    await courseStore.deleteLesson(lessonId)
    await loadCourses(courseStore.currentPage)
    const updatedCourse = courseStore.courses.find(c => c.id === selectedCourse.value.id)
    if (updatedCourse) selectedCourse.value = updatedCourse
  } catch (e) {
    lessonError.value = e.response?.data?.message || 'Failed to delete lesson'
  }
}

const moveLesson = async (lesson, direction) => {
  lessonError.value = ''
  try {
    if (direction === 'up') {
      await courseStore.moveUpLesson(lesson.id)
    } else {
      await courseStore.moveDownLesson(lesson.id)
    }
    await loadCourses(courseStore.currentPage)
    const updatedCourse = courseStore.courses.find(c => c.id === selectedCourse.value.id)
    if (updatedCourse) selectedCourse.value = updatedCourse
  } catch (e) {
    lessonError.value = e.response?.data?.message || `Failed to move lesson ${direction}`
  }
}

const swapLessons = async (lessonA, lessonB) => {
  try {
    const orderA = lessonA.order
    const orderB = lessonB.order
    
    // Optimistic update logic handled by backend calls
    
    await courseStore.updateLesson(lessonA.id, lessonA.courseId, lessonA.title, -1)
    await courseStore.updateLesson(lessonB.id, lessonB.courseId, lessonB.title, orderA)
    await courseStore.updateLesson(lessonA.id, lessonA.courseId, lessonA.title, orderB)
    
    await loadCourses(courseStore.currentPage)
    const updatedCourse = courseStore.courses.find(c => c.id === selectedCourse.value.id)
    if (updatedCourse) selectedCourse.value = updatedCourse
  } catch (e) {
    alert('Failed to reorder lessons')
    await loadCourses(courseStore.currentPage) // Revert
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
              <h1 class="text-xl font-bold text-gray-800">Course Manager</h1>
            </div>
          </div>
          <div class="flex items-center gap-4">
            <router-link to="/metrics" class="text-indigo-600 hover:text-indigo-800 font-medium">
              📊 Metrics
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
      <div class="mx-auto max-w-7xl py-6 sm:px-6 lg:px-8">
        
        <!-- Error Message -->
        <div v-if="actionError" class="mb-4 rounded bg-red-100 border border-red-400 text-red-700 px-4 py-3 relative">
          <strong class="font-bold">Error: </strong>
          <span class="block sm:inline">{{ actionError }}</span>
        </div>

        <!-- Controls -->
        <div class="mb-6 flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
          <!-- Create Course -->
          <div class="flex gap-2">
            <input v-model="newCourseTitle" type="text" placeholder="New Course Title" class="rounded border px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500">
            <button @click="handleCreateCourse" class="rounded bg-blue-600 px-4 py-2 font-bold text-white hover:bg-blue-700">Create</button>
          </div>

          <!-- Filter -->
          <div class="flex items-center gap-2">
            <label class="font-bold text-gray-700">Status:</label>
            <select v-model="selectedStatus" class="rounded border px-3 py-2">
              <option value="All">All</option>
              <option value="Draft">Draft</option>
              <option value="Published">Published</option>
            </select>
          </div>
        </div>

        <!-- Course List -->
        <div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          <div v-for="course in courseStore.courses" :key="course.id" class="rounded-lg bg-white p-6 shadow transition hover:shadow-lg">
            <div class="mb-4 flex items-start justify-between">
              <h3 class="text-xl font-bold text-gray-900">{{ course.title }}</h3>
              <span :class="{'bg-green-100 text-green-800': course.status === 'Published', 'bg-gray-100 text-gray-800': course.status !== 'Published'}" class="rounded-full px-2 py-1 text-xs font-semibold">
                {{ course.status }}
              </span>
            </div>
            
            <div class="mb-4 text-sm text-gray-600">
              Lessons: {{ course.lessons ? course.lessons.length : 0 }}
            </div>

            <div class="flex flex-wrap gap-2">
              <button @click="openLessonModal(course)" class="rounded bg-indigo-100 px-3 py-1 text-sm font-medium text-indigo-700 hover:bg-indigo-200">
                Manage Lessons
              </button>
              
              <button @click="openEditCourseModal(course)" class="rounded bg-blue-100 px-3 py-1 text-sm font-medium text-blue-700 hover:bg-blue-200">
                Edit
              </button>

              <button v-if="course.status === 'Draft'" @click="handlePublish(course.id)" class="rounded bg-green-100 px-3 py-1 text-sm font-medium text-green-700 hover:bg-green-200">
                Publish
              </button>
              <button v-else @click="handleUnpublish(course.id)" class="rounded bg-yellow-100 px-3 py-1 text-sm font-medium text-yellow-700 hover:bg-yellow-200">
                Unpublish
              </button>
              
              <button @click="handleDelete(course.id)" class="rounded bg-red-100 px-3 py-1 text-sm font-medium text-red-700 hover:bg-red-200">
                Delete
              </button>

              <button v-if="authStore.user?.roles?.includes('Admin')" @click="handleDelete(course.id, true)" class="rounded bg-red-600 px-3 py-1 text-sm font-medium text-white hover:bg-red-700">
                Hard Delete
              </button>
            </div>
          </div>
        </div>

        <!-- Pagination -->
        <div class="mt-8 flex justify-center gap-4">
          <button 
            @click="loadCourses(courseStore.currentPage - 1)" 
            :disabled="courseStore.currentPage === 1"
            class="rounded bg-gray-200 px-4 py-2 font-bold text-gray-700 disabled:opacity-50 hover:bg-gray-300">
            Previous
          </button>
          <span class="flex items-center font-bold text-gray-700">
            Page {{ courseStore.currentPage }} of {{ courseStore.totalPages }}
          </span>
          <button 
            @click="loadCourses(courseStore.currentPage + 1)" 
            :disabled="courseStore.currentPage === courseStore.totalPages"
            class="rounded bg-gray-200 px-4 py-2 font-bold text-gray-700 disabled:opacity-50 hover:bg-gray-300">
            Next
          </button>
        </div>

        <!-- Edit Course Modal -->
        <div v-if="showEditCourseModal" class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 p-4">
          <div class="w-full max-w-md rounded-lg bg-white p-6">
            <h3 class="mb-4 text-lg font-bold">Edit Course</h3>
            <form @submit.prevent="handleUpdateCourse">
              <div class="mb-4">
                <label class="mb-2 block text-sm font-bold text-gray-700">Title</label>
                <input v-model="editingCourseTitle" type="text" class="w-full rounded border px-3 py-2" required>
              </div>
              <div class="flex justify-end gap-2">
                <button type="button" @click="showEditCourseModal = false" class="rounded bg-gray-300 px-4 py-2 font-bold text-gray-700 hover:bg-gray-400">Cancel</button>
                <button type="submit" class="rounded bg-blue-600 px-4 py-2 font-bold text-white hover:bg-blue-700">Save</button>
              </div>
            </form>
          </div>
        </div>

        <!-- Lesson Modal -->
        <div v-if="showLessonModal" class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 p-4 overflow-y-auto">
          <div class="w-full max-w-2xl rounded-lg bg-white p-6">
            <div class="mb-4 flex items-center justify-between">
              <h3 class="text-xl font-bold">Manage Lessons: {{ selectedCourse?.title }}</h3>
              <button @click="showLessonModal = false" class="text-gray-500 hover:text-gray-700">✕</button>
            </div>

            <!-- Add Lesson Form -->
            <form @submit.prevent="handleAddLesson" class="mb-6 rounded bg-gray-50 p-4">
              <div v-if="lessonError" class="mb-3 rounded bg-red-100 border border-red-400 text-red-700 px-3 py-2 text-sm">
                {{ lessonError }}
              </div>
              <div class="flex gap-2">
                <input v-model="newLessonTitle" type="text" placeholder="Lesson Title" class="flex-1 rounded border px-3 py-2" required>
                <input v-model.number="newLessonOrder" type="number" min="1" placeholder="Order" class="w-20 rounded border px-3 py-2" required>
                <button type="submit" class="rounded bg-blue-600 px-4 py-2 font-bold text-white hover:bg-blue-700">Add</button>
              </div>
            </form>

            <!-- Lesson List -->
            <div class="space-y-2">
              <div v-for="lesson in selectedCourse?.lessons" :key="lesson.id" class="flex items-center justify-between rounded border p-3">
                
                <!-- View Mode -->
                <div v-if="editingLessonId !== lesson.id" class="flex flex-1 items-center justify-between">
                  <div class="flex items-center gap-3">
                    <span class="font-mono font-bold text-gray-500">#{{ lesson.order }}</span>
                    <span class="font-medium">{{ lesson.title }}</span>
                  </div>
                  <div class="flex items-center gap-2">
                    <button @click="startEditLesson(lesson)" class="text-blue-600 hover:text-blue-800">Edit</button>
                    <button @click="moveLesson(lesson, 'up')" class="rounded p-1 hover:bg-gray-100" title="Move Up">🔼</button>
                    <button @click="moveLesson(lesson, 'down')" class="rounded p-1 hover:bg-gray-100" title="Move Down">🔽</button>
                    <button @click="handleDeleteLesson(lesson.id)" class="ml-2 text-red-500 hover:text-red-700">Delete</button>
                  </div>
                </div>

                <!-- Edit Mode -->
                <div v-else class="flex flex-1 items-center gap-2">
                  <input v-model="editingLessonTitle" type="text" class="flex-1 rounded border px-2 py-1">
                  <input v-model.number="editingLessonOrder" type="number" min="1" class="w-16 rounded border px-2 py-1">
                  <button @click="saveEditLesson(lesson)" class="text-green-600 hover:text-green-800 font-medium">Save</button>
                  <button @click="cancelEditLesson" class="text-gray-500 hover:text-gray-700">Cancel</button>
                </div>

              </div>
              <div v-if="!selectedCourse?.lessons?.length" class="text-center text-gray-500">
                No lessons yet.
              </div>
            </div>
          </div>
        </div>

      </div>
    </main>
  </div>
</template>
