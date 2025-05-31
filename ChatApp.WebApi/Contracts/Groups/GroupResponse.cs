namespace ChatApp.WebApi.Contracts.Groups;

public record GroupMemberResponse(
    Guid Id,
    string Username
);

public record GroupResponse(
    Guid Id,
    string Name,
    Guid CreatorId,
    DateTime CreatedAt,
    List<GroupMemberResponse> Members
);