<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore();

</script>


<template>
  <div class="home">
    <div v-if="authStore.user" class="user-info">
      <h1>Welcome, {{ authStore.user.name }}!</h1>
      <div class="user-details">
        <p><strong>Username:</strong> {{ authStore.user.username }}</p>
        <p><strong>ID:</strong> {{ authStore.user.id }}</p>
      </div>
      <button @click="authStore.logout" class="logout-btn">Logout</button>
    </div>
    <div v-else class="login-prompt">
      <h2>Please log in to continue</h2>
      <router-link to="/login" class="login-btn">Login</router-link>
    </div>
  </div>
</template>

<style scoped>
.home {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  padding: 2rem;
  background-color: #f5f6fa;
  text-align: center;
}

.user-info {
  background: #fff;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.user-details {
  margin: 2rem 0;
  text-align: left;
}

.user-details p {
  margin: 0.5rem 0;
  font-size: 1.1rem;
}

.logout-btn {
  background: #ff4757;
  color: white;
  border: none;
  padding: 0.8rem 2rem;
  border-radius: 6px;
  font-size: 1rem;
  cursor: pointer;
  transition: background 0.3s ease;
}

.logout-btn:hover {
  background: #ff6b81;
}

.login-prompt {
  margin-top: 4rem;
}

.login-btn {
  display: inline-block;
  background: #2ed573;
  color: white;
  text-decoration: none;
  padding: 1rem 3rem;
  border-radius: 6px;
  font-size: 1.1rem;
  margin-top: 1rem;
  transition: background 0.3s ease;
}

.login-btn:hover {
  background: #7bed9f;
}
</style>
