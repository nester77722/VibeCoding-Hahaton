<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <div class="app">
    <main class="main-content">
      <router-view></router-view>
    </main>
  </div>
</template>

<style>
.app {
  display: flex;
  height: 100vh;
  overflow: hidden;
}

.main-content {
  flex: 1;
  overflow-y: auto;
  background-color: #f8fafc;
}
</style>
