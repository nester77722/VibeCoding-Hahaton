import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { GroupWithMessage } from '@/types/groups'
import api from '@/services/api'

export const useGroupsStore = defineStore('groups', () => {
    const groups = ref<GroupWithMessage[]>([])
    const allGroups = ref<GroupWithMessage[]>([])
    const isLoading = ref(false)
    const apiUrl = import.meta.env.VITE_CHAT_APP_API_URL;

    async function fetchGroups() {
        try {
            isLoading.value = true
            const response = await api.get(`${apiUrl}/groups`)
            groups.value = response.data
            allGroups.value = response.data
        } catch (error) {
            console.error('Failed to fetch groups:', error)
        } finally {
            isLoading.value = false
        }
    }

    function searchGroups(query: string) {
        if (!query.trim()) {
            groups.value = allGroups.value
            return
        }

        const searchTerm = query.toLowerCase().trim()
        groups.value = allGroups.value.filter(group => 
            group.name.toLowerCase().includes(searchTerm)
        )
    }

    return {
        groups,
        isLoading,
        fetchGroups,
        searchGroups
    }
}) 