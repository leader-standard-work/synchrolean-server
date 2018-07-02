using System;
using System.Linq;
using SynchroLean.Controllers;
using SynchroLean.Controllers.Resources;
using SynchroLean.Models;
using SynchroLean.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

/// <summary>
/// Unit Test file for TasksController
/// </summary>

namespace SynchroLean.Tests
{
    public class UnitTest1
    {
        /// <summary>
        /// Add user task using in memory database model from
        /// "https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory".
        /// </summary>
        [Fact]
        public void AddTaskInMemoryTest()
        {
            // Creates an InMemory database to be used for testing
            var options = new DbContextOptionsBuilder<SynchroLeanDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_task_to_database")
                .Options;

            // Create a UserTaskResource to send to HTTPPost method
            var userTaskResource = new UserTaskResource {
                Id = 0,
                Name = "Unit test add",
                Description = "Add a task using InMemory database",
                IsRecurring = true,
                Weekdays = 40,
                CreationDate = DateTime.Today,
                IsCompleted = false,
                CompletionDate = DateTime.Today.AddDays(2),
                IsRemoved = false
            };

            // Creates the TaskController with DbContext and adds userTaskResource to InMemory database
            using (var context = new SynchroLeanDbContext(options))
            {
                var controller = new TasksController(context);
                var result = controller.AddUserTaskAsync(userTaskResource);
            }

            // Used same context to verify changes are persistent
            using (var context = new SynchroLeanDbContext(options))
            {
                Assert.Equal(1, context.UserTasks.Count());

                // Not sure how to compare what's in the database to userTaskResource
                //Assert.True(context.UserTasks.Single().Equals(userTaskResource), "User Tasks Match!!");
            }
        }

        /// <summary>
        /// Add user task using a SQLite in memory database model from 
        /// "https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/sqlite".
        /// </summary>
        [Fact]
        public void AddTaskSQLiteTest()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<SynchroLeanDbContext>()
                    .UseSqlite(connection)
                    .Options;

                /// Create a UserTaskResource to send to HTTPPost method
                var userTaskResource = new UserTaskResource {
                    Id = 0,
                    Name = "SQLite unit test add",
                    Description = "Add a task using SQLite database",
                    IsRecurring = true,
                    Weekdays = 40,
                    CreationDate = DateTime.Today,
                    IsCompleted = false,
                    CompletionDate = DateTime.Today.AddDays(1),
                    IsRemoved = false
                };

                // Create the schema in the database
                using (var context = new SynchroLeanDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                // Run the test against one instance of the context
                using (var context = new SynchroLeanDbContext(options))
                { 
                    var service = new TasksController(context);
                    var result = service.AddUserTaskAsync(userTaskResource);
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new SynchroLeanDbContext(options))
                {
                    Assert.Equal(1, context.UserTasks.Count());
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
