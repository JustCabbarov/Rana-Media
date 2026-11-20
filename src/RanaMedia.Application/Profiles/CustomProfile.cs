using System.Text.Json;
using AutoMapper;
using RanaMedia.Contract.DTOs.Auth;
using RanaMedia.Contract.DTOs.Editorial;
using RanaMedia.Contract.DTOs.Engagement;
using RanaMedia.Contract.DTOs.Forms;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Enums;

namespace RanaMedia.Application.Profiles;

public class CustomProfile : Profile
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CustomProfile()
    {
        // System & Auth
        CreateMap<AppUser, UserDTO>()
            .ForMember(d => d.RoleName, o => o.Ignore())
            .ForMember(d => d.UnseenSavedArticleCount, o => o.Ignore())
            .ForMember(d => d.UnreadNotificationsCount, o => o.Ignore());

        CreateMap<CreateRoleDTO, AppRole>(MemberList.Source)
            .ReverseMap();
        CreateMap<Notification, NotificationDTO>();

        // Editorial Taxonomy & Contributors
        CreateMap<Section, SectionSummaryDTO>();
        CreateMap<Section, SectionDetailDTO>()
            .ForMember(d => d.Articles, o => o.Ignore());

        CreateMap<Topic, TopicSummaryDTO>();
        CreateMap<Topic, TopicDetailDTO>()
            .ForMember(d => d.SiblingTopics, o => o.Ignore())
            .ForMember(d => d.Articles, o => o.Ignore());

        CreateMap<Tag, TagDTO>();

        CreateMap<Contributor, ContributorShortDTO>();
        CreateMap<Contributor, ContributorProfileDTO>()
            .ForMember(d => d.AuthoredArticles, o => o.Ignore())
            .ForMember(d => d.EditedArticles, o => o.Ignore());
        CreateMap<CreateContributorDTO, Contributor>(MemberList.Source);

        // Articles
        CreateMap<Article, ArticleCardDTO>()
            .ForMember(d => d.Settings, o => o.MapFrom(s => ParseDisplaySettings(s.DisplaySettingsJson)))
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.ArticleTags.Select(at => at.Tag).ToList()))
            .ForMember(d => d.Authors, o => o.MapFrom(s =>
                s.ArticleContributors
                    .Where(ac => ac.Role == ContributorRole.Author)
                    .OrderBy(ac => ac.DisplayOrder)
                    .Select(ac => ac.Contributor)
                    .ToList()));

        CreateMap<Article, ArticleDetailDTO>()
            .IncludeBase<Article, ArticleCardDTO>()
            .ForMember(d => d.Editor, o => o.MapFrom(s =>
                s.ArticleContributors
                    .Where(ac => ac.Role == ContributorRole.Editor)
                    .Select(ac => ac.Contributor)
                    .FirstOrDefault()))
            .ForMember(d => d.Topics, o => o.MapFrom(s => s.ArticleTopics.Select(at => at.Topic).ToList()))
            .ForMember(d => d.Related, o => o.MapFrom(s =>
                s.RelatedFrom.OrderBy(r => r.DisplayOrder).Select(r => r.RelatedArticle).ToList()))
            .ForMember(d => d.TopComments, o => o.MapFrom(s =>
                s.Comments
                    .Where(c => !c.IsModerated)
                    .OrderByDescending(c => c.IsFeatured)
                    .ThenByDescending(c => c.LikesCount)
                    .Take(5)
                    .ToList()))
            .ForMember(d => d.CommentAndRepliesCount, o => o.MapFrom(s =>
                s.Comments.Count(c => !c.IsModerated) +
                s.Comments.Where(c => !c.IsModerated).Sum(c => c.Replies.Count(r => !r.IsModerated))));

        CreateMap<Article, VideoDetailDTO>()
            .IncludeBase<Article, ArticleDetailDTO>()
            .ForMember(d => d.IsExpired, o => o.MapFrom(s =>
                s.Status == ArticleStatus.Expired || (s.ExpiredAt.HasValue && s.ExpiredAt <= DateTime.UtcNow)))
            .ForMember(d => d.RecentVideos, o => o.Ignore());

        CreateMap<CreateArticleDTO, Article>(MemberList.Source)
            .ForSourceMember(s => s.TopicIds, o => o.DoNotValidate())
            .ForSourceMember(s => s.TagNames, o => o.DoNotValidate())
            .ForSourceMember(s => s.AuthorIds, o => o.DoNotValidate())
            .ForSourceMember(s => s.EditorId, o => o.DoNotValidate())
            .ForSourceMember(s => s.RelatedArticleIds, o => o.DoNotValidate())
            .ForSourceMember(s => s.FocusKeyphrase, o => o.DoNotValidate())
            .ForSourceMember(s => s.Synonyms, o => o.DoNotValidate())
            .ForSourceMember(s => s.RelatedKeyphrases, o => o.DoNotValidate())
            .ForSourceMember(s => s.Settings, o => o.DoNotValidate())
            .ForMember(d => d.DisplaySettingsJson, o => o.MapFrom(s => JsonSerializer.Serialize(s.Settings, JsonOpts)))
            .ForMember(d => d.ArticleTopics, o => o.Ignore())
            .ForMember(d => d.ArticleTags, o => o.Ignore())
            .ForMember(d => d.ArticleContributors, o => o.Ignore())
            .ForMember(d => d.RelatedFrom, o => o.Ignore())
            .ForMember(d => d.SeoProfile, o => o.Ignore());

        // Engagement
        CreateMap<PageMark, PageMarkDTO>()
            .ForMember(d => d.Index, o => o.MapFrom(s => s.OccurrenceIndex))
            .ForMember(d => d.Text, o => o.MapFrom(s => s.SelectedText))
            .ForMember(d => d.Comment, o => o.MapFrom(s => s.CommentText));

        CreateMap<PageMarkDTO, PageMark>(MemberList.Source)
            .ForMember(d => d.OccurrenceIndex, o => o.MapFrom(s => s.Index))
            .ForMember(d => d.SelectedText, o => o.MapFrom(s => s.Text))
            .ForMember(d => d.CommentText, o => o.MapFrom(s => s.Comment));

        CreateMap<CommentReply, CommentReplyDTO>()
            .ForMember(d => d.CanEditNow, o => o.MapFrom(s => DateTime.UtcNow <= s.EditableUntil))
            .ForMember(d => d.User, o => o.MapFrom(s => new CommentUserDTO
            {
                Id = s.UserId,
                Name = s.User != null ? s.User.FullName : "Reader",
                AvatarUrl = s.User != null ? s.User.AvatarUrl : null
            }));

        CreateMap<Comment, CommentDTO>()
            .ForMember(d => d.CanEditNow, o => o.MapFrom(s => DateTime.UtcNow <= s.EditableUntil))
            .ForMember(d => d.User, o => o.MapFrom(s => new CommentUserDTO
            {
                Id = s.UserId,
                Name = s.User != null ? s.User.FullName : "Reader",
                AvatarUrl = s.User != null ? s.User.AvatarUrl : null
            }));

        // Forms & Static
        CreateMap<StaticPage, StaticPageDTO>();
        CreateMap<StaticPageDTO, StaticPage>(MemberList.Source);
        CreateMap<CallToAction, CallToActionDTO>();
        CreateMap<CallToActionDTO, CallToAction>(MemberList.Source);
        CreateMap<FeatureFlag, FeatureFlagDTO>();
        CreateMap<FeatureFlagDTO, FeatureFlag>(MemberList.Source);
        CreateMap<PitchSubmission, PitchResponseDTO>();
        CreateMap<SyndicationRequest, SyndicationResponseDTO>()
            .ForMember(d => d.ArticleSlug, o => o.MapFrom(s => s.Article != null ? s.Article.Slug : string.Empty));
    }

    private static DisplaySettingsDTO ParseDisplaySettings(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new DisplaySettingsDTO();
        try
        {
            return JsonSerializer.Deserialize<DisplaySettingsDTO>(json, JsonOpts) ?? new DisplaySettingsDTO();
        }
        catch
        {
            return new DisplaySettingsDTO();
        }
    }
}
