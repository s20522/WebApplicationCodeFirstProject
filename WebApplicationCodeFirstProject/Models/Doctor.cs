using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationCodeFirstProject.Models;

[Table("Doctor")]
public class Doctor
{
    [Key]
    public int IdDoctor {get; set;}
    
    [MaxLength(100)] 
    public string FirstName { get; set; }
    
    [MaxLength(100)] 
    public string LastName { get; set; }
    
    [MaxLength(100)] 
    public string Email { get; set; }
    
    //Właściwość nawigująca
    public virtual ICollection<Prescription> Prescriptions { get; set; } = null!;
}