using CSharpFunctionalExtensions;

namespace Domain.ValueObjects;

public class BaseReference : ValueObject {
    public BaseReference(string en, string kk, string code) {
        En = en;
        Kk = kk;
        Code = code;
    }
    public BaseReference() {

    }

    public string En { get; protected set; }
    public string Kk { get; protected set; }
    public string Code { get; protected set; }

    protected override IEnumerable<IComparable> GetEqualityComponents() {
        yield return $"{En} {Kk} {Code}";
    }
}