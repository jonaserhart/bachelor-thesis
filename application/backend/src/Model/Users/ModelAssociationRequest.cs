using backend.Model.Analysis;

namespace backend.Model.Users;

public class ModelAssociationRequest
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public AnalysisModel? Model { get; set; }
    public Guid ModelId { get; set; }
    public User? IssuedBy { get; set; }
    public Guid IssuedById { get; set; }
    public long IssuedAt { get; set; }
    public bool Completed { get; set; } = false;
    public long CompletedAt { get; set; }
    public ModelPermission Permission { get; set; }
}