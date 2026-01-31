# ChatHub Implementation Guide

## Overview
The ChatHub is a real-time chat implementation using ASP.NET Core SignalR with the following features:
- End-to-end message encryption
- Rate limiting (30 messages/minute per user)
- Push notifications for offline users via FCM
- Online status tracking with Redis
- Group-based message broadcasting

## Architecture

### Components

#### 1. ChatHub ([Tindro.Api/Hubs/ChatHub.cs](Tindro.Api/Hubs/ChatHub.cs))
SignalR hub managing real-time chat connections
- **JoinConversation**: User joins a chat group
- **SendMessage**: Send encrypted message with rate limiting
- **OnConnectedAsync**: Track user online status
- **OnDisconnectedAsync**: Clean up user session
- **GetOtherParticipant**: Helper to find conversation recipient

#### 2. Data Models

**Message** ([Tindro.Domain/Chat/Message.cs](Tindro.Domain/Chat/Message.cs))
- `Id`: Message identifier
- `ConversationId`: Reference to conversation
- `SenderId`: Message sender
- `Text`: Plain text (for display)
- `CipherText`: Encrypted message content
- `CreatedAt`: Message timestamp
- `SentAt`: Original sent time
- `IsRead`: Read status

**Conversation** ([Tindro.Domain/Chat/Conversation.cs](Tindro.Domain/Chat/Conversation.cs))
- `Id`: Conversation identifier
- `User1Id`, `User2Id`: Participants
- `Participants`: Collection of ConversationParticipant

**ConversationParticipant** ([Tindro.Domain/Chat/Conversation.cs](Tindro.Domain/Chat/Conversation.cs))
- `Id`: Participant identifier
- `ConversationId`: Reference to conversation
- `UserId`: User identifier
- `JoinedAt`: Join timestamp

#### 3. Services

**IEncryptionService** ([Tindro.Application/Common/Interfaces/IEncryptionService.cs](Tindro.Application/Common/Interfaces/IEncryptionService.cs))
- `Encrypt(plaintext)`: Encrypts message using AES
- `Decrypt(ciphertext)`: Decrypts message

**IPushService** ([Tindro.Application/Common/Interfaces/IPushService.cs](Tindro.Application/Common/Interfaces/IPushService.cs))
- `SendAsync(userId, type, data)`: Send notification by type
- `SendAsync(userId, title, message)`: Send titled notification

**IRedisService** (Enhanced)
- `SetAsync(key, value, ttl)`: Set with expiration
- `ExistsAsync(key)`: Check key existence
- `DeleteAsync(key)`: Delete key
- `AllowAsync(key, limit, window)`: Rate limiting support

#### 4. Implementations

**EncryptionService** ([Tindro.Infrastructure/Security/EncryptionService.cs](Tindro.Infrastructure/Security/EncryptionService.cs))
- Uses AES-256 encryption
- Key: 32 characters (from config)
- IV: 16 characters (from config)
- Base64 encoded output

**PushService** ([Tindro.Infrastructure/Notifications/PushService.cs](Tindro.Infrastructure/Notifications/PushService.cs))
- Stores notifications in Redis
- Sends via Firebase Cloud Messaging (FCM)
- Retrieves device tokens from NotificationRepository
- Handles FCM errors gracefully

## Configuration

### appsettings.json
```json
{
  "Encryption": {
    "Key": "your-32-character-encryption-key",
    "IV": "your-16-char-iv"
  }
}
```

### Environment Variables (FCM)
```
FCM_PROJECT_ID=your-project-id
FCM_CLIENT_EMAIL=your-service-account-email
FCM_PRIVATE_KEY=-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----
```

### SignalR Hub Registration ([Program.cs](Tindro.Api/Program.cs))
```csharp
builder.Services.AddSignalR();
// ...
app.MapHub<ChatHub>("/hubs/chat");
```

## Flow Diagram

### Message Send Flow
```
Client connects to /hubs/chat
    ↓
OnConnectedAsync() → Set online:{userId} in Redis (2 min TTL)
    ↓
JoinConversation(conversationId) → Add to group
    ↓
SendMessage(conversationId, text)
    ↓
Validate (not empty, < 2000 chars)
    ↓
Rate limit check (30/min via AllowAsync)
    ↓
Encrypt message using AES → CipherText
    ↓
Save to database with CipherText
    ↓
Broadcast to group → Online users receive in plaintext
    ↓
Check if receiver offline (ExistsAsync check)
    ↓
If offline → SendAsync via PushService → FCM notification
```

### Offline Push Flow
```
PushService.SendAsync()
    ↓
Get device tokens from NotificationRepository
    ↓
Create FCM message (title, body, metadata)
    ↓
Send via FirebaseMessaging.DefaultInstance
    ↓
Graceful error handling
```

## Database Schema

### Messages Table
```sql
CREATE TABLE messages (
  id UUID PRIMARY KEY,
  conversation_id UUID NOT NULL REFERENCES conversations(id),
  sender_id UUID NOT NULL,
  text TEXT NOT NULL,
  cipher_text TEXT,
  sent_at TIMESTAMP NOT NULL,
  created_at TIMESTAMP NOT NULL,
  is_read BOOLEAN DEFAULT FALSE
);
```

### Conversations Table
```sql
CREATE TABLE conversations (
  id UUID PRIMARY KEY,
  user1_id UUID NOT NULL,
  user2_id UUID NOT NULL
);
```

### Conversation Participants Table
```sql
CREATE TABLE conversation_participants (
  id UUID PRIMARY KEY,
  conversation_id UUID NOT NULL REFERENCES conversations(id),
  user_id UUID NOT NULL,
  joined_at TIMESTAMP NOT NULL,
  UNIQUE(conversation_id, user_id)
);
```

## API Endpoints

### SignalR Methods

**Client → Server:**
- `JoinConversation(conversationId: Guid)` - Join chat group
- `SendMessage(conversationId: Guid, text: string)` - Send message

**Server → Client:**
- `ReceiveMessage(message)` - Receive sent messages
  ```
  {
    id: string,
    senderId: string,
    text: string,
    createdAt: datetime
  }
  ```

### REST Endpoints

**POST /api/notifications/register-token**
```json
{
  "token": "fcm-device-token",
  "platform": "android|ios"
}
```

## Security Considerations

1. **Message Encryption**: Messages stored with AES-256 encrypted CipherText
2. **Rate Limiting**: 30 messages per minute per user
3. **Authentication**: [Authorize] attribute on ChatHub
4. **Online Status**: Auto-cleanup with Redis TTL
5. **Device Tokens**: Scoped to user, rotated per app install

## Deployment Checklist

- [ ] Generate secure 32-char encryption key
- [ ] Generate secure 16-char encryption IV
- [ ] Configure FCM credentials (service account JSON)
- [ ] Set environment variables for FCM
- [ ] Run database migrations for new entities
- [ ] Test encryption/decryption cycle
- [ ] Test FCM integration with real devices
- [ ] Configure CORS for SignalR if needed
- [ ] Set up log monitoring for push failures

## Testing

### Local Testing
```bash
# 1. Ensure Redis is running
docker run -d -p 6379:6379 redis

# 2. Ensure databases are created
# Migrations will run on startup in Development

# 3. Connect to SignalR hub
# Use SignalR Test Client or JavaScript client

# 4. Send messages and verify encryption
```

### Firebase Emulator (Optional)
```bash
firebase emulators:start
```

## Troubleshooting

### Messages not encrypting
- Check `Encryption:Key` is 32 chars
- Check `Encryption:IV` is 16 chars
- Verify EncryptionService is registered in DependencyInjection

### Push notifications not sending
- Verify FCM credentials are valid
- Check device tokens are registered
- Review FCM configuration in appsettings
- Check FirebaseAdmin package version

### Rate limiting too aggressive
- Adjust limit in `SendMessage()` method (currently 30/min)
- Modify `AllowAsync()` window in RedisService

### Offline users not getting notified
- Verify device tokens are being saved via `/register-token` endpoint
- Check INotificationRepository.GetUserDeviceTokensAsync() returns tokens
- Verify FCM is initialized in Program.cs
