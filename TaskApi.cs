using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestFuncApp
{
    public static class TaskApi
    {

        public static readonly List<Task> Items = new List<Task>();

        [FunctionName("CreateTask")]
        public static async Task<IActionResult> CreateTask(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "task")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creatin a new task list item");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TaskCreateModel>(requestBody);

            var task = new Task() { TaskDescription = input.TaskDescription };
            Items.Add(task);

            return new OkObjectResult(task);
        }


        [FunctionName("GetAllTasks")]
        public static IActionResult GetAllTasks(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "task")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Getting task list items");

           return new OkObjectResult(Items);
        }


        [FunctionName("GetTaskById")]
        public static IActionResult GetTaskById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "task/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("Getting task item for id {0}", id);

            var task = Items.Find(t => t.Id == id);
            if(task == null)
            {

                return new NotFoundResult();
            }

            return new OkObjectResult(task);
        }



        [FunctionName("UpdateTask")]
        public static async Task<IActionResult> UpdateTask(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "task/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("Update task item for id {0}", id);

            var task = Items.Find(t => t.Id == id);
            if (task == null)
            {

                return new NotFoundResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<TaskUpdateModel>(requestBody);

            task.IsCompleted = updated.IsCompleted;
            if(!string.IsNullOrEmpty(updated.TaskDescription))
            {
                task.TaskDescription = updated.TaskDescription;
            }

            return new OkObjectResult(task);
        }


        [FunctionName("DeleteTask")]
        public static IActionResult DeleteTask(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "task/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("Delete task id {0}", id);

            var task = Items.Find(t => t.Id == id);
            if (task == null)
            {

                return new NotFoundResult();
            }
            Items.Remove(task);

            return new OkResult();
        }


    }
}
