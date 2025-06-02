export interface Contact {
    id: string;
    username: string;
    name: string;
    addedAt: string;
}

export interface ContactWithMessage extends Contact {
    lastMessage?: {
        content: string;
        sentAt: string;
    };
}

export interface AddContactRequest {
    userId: string;
} 