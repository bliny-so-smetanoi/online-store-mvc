using Domain.SeedWork;

namespace Domain.Entities.Orders;

public class VisitorAuthenticationCode : AuditableEntity
{
    public Visitor Visitor { get; protected set; }
    public string Code  { get; protected set; }
    
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
}