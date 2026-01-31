using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Application.Feed.Commands.Comments;
using Tindro.Application.Feed.Commands;
using Tindro.Infrastructure.Persistence;
using Tindro.Api.Extensions;

[Authorize]
[ApiController]
[Route("api/feed")]
public class FeedController : ControllerBase
{
    private readonly QueryDbContext _db;
    private readonly IMediator _mediator;
   
    public FeedController(QueryDbContext db, IMediator mediator)
    {
        _db = db;
        _mediator = mediator;
    }

    [HttpGet]
    public IActionResult GetFeed()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var swiped = _db.Swipes
            .Where(x => x.FromUserId == userId)
            .Select(x => x.ToUserId);

        var matched = _db.Matches
            .Where(x => x.User1Id == userId || x.User2Id == userId)
            .Select(x => x.User1Id == userId ? x.User2Id : x.User1Id);

        var boostUsers = _db.Boosts
            .Where(x => x.ActiveUntil > DateTime.UtcNow)
            .Select(x => x.UserId);

        var goldUsers = _db.Subscriptions
            .Where(x => x.Plan == "Gold" && x.EndDate > DateTime.UtcNow)
            .Select(x => x.UserId);

       var me = _db.Profiles.First(x => x.UserId == userId);

var feed = _db.Users
    .Where(x => x.Profile.Gender == me.GenderPreference || me.GenderPreference == "Both")
    .Where(x =>
        DateTime.UtcNow.Year - x.Profile.DateOfBirth.Year >= me.MinAgePreference &&
        DateTime.UtcNow.Year - x.Profile.DateOfBirth.Year <= me.MaxAgePreference
    )

            .Where(x => x.Id != userId)
            .Where(x => !swiped.Contains(x.Id))
            .Where(x => !matched.Contains(x.Id))
            .Where(x => !x.IsShadowBanned)
            .Select(x => new
            {
                User = x,
                Score =
                    (boostUsers.Contains(x.Id) ? 50 : 0) +
                    (goldUsers.Contains(x.Id) ? 20 : 0) +
                    (x.LastActive > DateTime.UtcNow.AddMinutes(-15) ? 10 : 0)
            })
            .OrderByDescending(x => x.Score)
            .Take(50)
            .Select(x => x.User)
            .ToList();

        return Ok(feed);
    }
    
       

       

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostCommand command)
        {
            var post = await _mediator.Send(command);
            return Ok(post);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> Get(Guid postId)
        {
            return Ok(await _mediator.Send(new GetPostByIdQuery(postId)));
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> Delete(Guid postId)
        {
            var userId = User.GetUserId();
            await _mediator.Send(new DeletePostCommand(postId, userId));
                return NoContent();
        }


    [HttpPost("{postId}/like")]
    public async Task<IActionResult> Like(Guid postId)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new LikePostCommand(postId, userId));
        return Ok();
    }

    [HttpDelete("{postId}/like")]
    public async Task<IActionResult> Unlike(Guid postId)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new UnlikePostCommand(postId, userId));
        return Ok();
    }

    [HttpPost("{postId}/comment")]
    public async Task<IActionResult> Comment(Guid postId, [FromBody] string text)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new AddCommentCommand(postId, userId, text));
        return Ok();
    }

    [HttpGet("{postId}/comments")]
    public async Task<IActionResult> GetComments(Guid postId)
    {
        return Ok(await _mediator.Send(new GetCommentsQuery(postId)));
    }



}
