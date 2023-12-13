namespace Laan.Sql.Parser.Entities
{
    public class SetOperation
    {
        public SelectStatement Statement { get; set; }
        public SetType Type { get; set; }
    }
}