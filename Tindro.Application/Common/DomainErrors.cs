namespace Tindro.Application.Common;

public static class DomainErrors
{
    public static class Match
    {
        public const string NotFound = "Match.NotFound";
        public const string NotAuthorized = "Match.NotAuthorized";
        public const string AlreadyExists = "Match.AlreadyExists";
        public const string CannotMatchWithYourself = "Match.CannotMatchWithYourself";
    }

    public static class User
    {
        public const string NotFound = "User.NotFound";
    }

    public static class Swipe
    {
        public const string CannotSwipeYourself = "Swipe.CannotSwipeYourself";
        public const string AlreadySwiped = "Swipe.AlreadySwiped";
    }

    public static class Profile
    {
        public const string NotFound = "Profile.NotFound";
    }
}
