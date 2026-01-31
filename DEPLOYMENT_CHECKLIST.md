# ChatHub Deployment Checklist

## Pre-Deployment ✓

### Code Review
- [ ] All methods reviewed for security
- [ ] No hardcoded credentials in code
- [ ] Error handling is comprehensive
- [ ] Logging is in place for debugging
- [ ] No N+1 queries in database calls

### Security Review
- [ ] Encryption keys are cryptographically secure (32+ chars)
- [ ] IV is random/secure (16 chars)
- [ ] Rate limiting thresholds are appropriate
- [ ] FCM credentials are rotated
- [ ] JWT claims contain user ID
- [ ] CORS is configured if needed for SignalR

### Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Load test completed (simulate 100+ concurrent users)
- [ ] Message encryption/decryption verified
- [ ] Rate limiting verified
- [ ] FCM integration tested with real devices
- [ ] Connection cleanup tested
- [ ] Reconnection handling tested

---

## Configuration

### Environment Variables (Production)
```bash
# Firebase Cloud Messaging
FCM_PROJECT_ID=tindro-prod-xxxxx
FCM_CLIENT_EMAIL=firebase-adminsdk-xxxxx@tindro-prod-xxxxx.iam.gserviceaccount.com
FCM_PRIVATE_KEY="-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----"

# Application
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=https://+:443
Kestrel__Certificates__Default__Path=/app/certs/prod.pfx
Kestrel__Certificates__Default__Password=<cert-password>
```

### appsettings.Production.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "Encryption": {
    "Key": "your-production-32-char-key",
    "IV": "your-production-16-char-iv"
  },
  "ConnectionStrings": {
    "CommandDb": "Host=prod-db-1;Port=5432;Database=tindro_command;Username=<user>;Password=<pass>;",
    "QueryDb": "Host=prod-db-2;Port=5432;Database=tindro_query;Username=<user>;Password=<pass>;"
  },
  "Redis": "prod-redis-1:6379"
}
```

### Secrets Management
- [ ] Store encryption keys in Secret Manager / Vault
- [ ] Store FCM credentials in Secret Manager / Vault
- [ ] Store database passwords in Secret Manager / Vault
- [ ] Rotate credentials every 90 days
- [ ] Never commit secrets to git

---

## Database Migration

### Steps
1. [ ] Backup production databases
2. [ ] Run migrations in staging first
3. [ ] Verify data integrity after migration
4. [ ] Schedule maintenance window
5. [ ] Run migrations in production:
   ```bash
   dotnet ef database update -c CommandDbContext
   dotnet ef database update -c QueryDbContext
   ```
6. [ ] Verify all tables created
7. [ ] Verify indexes created

### Verification Queries
```sql
-- Verify Message table
SELECT * FROM information_schema.columns WHERE table_name = 'messages' AND column_name IN ('cipher_text', 'created_at');

-- Verify Conversation Participants table
SELECT * FROM information_schema.columns WHERE table_name = 'conversation_participants';

-- Verify indexes
SELECT * FROM information_schema.statistics WHERE table_name IN ('messages', 'conversations', 'conversation_participants');
```

---

## Deployment

### Before Go-Live
- [ ] Staging environment fully tested
- [ ] Load testing completed
- [ ] Rollback plan documented
- [ ] Monitoring and alerts configured
- [ ] On-call team briefed
- [ ] Runbook prepared

### Deployment Steps
1. [ ] Build Docker image
   ```bash
   docker build -t tindro-api:latest .
   docker push your-registry/tindro-api:latest
   ```

2. [ ] Deploy to production
   ```bash
   kubectl apply -f deployment.yaml
   kubectl rollout status deployment/tindro-api
   ```

3. [ ] Verify deployment
   - [ ] Health check endpoint returns 200
   - [ ] SignalR hub is accessible
   - [ ] Database connection successful
   - [ ] Redis connection successful
   - [ ] FCM can be initialized

4. [ ] Monitor for errors
   - [ ] Check application logs
   - [ ] Monitor error rates
   - [ ] Monitor resource usage
   - [ ] Check user reports

### Rollback Plan
```bash
# If deployment fails
kubectl rollout undo deployment/tindro-api
kubectl rollout status deployment/tindro-api
```

---

## Post-Deployment

### Monitoring Setup
- [ ] Application performance monitoring (APM) enabled
- [ ] Error tracking (Sentry/AppDynamics) configured
- [ ] Log aggregation (ELK/Datadog) set up
- [ ] Alerts configured for:
  - [ ] High error rate (> 1%)
  - [ ] High latency (p99 > 500ms)
  - [ ] High memory usage (> 80%)
  - [ ] High CPU usage (> 80%)
  - [ ] FCM failures (> 5%)
  - [ ] Database connection failures
  - [ ] Redis connection failures

### Health Checks
```bash
# Manual verification
curl https://api.example.com/health

# SignalR connectivity test
# Use test client or browser console
const connection = new signalR.HubConnectionBuilder()
  .withUrl("wss://api.example.com/hubs/chat")
  .build();
await connection.start();
console.log("Connected!");
```

### Performance Baseline
- [ ] Record baseline response times
- [ ] Record baseline resource usage
- [ ] Document throughput (messages/sec)
- [ ] Document concurrent connection limits

---

## Feature Verification

### Core Functionality
- [ ] Users can connect to ChatHub
- [ ] Users can join conversations
- [ ] Users can send messages
- [ ] Messages are received in real-time
- [ ] Messages are encrypted in database
- [ ] Online status is tracked
- [ ] Users are removed from online when disconnected

### Rate Limiting
- [ ] Rate limit enforced at 30 messages/minute
- [ ] Excess messages are silently dropped
- [ ] Limit resets after time window

### Push Notifications
- [ ] FCM tokens can be registered
- [ ] Offline users receive push notifications
- [ ] Push notifications include correct data
- [ ] Failed pushes don't crash the server

### Security
- [ ] Unauthenticated users cannot connect
- [ ] Users cannot see messages from other conversations
- [ ] Rate limiting prevents spam
- [ ] Encryption keys are not logged

---

## Performance Targets

Establish and monitor these metrics:

| Metric | Target | Alert Threshold |
|--------|--------|-----------------|
| Message latency (p95) | < 100ms | > 200ms |
| Connection time | < 500ms | > 1000ms |
| Encryption time/msg | < 2ms | > 5ms |
| FCM send time | < 100ms | > 500ms |
| Database insert | < 10ms | > 25ms |
| Concurrent connections | 1000+ | > 90% capacity |
| Message throughput | 100 msg/s | < 50 msg/s |
| Error rate | < 0.1% | > 1% |

---

## Support and Operations

### Runbook
Document solutions for:
- [ ] High error rates
- [ ] Connection failures
- [ ] Rate limiting issues
- [ ] FCM failures
- [ ] Database performance degradation
- [ ] Redis unavailability
- [ ] Memory leaks

### Escalation Procedure
1. [ ] Monitor alerts
2. [ ] Check logs and metrics
3. [ ] Page on-call engineer
4. [ ] Follow runbook procedures
5. [ ] Document incident

### Communication
- [ ] Slack channel for alerts: #tindro-chat-alerts
- [ ] War room procedure documented
- [ ] Customer communication template prepared

---

## Rollback Conditions

Rollback immediately if:
- [ ] Error rate > 5%
- [ ] > 10% message loss
- [ ] FCM failures > 20%
- [ ] Database unavailable
- [ ] Authentication failures
- [ ] Memory leak detected

---

## Sign-Off

- [ ] Tech Lead: _________________ Date: _______
- [ ] DevOps: _________________ Date: _______
- [ ] Security: _________________ Date: _______
- [ ] Product: _________________ Date: _______

---

## Post-Launch Review

Scheduled for: **[Date 1 week after launch]**

Review points:
- [ ] Actual vs expected performance
- [ ] Issue resolution time
- [ ] User feedback analysis
- [ ] Scaling requirements
- [ ] Cost impact
- [ ] Process improvements

---

**Status**: Ready for Deployment  
**Last Updated**: January 27, 2026  
**Version**: 1.0
