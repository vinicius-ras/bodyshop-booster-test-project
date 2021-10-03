using Microsoft.EntityFrameworkCore;

namespace BodyShopBoosterTest.Data
{
	/// <summary>Represents a context used to have access to database operations.</summary>
	public class AppDbContext : DbContext
	{
		// INSTANCE METHODS
		/// <summary>Constructor.</summary>
		/// <param name="options">Options used to initialize and configure the <see cref="DbContext"/> instance.</param>
		public AppDbContext(DbContextOptions options)
			: base(options)
		{
		}
	}
}