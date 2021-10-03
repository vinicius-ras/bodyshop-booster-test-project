using BodyShopBoosterTest.Models;
using Microsoft.EntityFrameworkCore;

namespace BodyShopBoosterTest.Data
{
	/// <summary>Represents a context used to have access to database operations.</summary>
	public class AppDbContext : DbContext
	{
		// INSTANCE PROPERTIES
		/// <summary>Allows for the access of <see cref="Estimate"/> entities in the database.</summary>
		public DbSet<Estimate> Estimates { get; set; }





		// INSTANCE METHODS
		/// <summary>Constructor.</summary>
		public AppDbContext()
		{
		}


		/// <summary>Constructor.</summary>
		/// <param name="options">Options used to initialize and configure the <see cref="DbContext"/> instance.</param>
		public AppDbContext(DbContextOptions options)
			: base(options)
		{
		}


		/// <inheritdoc/>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Represent Enum-typed members from the Estimate model as strings in the database, for better readability/debugging and maintenability
			modelBuilder.Entity<Estimate>()
				.Property(estimate => estimate.CarType)
				.HasConversion<string>();
			modelBuilder.Entity<Estimate>()
				.Property(estimate => estimate.Status)
				.HasConversion<string>();
		}
	}
}