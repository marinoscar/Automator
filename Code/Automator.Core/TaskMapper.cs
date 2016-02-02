using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automator.Core
{
    public class TaskMapper
    {

        private Dictionary<string, Type> _map;

        private TaskMapper()
        {
            _map = new Dictionary<string, Type>();
        }

        public static TaskMapper CreateDefault()
        {
            var fileName = ConfigurationManager.AppSettings["taskMapperFilePath"] ?? "taskMapper.json";
            if (File.Exists(fileName)){
                return LoadFromFile(fileName);
            }
            //Dynamic load
            var result = new TaskMapper();
            var taskInterface = typeof(ITask);
            var query = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(t => t.GetTypes())
                        .Where(i => taskInterface.IsAssignableFrom(i))
                        .Select(o => new { Name = o.Name, TaskQualifiedName = string.Format("{0},{1}", o.AssemblyQualifiedName, o.FullName) })
                        .ToList();
            query.ForEach(c => result.LoadItem(c.Name, c.TaskQualifiedName));
            return result;
        }


        public Type this[string taskType] { get { return _map[taskType]; } }

        public static TaskMapper LoadFromFile(string fileName)
        {
            return JsonConvert.DeserializeObject<TaskMapper>(fileName); 
        }

        public void LoadItem(string taskName, string taskQualifiedName)
        {
            _map[taskName] = Type.GetType(taskQualifiedName);
        }
    }

    public class TaskItem
    {
        public string TaskName { get; set; }
        public string TaskQualifiedName { get; set; }
    }
}
