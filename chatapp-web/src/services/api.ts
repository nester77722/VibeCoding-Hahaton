import axios from 'axios'
import { useAuthStore } from '@/stores/auth'
import router from '@/router'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor
api.interceptors.request.use(
  (config) => {
    const authStore = useAuthStore()
    if (authStore.token) {
      config.headers.Authorization = `Bearer ${authStore.token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      const { status } = error.response
      const authStore = useAuthStore()

      switch (status) {
        case 401:
          // Unauthorized - clear auth state and redirect to login
          authStore.logout()
          router.push('/login')
          break
        case 404:
          // Not found - redirect to 404 page
          router.push('/404')
          break
      }
    }
    return Promise.reject(error)
  }
)

export default api 