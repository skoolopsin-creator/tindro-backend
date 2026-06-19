using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tindro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PostEnhancement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Reports",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "Reports");

            migrationBuilder.RenameTable(
                name: "Reports",
                newName: "reports");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "reports",
                newName: "ReportedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_TargetUserId",
                table: "reports",
                newName: "IX_reports_TargetUserId");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Posts",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "FirebaseUid",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "MediaUrl",
                table: "Stories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "AllowComments",
                table: "Stories",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSharing",
                table: "Stories",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "AllowedUserIds",
                table: "Stories",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "Stories",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "Stories",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommentCount",
                table: "Stories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Stories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "Stories",
                type: "integer",
                nullable: true,
                defaultValue: 5);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Stories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "Stories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Stories",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Stories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MediaType",
                table: "Stories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Stories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShareCount",
                table: "Stories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TextColor",
                table: "Stories",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true,
                defaultValue: "#FFFFFF");

            migrationBuilder.AddColumn<string>(
                name: "TextContent",
                table: "Stories",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Stories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Stories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VisibilityType",
                table: "Stories",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "everyone");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "reports",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ReportType",
                table: "reports",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResolutionNotes",
                table: "reports",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "reports",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResolvedBy",
                table: "reports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "reports",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Posts",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Posts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ShareCount",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Posts",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "PostLikes",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "PostComments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_reports",
                table: "reports",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BackgroundCheckResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProviderReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Summary = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    HasCriminalRecord = table.Column<bool>(type: "boolean", nullable: false),
                    HasSexOffenderRecord = table.Column<bool>(type: "boolean", nullable: false),
                    Findings = table.Column<string>(type: "jsonb", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundCheckResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackgroundCheckResults_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversationSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MuteNotifications = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ShowReadReceipts = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ShowTypingIndicators = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    AllowMediaMessages = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    AllowVoiceNotes = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    AllowCallInvites = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConversationId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationSettings_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationSettings_Conversations_ConversationId1",
                        column: x => x.ConversationId1,
                        principalTable: "Conversations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConversationSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationSettings_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FilterPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MinAge = table.Column<int>(type: "integer", nullable: false, defaultValue: 18),
                    MaxAge = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    MinHeight = table.Column<int>(type: "integer", nullable: true),
                    MaxHeight = table.Column<int>(type: "integer", nullable: true),
                    MaxDistance = table.Column<int>(type: "integer", nullable: true),
                    EducationLevel = table.Column<string>(type: "text", nullable: true),
                    SmokingPreference = table.Column<string>(type: "text", nullable: true),
                    DrinkingPreference = table.Column<string>(type: "text", nullable: true),
                    ExerciseFrequency = table.Column<string>(type: "text", nullable: true),
                    Religion = table.Column<string>(type: "text", nullable: true),
                    RelationshipGoal = table.Column<string>(type: "text", nullable: true),
                    FamilyPlans = table.Column<string>(type: "text", nullable: true),
                    FilterByPersonality = table.Column<bool>(type: "boolean", nullable: false),
                    PersonalityTraits = table.Column<string>(type: "text", nullable: true),
                    FilterByInterests = table.Column<bool>(type: "boolean", nullable: false),
                    MinSharedInterests = table.Column<int>(type: "integer", nullable: true),
                    RequireVerified = table.Column<bool>(type: "boolean", nullable: false),
                    RequirePhotos = table.Column<bool>(type: "boolean", nullable: false),
                    MinPhotos = table.Column<int>(type: "integer", nullable: true),
                    MinProfileCompletion = table.Column<int>(type: "integer", nullable: true),
                    SortBy = table.Column<string>(type: "text", nullable: true),
                    ShowOnlineOnly = table.Column<bool>(type: "boolean", nullable: false),
                    ShowRecentlyActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilterPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "interests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IconKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_interests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageExtensions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryStatus = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DeliveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReadByCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsEdited = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeviceInfo = table.Column<string>(type: "text", nullable: true),
                    MessageId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageExtensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageExtensions_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageExtensions_Messages_MessageId1",
                        column: x => x.MessageId1,
                        principalTable: "Messages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PermanentPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    MediaUrl = table.Column<string>(type: "text", nullable: true),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    LikeCount = table.Column<int>(type: "integer", nullable: false),
                    ShareCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermanentPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermanentPosts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileBoosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BoostType = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ProfileViewsGained = table.Column<int>(type: "integer", nullable: false),
                    LikesReceived = table.Column<int>(type: "integer", nullable: false),
                    MatchesCreated = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileBoosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileBoosts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recommendation_feedback",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecommendedUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeedbackType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recommendation_feedback", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "recommendation_scores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecommendedUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgeCompatibility = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    LocationCompatibility = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    InterestCompatibility = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    LifestyleCompatibility = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ProfileCompleteness = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    VerificationScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    OverallScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HasLiked = table.Column<bool>(type: "boolean", nullable: true),
                    HasSkipped = table.Column<bool>(type: "boolean", nullable: true),
                    HasMatched = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recommendation_scores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "skip_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkippedUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SkippedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skip_profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryAnalytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalImpressions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    UniqueViewers = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    AverageWatchTimeSeconds = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    AverageWatchPercentage = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    ClickThroughCount = table.Column<int>(type: "integer", nullable: false),
                    ShareCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SaveCount = table.Column<int>(type: "integer", nullable: false),
                    TopViewersLastDay = table.Column<List<string>>(type: "text[]", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryAnalytics_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LikeCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsEdited = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    StoryId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentCommentId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryComments_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryComments_Stories_StoryId1",
                        column: x => x.StoryId1,
                        principalTable: "Stories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoryComments_StoryComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "StoryComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryComments_StoryComments_ParentCommentId1",
                        column: x => x.ParentCommentId1,
                        principalTable: "StoryComments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoryComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryLikes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReactionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "like"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StoryId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryLikes_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryLikes_Stories_StoryId1",
                        column: x => x.StoryId1,
                        principalTable: "Stories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoryLikes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    WatchPercentage = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    WatchTimeSeconds = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsCompleteView = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ViewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StoryId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryViews_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryViews_Stories_StoryId1",
                        column: x => x.StoryId1,
                        principalTable: "Stories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoryViews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PricePerMonth = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceSixMonths = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceYearly = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPopular = table.Column<bool>(type: "boolean", nullable: false),
                    BadgeText = table.Column<string>(type: "text", nullable: true),
                    Features = table.Column<string>(type: "text", nullable: false),
                    UnlimitedLikes = table.Column<int>(type: "integer", nullable: false),
                    SuperLikesPerWeek = table.Column<int>(type: "integer", nullable: false),
                    BoostsPerMonth = table.Column<int>(type: "integer", nullable: false),
                    PermanentPostsPerMonth = table.Column<int>(type: "integer", nullable: false),
                    PermanentPostsPerWeek = table.Column<int>(type: "integer", nullable: false),
                    PermanentStoriesPerWeek = table.Column<int>(type: "integer", nullable: false),
                    ViewStoryMetrics = table.Column<int>(type: "integer", nullable: false),
                    SeeWhoLikes = table.Column<int>(type: "integer", nullable: false),
                    ProfilVisibilityBoost = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SuperLikes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SentByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    IsViewed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuperLikes_Users_ReceivedByUserId",
                        column: x => x.ReceivedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SuperLikes_Users_SentByUserId",
                        column: x => x.SentByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TypingIndicators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StoppedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ConversationId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypingIndicators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TypingIndicators_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TypingIndicators_Conversations_ConversationId1",
                        column: x => x.ConversationId1,
                        principalTable: "Conversations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TypingIndicators_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TypingIndicators_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "user_preferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MinAgePreference = table.Column<int>(type: "integer", nullable: false),
                    MaxAgePreference = table.Column<int>(type: "integer", nullable: false),
                    MinHeightPreference = table.Column<int>(type: "integer", nullable: true),
                    MaxHeightPreference = table.Column<int>(type: "integer", nullable: true),
                    MaxDistancePreference = table.Column<int>(type: "integer", nullable: false),
                    SmokingPreference = table.Column<bool>(type: "boolean", nullable: true),
                    DrinkingPreference = table.Column<bool>(type: "boolean", nullable: true),
                    WantChildrenPreference = table.Column<bool>(type: "boolean", nullable: true),
                    HaveChildrenPreference = table.Column<bool>(type: "boolean", nullable: true),
                    EducationPreference = table.Column<string>(type: "text", nullable: true),
                    RelationshipType = table.Column<string>(type: "text", nullable: true),
                    ReligionPreference = table.Column<string>(type: "text", nullable: true),
                    EthnicityPreference = table.Column<string>(type: "text", nullable: true),
                    InterestCategories = table.Column<string>(type: "text", nullable: false),
                    PersonalityTraits = table.Column<string>(type: "text", nullable: false),
                    OnlyVerifiedProfiles = table.Column<bool>(type: "boolean", nullable: false),
                    HideInactiveProfiles = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_preferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserVerificationBadges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BadgeType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BadgeIcon = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AwardedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Criteria = table.Column<string>(type: "jsonb", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVerificationBadges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVerificationBadges_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerificationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AdministratorNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AdministratorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerificationRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    VerificationType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RejectionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationScore = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoiceNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AudioUrl = table.Column<string>(type: "text", nullable: false),
                    MimeType = table.Column<string>(type: "text", nullable: false),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    TranscribedText = table.Column<string>(type: "text", nullable: true),
                    IsTranscribing = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PlayCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MessageId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoiceNotes_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoiceNotes_Messages_MessageId1",
                        column: x => x.MessageId1,
                        principalTable: "Messages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VoiceNotes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoiceNotes_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FilterApplicationHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FilterPreferencesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultCount = table.Column<int>(type: "integer", nullable: false),
                    ProfilesViewed = table.Column<int>(type: "integer", nullable: true),
                    Matches = table.Column<int>(type: "integer", nullable: true),
                    Messages = table.Column<int>(type: "integer", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterApplicationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilterApplicationHistories_FilterPreferences_FilterPreferen~",
                        column: x => x.FilterPreferencesId,
                        principalTable: "FilterPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FilterApplicationHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilterCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FilterPreferencesId = table.Column<Guid>(type: "uuid", nullable: false),
                    CriteriaType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Operator = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false, defaultValue: 5),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilterCriteria_FilterPreferences_FilterPreferencesId",
                        column: x => x.FilterPreferencesId,
                        principalTable: "FilterPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedFilters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FilterPreferencesId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastAppliedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FilterData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedFilters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedFilters_FilterPreferences_FilterPreferencesId",
                        column: x => x.FilterPreferencesId,
                        principalTable: "FilterPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SavedFilters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_interests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    InterestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfidenceScore = table.Column<int>(type: "integer", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_interests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_interests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_interests_interests_InterestId",
                        column: x => x.InterestId,
                        principalTable: "interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageReadReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MessageId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    MessageExtensionId = table.Column<Guid>(type: "uuid", nullable: true),
                    MessageExtensionId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReadReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReadReceipts_MessageExtensions_MessageExtensionId",
                        column: x => x.MessageExtensionId,
                        principalTable: "MessageExtensions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReadReceipts_MessageExtensions_MessageExtensionId1",
                        column: x => x.MessageExtensionId1,
                        principalTable: "MessageExtensions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MessageReadReceipts_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReadReceipts_Messages_MessageId1",
                        column: x => x.MessageId1,
                        principalTable: "Messages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MessageReadReceipts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReadReceipts_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StoryCommentLikes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryCommentLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryCommentLikes_StoryComments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "StoryComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryCommentLikes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanType = table.Column<int>(type: "integer", nullable: false),
                    BillingPeriod = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RenewalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    PaymentMethodId = table.Column<string>(type: "text", nullable: true),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    TransactionId = table.Column<string>(type: "text", nullable: true),
                    SuperLikesUsedThisWeek = table.Column<int>(type: "integer", nullable: false),
                    BoostsUsedThisMonth = table.Column<int>(type: "integer", nullable: false),
                    PermanentPostsUsedThisMonth = table.Column<int>(type: "integer", nullable: false),
                    PermanentStoriesUsedThisWeek = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_SubscriptionPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerificationAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VerificationRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    DeviceInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FlagReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FraudScore = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdditionalData = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationAttempts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VerificationAttempts_VerificationRecords_VerificationRecord~",
                        column: x => x.VerificationRecordId,
                        principalTable: "VerificationRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerificationDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VerificationRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StorageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    MetadataJson = table.Column<string>(type: "jsonb", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    ProcessingStatus = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationDocuments_VerificationRecords_VerificationRecor~",
                        column: x => x.VerificationRecordId,
                        principalTable: "VerificationRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    PaymentGatewayId = table.Column<string>(type: "text", nullable: true),
                    PaymentMethod = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FailureReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionTransactions_UserSubscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "UserSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stories_ExpiresAt",
                table: "Stories",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_IsDeleted_CreatedAt",
                table: "Stories",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Stories_UserId_CreatedAt",
                table: "Stories",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_reports_ReportedAt",
                table: "reports",
                column: "ReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_reports_ReporterId",
                table: "reports",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_reports_Status",
                table: "reports",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundCheckResult_ExpiresAt",
                table: "BackgroundCheckResults",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundCheckResult_UserId",
                table: "BackgroundCheckResults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundCheckResult_UserId_Status",
                table: "BackgroundCheckResults",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ConversationSettings_ConversationId_UserId",
                table: "ConversationSettings",
                columns: new[] { "ConversationId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationSettings_ConversationId1",
                table: "ConversationSettings",
                column: "ConversationId1");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationSettings_UserId",
                table: "ConversationSettings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationSettings_UserId1",
                table: "ConversationSettings",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_FilterApplicationHistories_FilterPreferencesId",
                table: "FilterApplicationHistories",
                column: "FilterPreferencesId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterApplicationHistory_ExpiresAt",
                table: "FilterApplicationHistories",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_FilterApplicationHistory_UserId",
                table: "FilterApplicationHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterApplicationHistory_UserId_AppliedAt",
                table: "FilterApplicationHistories",
                columns: new[] { "UserId", "AppliedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FilterCriteria_FilterPreferencesId",
                table: "FilterCriteria",
                column: "FilterPreferencesId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterPreferences_UserId",
                table: "FilterPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterPreferences_UserId_IsActive",
                table: "FilterPreferences",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_interests_Name",
                table: "interests",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageExtensions_DeliveredAt",
                table: "MessageExtensions",
                column: "DeliveredAt");

            migrationBuilder.CreateIndex(
                name: "IX_MessageExtensions_DeliveryStatus",
                table: "MessageExtensions",
                column: "DeliveryStatus");

            migrationBuilder.CreateIndex(
                name: "IX_MessageExtensions_MessageId",
                table: "MessageExtensions",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageExtensions_MessageId1",
                table: "MessageExtensions",
                column: "MessageId1");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadReceipts_MessageExtensionId",
                table: "MessageReadReceipts",
                column: "MessageExtensionId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadReceipts_MessageExtensionId1",
                table: "MessageReadReceipts",
                column: "MessageExtensionId1");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadReceipts_MessageId_UserId",
                table: "MessageReadReceipts",
                columns: new[] { "MessageId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadReceipts_MessageId1",
                table: "MessageReadReceipts",
                column: "MessageId1");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadReceipts_ReadAt",
                table: "MessageReadReceipts",
                column: "ReadAt");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadReceipts_UserId",
                table: "MessageReadReceipts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadReceipts_UserId1",
                table: "MessageReadReceipts",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_PermanentPosts_UserId",
                table: "PermanentPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileBoosts_UserId",
                table: "ProfileBoosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_feedback_FeedbackType",
                table: "recommendation_feedback",
                column: "FeedbackType");

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_feedback_UserId_CreatedAt",
                table: "recommendation_feedback",
                columns: new[] { "UserId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_scores_ExpiresAt",
                table: "recommendation_scores",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_scores_UserId_OverallScore",
                table: "recommendation_scores",
                columns: new[] { "UserId", "OverallScore" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_scores_UserId_RecommendedUserId",
                table: "recommendation_scores",
                columns: new[] { "UserId", "RecommendedUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedFilter_UserId",
                table: "SavedFilters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedFilter_UserId_IsDefault",
                table: "SavedFilters",
                columns: new[] { "UserId", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedFilters_FilterPreferencesId",
                table: "SavedFilters",
                column: "FilterPreferencesId");

            migrationBuilder.CreateIndex(
                name: "IX_skip_profiles_ExpiresAt",
                table: "skip_profiles",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_skip_profiles_UserId_SkippedUserId",
                table: "skip_profiles",
                columns: new[] { "UserId", "SkippedUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryAnalytics_StoryId_Unique",
                table: "StoryAnalytics",
                column: "StoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCommentLikes_CommentId_UserId_Unique",
                table: "StoryCommentLikes",
                columns: new[] { "CommentId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCommentLikes_UserId",
                table: "StoryCommentLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryComments_IsDeleted_CreatedAt",
                table: "StoryComments",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryComments_ParentCommentId",
                table: "StoryComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryComments_ParentCommentId1",
                table: "StoryComments",
                column: "ParentCommentId1");

            migrationBuilder.CreateIndex(
                name: "IX_StoryComments_StoryId",
                table: "StoryComments",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryComments_StoryId1",
                table: "StoryComments",
                column: "StoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_StoryComments_UserId",
                table: "StoryComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryLikes_CreatedAt",
                table: "StoryLikes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StoryLikes_StoryId_UserId_Unique",
                table: "StoryLikes",
                columns: new[] { "StoryId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryLikes_StoryId1",
                table: "StoryLikes",
                column: "StoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_StoryLikes_UserId",
                table: "StoryLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryViews_IsCompleteView",
                table: "StoryViews",
                column: "IsCompleteView");

            migrationBuilder.CreateIndex(
                name: "IX_StoryViews_StoryId",
                table: "StoryViews",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryViews_StoryId_UserId",
                table: "StoryViews",
                columns: new[] { "StoryId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryViews_StoryId1",
                table: "StoryViews",
                column: "StoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_StoryViews_UserId",
                table: "StoryViews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryViews_ViewedAt",
                table: "StoryViews",
                column: "ViewedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionTransactions_SubscriptionId",
                table: "SubscriptionTransactions",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionTransactions_UserId",
                table: "SubscriptionTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SuperLikes_ReceivedByUserId",
                table: "SuperLikes",
                column: "ReceivedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SuperLikes_SentByUserId",
                table: "SuperLikes",
                column: "SentByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TypingIndicators_ConversationId_IsActive",
                table: "TypingIndicators",
                columns: new[] { "ConversationId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_TypingIndicators_ConversationId_UserId",
                table: "TypingIndicators",
                columns: new[] { "ConversationId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_TypingIndicators_ConversationId1",
                table: "TypingIndicators",
                column: "ConversationId1");

            migrationBuilder.CreateIndex(
                name: "IX_TypingIndicators_StartedAt",
                table: "TypingIndicators",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TypingIndicators_UserId",
                table: "TypingIndicators",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TypingIndicators_UserId1",
                table: "TypingIndicators",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_user_interests_InterestId",
                table: "user_interests",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_user_interests_UserId_InterestId",
                table: "user_interests",
                columns: new[] { "UserId", "InterestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_preferences_UserId",
                table: "user_preferences",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_PlanId",
                table: "UserSubscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVerificationBadge_BadgeType",
                table: "UserVerificationBadges",
                column: "BadgeType");

            migrationBuilder.CreateIndex(
                name: "IX_UserVerificationBadge_ExpiresAt",
                table: "UserVerificationBadges",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserVerificationBadge_UserId",
                table: "UserVerificationBadges",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVerificationBadge_UserId_IsActive",
                table: "UserVerificationBadges",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationAttempt_Status",
                table: "VerificationAttempts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationAttempt_UserId",
                table: "VerificationAttempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationAttempt_UserId_CreatedAt",
                table: "VerificationAttempts",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationAttempts_VerificationRecordId",
                table: "VerificationAttempts",
                column: "VerificationRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationDocument_ProcessingStatus",
                table: "VerificationDocuments",
                column: "ProcessingStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationDocument_VerificationRecordId",
                table: "VerificationDocuments",
                column: "VerificationRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationLog_UserId",
                table: "VerificationLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationLog_UserId_CreatedAt",
                table: "VerificationLogs",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRecord_ExpiresAt",
                table: "VerificationRecords",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRecord_Status",
                table: "VerificationRecords",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRecord_UserId",
                table: "VerificationRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRecord_UserId_Status",
                table: "VerificationRecords",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_VoiceNotes_CreatedAt",
                table: "VoiceNotes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceNotes_MessageId",
                table: "VoiceNotes",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoiceNotes_MessageId1",
                table: "VoiceNotes",
                column: "MessageId1");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceNotes_UserId",
                table: "VoiceNotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceNotes_UserId1",
                table: "VoiceNotes",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_reports_Users_ReporterId",
                table: "reports",
                column: "ReporterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_reports_Users_TargetUserId",
                table: "reports",
                column: "TargetUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_Users_UserId",
                table: "Stories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reports_Users_ReporterId",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK_reports_Users_TargetUserId",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Stories_Users_UserId",
                table: "Stories");

            migrationBuilder.DropTable(
                name: "BackgroundCheckResults");

            migrationBuilder.DropTable(
                name: "ConversationSettings");

            migrationBuilder.DropTable(
                name: "FilterApplicationHistories");

            migrationBuilder.DropTable(
                name: "FilterCriteria");

            migrationBuilder.DropTable(
                name: "MessageReadReceipts");

            migrationBuilder.DropTable(
                name: "PermanentPosts");

            migrationBuilder.DropTable(
                name: "ProfileBoosts");

            migrationBuilder.DropTable(
                name: "recommendation_feedback");

            migrationBuilder.DropTable(
                name: "recommendation_scores");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "SavedFilters");

            migrationBuilder.DropTable(
                name: "skip_profiles");

            migrationBuilder.DropTable(
                name: "StoryAnalytics");

            migrationBuilder.DropTable(
                name: "StoryCommentLikes");

            migrationBuilder.DropTable(
                name: "StoryLikes");

            migrationBuilder.DropTable(
                name: "StoryViews");

            migrationBuilder.DropTable(
                name: "SubscriptionTransactions");

            migrationBuilder.DropTable(
                name: "SuperLikes");

            migrationBuilder.DropTable(
                name: "TypingIndicators");

            migrationBuilder.DropTable(
                name: "user_interests");

            migrationBuilder.DropTable(
                name: "user_preferences");

            migrationBuilder.DropTable(
                name: "UserVerificationBadges");

            migrationBuilder.DropTable(
                name: "VerificationAttempts");

            migrationBuilder.DropTable(
                name: "VerificationDocuments");

            migrationBuilder.DropTable(
                name: "VerificationLogs");

            migrationBuilder.DropTable(
                name: "VoiceNotes");

            migrationBuilder.DropTable(
                name: "MessageExtensions");

            migrationBuilder.DropTable(
                name: "FilterPreferences");

            migrationBuilder.DropTable(
                name: "StoryComments");

            migrationBuilder.DropTable(
                name: "UserSubscriptions");

            migrationBuilder.DropTable(
                name: "interests");

            migrationBuilder.DropTable(
                name: "VerificationRecords");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropIndex(
                name: "IX_Stories_ExpiresAt",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_IsDeleted_CreatedAt",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_UserId_CreatedAt",
                table: "Stories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_reports",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "IX_reports_ReportedAt",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "IX_reports_ReporterId",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "IX_reports_Status",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "FirebaseUid",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AllowComments",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "AllowSharing",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "AllowedUserIds",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Caption",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "CommentCount",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ShareCount",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "TextColor",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "TextContent",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "VisibilityType",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ReportType",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "ResolutionNotes",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "ResolvedBy",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ShareCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Posts");

            migrationBuilder.RenameTable(
                name: "reports",
                newName: "Reports");

            migrationBuilder.RenameColumn(
                name: "ReportedAt",
                table: "Reports",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_reports_TargetUserId",
                table: "Reports",
                newName: "IX_Reports_TargetUserId");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Posts",
                newName: "Content");

            migrationBuilder.AlterColumn<string>(
                name: "MediaUrl",
                table: "Stories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Reports",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "Reports",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PostLikes",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PostComments",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reports",
                table: "Reports",
                column: "Id");
        }
    }
}
