using Microsoft.EntityFrameworkCore;
using Weavly.Configuration.Persistence;

namespace Weavly.Configuration.Tests;

public class TestConfigurationDbContext(DbContextOptions options) : ConfigurationDbContext(options);
