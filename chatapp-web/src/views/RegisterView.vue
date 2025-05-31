<template>
  <div class="register">
    <h1>Register</h1>
    <form @submit.prevent="handleSubmit" class="register-form">
      <div class="form-group">
        <label for="username">Username</label>
        <input
          type="text"
          id="username"
          v-model="username"
          required
          autocomplete="username"
        />
      </div>
      <div class="form-group">
        <label for="name">Full Name</label>
        <input
          type="text"
          id="name"
          v-model="name"
          required
        />
      </div>
      <div class="form-group">
        <label for="password">Password</label>
        <input
          type="password"
          id="password"
          v-model="password"
          required
          autocomplete="new-password"
        />
      </div>
      <div class="form-group">
        <label for="confirmPassword">Confirm Password</label>
        <input
          type="password"
          id="confirmPassword"
          v-model="confirmPassword"
          required
          autocomplete="new-password"
        />
      </div>
      <div v-if="error" class="error">{{ error }}</div>
      <button type="submit" :disabled="loading || !isValid">
        {{ loading ? 'Registering...' : 'Register' }}
      </button>
      <p class="login-link">
        Already have an account?
        <router-link to="/login">Login</router-link>
      </p>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const username = ref('')
const name = ref('')
const password = ref('')
const confirmPassword = ref('')
const error = ref('')
const loading = ref(false)

const isValid = computed(() => {
  return (
    username.value.length >= 3 &&
    name.value.length >= 2 &&
    password.value.length >= 6 &&
    password.value === confirmPassword.value
  )
})

async function handleSubmit() {
  if (!isValid.value) {
    error.value = 'Please check your input'
    return
  }

  loading.value = true
  error.value = ''

  try {
    const success = await authStore.register({
      username: username.value,
      password: password.value,
      name: name.value
    })
    if (success) {
      router.push('/login')
    } else {
      error.value = 'Registration failed'
    }
  } catch (e) {
    error.value = 'An error occurred during registration'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.register {
  max-width: 400px;
  margin: 2rem auto;
  padding: 2rem;
}

.register h1 {
  text-align: center;
  color: #2c3e50;
  margin-bottom: 2rem;
}

.register-form {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
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

.login-link {
  text-align: center;
  margin-top: 1.5rem;
  color: #2c3e50;
}

.login-link a {
  color: #2ed573;
  text-decoration: none;
  font-weight: 500;
}

.login-link a:hover {
  text-decoration: underline;
}
</style> 