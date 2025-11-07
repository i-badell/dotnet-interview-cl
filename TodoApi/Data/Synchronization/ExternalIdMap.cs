namespace TodoApi.Data.Synchronization;

public class ExternalIdMap
{
	public long Id { get; set; }
	public EntityTypes EntityType { get; set; }
	public long LocalId { get; set; }
	public long? ParentLocalListId { get; set; }
	public string ExternalId { get; set; } = default!;
	public string SourceId { get; set; } = default!; 
	public DateTime? ExternalUpdatedAt { get; set; }
	public DateTime? LocalUpdatedAt { get; set; }
	public string? Hash { get; set; }
	public DateTime? LastSyncedAt { get; set; }
}
