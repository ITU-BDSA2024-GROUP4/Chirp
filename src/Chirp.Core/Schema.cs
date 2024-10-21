using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core;

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