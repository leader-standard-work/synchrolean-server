using System;
using System.Linq;
using SynchroLean.Controllers;
using SynchroLean.Controllers.Resources;
using SynchroLean.Core.Models;
using SynchroLean.Core;
using SynchroLean.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;
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
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// Add user task using in memory database model from
        /// "https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory".
        /// </summary>
        [Fact]
        public async void AddTaskInMemoryTestAsync()
        {
            // Creates an InMemory database to be used for testing
            var options = new DbContextOptionsBuilder<SynchroLeanDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_task_to_database")
                .Options;

            // Create a userTaskOrigina to send to HTTPPost method
            var userTaskResource = new UserTaskResource {
                Name = "Unit test add",
                Description = "Add a task using InMemory database",
                IsRecurring = true,
                Weekdays = 40,
                //CreationDate = DateTime.Today,
                IsCompleted = false,
                //CompletionDate = DateTime.MinValue,
                IsRemoved = false
            };
            
            var userTaskId = 0;

            IUnitOfWork unitOfWork;

            // Creates the TaskController with DbContext and adds userTaskOrigina to InMemory database
            using (var context = new SynchroLeanDbContext(options))
            {
                var controller = new TasksController(context);
                var result = await controller.AddUserTaskAsync(userTaskResource);

                //Assert
                var okTaskResult = result as OkObjectResult;
                Assert.NotNull(result);

                var model = okTaskResult.Value as SynchroLean.Controllers.Resources.UserTaskResource;
                Assert.NotNull(model);

                userTaskId = model.Id;
                //output.WriteLine("userTaskId = " + userTaskId);
            }

            // Used same context to verify changes are persistent
            using (var context = new SynchroLeanDbContext(options))
            {
                Assert.Equal(1, context.UserTasks.Count());
                var userTask = context.UserTasks.SingleOrDefault(ut => ut.Id.Equals(userTaskId));
                Assert.NotNull(userTask);

                // Not sure why userTask.Name needs to be trimmed but userTask.Description doesn't
                Assert.True(userTaskResource.Name == userTask.Name.Trim());
                Assert.True(userTaskResource.Description == userTask.Description);
                Assert.True(userTaskResource.IsRecurring.Equals(userTask.IsRecurring));
                Assert.True(userTaskResource.Weekdays.Equals(userTask.Weekdays));

                // Can't test dates without changing how UserTask POST method assigns them
                /*
                int result = DateTime.Compare(userTaskResource.CreationDate, userTask.CreationDate);
                Assert.Equal(0, result);
                output.WriteLine(userTaskResource.CreationDate + ", " + userTask.CreationDate);
                result = DateTime.Compare(userTaskResource.CompletionDate, userTask.CompletionDate);
                Assert.Equal(0, result);
                output.WriteLine(userTaskResource.CompletionDate + ", " + userTask.CompletionDate);
                */

                Assert.True(userTaskResource.IsCompleted.Equals(userTask.IsCompleted));
                Assert.True(userTaskResource.IsRemoved.Equals(userTask.IsRemoved));
            }
        }

        /// <summary>
        /// Add user task using a SQLite in memory database model from 
        /// "https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/sqlite".
        /// </summary>
        [Fact]
        public async void AddTaskSQLiteTestAsync()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<SynchroLeanDbContext>()
                    .UseSqlite(connection)
                    .Options;

                /// Create a userTaskResource to send to HTTPPost method
                var userTaskResource = new UserTaskResource {
                    Name = "SQLite unit test add",
                    Description = "Add a task using SQLite database",
                    IsRecurring = true,
                    Weekdays = 40,
                    //CreationDate = DateTime.Today,
                    IsCompleted = false,
                    //CompletionDate = DateTime.MinValue,
                    IsRemoved = false
                };

                // Create the schema in the database
                using (var context = new SynchroLeanDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                var userTaskId = 0;

                // Run the test against one instance of the context
                using (var context = new SynchroLeanDbContext(options))
                { 
                    var service = new TasksController(context);
                    var result = await service.AddUserTaskAsync(userTaskResource);

                    //Assert
                    var okTaskResult = result as OkObjectResult;
                    Assert.NotNull(result);

                    var model = okTaskResult.Value as SynchroLean.Controllers.Resources.UserTaskResource;
                    Assert.NotNull(model);

                    userTaskId = model.Id;
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new SynchroLeanDbContext(options))
                {
                    Assert.Equal(1, context.UserTasks.Count());
                    var userTask = context.UserTasks.SingleOrDefault(ut => ut.Id.Equals(userTaskId));
                    Assert.NotNull(userTask);

                    // Not sure why userTask.Name needs to be trimmed but userTask.Description doesn't
                    Assert.True(userTaskResource.Name == userTask.Name.Trim());
                    Assert.True(userTaskResource.Description == userTask.Description);
                    Assert.True(userTaskResource.IsRecurring.Equals(userTask.IsRecurring));
                    Assert.True(userTaskResource.Weekdays.Equals(userTask.Weekdays));

                    // Can't test dates without changing how UserTask POST method assigns them
                    /*
                    int result = DateTime.Compare(userTaskResource.CreationDate, userTask.CreationDate);
                    Assert.Equal(0, result);
                    output.WriteLine(userTaskResource.CreationDate + ", " + userTask.CreationDate);
                    result = DateTime.Compare(userTaskResource.CompletionDate, userTask.CompletionDate);
                    Assert.Equal(0, result);
                    output.WriteLine(userTaskResource.CompletionDate + ", " + userTask.CompletionDate);
                    */

                    Assert.True(userTaskResource.IsCompleted.Equals(userTask.IsCompleted));
                    Assert.True(userTaskResource.IsRemoved.Equals(userTask.IsRemoved));
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
