namespace SkyTower.Application.MesoscaleDiscussions;

public interface IMesoscaleDiscussionService
{
	Task FetchNewDiscussions();

	Task<MesoscaleDiscussionSummary> GetDiscussion(int year, int number);
	
	MesoscaleDiscussionDetails ParseDiscussionText(string rawText);
}