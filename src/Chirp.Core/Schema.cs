using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core;

// If any changes are made to the "schema" then you need to run following commands to update the migration
// 1: Be in Chirp/src directory
// 2: dotnet ef migrations add "name of change" --project Chirp.Infrastructure --startup-project Chirp.Web
// 2.5 the "name of change" should not be a in " when typing command, could be: intialCreate 

public class Author
{
    [Key]
    public int AuthorId { get; set;}

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required List<Cheep> Cheeps { get; set; }
}

public class Cheep
{
    [Key]
    public int CheepId { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [Required]
    public required Author Author { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR(160)")]
    [MaxLength(160)]
    public required string Text { get; set; } 

    [Required]
    public System.DateTime TimeStamp { get; set; }
}

public class Follows
{
    [Required]
    public int User { get; set; }
    [Required]
    public int Following { get; set; }
}