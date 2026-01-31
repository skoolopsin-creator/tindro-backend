using Tindro.Application.Payments.Interfaces;
using Tindro.Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace Tindro.Infrastructure.Persistence.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly QueryDbContext _context;

        public SubscriptionRepository(QueryDbContext context)
        {
            _context = context;
        }

        // Plans
        public async Task<SubscriptionPlan?> GetPlanAsync(Guid planId)
        {
            return await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == planId && p.IsActive);
        }

        public async Task<List<SubscriptionPlan>> GetAllPlansAsync()
        {
            return await _context.SubscriptionPlans.Where(p => p.IsActive).OrderBy(p => p.PricePerMonth).ToListAsync();
        }

        public async Task<SubscriptionPlan> CreatePlanAsync(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task UpdatePlanAsync(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Update(plan);
            await _context.SaveChangesAsync();
        }

        // User subscriptions
        public async Task<UserSubscription?> GetUserSubscriptionAsync(Guid userId)
        {
            return await _context.UserSubscriptions
                .Where(s => s.UserId == userId && s.Status == "active")
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();
        }

        public async Task<UserSubscription> CreateUserSubscriptionAsync(UserSubscription subscription)
        {
            _context.UserSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<UserSubscription> UpdateUserSubscriptionAsync(UserSubscription subscription)
        {
            subscription.UpdatedAt = DateTime.UtcNow;
            _context.UserSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task CancelSubscriptionAsync(Guid subscriptionId)
        {
            var subscription = await _context.UserSubscriptions.FindAsync(subscriptionId);
            if (subscription != null)
            {
                subscription.Status = "cancelled";
                subscription.CancelledAt = DateTime.UtcNow;
                _context.UserSubscriptions.Update(subscription);
                await _context.SaveChangesAsync();
            }
        }

        // Transactions
        public async Task<SubscriptionTransaction> CreateTransactionAsync(SubscriptionTransaction transaction)
        {
            _context.SubscriptionTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<SubscriptionTransaction>> GetUserTransactionsAsync(Guid userId, int limit = 50)
        {
            return await _context.SubscriptionTransactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        // Super likes
        public async Task<SuperLike?> GetSuperLikeAsync(Guid superLikeId)
        {
            return await _context.SuperLikes.FirstOrDefaultAsync(sl => sl.Id == superLikeId);
        }

        public async Task<SuperLike> CreateSuperLikeAsync(SuperLike superLike)
        {
            _context.SuperLikes.Add(superLike);
            await _context.SaveChangesAsync();
            return superLike;
        }

        public async Task<List<SuperLike>> GetReceivedSuperLikesAsync(Guid userId)
        {
            return await _context.SuperLikes
                .Where(sl => sl.ReceivedByUserId == userId && !sl.IsViewed)
                .OrderByDescending(sl => sl.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkSuperLikeViewedAsync(Guid superLikeId)
        {
            var superLike = await _context.SuperLikes.FindAsync(superLikeId);
            if (superLike != null)
            {
                superLike.IsViewed = true;
                superLike.ViewedAt = DateTime.UtcNow;
                _context.SuperLikes.Update(superLike);
                await _context.SaveChangesAsync();
            }
        }

        // Boosts
        public async Task<ProfileBoost?> GetActiveBoostAsync(Guid userId)
        {
            return await _context.ProfileBoosts
                .Where(b => b.UserId == userId && b.IsActive && b.EndTime > DateTime.UtcNow)
                .OrderByDescending(b => b.StartTime)
                .FirstOrDefaultAsync();
        }

        public async Task<ProfileBoost> CreateBoostAsync(ProfileBoost boost)
        {
            _context.ProfileBoosts.Add(boost);
            await _context.SaveChangesAsync();
            return boost;
        }

        public async Task<List<ProfileBoost>> GetUserBoostHistoryAsync(Guid userId)
        {
            return await _context.ProfileBoosts
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.StartTime)
                .Take(20)
                .ToListAsync();
        }

        // Permanent posts
        public async Task<PermanentPost?> GetPermanentPostAsync(Guid postId)
        {
            return await _context.PermanentPosts.FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task<List<PermanentPost>> GetUserPermanentPostsAsync(Guid userId)
        {
            return await _context.PermanentPosts
                .Where(p => p.UserId == userId && (p.ExpiresAt == null || p.ExpiresAt > DateTime.UtcNow))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<PermanentPost> CreatePermanentPostAsync(PermanentPost post)
        {
            _context.PermanentPosts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task UpdatePermanentPostAsync(PermanentPost post)
        {
            _context.PermanentPosts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePermanentPostAsync(Guid postId)
        {
            var post = await _context.PermanentPosts.FindAsync(postId);
            if (post != null)
            {
                _context.PermanentPosts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }
    }
}
