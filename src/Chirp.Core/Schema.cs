#nullable disable
using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core;

// If any changes are made to the "schema" then you need to run following commands to update the migration
// 1: Be in Chirp/src directory
// 2: dotnet ef migrations add <NAVN> --context ChirpDBContext --project Chirp.Infrastructure --startup-project Chirp.Web
// 2.5 the "name of change" should not be a in " when typing command, could be: intialCreate 

public class Author
{
    [Key] public int AuthorId { get; set; }

    [Required] public required string Name { get; set; }

    [Required] public required string Email { get; set; }

    [Required] public required List<Cheep> Cheeps { get; set; }
}

public class Cheep
{
    [Key] public int CheepId { get; set; }

    [Required] public int AuthorId { get; set; }

    [Required] public required Author Author { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR(160)")]
    [MaxLength(160)]
    public required string Text { get; set; }

    [Required] public System.DateTime TimeStamp { get; set; }
}

public class Follows
{
    [Key] [Column(Order = 1)] [Required] public int UserId { get; set; }

    [Column(Order = 2)] [Required] public int FollowingId { get; set; }

    [Required] public Author User { get; set; }

    [Required] public Author Following { get; set; }
}

public class Likes
{
    [Key] [Required] public int LikeId { get; set; }

    [Required] public Author User { get; set; }

    [Required] public Cheep cheep { get; set; }
}

public class Blocked
{
    [Key] [Required] public int BlockedId { get; set; }

    [Key] [Required] public Author User { get; set; }

    [Required] public Author BlockedUser { get; set; }
}