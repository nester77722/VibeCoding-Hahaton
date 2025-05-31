<template>
    <div class="login">
        <h1>Login</h1>
        <form @submit.prevent="handleSubmit" class="login-form">
            <div class="form-group">
                <label for="username">Username</label>
                <input type="text" id="username" v-model="username" required autocomplete="username" />
            </div>
            <div class="form-group">
                <label for="password">Password</label>
                <input type="password" id="password" v-model="password" required autocomplete="current-password" />
            </div>
            <div v-if="error" class="error">{{ error }}</div>
            <button type="submit" :disabled="loading">
                {{ loading ? 'Logging in...' : 'Login' }}
            </button>
            <p class="register-link">
                Don't have an account?
                <router-link to="/register">Register</router-link>
            </p>
        </form>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const username = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

async function handleSubmit() {
    loading.value = true
    error.value = ''

    try {
        const success = await authStore.login({ username: username.value, password: password.value })
        if (success) {
            router.push('/')
        } else {
            error.value = 'Invalid username or password'
        }
    } catch (e) {
        error.value = 'An error occurred during login'
    } finally {
        loading.value = false
    }
}
</script>

<style scoped>
.login {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
    padding: 2rem;
    background-color: #f5f6fa;
}

.login h1 {
    text-align: center;
    color: #2c3e50;
    margin-bottom: 2rem;
}

.login-form {
    background: white;
    padding: 2rem;
    border-radius: 12px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    width: 100%;
    max-width: 400px;
}

.form-group {
    margin-bottom: 1.5rem;
}

label {
    display: block;
    margin-bottom: 0.5rem;
    color: #2c3e50;
    font-weight: 500;
}

input {
    width: 100%;
    padding: 0.8rem;
    border: 2px solid #dfe4ea;
    border-radius: 6px;
    font-size: 1rem;
    transition: border-color 0.3s ease;
}

input:focus {
    outline: none;
    border-color: #2ed573;
}

button {
    width: 100%;
    padding: 1rem;
    background: #2ed573;
    color: white;
    border: none;
    border-radius: 6px;
    font-size: 1rem;
    cursor: pointer;
    transition: background 0.3s ease;
}

button:hover:not(:disabled) {
    background: #7bed9f;
}

button:disabled {
    background: #a4b0be;
    cursor: not-allowed;
}

.error {
    color: #ff4757;
    margin-bottom: 1rem;
    font-size: 0.9rem;
}

.register-link {
    text-align: center;
    margin-top: 1.5rem;
    color: #2c3e50;
}

.register-link a {
    color: #2ed573;
    text-decoration: none;
    font-weight: 500;
}

.register-link a:hover {
    text-decoration: underline;
}
</style>