using Xunit;
using ReelBuy.Frontend.Shared;
using Microsoft.AspNetCore.Components;
using System;

namespace ReelBuy.Tests;

public class FilterComponentTests
{
    [Fact]
    public void FilterValue_WhenSet_TriggersEvent()
    {
        // Arrange
        var filterValue = "";
        var eventTriggered = false;
        var component = new FilterComponent
        {
            ApplyFilter = new EventCallback<string>(null, (value) => 
            {
                filterValue = value;
                eventTriggered = true;
            })
        };

        // Act
        component.FilterValue = "test";
        component.OnFilterApply();

        // Assert
        Assert.True(eventTriggered);
        Assert.Equal("test", filterValue);
    }

    [Fact]
    public void CleanFilter_WhenCalled_ResetsValue()
    {
        // Arrange
        var component = new FilterComponent
        {
            FilterValue = "test"
        };

        // Act
        component.CleanFilter();

        // Assert
        Assert.Equal("", component.FilterValue);
    }

    [Fact]
    public void FilterValue_WhenSet_UpdatesUI()
    {
        // Arrange
        var component = new FilterComponent();

        // Act
        component.FilterValue = "test";

        // Assert
        Assert.Equal("test", component.FilterValue);
    }
} 