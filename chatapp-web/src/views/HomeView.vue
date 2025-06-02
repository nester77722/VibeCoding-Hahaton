<script setup lang="ts">
import { ref } from 'vue'
import { useAuthStore } from '@/stores/auth'
import Sidebar from '@/components/Sidebar.vue'

const authStore = useAuthStore()
const sidebarRef = ref<InstanceType<typeof Sidebar> | null>(null)

function highlightSidebar() {
  if (sidebarRef.value) {
    sidebarRef.value.highlight()
  }
}
</script>

<template>
  <div class="home">
    <Sidebar ref="sidebarRef" class="home__sidebar" />
    <main class="home__main">
      <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>
      <div v-if="$route.path === '/'" class="welcome">
        <h1>Welcome to VibeCoding Chat</h1>
        <p class="subtitle">Start connecting with your team members!</p>

        <div class="quick-actions">
          <div class="action-card" @click="highlightSidebar">
            <div class="action-card__icon">💬</div>
            <h2>View your conversations</h2>
            <p>Check your messages and groups</p>
          </div>

          <div class="action-card" @click="authStore.logout">
            <div class="action-card__icon">👋</div>
            <h2>Logout</h2>
            <p>Sign out of your account</p>
          </div>
        </div>

        <div class="user-info" v-if="authStore.user">
          <p>Logged in as <strong>{{ authStore.user.name }}</strong></p>
          <p class="username">{{ authStore.user.username }}</p>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
.home {
  display: flex;
  height: 100vh;
  background: #f8fafc;
  overflow: hidden;
}

.home__sidebar {
  flex-shrink: 0;
}

.home__main {
  flex: 1;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.welcome {
  padding: 2rem;
  text-align: center;
  max-width: 800px;
  margin: 0 auto;
  height: 100%;
  overflow-y: auto;
}

h1 {
  font-size: 2.5rem;
  color: #1e293b;
  margin-bottom: 1rem;
}

.subtitle {
  font-size: 1.25rem;
  color: #64748b;
  margin-bottom: 3rem;
}

.quick-actions {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.action-card {
  background: white;
  padding: 2rem;
  border-radius: 1rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s, box-shadow 0.2s;
  cursor: pointer;
}

.action-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 12px -1px rgba(0, 0, 0, 0.1);
}

.action-card__icon {
  font-size: 2.5rem;
  margin-bottom: 1rem;
}

.action-card h2 {
  font-size: 1.5rem;
  color: #1e293b;
  margin-bottom: 0.5rem;
}

.action-card p {
  color: #64748b;
}

.user-info {
  margin-top: 2rem;
  padding: 1rem;
  background: white;
  border-radius: 0.5rem;
  display: inline-block;
}

.user-info p {
  margin: 0;
}

.username {
  color: #64748b;
  font-size: 0.875rem;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
