using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olbrasoft.Mediation.Tests;
public class MediationBuilderTests
{
    // This is a test method for the MediationBuilder class that checks if the Services property is set correctly.
    [Fact]
    public void Constructor_Should_Set_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        var result = builder.Services;

        // Assert
        Assert.Equal(services, result);
    }

    //
}
