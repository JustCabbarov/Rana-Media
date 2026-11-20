namespace RanaMedia.Domain.Enums;

public enum ArticleType
{
    Essay = 1,
    Video = 2,
    Idea = 3,
    Classic = 4
}

public enum ArticleStatus
{
    Draft = 1,
    Scheduled = 2,
    Published = 3,
    Expired = 4,
    Archived = 5
}

public enum ContributorRole
{
    Author = 1,
    Editor = 2,
    Director = 3
}

public enum VideoHoster
{
    None = 0,
    YouTube = 1,
    Vimeo = 2,
    SelfHosted = 3
}

public enum PageMarkType
{
    Highlight = 1,
    Annotation = 2
}

public enum ViewEventType
{
    View = 1,
    EngagedView = 2,
    AudioPlay = 3
}

public enum NewsletterStatus
{
    Pending = 1,
    Active = 2,
    Unsubscribed = 3
}

public enum SubmissionStatus
{
    New = 1,
    InReview = 2,
    Approved = 3,
    Rejected = 4
}

public enum MediaFolder
{
    Images = 1,
    Videos = 2,
    Audio = 3,
    Avatars = 4
}

public enum SeoTrafficLight
{
    Red = 1,
    Orange = 2,
    Green = 3
}

public enum SeoCheckSeverity
{
    Problem = 1,
    Improvement = 2,
    Good = 3
}

