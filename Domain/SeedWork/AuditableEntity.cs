namespace Domain.SeedWork;

public class AuditableEntity {
    public virtual Guid Id { get; protected set; }
    public virtual bool IsDeleted { get; protected set; }
    public virtual DateTime Created { get; protected set; } = DateTime.Now;
    public virtual DateTime? LastModified { get; protected set; }
    public virtual string? ModifyUser { get; protected set; }

    public virtual void Delete() {
        this.IsDeleted = true;
        this.LastModified = DateTime.Now;
    }

    public virtual void MakeModified() {
        this.LastModified = DateTime.Now;
    }

    public virtual void MakeCreated() {
        this.Created = DateTime.Now;
    }

    public virtual void MakeModifiedUser(string user) {
        this.ModifyUser = user;
    }
}