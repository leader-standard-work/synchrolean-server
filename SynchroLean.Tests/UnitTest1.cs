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
            var newUserTask = new UserTask {
                Name = "Unit test add",
                Description = "Add a task using InMemory database",
                IsRecurring = true,
                Weekdays = 40,
                //CreationDate = DateTime.Today,
                IsCompleted = false,
                //CompletionDate = DateTime.MinValue,
                IsRemoved = false
            };
            
            IUnitOfWork unitOfWork;

            // Creates the TaskController with DbContext and adds newUserTask to InMemory database
            using (var context = new SynchroLeanDbContext(options))
            {
                // Bind the context to the UnitOfWork object
                unitOfWork = new UnitOfWork(context);

                // Add the task to the Db asynchronously
                await unitOfWork.userTaskRepository.AddAsync(newUserTask);
                await unitOfWork.CompleteAsync();
            }

            // Used same context to verify changes are persistent
            using (var context = new SynchroLeanDbContext(options))
            {
                // Assert that UserTasks table contains one entry
                Assert.Equal(1, context.UserTasks.Count());
                
                // Bind the context to the UnitOfWork object
                unitOfWork = new UnitOfWork(context);

                // Retrieve the task from the Db asynchronously
                var userTask = await unitOfWork.userTaskRepository.GetTaskAsync(newUserTask.Id);
                await unitOfWork.CompleteAsync();
                Assert.NotNull(userTask);

                // Not sure why userTask.Name needs to be trimmed but userTask.Description doesn't
                Assert.True(newUserTask.Name == userTask.Name.Trim());
                Assert.True(newUserTask.Description == userTask.Description);
                Assert.True(newUserTask.IsRecurring.Equals(userTask.IsRecurring));
                Assert.True(newUserTask.Weekdays.Equals(userTask.Weekdays));

                // Can't test dates without changing how UserTask POST method assigns them
                /*
                int result = DateTime.Compare(newUserTask.CreationDate, userTask.CreationDate);
                Assert.Equal(0, result);
                output.WriteLine(newUserTask.CreationDate + ", " + userTask.CreationDate);
                result = DateTime.Compare(newUserTask.CompletionDate, userTask.CompletionDate);
                Assert.Equal(0, result);
                output.WriteLine(newUserTask.CompletionDate + ", " + userTask.CompletionDate);
                */

                Assert.True(newUserTask.IsCompleted.Equals(userTask.IsCompleted));
                Assert.True(newUserTask.IsRemoved.Equals(userTask.IsRemoved));
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

                /// Create a newUserTask to send to HTTPPost method
                var newUserTask = new UserTask {
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

                var userTaskId = newUserTask.Id;

                IUnitOfWork unitOfWork;

                // Run the test against one instance of the context
                using (var context = new SynchroLeanDbContext(options))
                { 
                    // Bind the context to the UnitOfWork object
                    unitOfWork = new UnitOfWork(context);

                    // Add newUserTask to UserTasks table in Db asynchronously
                    await unitOfWork.userTaskRepository.AddAsync(newUserTask);
                    await unitOfWork.CompleteAsync();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new SynchroLeanDbContext(options))
                {
                    // Assert that UserTasks table in Db contains 1 entry
                    Assert.Equal(1, context.UserTasks.Count());
                    
                    // Bind the Db context to the UnitOfWork object
                    unitOfWork = new UnitOfWork(context);

                    // Retrieve the task from the Db asynchronously
                    var userTask = await unitOfWork.userTaskRepository.GetTaskAsync(newUserTask.Id);
                    await unitOfWork.CompleteAsync();
                    Assert.NotNull(userTask);

                    // Not sure why userTask.Name needs to be trimmed but userTask.Description doesn't
                    Assert.True(newUserTask.Name == userTask.Name.Trim());
                    Assert.True(newUserTask.Description == userTask.Description);
                    Assert.True(newUserTask.IsRecurring.Equals(userTask.IsRecurring));
                    Assert.True(newUserTask.Weekdays.Equals(userTask.Weekdays));

                    // Can't test dates without changing how UserTask POST method assigns them
                    /*
                    int result = DateTime.Compare(newUserTask.CreationDate, userTask.CreationDate);
                    Assert.Equal(0, result);
                    output.WriteLine(newUserTask.CreationDate + ", " + userTask.CreationDate);
                    result = DateTime.Compare(newUserTask.CompletionDate, userTask.CompletionDate);
                    Assert.Equal(0, result);
                    output.WriteLine(newUserTask.CompletionDate + ", " + userTask.CompletionDate);
                    */

                    Assert.True(newUserTask.IsCompleted.Equals(userTask.IsCompleted));
                    Assert.True(newUserTask.IsRemoved.Equals(userTask.IsRemoved));
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
