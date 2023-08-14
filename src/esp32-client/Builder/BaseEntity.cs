

using LinqToDB.Mapping;

namespace esp32_client.Builder;

public class BaseEntity
{
    [Column(IsIdentity = true)]
    public int Id { get; set; }
}