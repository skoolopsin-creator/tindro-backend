# Advanced Discovery Filters & Safety/Verification System

## Overview

Comprehensive system for advanced user discovery with sophisticated filtering and safety/verification features. The system includes privacy-respecting filters, identity verification, background checks, and a badge system.

## 1. Advanced Discovery Filters

### Features

- **Multi-criteria filtering** - Age, height, distance, education, lifestyle, religion, relationships, family plans
- **Smart caching** - Redis-backed filter result caching (24-hour TTL)
- **Saved filters** - Template filters for quick access
- **Filter validation** - Real-time filter validation with estimates
- **Analytics** - Usage statistics and conversion tracking

### Key Entities

#### FilterPreferences
```csharp
public class FilterPreferences
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = "Default";
    public bool IsActive { get; set; } = true;

    // Age filter
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;

    // Height filter (cm)
    public int? MinHeight { get; set; }
    public int? MaxHeight { get; set; }

    // Distance filter (km)
    public int? MaxDistance { get; set; }

    // Education level
    public string? EducationLevel { get; set; }

    // Lifestyle preferences
    public string? SmokingPreference { get; set; } // "smoker", "non-smoker", "any"
    public string? DrinkingPreference { get; set; } // "drinker", "non-drinker", "social", "any"
    public string? ExerciseFrequency { get; set; } // "daily", "regularly", "sometimes", "never", "any"

    // Religion
    public string? Religion { get; set; }

    // Relationship goals
    public string? RelationshipGoal { get; set; } // "dating", "long-term", "marriage", "any"

    // Family plans
    public string? FamilyPlans { get; set; } // "wants-kids", "no-kids", "unsure", "any"

    // Personality & Interests
    public bool FilterByPersonality { get; set; }
    public string? PersonalityTraits { get; set; }
    public bool FilterByInterests { get; set; }
    public int? MinSharedInterests { get; set; } = 2;

    // Verification requirements
    public bool RequireVerified { get; set; }
    public bool RequirePhotos { get; set; } = true;
    public int? MinPhotos { get; set; } = 1;

    // Profile completion
    public int? MinProfileCompletion { get; set; } = 50;

    // Sorting
    public string? SortBy { get; set; } = "compatibility"; // "compatibility", "distance", "newest", "verified"

    // Premium filters
    public bool ShowOnlineOnly { get; set; }
    public bool ShowRecentlyActive { get; set; }

    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
```

#### SavedFilter
```csharp
public class SavedFilter
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? FilterPreferencesId { get; set; }
    public bool IsDefault { get; set; }
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastAppliedAt { get; set; }
    public string FilterData { get; set; } = string.Empty; // JSON
}
```

### API Endpoints

#### Apply Filters
```http
POST /api/v1/discovery/filters/apply
Authorization: Bearer {token}
Content-Type: application/json

{
  "filterPreferences": {
    "name": "My Preferences",
    "minAge": 25,
    "maxAge": 35,
    "maxDistance": 50,
    "educationLevel": "bachelors",
    "relationshipGoal": "long-term",
    "sortBy": "compatibility"
  },
  "page": 1,
  "pageSize": 20
}

Response: 200 OK
{
  "profiles": [...],
  "totalCount": 245,
  "page": 1,
  "pageSize": 20,
  "totalPages": 13,
  "criteriaMatched": 245,
  "appliedFilterName": "My Preferences",
  "appliedAt": "2026-01-27T12:00:00Z"
}
```

#### Validate Filter
```http
POST /api/v1/discovery/filters/validate
Authorization: Bearer {token}
Content-Type: application/json

{
  "minAge": 25,
  "maxAge": 35,
  "minHeight": 160,
  "maxHeight": 200,
  "minProfileCompletion": 60
}

Response: 200 OK
{
  "isValid": true,
  "errors": [],
  "warnings": [],
  "estimatedResultCount": 245,
  "suggestedOptimization": "Filter is optimal"
}
```

#### Save Filter
```http
POST /api/v1/discovery/filters/preferences
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Career-Focused Women",
  "minAge": 28,
  "maxAge": 38,
  "educationLevel": "bachelors",
  "minProfileCompletion": 70
}

Response: 200 OK
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Career-Focused Women",
  "minAge": 28,
  "maxAge": 38,
  "educationLevel": "bachelors",
  "isActive": true
}
```

#### Get Filter Analytics
```http
GET /api/v1/discovery/filters/analytics
Authorization: Bearer {token}

Response: 200 OK
{
  "totalFiltersUsed": 12,
  "mostFrequentFilter": "Career-Focused Women",
  "averageResultsPerFilter": 87,
  "conversionRate": 35,
  "matchRate": 12,
  "topFilters": [
    {
      "filterName": "Career-Focused Women",
      "usageCount": 45,
      "resultCount": 245,
      "profilesViewed": 89,
      "matches": 12,
      "conversionPercentage": 36.3,
      "lastUsed": "2026-01-27T10:30:00Z"
    }
  ]
}
```

#### Saved Filters Management
```http
POST /api/v1/discovery/filters/saved
Authorization: Bearer {token}
{
  "name": "My Template",
  "filterPreferences": { ... }
}
Response: 201 Created

GET /api/v1/discovery/filters/saved
Response: 200 OK [...]

GET /api/v1/discovery/filters/saved/{savedFilterId}
Response: 200 OK

DELETE /api/v1/discovery/filters/saved/{savedFilterId}
Response: 204 No Content
```

---

## 2. Safety & Verification System

### Features

- **Multi-type verification** - ID, phone, email, photo, background check
- **Background checks** - Third-party integration for criminal/sex offender records
- **Fraud detection** - IP tracking, device fingerprinting, rate limiting
- **Badge system** - Visual indicators of verification status
- **Verification timeline** - Complete audit trail of all verification actions
- **Admin approval workflow** - Moderator review and approval/rejection

### Key Entities

#### VerificationRecord
```csharp
public class VerificationRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string VerificationType { get; set; } = "id"; // "id", "phone", "email", "photo", "background"
    public string Status { get; set; } = "pending"; // "pending", "approved", "rejected", "expired"
    public string? RejectionReason { get; set; }
    public int AttemptCount { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? VerificationScore { get; set; } // AI confidence score
}
```

#### UserVerificationBadge
```csharp
public class UserVerificationBadge
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BadgeType { get; set; } = string.Empty; // "verified", "id_verified", "selfie_verified", "background_clear"
    public string DisplayName { get; set; } = string.Empty;
    public string BadgeIcon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; };
    public bool IsActive { get; set; } = true;
    public DateTime AwardedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
```

#### BackgroundCheckResult
```csharp
public class BackgroundCheckResult
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; } = "pending"; // "pending", "clear", "issues_found", "failed"
    public string ProviderName { get; set; } = string.Empty;
    public string ProviderReferenceId { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Summary { get; set; }
    public bool HasCriminalRecord { get; set; }
    public bool HasSexOffenderRecord { get; set; }
    public string? Findings { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
```

#### VerificationAttempt (Fraud Detection)
```csharp
public class VerificationAttempt
{
    public Guid Id { get; set; }
    public Guid VerificationRecordId { get; set; }
    public Guid UserId { get; set; }
    public int AttemptNumber { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }
    public string Status { get; set; } = "pending";
    public string? FlagReason { get; set; }
    public decimal? FraudScore { get; set; } // 0-1, higher = suspicious
    public DateTime CreatedAt { get; set; }
}
```

### API Endpoints

#### Submit Verification
```http
POST /api/v1/verification/submit
Authorization: Bearer {token}
Content-Type: multipart/form-data

{
  "verificationType": "id",
  "documents": [
    { "file": "passport.jpg", "documentType": "passport" }
  ]
}

Response: 200 OK
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "user-id",
  "verificationType": "id",
  "status": "pending",
  "attemptCount": 1,
  "submittedAt": "2026-01-27T12:00:00Z",
  "documents": [...]
}
```

#### Check Verification Status
```http
GET /api/v1/verification/status
Authorization: Bearer {token}

Response: 200 OK
{
  "userId": "user-id",
  "isFullyVerified": true,
  "isIdVerified": true,
  "isPhotoVerified": true,
  "isBackgroundClear": true,
  "verificationScore": 100,
  "lastVerificationDate": "2026-01-25T08:30:00Z",
  "activeBadges": ["verified", "id_verified", "background_clear"],
  "verificationRecords": [...]
}
```

#### Get User Verification Progress
```http
GET /api/v1/verification/progress
Authorization: Bearer {token}

Response: 200 OK
{
  "totalSteps": 3,
  "completedSteps": 2,
  "progressPercentage": 66.67,
  "nextStep": "Background Check",
  "steps": [
    {
      "stepName": "ID Verification",
      "status": "completed",
      "description": "Verify your identity with a government ID",
      "completedAt": "2026-01-25T08:30:00Z"
    },
    {
      "stepName": "Selfie Verification",
      "status": "completed",
      "description": "Take a selfie for liveness verification",
      "completedAt": "2026-01-26T10:15:00Z"
    },
    {
      "stepName": "Background Check",
      "status": "pending",
      "description": "Complete background check",
      "completedAt": null
    }
  ]
}
```

#### Request Background Check
```http
POST /api/v1/verification/background-check
Authorization: Bearer {token}
Content-Type: application/json

{
  "fullName": "John Doe",
  "dateOfBirth": "1990-05-15",
  "phoneNumber": "+1-555-0123"
}

Response: 200 OK
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "status": "pending",
  "completedAt": null,
  "summary": null,
  "hasCriminalRecord": false,
  "hasSexOffenderRecord": false,
  "expiresAt": "2027-01-27T12:00:00Z"
}
```

#### Get User Badges
```http
GET /api/v1/verification/badges
Authorization: Bearer {token}

Response: 200 OK
[
  {
    "id": "badge-id-1",
    "badgeType": "verified",
    "displayName": "Verified",
    "badgeIcon": "✓",
    "description": "User has passed all verification checks",
    "priority": 8,
    "isActive": true,
    "awardedAt": "2026-01-25T08:30:00Z",
    "expiresAt": null
  },
  {
    "id": "badge-id-2",
    "badgeType": "background_clear",
    "displayName": "Background Clear",
    "badgeIcon": "✅",
    "description": "User has a clear background check",
    "priority": 10,
    "isActive": true,
    "awardedAt": "2026-01-26T10:15:00Z",
    "expiresAt": "2027-01-26T10:15:00Z"
  }
]
```

#### Get Verification Timeline
```http
GET /api/v1/verification/timeline
Authorization: Bearer {token}

Response: 200 OK
[
  {
    "id": "event-1",
    "action": "submitted",
    "details": "User submitted ID verification",
    "administratorNotes": null,
    "createdAt": "2026-01-25T08:30:00Z"
  },
  {
    "id": "event-2",
    "action": "approved",
    "details": "ID verification approved",
    "administratorNotes": "Documents clear and valid",
    "createdAt": "2026-01-25T15:45:00Z"
  }
]
```

### Admin Endpoints

#### Get Pending Verifications
```http
GET /api/v1/verification/admin/pending?limit=50
Authorization: Bearer {token}
X-Admin-Role: Admin

Response: 200 OK
[...]
```

#### Approve Verification
```http
POST /api/v1/verification/admin/{recordId}/approve
Authorization: Bearer {token}
X-Admin-Role: Admin

Response: 204 No Content
```

#### Reject Verification
```http
POST /api/v1/verification/admin/{recordId}/reject
Authorization: Bearer {token}
X-Admin-Role: Admin
Content-Type: application/json

{
  "reason": "Document quality too low"
}

Response: 204 No Content
```

#### Get Flagged Attempts
```http
GET /api/v1/verification/admin/flagged
Authorization: Bearer {token}
X-Admin-Role: Admin

Response: 200 OK
[
  {
    "id": "attempt-id",
    "userId": "user-id",
    "attemptNumber": 3,
    "ipAddress": "192.168.1.100",
    "deviceInfo": "Chrome on Windows",
    "status": "flagged",
    "flagReason": "High fraud score - multiple IP addresses",
    "fraudScore": 0.85,
    "createdAt": "2026-01-27T11:30:00Z"
  }
]
```

#### Award Badge
```http
POST /api/v1/verification/admin/badges/award
Authorization: Bearer {token}
X-Admin-Role: Admin
Content-Type: application/json

{
  "userId": "user-id",
  "badgeType": "community_contributor",
  "reason": "Active community participant",
  "expiresAt": "2027-01-27T12:00:00Z"
}

Response: 200 OK
{
  "id": "badge-id",
  "badgeType": "community_contributor",
  "displayName": "Community Contributor",
  "badgeIcon": "👥",
  "description": "Active community member",
  "isActive": true,
  "awardedAt": "2026-01-27T12:00:00Z",
  "expiresAt": "2027-01-27T12:00:00Z"
}
```

#### Get Verification Statistics
```http
GET /api/v1/verification/admin/stats
Authorization: Bearer {token}
X-Admin-Role: Admin

Response: 200 OK
{
  "totalVerifiedUsers": 15432,
  "idVerifiedCount": 14890,
  "photoVerifiedCount": 13456,
  "backgroundClearCount": 12340,
  "verificationRate": 87.5,
  "pendingVerifications": 234,
  "flaggedAttempts": 12
}
```

---

## Database Schema

### Filter Tables
```sql
CREATE TABLE filter_preferences (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id),
    name VARCHAR(100) NOT NULL,
    is_active BOOLEAN DEFAULT true,
    min_age INTEGER DEFAULT 18,
    max_age INTEGER DEFAULT 100,
    min_height INTEGER,
    max_height INTEGER,
    max_distance INTEGER,
    education_level VARCHAR(50),
    smoking_preference VARCHAR(20),
    drinking_preference VARCHAR(20),
    exercise_frequency VARCHAR(20),
    religion VARCHAR(50),
    relationship_goal VARCHAR(50),
    family_plans VARCHAR(50),
    filter_by_personality BOOLEAN DEFAULT false,
    personality_traits TEXT,
    filter_by_interests BOOLEAN DEFAULT false,
    min_shared_interests INTEGER DEFAULT 2,
    require_verified BOOLEAN DEFAULT false,
    require_photos BOOLEAN DEFAULT true,
    min_photos INTEGER DEFAULT 1,
    min_profile_completion INTEGER DEFAULT 50,
    sort_by VARCHAR(20) DEFAULT 'compatibility',
    show_online_only BOOLEAN DEFAULT false,
    show_recently_active BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP,
    last_used_at TIMESTAMP,
    UNIQUE(user_id, name),
    INDEX(user_id, is_active),
    INDEX(user_id, last_used_at DESC)
);

CREATE TABLE saved_filters (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id),
    filter_preferences_id UUID REFERENCES filter_preferences(id) ON DELETE SET NULL,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(500),
    is_default BOOLEAN DEFAULT false,
    usage_count INTEGER DEFAULT 0,
    filter_data JSONB,
    created_at TIMESTAMP DEFAULT NOW(),
    last_applied_at TIMESTAMP,
    INDEX(user_id),
    INDEX(user_id, is_default),
    UNIQUE(user_id, name)
);

CREATE TABLE filter_application_history (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id),
    filter_preferences_id UUID REFERENCES filter_preferences(id),
    result_count INTEGER,
    profiles_viewed INTEGER,
    matches INTEGER,
    messages INTEGER,
    applied_at TIMESTAMP DEFAULT NOW(),
    expires_at TIMESTAMP,
    INDEX(user_id),
    INDEX(user_id, applied_at DESC),
    INDEX(expires_at)
);
```

### Verification Tables
```sql
CREATE TABLE verification_records (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id),
    verification_type VARCHAR(50) NOT NULL,
    status VARCHAR(20) DEFAULT 'pending',
    rejection_reason VARCHAR(500),
    attempt_count INTEGER DEFAULT 0,
    submitted_at TIMESTAMP DEFAULT NOW(),
    reviewed_at TIMESTAMP,
    reviewed_by UUID,
    expires_at TIMESTAMP,
    verification_score VARCHAR(10),
    INDEX(user_id),
    INDEX(user_id, status),
    INDEX(status),
    INDEX(expires_at)
);

CREATE TABLE verification_documents (
    id UUID PRIMARY KEY,
    verification_record_id UUID NOT NULL REFERENCES verification_records(id) ON DELETE CASCADE,
    document_type VARCHAR(50) NOT NULL,
    storage_url VARCHAR(500) NOT NULL,
    mime_type VARCHAR(50),
    file_size_bytes BIGINT,
    metadata_json JSONB,
    uploaded_at TIMESTAMP DEFAULT NOW(),
    is_processed BOOLEAN DEFAULT false,
    processing_status VARCHAR(50),
    INDEX(verification_record_id),
    INDEX(processing_status)
);

CREATE TABLE user_verification_badges (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id),
    badge_type VARCHAR(50) NOT NULL,
    display_name VARCHAR(100) NOT NULL,
    badge_icon VARCHAR(255),
    description VARCHAR(500),
    priority INTEGER,
    is_active BOOLEAN DEFAULT true,
    awarded_at TIMESTAMP DEFAULT NOW(),
    expires_at TIMESTAMP,
    criteria JSONB,
    display_order INTEGER,
    INDEX(user_id),
    INDEX(user_id, is_active),
    INDEX(badge_type),
    INDEX(expires_at)
);

CREATE TABLE verification_attempts (
    id UUID PRIMARY KEY,
    verification_record_id UUID NOT NULL REFERENCES verification_records(id),
    user_id UUID NOT NULL REFERENCES users(id),
    attempt_number INTEGER,
    ip_address VARCHAR(45) NOT NULL,
    device_info VARCHAR(500),
    status VARCHAR(20) DEFAULT 'pending',
    flag_reason VARCHAR(255),
    fraud_score DECIMAL(3,2),
    additional_data JSONB,
    created_at TIMESTAMP DEFAULT NOW(),
    INDEX(user_id),
    INDEX(status),
    INDEX(user_id, created_at DESC)
);

CREATE TABLE background_check_results (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id),
    status VARCHAR(20) DEFAULT 'pending',
    provider_name VARCHAR(100) NOT NULL,
    provider_reference_id VARCHAR(100) NOT NULL,
    requested_at TIMESTAMP DEFAULT NOW(),
    completed_at TIMESTAMP,
    summary VARCHAR(1000),
    has_criminal_record BOOLEAN DEFAULT false,
    has_sex_offender_record BOOLEAN DEFAULT false,
    findings JSONB,
    expires_at TIMESTAMP,
    INDEX(user_id),
    INDEX(user_id, status),
    INDEX(expires_at)
);

CREATE TABLE verification_logs (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id),
    action VARCHAR(50) NOT NULL,
    details VARCHAR(500),
    administrator_notes VARCHAR(1000),
    administrator_id UUID,
    ip_address VARCHAR(45),
    created_at TIMESTAMP DEFAULT NOW(),
    INDEX(user_id),
    INDEX(user_id, created_at DESC)
);
```

---

## Services

### FilterService
- **ApplyFilterAsync** - Apply filters to get matching profiles
- **ValidateFilterAsync** - Validate filter criteria
- **SaveFilterPreferencesAsync** - Save user filter preferences
- **GetRecommendedProfilesAsync** - Get profiles matching filters
- **GetFilterAnalyticsAsync** - Get filter usage analytics

### VerificationService
- **SubmitVerificationAsync** - Submit verification documents
- **GetVerificationStatusAsync** - Get complete verification status
- **RequestBackgroundCheckAsync** - Trigger background check
- **IsUserVerifiedAsync** - Quick check if user is verified
- **GetVerificationScoreAsync** - Get verification score (0-100)
- **GetVerificationProgressAsync** - Get verification completion progress

### BadgeService
- **AwardBadgeAsync** - Award badge to user
- **GetUserBadgesAsync** - Get all user badges
- **AwardVerificationBadgeAsync** - Auto-award verification badge
- **AwardBackgroundClearBadgeAsync** - Auto-award background clear badge

---

## Setup & Migration

```bash
# Create migration
dotnet ef migrations add AddAdvancedFiltersAndVerification --context AppDbContext

# Apply migration
dotnet ef database update
```

## Implementation Notes

1. **Caching** - Filters are cached in Redis for 24 hours
2. **Fraud Detection** - Multiple failed attempts trigger rate limiting
3. **Privacy** - All verification data is encrypted at rest
4. **Compliance** - Designed to support GDPR and local regulations
5. **Scalability** - Database indexes optimized for high-volume queries

