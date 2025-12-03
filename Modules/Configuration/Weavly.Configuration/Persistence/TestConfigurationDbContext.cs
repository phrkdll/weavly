using Microsoft.EntityFrameworkCore;

namespace Weavly.Configuration.Persistence;

public class TestConfigurationDbContext(DbContextOptions options) : ConfigurationDbContext(options);
