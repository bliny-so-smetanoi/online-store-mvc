using Domain.SeedWork;

namespace Domain.Entities.Orders;

public class VisitorAuthenticationCode : AuditableEntity
{
    public const int MaxFailedAttempts = 5;
    public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(5);

    public Visitor Visitor { get; protected set; }
    public string Code  { get; protected set; }
    public int FailedAttempts { get; protected set; }
    
    protected VisitorAuthenticationCode(){}
    
    protected  VisitorAuthenticationCode(Visitor visitor, string code)
    {
        Visitor = visitor;
        Code = code;
    }

    public static VisitorAuthenticationCode Create(Visitor visitor, string code)
    {
        return new VisitorAuthenticationCode(visitor, code);
    }

    public bool IsActive(DateTime now)
    {
        return !IsDeleted && FailedAttempts < MaxFailedAttempts && Created.Add(Lifetime) > now;
    }

    public bool TryUse(string code)
    {
        if (!IsActive(DateTime.Now))
            return false;

        if (Code != code)
        {
            FailedAttempts++;
            MakeModified();
            return false;
        }

        Delete();
        return true;
    }
}
