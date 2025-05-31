import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'
import type { User } from '@/types/user'

export interface RegisterRequest {
    username: string;
    password: string;
    name: string;
}

export interface LoginRequest {
    username: string;
    password: string;
}

export const useAuthStore = defineStore('auth', () => {
    const user = ref<User | null>(null);
    const token = ref<string | null>(null);

    const apiUrl = import.meta.env.VITE_CHAT_APP_API_URL;

    async function login(loginRequest: LoginRequest) {
        try {
            const response = await axios.post(`${apiUrl}/auth/login`, {
                username: loginRequest.username,
                password: loginRequest.password
            })
            token.value = response.data.token
            localStorage.setItem('token', response.data.token)
            await fetchUser()
            return true
        } catch (error) {
            console.error('Login failed:', error)
            return false
        }
    }

    async function register(registerRequest: RegisterRequest) {
        try {
            await axios.post(`${apiUrl}/auth/register`, {
                username: registerRequest.username,
                password: registerRequest.password,
                name: registerRequest.name
            })
            return true
        } catch (error) {
            console.error('Registration failed:', error)
            return false
        }
    }

    async function fetchUser() {
        try {
            const response = await axios.get(`${apiUrl}/users/me`, {
                headers: {
                    Authorization: `Bearer ${token.value}`
                }
            })
            user.value = response.data
        } catch (error) {
            console.error('Failed to fetch user:', error)
            logout()
        }
    }

    function logout() {
        user.value = null
        token.value = null
        localStorage.removeItem('token')
    }

    // Initialize from localStorage
    const storedToken = localStorage.getItem('token')
    if (storedToken) {
        token.value = storedToken
        fetchUser()
    }

    return {
        user,
        token,
        login,
        register,
        logout,
        fetchUser
    }
}) 