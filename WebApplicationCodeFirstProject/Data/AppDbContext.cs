using Microsoft.EntityFrameworkCore;
using WebApplicationCodeFirstProject.Models;

namespace WebApplicationCodeFirstProject.Data;

public class AppDbContext : DbContext
{
    
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}