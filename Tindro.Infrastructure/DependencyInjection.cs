using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Tindro.Application.Common.Interfaces;
using Tindro.Infrastructure.Persistence;
using Tindro.Infrastructure.Redis;
using Tindro.Infrastructure.Storage;
using Tindro.Infrastructure.Payments;
using Tindro.Infrastructure.Moderation;
using Tindro.Infrastructure.Security;
using Tindro.Infrastructure.Notifications;
using Tindro.Infrastructure.Persistence.Repositories;
using Tindro.Application.Location.Services;
using Tindro.Application.Recommendations.Interfaces;
using Tindro.Application.Recommendations.Services;
using Tindro.Application.Discovery.Interfaces;
using Tindro.Application.Discovery.Services;
using Tindro.Application.Verification.Interfaces;
using Tindro.Application.Verification.Services;
using Tindro.Application.Stories.Interfaces;
using Tindro.Application.Stories.Services;
using Tindro.Application.Payments.Interfaces;
using Tindro.Application.Payments.Services;
using Tindro.Application.Chat.Interfaces;
using Tindro.Application.Chat.Services;

namespace Tindro.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration config)
        {
            // ------------------------
            // DATABASES
            // ------------------------
     var commandConn =
    config["ConnectionStrings:CommandDb"];

var queryConn =
    config["ConnectionStrings:QueryDb"];


services.AddDbContext<CommandDbContext>(options =>
    options.UseNpgsql(commandConn));

services.AddDbContext<QueryDbContext>(options =>
    options.UseNpgsql(queryConn));


            // ------------------------
            // STORAGE SETTINGS
            // ------------------------
            services.Configure<StorageSettings>(
                config.GetSection("Storage"));

            // ------------------------
            // REDIS (SAFE CONFIG)
            // ------------------------
            var redisOptions = ConfigurationOptions.Parse(
                config.GetConnectionString("Redis") ?? "localhost:6379"
            );
            redisOptions.AbortOnConnectFail = false;

            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisOptions));

            services.AddScoped<IRedisService, RedisService>();

            // ------------------------
            // SECURITY
            // ------------------------
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.AddScoped<IEncryptionService>(sp =>
                new EncryptionService(
                    config["Encryption:Key"]
                        ?? throw new Exception("Encryption Key missing"),
                    config["Encryption:IV"]
                        ?? throw new Exception("Encryption IV missing")
                )
            );

            // ------------------------
            // CORE REPOSITORIES
            // ------------------------
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            // ------------------------
            // INFRASTRUCTURE SERVICES
            // ------------------------
            services.AddScoped<IFileStorage, CloudStorageService>();
            services.AddScoped<IPaymentGateway, RazorpayPaymentGateway>();
            services.AddScoped<IModerationService, AiModerationService>();
            services.AddScoped<IPushService, PushService>();

            // ------------------------
            // LOCATION & MAP
            // ------------------------
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ICrossedPathRepository, CrossedPathRepository>();
            services.AddScoped<ILocationPrivacyRepository, LocationPrivacyRepository>();
            services.AddScoped<ICityRepository, CityRepository>();

            services.AddScoped<GeohashService>();
            services.AddScoped<LocationService>();
            services.AddScoped<CrossedPathsService>();
            services.AddScoped<MapCardService>();

            // ------------------------
            // RECOMMENDATIONS
            // ------------------------
            services.AddScoped<IRecommendationRepository, RecommendationRepository>();
            services.AddScoped<IPreferenceRepository, PreferenceRepository>();
            services.AddScoped<ISkipRepository, SkipRepository>();

            services.AddScoped<IRecommendationService, RecommendationService>();
            services.AddScoped<IPreferenceMatchingService, PreferenceMatchingService>();
            services.AddScoped<IInterestMatchingService, InterestMatchingService>();
            services.AddScoped<IProfileScoreService, ProfileScoreService>();

            // ------------------------
            // DISCOVERY
            // ------------------------
            services.AddScoped<IFilterRepository, FilterRepository>();
            services.AddScoped<IFilterService, FilterService>();
            services.AddScoped<IProfileMatcher, ProfileMatcher>();

            // ------------------------
            // VERIFICATION & SAFETY
            // ------------------------
            services.AddScoped<IVerificationRepository, VerificationRepository>();
            services.AddScoped<IVerificationService, VerificationService>();
            services.AddScoped<IBadgeService, BadgeService>();

            // ------------------------
            // STORIES
            // ------------------------
            services.AddScoped<IStoryRepository, StoryRepository>();
            services.AddScoped<IStoryService, StoryService>();
            services.AddScoped<IStoryHighlightService, StoryHighlightService>();

            // ------------------------
            // SUBSCRIPTIONS
            // ------------------------
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IBoostService, BoostService>();

            // ------------------------
            // CHAT / MESSAGING
            // ------------------------
            services.AddScoped<IMessagingRepository, MessagingRepository>();
            services.AddScoped<IMessagingQueryRepository, MessagingQueryRepository>();
            services.AddScoped<IMessagingService, MessagingService>();
            services.AddScoped<IReadReceiptService, ReadReceiptService>();
            services.AddScoped<ITypingIndicatorService, TypingIndicatorService>();
            services.AddScoped<IVoiceNoteService, VoiceNoteService>();
            services.AddScoped<IConversationSettingsService, ConversationSettingsService>();

            return services;
        }
    }
}
