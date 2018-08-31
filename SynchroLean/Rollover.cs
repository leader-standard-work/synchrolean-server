using System;
using System.Threading.Tasks;
using SynchroLean.Persistence;
using SynchroLean.Core;
using Microsoft.Extensions.DependencyInjection;

namespace SynchroLean
{
    public class Rollover
    {
        private readonly IServiceProvider _serviceProvider;

        public Rollover( IServiceProvider serviceProvider){
            _serviceProvider = serviceProvider;
        }

        public async void RunRollover()
        {
            using(var scope = _serviceProvider.CreateScope()){

                var context = scope.ServiceProvider.GetService<SynchroLeanDbContext>();
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

                //Clean up the to-do list for the night
                await unitOfWork.TodoRepository.Clean();
                Task.WaitAll(unitOfWork.CompleteAsync());

                //Do cleanup
                await unitOfWork.UserTeamRepository.Clean();
                await unitOfWork.UserAccountRepository.Clean();
                await unitOfWork.UserTaskRepository.Clean();
                await unitOfWork.CompletionLogEntryRepository.Clean();

                //Add daily todos
                var tasks = await unitOfWork.UserTaskRepository.GetAllTasksAsync();
                foreach (var task in tasks) await unitOfWork.TodoRepository.AddTodoAsync(task.Id);

                context.SaveChanges();
            }
        }
    }
}
