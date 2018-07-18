using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using SynchroLean.Controllers;
using SynchroLean.Controllers.Resources;
using SynchroLean.Core.Models;
using SynchroLean.Core;
using SynchroLean.Persistence;
using SynchroLean.Profile;

/// <summary>
/// Unit Test file for TasksController
/// </summary>

namespace SynchroLean.Tests
{
    public class UserTaskUnitTests
    {
        private readonly ITestOutputHelper output;

        private readonly IMapper mapper;

        public UserTaskUnitTests(ITestOutputHelper output)
        {
            this.output = output;
              
            mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new ApplicationProfile());
                }).CreateMapper();
        }

        /// <summary>
        /// Add user task using in memory database model from
        /// "https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory".
        /// Testing AddUserTaskRepository
        /// </summary>
        [Fact]
        public async void AddTaskInMemoryTestAsync()
        {
            IUnitOfWork unitOfWork;

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
                IsCompleted = false,
                IsRemoved = false
            };
            
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
                //output.WriteLine("context.UserTasks.Count() = " + context.UserTasks.Count().ToString());
                
                // Bind the context to the UnitOfWork object
                unitOfWork = new UnitOfWork(context);

                // Retrieve the task from the Db asynchronously
                var userTask = await unitOfWork.userTaskRepository.GetTaskAsync(newUserTask.Id);
                await unitOfWork.CompleteAsync();
                Assert.NotNull(userTask);

                // Validate data equals UserTask passed in
                // Not sure why userTask.Name needs to be trimmed but userTask.Description doesn't
                Assert.True(newUserTask.Name == userTask.Name.Trim());
                Assert.True(newUserTask.Description == userTask.Description);
                Assert.True(newUserTask.IsRecurring.Equals(userTask.IsRecurring));
                Assert.True(newUserTask.Weekdays.Equals(userTask.Weekdays));
                Assert.True(newUserTask.IsCompleted.Equals(userTask.IsCompleted));
                Assert.True(newUserTask.IsRemoved.Equals(userTask.IsRemoved));
            }
        }

        /// <summary>
        /// Add user task using a SQLite in memory database model from 
        /// "https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/sqlite".
        /// Testing TaskController AddTask method
        /// </summary>
        [Fact]
        public async void AddTaskSQLiteTest()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                IUnitOfWork unitOfWork;

                var options = new DbContextOptionsBuilder<SynchroLeanDbContext>()
                    .UseSqlite(connection)
                    .Options;

                /// Create a newUserTask to send to HTTPPost method (without dates)
                var newUserTask = new UserTask {
                    Name = "SQLite unit test add",
                    Description = "Add a task using SQLite database",
                    IsRecurring = true,
                    Weekdays = 40,
                    IsCompleted = false,
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
                    // Bind the context to the UnitOfWork object
                    unitOfWork = new UnitOfWork(context);

                    // Add newUserTask to UserTasks table in Db asynchronously
                    await unitOfWork.userTaskRepository.AddAsync(newUserTask);
                    await unitOfWork.CompleteAsync();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new SynchroLeanDbContext(options))
                {
                    // Bind the Db context to the UnitOfWork object
                    unitOfWork = new UnitOfWork(context);
                    
                    // Assert that UserTasks table in Db contains 1 entry
                    Assert.Equal(1, context.UserTasks.Count());
                    
                    // Retrieve the task from the Db asynchronously
                    var userTask = await unitOfWork.userTaskRepository.GetTaskAsync(newUserTask.Id);
                    await unitOfWork.CompleteAsync();
                    Assert.NotNull(userTask);
                    
                    // Validate data equals UserTask passed in
                    // Not sure why userTask.Name needs to be trimmed but userTask.Description doesn't
                    Assert.True(newUserTask.Name == userTask.Name.Trim());
                    Assert.True(newUserTask.Description == userTask.Description);
                    Assert.True(newUserTask.IsRecurring.Equals(userTask.IsRecurring));
                    Assert.True(newUserTask.Weekdays.Equals(userTask.Weekdays));
                    Assert.True(newUserTask.IsCompleted.Equals(userTask.IsCompleted));
                    Assert.True(newUserTask.IsRemoved.Equals(userTask.IsRemoved));
                }
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Tests TasksController AddUserTaskAsync method
        /// </summary>
        [Fact]
        public void TaskControllerAddTaskTest()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                IUnitOfWork unitOfWork;
                TasksController controller;

                var options = new DbContextOptionsBuilder<SynchroLeanDbContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create database schema
                var context = new SynchroLeanDbContext(options);
                context.Database.EnsureCreated();

                // Bind the context to the UnitOfWork object
                unitOfWork = new UnitOfWork(context);

                // Create an instance of TasksController
                controller = new TasksController(unitOfWork, mapper);

                /// Create a newUserTask to send to HTTPPost method (without dates)
                var newUserTask = new UserTaskResource {
                    Name = "SQLite unit test add",
                    Description = "Add a task using SQLite database",
                    IsRecurring = true,
                    Weekdays = 40,
                    IsCompleted = false,
                    IsRemoved = false
                };

                // Add newUserTask to UserTasks table in Db asynchronously
                var taskResult = controller.AddUserTaskAsync(newUserTask);
                var actionResult = taskResult.Result;
                var okObjectResult = actionResult as OkObjectResult;
                var userTask = okObjectResult.Value as UserTaskResource;

                // Validate data equals UserTaskResource passed in
                // Not sure why userTask.Name needs to be trimmed but userTask.Description doesn't
                Assert.True(newUserTask.Name == userTask.Name.Trim());
                Assert.True(newUserTask.Description == userTask.Description);
                Assert.True(newUserTask.IsRecurring.Equals(userTask.IsRecurring));
                Assert.True(newUserTask.Weekdays.Equals(userTask.Weekdays));
                Assert.True(newUserTask.IsCompleted.Equals(userTask.IsCompleted));
                Assert.True(newUserTask.IsRemoved.Equals(userTask.IsRemoved));
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Test AddUserTaskAsync in TasksController 
        /// "https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/sqlite".
        /// Testing TaskController AddTask method
        /// </summary>
        [Fact]
        public async void TaskControllerGetTaskTestAsync()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                IUnitOfWork unitOfWork;
                TasksController controller;

                var options = new DbContextOptionsBuilder<SynchroLeanDbContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var context = new SynchroLeanDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                /// Create a newUserTask to send to HTTPPost method (without dates)
                var newUserTask = new UserTaskResource {
                    Name = "SQLite unit test add",
                    Description = "Add a task using SQLite database",
                    IsRecurring = true,
                    Weekdays = 40,
                    IsCompleted = false,
                    IsRemoved = false
                };
                
                // Run the test against one instance of the context
                using (var context = new SynchroLeanDbContext(options))
                { 
                    // Bind the context to the UnitOfWork object
                    unitOfWork = new UnitOfWork(context);

                    // Create an instance of TasksController
                    controller = new TasksController(unitOfWork, mapper);

                    // Add newUserTask to UserTasks table in Db asynchronously
                    await controller.AddUserTaskAsync(newUserTask);
                }
                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new SynchroLeanDbContext(options))
                {
                    // Bind the Db context to the UnitOfWork object
                    unitOfWork = new UnitOfWork(context);

                    // Create new instance of task controller
                    controller = new TasksController(unitOfWork, mapper);

                    // Retrieve the task from the Db asynchronously
                    var taskResult = controller.GetTasksAsync(newUserTask.Id);
                    var actionResult = taskResult.Result;
                    var okObjectResult = actionResult as OkObjectResult;
                    var userTaskList = okObjectResult.Value as List<UserTaskResource>;

                    // Assert that UserTasks table in Db contains 1 entry
                    Assert.True(userTaskList.Count().Equals(1));

                    // Retreive the first element from the list
                    UserTaskResource userTask = userTaskList[0];

                    // Validate data equals UserTaskResource passed in
                    // Not sure why userTask.Name needs to be trimmed but userTask.Description doesn't
                    Assert.True(newUserTask.Name == userTask.Name.Trim());
                    Assert.True(newUserTask.Description == userTask.Description);
                    Assert.True(newUserTask.IsRecurring.Equals(userTask.IsRecurring));
                    Assert.True(newUserTask.Weekdays.Equals(userTask.Weekdays));
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
