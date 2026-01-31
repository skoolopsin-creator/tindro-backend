# ChatHub Quick Start Guide

## 🚀 Getting Started

### Prerequisites
- Redis running on `localhost:6379`
- PostgreSQL databases configured
- Firebase project with service account credentials

### 1. Configuration Setup

**Development Environment:**
```json
// appsettings.Development.json
{
  "Encryption": {
    "Key": "dev-32-character-encryption-key1",
    "IV": "dev-16-char-iv2"
  }
}
```

**Production Environment:**
```bash
# Set environment variables
export FCM_PROJECT_ID=your-project-id
export FCM_CLIENT_EMAIL=your-service-email
export FCM_PRIVATE_KEY="-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----"
```

### 2. Database Migration

```bash
cd Tindro.Backend

# Generate migration
dotnet ef migrations add AddChatEnhancements -p Tindro.Infrastructure -s Tindro.Api

# Apply migration
dotnet ef database update
```

### 3. Run Application

```bash
dotnet run --project Tindro.Api
```

The ChatHub will be available at: `ws://localhost:5000/hubs/chat`

---

## 📱 Client Integration

### JavaScript/TypeScript

```typescript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://api.example.com/hubs/chat", {
    accessTokenFactory: () => jwtToken,
    withCredentials: true
  })
  .withAutomaticReconnect()
  .build();

// Connect
await connection.start();

// Join conversation
await connection.invoke("JoinConversation", conversationId);

// Send message
await connection.invoke("SendMessage", conversationId, "Hello!");

// Receive messages
connection.on("ReceiveMessage", (message) => {
  console.log(`${message.senderId}: ${message.text}`);
});

// Disconnect
await connection.stop();
```

### Register FCM Token

```typescript
async function registerFcmToken(token) {
  const response = await fetch("/api/notifications/register-token", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ token, platform: "web" })
  });
  return response.ok;
}
```

### Flutter/Dart

```dart
import 'package:signalr_netcore/signalr_client.dart';

final hubConnection = HubConnectionBuilder()
  .withUrl('https://api.example.com/hubs/chat',
    HttpConnectionOptions(
      accessTokenFactory: () async => jwtToken,
    ),
  )
  .withAutomaticReconnect()
  .build();

await hubConnection.start();

// Join conversation
await hubConnection.invoke('JoinConversation', conversationId);

// Send message
await hubConnection.invoke('SendMessage', conversationId, 'Hello!');

// Receive messages
hubConnection.on('ReceiveMessage', (message) {
  print('${message['senderId']}: ${message['text']}');
});

await hubConnection.stop();
```

---

## 🔐 Security Notes

### Encryption
- Messages are encrypted with AES-256 before storage
- Client receives plain text for display
- CipherText stored in database for audit/compliance

### Rate Limiting
- 30 messages per minute per user
- Enforced server-side via Redis
- Returns silently when limit exceeded

### Authentication
- Requires JWT token in SignalR query string or header
- User ID extracted from JWT claims
- Authorization enforced on hub methods

---

## 🧪 Testing

### Test Conversation Flow

```csharp
// Using SignalR Test Client or Postman
1. Connect to /hubs/chat with auth token
2. Send: JoinConversation("550e8400-e29b-41d4-a716-446655440000")
3. Send: SendMessage("550e8400-e29b-41d4-a716-446655440000", "Test message")
4. Verify: Message received via ReceiveMessage event
```

### Test Rate Limiting

```javascript
// Send 31 messages rapidly
for (let i = 0; i < 31; i++) {
  connection.invoke("SendMessage", convId, `Message ${i}`);
}
// Last message should be silently dropped
```

### Test Offline Notifications

```javascript
// 1. Register FCM token
await fetch("/api/notifications/register-token", {
  method: "POST",
  body: JSON.stringify({ token: "fcm-token-here", platform: "web" })
});

// 2. Go offline
await connection.stop();

// 3. Send message from other user
// Should trigger push notification
```

---

## 🐛 Common Issues

### "Connection refused on /hubs/chat"
- **Solution**: Ensure `app.MapHub<ChatHub>("/hubs/chat")` is in Program.cs
- Check SignalR is registered: `builder.Services.AddSignalR()`

### "Unauthorized" on connection
- **Solution**: Verify JWT token is valid and includes user ID claim
- Check [Authorize] attribute on ChatHub

### Messages not encrypting
- **Solution**: Verify Encryption:Key is exactly 32 characters
- Check Encryption:IV is exactly 16 characters

### Push notifications not sending
- **Solution**: Check FCM credentials are valid
- Verify device token is registered via `/register-token` endpoint
- Check Firebase project is active and has quota

### Rate limiting too strict
- **Solution**: Adjust the `30` value in ChatHub.SendMessage()
- Or modify the window in RedisService.AllowAsync()

---

## 📊 Monitoring

### Redis Keys to Monitor
```bash
redis-cli KEYS "chat:rate:*"       # Rate limit counters
redis-cli KEYS "online:*"          # Online user tracking
redis-cli KEYS "notifications:*"   # Cached notifications
redis-cli KEYS "device_tokens:*"   # FCM tokens
```

### Logs to Watch
```
[SignalR] Hub method invoked: JoinConversation
[SignalR] Hub method invoked: SendMessage
[Push] FCM send failed: {error}
[Encryption] Message encrypted: {messageId}
```

---

## 📚 Additional Resources

- [CHATHUB_IMPLEMENTATION.md](CHATHUB_IMPLEMENTATION.md) - Full technical guide
- [IMPLEMENTATION_CHANGES.md](IMPLEMENTATION_CHANGES.md) - Detailed change list
- [SignalR Documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
- [Firebase Admin SDK](https://firebase.google.com/docs/admin/setup)
- [ASP.NET Core Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/)

---

**Last Updated**: January 27, 2026  
**Maintainer**: Tindro Backend Team
