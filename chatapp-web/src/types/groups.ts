export interface GroupMember {
    id: string;
    username: string;
}

export interface Group {
    id: string;
    name: string;
    creatorId: string;
    createdAt: string;
    members: GroupMember[];
}

export interface GroupWithMessage extends Group {
    lastMessage?: {
        content: string;
        sentAt: string;
    };
}

export interface CreateGroupRequest {
    name: string;
}

export interface AddGroupMemberRequest {
    userId: string;
} 