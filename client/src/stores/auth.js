import { defineStore } from 'pinia'
import axios from '../axios'
import router from '../router'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: localStorage.getItem('token') || null
  }),
  actions: {
    async login(email, password) {
      try {
        const response = await axios.post('/auth/login', { email, password })
        this.token = response.data.token
        this.user = { email: response.data.email }
        localStorage.setItem('token', this.token)
        router.push('/')
      } catch (error) {
        console.error('Login failed', error)
        throw error
      }
    },
    async register(email, password) {
      try {
        const response = await axios.post('/auth/register', { email, password })
        this.token = response.data.token
        this.user = { email: response.data.email }
        localStorage.setItem('token', this.token)
        router.push('/')
      } catch (error) {
        console.error('Registration failed', error)
        throw error
      }
    },
    logout() {
      this.token = null
      this.user = null
      localStorage.removeItem('token')
      router.push('/login')
    }
  }
})
