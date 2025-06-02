<template>
    <div class="message-view">
        <!-- Messages Container -->
        <div 
            ref="messagesContainer" 
            class="messages-container"
            @scroll="handleScroll"
        >
            <div v-if="messagesStore.isLoading && !messagesStore.messages.length" class="loading-spinner">
                Loading messages...
            </div>
            
            <div v-else-if="!messagesStore.messages.length" class="no-messages">
                No messages yet. Start the conversation!
            </div>

            <template v-else>
                <div 
                    v-for="message in messagesStore.messages" 
                    :key="message.id"
                    :class="['message', { 'message--own': message.senderId === authStore.user?.id }]"
                >
                    <div class="message__content">
                        <div class="message__sender" v-if="message.senderId !== authStore.user?.id">
                            {{ message.senderName }}
                        </div>
                        <div class="message__text">{{ message.content }}</div>
                        <div class="message__time">
                            {{ new Date(message.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' }) }}
                        </div>
                    </div>
                </div>
            </template>

            <div v-if="messagesStore.isLoading" class="loading-indicator">
                Loading more messages...
            </div>
        </div>

        <!-- Message Input -->
        <div class="message-input">
            <form @submit.prevent="sendMessage">
                <input
                    v-model="newMessage"
                    type="text"
                    placeholder="Type a message..."
                    :disabled="messagesStore.isLoading"
                />
                <button type="submit" :disabled="!newMessage.trim() || messagesStore.isLoading">
                    Send
                </button>
            </form>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useMessagesStore } from '@/stores/messages'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const messagesStore = useMessagesStore()
const authStore = useAuthStore()

const messagesContainer = ref<HTMLElement | null>(null)
const newMessage = ref('')
// Watch for route changes to load new messages
watch(
    () => route.params,
    async () => {
        const contactId = route.params.contactId as string
        const groupId = route.params.groupId as string
        
        if (contactId || groupId) {
            await authStore.fetchUser();
            console.log(authStore.user);
            await messagesStore.fetchMessages(contactId, groupId)
            console.log(messagesStore.messages);
            scrollToBottom()
        }
    },
    { immediate: true }
)

// Watch for new messages to scroll to bottom
watch(messagesStore.messages, () => {
    scrollToBottom()
})

function scrollToBottom() {
    setTimeout(() => {
        if (messagesContainer.value) {
            messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
        }
    }, 100)
}

async function handleScroll(event: Event) {
    const container = event.target as HTMLElement
    const isAtTop = container.scrollTop === 0

    if (isAtTop && messagesStore.hasMoreMessages && !messagesStore.isLoading) {
        const previousHeight = container.scrollHeight
        await messagesStore.loadMoreMessages()
        // Maintain scroll position after loading more messages
        container.scrollTop = container.scrollHeight - previousHeight
    }
}

async function sendMessage() {
    if (!newMessage.value.trim() || messagesStore.isLoading) return

    try {
        await messagesStore.sendMessage(newMessage.value)
        newMessage.value = ''
        scrollToBottom()
    } catch (error) {
        console.error('Failed to send message:', error)
    }
}
</script>

<style scoped>
.message-view {
    display: flex;
    flex-direction: column;
    height: 100%;
    background: #f8fafc;
}

.messages-container {
    flex: 1;
    overflow-y: auto;
    padding: 1rem;
    display: flex;
    flex-direction: column;
    gap: 1rem;
}

.message {
    display: flex;
    margin-bottom: 0.5rem;
}

.message--own {
    justify-content: flex-end;
}

.message__content {
    max-width: 70%;
    padding: 0.75rem 1rem;
    border-radius: 1rem;
    background: white;
    box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.message--own .message__content {
    background: #2ed573;
    color: white;
}

.message__sender {
    font-size: 0.875rem;
    font-weight: 500;
    margin-bottom: 0.25rem;
    color: #64748b;
}

.message__text {
    margin-bottom: 0.25rem;
    word-wrap: break-word;
}

.message__time {
    font-size: 0.75rem;
    color: #94a3b8;
    text-align: right;
}

.message--own .message__time {
    color: rgba(255, 255, 255, 0.8);
}

.message-input {
    padding: 1rem;
    background: white;
    border-top: 1px solid #e2e8f0;
}

.message-input form {
    display: flex;
    gap: 0.5rem;
}

.message-input input {
    flex: 1;
    padding: 0.75rem;
    border: 1px solid #e2e8f0;
    border-radius: 0.5rem;
    outline: none;
    transition: border-color 0.2s;
}

.message-input input:focus {
    border-color: #2ed573;
}

.message-input button {
    padding: 0.75rem 1.5rem;
    background: #2ed573;
    color: white;
    border: none;
    border-radius: 0.5rem;
    cursor: pointer;
    transition: opacity 0.2s;
}

.message-input button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

.loading-spinner,
.no-messages,
.loading-indicator {
    text-align: center;
    color: #64748b;
    padding: 1rem;
}

.loading-indicator {
    font-size: 0.875rem;
}
</style> 