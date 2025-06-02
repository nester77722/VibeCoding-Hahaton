<template>
    <div v-if="authStore.user">
        <div :class="['sidebar', { 'sidebar--collapsed': !isOpen, 'sidebar--highlight': isHighlighted }]">
    <!-- Toggle Button -->
    <button class="sidebar__toggle" @click="isOpen = !isOpen">
      <span v-if="isOpen">◀</span>
      <span v-else>▶</span>
    </button>

    <!-- Main Content -->
    <div v-if="isOpen" class="sidebar__content">
      <!-- Tab Buttons -->
      <div class="sidebar__tabs">
        <button
          :class="['tab-button', { active: activeTab === 'contacts' }]"
          @click="handleTabChange('contacts')"
        >
          Contacts
        </button>
        <button
          :class="['tab-button', { active: activeTab === 'groups' }]"
          @click="handleTabChange('groups')"
        >
          Groups
        </button>
      </div>

      <!-- Search Box -->
      <div class="sidebar__search">
        <input
          type="text"
          v-model="searchQuery"
          :placeholder="activeTab === 'contacts' ? 'Search contacts...' : 'Search groups...'"
          class="search-input"
        />
        <div v-if="isLoading" class="loading-indicator">
          <span class="loading-spinner"></span>
        </div>
      </div>

      <!-- List -->
      <div class="sidebar__list">
        <template v-if="activeTab === 'contacts'">
          <router-link
            v-for="contact in contactsStore.contacts"
            :key="contact.id"
            :to="{ name: 'contact-messages', params: { contactId: contact.id }}"
            class="list-item"
            active-class="list-item--active"
          >
            <div class="item-info">
              <div class="item-name">{{ contact.username }}</div>
            </div>
          </router-link>
        </template>

        <template v-else>
          <router-link
            v-for="group in groupsStore.groups"
            :key="group.id"
            :to="{ name: 'group-messages', params: { groupId: group.id }}"
            class="list-item"
            active-class="list-item--active"
          >
            <div class="item-info">
              <div class="item-name">{{ group.name }}</div>
              <div class="item-members">{{ group.members.length }} members</div>
            </div>
          </router-link>
        </template>
      </div>

      <!-- Logout Button -->
      <div class="sidebar__footer">
        <div class="user-info">
          <div class="user-info__name">{{ authStore.user?.name }}</div>
          <div class="user-info__username">{{ authStore.user?.username }}</div>
        </div>
        <button class="logout-button" @click="logout()">
          <span class="logout-button__icon">👋</span>
          <span class="logout-button__text">Logout</span>
        </button>
      </div>
    </div>
  </div>
    </div>
  
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useContactsStore } from '@/stores/contacts'
import { useGroupsStore } from '@/stores/groups'
import debounce from 'lodash/debounce'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

type Tab = 'contacts' | 'groups'

const authStore = useAuthStore();

const contactsStore = useContactsStore()
const groupsStore = useGroupsStore()

const isOpen = ref(true)
const activeTab = ref<Tab>('contacts')
const searchQuery = ref('')
const router = useRouter();

const isLoading = computed(() => 
  activeTab.value === 'contacts' ? contactsStore.isLoading : groupsStore.isLoading
)

const isHighlighted = ref(false)

// Initialize data
onMounted(async () => {
  if (activeTab.value === 'contacts') {
    await contactsStore.fetchContacts()
  } else {
    await groupsStore.fetchGroups()
  }
})

function logout(){
  authStore.logout();
  router.push('/login');
}

// Handle tab changes
async function handleTabChange(tab: Tab) {
  activeTab.value = tab
  searchQuery.value = ''
  if (tab === 'contacts') {
    await contactsStore.fetchContacts()
  } else {
    await groupsStore.fetchGroups()
  }
}

// Debounced search function
const debouncedSearch = debounce(async (query: string) => {
  if (activeTab.value === 'contacts') {
    await contactsStore.searchContacts(query)
  } else {
    await groupsStore.searchGroups(query)
  }
}, 1000)

// Watch for search query changes
watch(searchQuery, (newQuery) => {
  debouncedSearch(newQuery)
})

function highlight() {
  isHighlighted.value = true
  setTimeout(() => {
    isHighlighted.value = false
  }, 2000)
}

defineExpose({ highlight })
</script>

<style scoped>
.sidebar {
  display: flex;
  flex-direction: column;
  height: 100vh;
  width: 300px;
  min-width: 300px;
  background: white;
  border-right: 1px solid #e2e8f0;
  transition: width 0.3s ease;
  position: relative;
}

.sidebar--collapsed {
  width: 50px;
  min-width: 50px;
}

.sidebar__toggle {
  position: absolute;
  right: -12px;
  top: 20px;
  width: 24px;
  height: 24px;
  background: #2ed573;
  border: none;
  border-radius: 50%;
  color: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10;
}

.sidebar__content {
  flex: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  padding: 1rem;
}

.sidebar__tabs {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.tab-button {
  flex: 1;
  padding: 0.5rem;
  border: none;
  background: #f1f5f9;
  color: #64748b;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.tab-button.active {
  background: #2ed573;
  color: white;
}

.sidebar__search {
  margin-bottom: 1rem;
  position: relative;
}

.search-input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  outline: none;
  transition: border-color 0.2s ease;
}

.search-input:focus {
  border-color: #2ed573;
}

.loading-indicator {
  position: absolute;
  right: 1.5rem;
  top: 50%;
  transform: translateY(-50%);
}

.loading-spinner {
  display: inline-block;
  width: 16px;
  height: 16px;
  border: 2px solid #e2e8f0;
  border-top-color: #2ed573;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

.sidebar__list {
  flex: 1;
  overflow-y: auto;
}

.list-item {
  display: flex;
  align-items: center;
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 0.5rem;
  text-decoration: none;
  color: inherit;
  transition: background-color 0.2s ease;
}

.list-item:hover {
  background-color: #f1f5f9;
}

.list-item--active {
  background-color: #e2e8f0;
}

.item-info {
  flex: 1;
  min-width: 0;
}

.item-name {
  font-weight: 500;
  margin-bottom: 0.25rem;
}

.item-members {
  font-size: 0.875rem;
  color: #64748b;
  margin-bottom: 0.25rem;
}

.item-preview {
  font-size: 0.875rem;
  color: #64748b;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.sidebar--highlight {
  animation: highlight 2s ease-in-out;
}

@keyframes highlight {
  0%, 100% {
    box-shadow: none;
  }
  50% {
    box-shadow: 0 0 20px rgba(46, 213, 115, 0.5);
  }
}

.sidebar__footer {
  padding: 1rem;
  border-top: 1px solid #e2e8f0;
  background: white;
}

.user-info {
  margin-bottom: 1rem;
  padding: 0.5rem;
  border-radius: 0.5rem;
  background: #f8fafc;
}

.user-info__name {
  font-weight: 500;
  color: #1e293b;
  margin-bottom: 0.25rem;
}

.user-info__username {
  font-size: 0.875rem;
  color: #64748b;
}

.logout-button {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.75rem;
  background: #ef4444;
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.logout-button:hover {
  background: #dc2626;
}

.logout-button__icon {
  font-size: 1.25rem;
}
</style> 