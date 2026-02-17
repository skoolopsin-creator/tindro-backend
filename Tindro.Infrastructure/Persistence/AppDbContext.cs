using Microsoft.EntityFrameworkCore;
using Tindro.Domain.Users;
using Tindro.Domain.Match;
using Tindro.Domain.Chat;
using Tindro.Domain.Stories;
using Tindro.Domain.Payments;
using Tindro.Domain.Moderation;
using Tindro.Domain.Location;
using Tindro.Domain.Recommendations;
using Tindro.Domain.Discovery;
using Tindro.Domain.Verification;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;
using Tindro.Domain.Feed;
using Tindro.Domain.Common;

namespace Tindro.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
       : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Swipe> Swipes => Set<Swipe>();
    public DbSet<Tindro.Domain.Match.Match> Matches
    => Set<Tindro.Domain.Match.Match>();

    public DbSet<Boost> Boosts => Set<Boost>();

    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageReadReceipt> MessageReadReceipts => Set<MessageReadReceipt>();
    public DbSet<TypingIndicator> TypingIndicators => Set<TypingIndicator>();
    public DbSet<VoiceNote> VoiceNotes => Set<VoiceNote>();
    public DbSet<MessageExtension> MessageExtensions => Set<MessageExtension>();
    public DbSet<ConversationSettings> ConversationSettings => Set<ConversationSettings>();
    public DbSet<Interest> Interests => Set<Interest>();
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<StoryLike> StoryLikes => Set<StoryLike>();
    public DbSet<StoryComment> StoryComments => Set<StoryComment>();
    public DbSet<StoryCommentLike> StoryCommentLikes => Set<StoryCommentLike>();
    public DbSet<StoryView> StoryViews => Set<StoryView>();
    public DbSet<StoryAnalytics> StoryAnalytics => Set<StoryAnalytics>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<SubscriptionTransaction> SubscriptionTransactions => Set<SubscriptionTransaction>();
    public DbSet<ProfileBoost> ProfileBoosts => Set<ProfileBoost>();
    public DbSet<SuperLike> SuperLikes => Set<SuperLike>();
    public DbSet<PermanentPost> PermanentPosts => Set<PermanentPost>();

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostComment> PostComments => Set<PostComment>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();
    public DbSet<Report> Reports => Set<Report>();

    // Location-based discovery
    public DbSet<UserLocation> UserLocations => Set<UserLocation>();
    public DbSet<CrossedPath> CrossedPaths => Set<CrossedPath>();
    public DbSet<LocationPrivacyPreferences> LocationPrivacyPreferences => Set<LocationPrivacyPreferences>();
    public DbSet<City> Cities => Set<City>();

    // Recommendations
    public DbSet<UserPreferences> UserPreferences => Set<UserPreferences>();
    public DbSet<RecommendationScore> RecommendationScores => Set<RecommendationScore>();
    public DbSet<UserInterest> UserInterests => Set<UserInterest>();
    public DbSet<SkipProfile> SkipProfiles => Set<SkipProfile>();
    public DbSet<RecommendationFeedback> RecommendationFeedback => Set<RecommendationFeedback>();

    // Discovery Filters
    public DbSet<FilterPreferences> FilterPreferences => Set<FilterPreferences>();
    public DbSet<FilterCriteria> FilterCriteria => Set<FilterCriteria>();
    public DbSet<SavedFilter> SavedFilters => Set<SavedFilter>();
    public DbSet<FilterApplicationHistory> FilterApplicationHistories => Set<FilterApplicationHistory>();

    // Verification & Safety
    public DbSet<VerificationRecord> VerificationRecords => Set<VerificationRecord>();
    public DbSet<VerificationDocument> VerificationDocuments => Set<VerificationDocument>();
    public DbSet<UserVerificationBadge> UserVerificationBadges => Set<UserVerificationBadge>();
    public DbSet<VerificationAttempt> VerificationAttempts => Set<VerificationAttempt>();
    public DbSet<BackgroundCheckResult> BackgroundCheckResults => Set<BackgroundCheckResult>();
    public DbSet<VerificationLog> VerificationLogs => Set<VerificationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
