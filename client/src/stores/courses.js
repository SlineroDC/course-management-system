import { defineStore } from 'pinia'
import axios from '../axios'

export const useCourseStore = defineStore('courses', {
  state: () => ({
    courses: [],
    totalItems: 0,
    totalPages: 0,
    currentPage: 1,
    loading: false,
    error: null
  }),
  actions: {
    async fetchCourses(page = 1, status = '') {
      this.loading = true
      try {
        const params = { pageNumber: page, pageSize: 10 }
        if (status && status !== 'All') {
          params.status = status
        }
        
        const response = await axios.get('/courses', { params })
        this.courses = response.data.items
        this.totalItems = response.data.totalItems
        this.totalPages = response.data.totalPages
        this.currentPage = response.data.currentPage
      } catch (e) {
        this.error = e.response?.data?.message || 'Failed to fetch courses'
      } finally {
        this.loading = false
      }
    },
    async createCourse(title) {
      try {
        await axios.post('/courses', { title })
        await this.fetchCourses(this.currentPage) // Refresh
      } catch (e) {
        throw e
      }
    },
    async updateCourse(id, title) {
      try {
        await axios.put(`/courses/${id}`, { title })
        await this.fetchCourses(this.currentPage)
      } catch (e) {
        throw e
      }
    },
    async deleteCourse(id) {
      try {
        await axios.delete(`/courses/${id}`)
        await this.fetchCourses(this.currentPage) // Refresh
      } catch (e) {
        throw e
      }
    },
    async publishCourse(id) {
      try {
        await axios.post(`/courses/${id}/publish`)
        // Optimistic update or refresh
        const course = this.courses.find(c => c.id === id)
        if (course) course.status = 'Published'
      } catch (e) {
        throw e
      }
    },
    async unpublishCourse(id) {
      try {
        await axios.post(`/courses/${id}/unpublish`)
        const course = this.courses.find(c => c.id === id)
        if (course) course.status = 'Draft'
      } catch (e) {
        throw e
      }
    },
    async createLesson(courseId, title, order) {
      try {
        await axios.post('/lessons', { courseId, title, order })
      } catch (e) {
        throw e
      }
    },
    async updateLesson(id, courseId, title, order) {
      try {
        await axios.put(`/lessons/${id}`, { courseId, title, order })
      } catch (e) {
        throw e
      }
    },
    async deleteLesson(id) {
      try {
        await axios.delete(`/lessons/${id}`)
      } catch (e) {
        throw e
      }
    },
    async moveUpLesson(id) {
      try {
        await axios.put(`/lessons/${id}/move-up`)
      } catch (e) {
        throw e
      }
    },
    async moveDownLesson(id) {
      try {
        await axios.put(`/lessons/${id}/move-down`)
      } catch (e) {
        throw e
      }
    }
  }
})
