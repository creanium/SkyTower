namespace SkyTower.Application.MesoscaleDiscussions;

public class MesoscaleDiscussionSummary(Uri permalink, Uri imageUri, string discussionText)
{
	public Uri Permalink { get; private set; } = permalink;
	
	public Uri ImageUri { get; private set; } = imageUri;
	
	public string DiscussionText { get; set; } = discussionText;
}