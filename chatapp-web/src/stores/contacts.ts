import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Contact } from '@/types/contacts'
import api from '@/services/api'

export const useContactsStore = defineStore('contacts', () => {
    const contacts = ref<Contact[]>([])
    const allContacts = ref<Contact[]>([])
    const isLoading = ref(false)
    const apiUrl = import.meta.env.VITE_CHAT_APP_API_URL;

    async function fetchContacts() {
        try {
            isLoading.value = true
            const response = await api.get(`${apiUrl}/contacts`)
            contacts.value = response.data
            allContacts.value = response.data
            console.log(contacts.value);
        } catch (error) {
            console.error('Failed to fetch contacts:', error)
        } finally {
            isLoading.value = false
        }
    }

    function searchContacts(query: string) {
        if (!query.trim()) {
            contacts.value = allContacts.value
            return
        }

        const searchTerm = query.toLowerCase().trim()
        contacts.value = allContacts.value.filter(contact => 
            contact.username.toLowerCase().includes(searchTerm)
        )
    }

    return {
        contacts,
        isLoading,
        fetchContacts,
        searchContacts
    }
}) 