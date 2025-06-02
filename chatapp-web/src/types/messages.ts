export interface Message {
    id: string;
    content: string;
    senderId: string;
    senderUsername: string;
    senderName: string;
    sentAt: string;
    recipientUserId?: string;
    recipientGroupId?: string;
}

export interface SendMessageRequest {
    content: string;
    recipientUserId?: string;
    recipientGroupId?: string;
}

export interface PaginatedMessages {
    items: Message[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
} 