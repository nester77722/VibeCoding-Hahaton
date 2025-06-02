import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Message, PaginatedMessages, SendMessageRequest } from '@/types/messages'
import api from '@/services/api'

export const useMessagesStore = defineStore('messages', () => {
    const messages = ref<Message[]>([])
    const currentPage = ref(1)
    const totalPages = ref(0)
    const totalCount = ref(0)
    const pageSize = ref(20)
    const isLoading = ref(false)
    const currentRecipientUserId = ref<string | null>(null)
    const currentRecipientGroupId = ref<string | null>(null)
    const apiUrl = import.meta.env.VITE_CHAT_APP_API_URL

    const hasMoreMessages = computed(() => currentPage.value < totalPages.value)

    async function fetchMessages(recipientUserId?: string, recipientGroupId?: string, page: number = 1) {
        try {
            isLoading.value = true
            
            // Build query parameters
            const params = new URLSearchParams({
                page: page.toString(),
                pageSize: pageSize.value.toString()
            })
            
            if (recipientUserId) {
                params.append('contactId', recipientUserId)
                currentRecipientUserId.value = recipientUserId
                currentRecipientGroupId.value = null
            } else if (recipientGroupId) {
                params.append('groupId', recipientGroupId)
                currentRecipientGroupId.value = recipientGroupId
                currentRecipientUserId.value = null
            }

            const response = await api.get<PaginatedMessages>(`${apiUrl}/messages?${params}`)
            
            // If it's the first page, replace messages, otherwise append
            if (page === 1) {
                messages.value = response.data.items
            } else {
                messages.value = [...messages.value, ...response.data.items]
            }
            
            currentPage.value = response.data.page
            totalPages.value = response.data.totalPages
            totalCount.value = response.data.totalCount
            
        } catch (error) {
            console.error('Failed to fetch messages:', error)
            throw error
        } finally {
            isLoading.value = false
        }
    }

    async function loadMoreMessages() {
        if (hasMoreMessages.value && !isLoading.value) {
            await fetchMessages(
                currentRecipientUserId.value ?? undefined,
                currentRecipientGroupId.value ?? undefined,
                currentPage.value + 1
            )
        }
    }

    async function sendMessage(content: string) {
        try {
            isLoading.value = true
            
            const request: SendMessageRequest = {
                content,
                recipientUserId: currentRecipientUserId.value ?? undefined,
                recipientGroupId: currentRecipientGroupId.value ?? undefined
            }

            const response = await api.post<Message>(`${apiUrl}/messages`, request)
            
            // Add the new message to the list
            messages.value = [...messages.value, response.data]
            totalCount.value++
            
            return response.data
        } catch (error) {
            console.error('Failed to send message:', error)
            throw error
        } finally {
            isLoading.value = false
        }
    }

    function clearMessages() {
        messages.value = []
        currentPage.value = 1
        totalPages.value = 0
        totalCount.value = 0
        currentRecipientUserId.value = null
        currentRecipientGroupId.value = null
    }

    return {
        messages,
        isLoading,
        hasMoreMessages,
        currentRecipientUserId,
        currentRecipientGroupId,
        totalCount,
        fetchMessages,
        loadMoreMessages,
        sendMessage,
        clearMessages
    }
}) 