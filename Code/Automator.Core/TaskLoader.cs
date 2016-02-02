using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automator.Core
{
    public class TaskLoader
    {

        private TaskMapper _taskMapper;

        public TaskLoader():this(TaskMapper.CreateDefault())
        {

        }

        public TaskLoader(TaskMapper mapper)
        {
            _taskMapper = mapper;
        }


        public List<ITask> LoadTasks(string fileName)
        {
            var jsonData = File.ReadAllText(fileName);
            var json = JsonConvert.DeserializeObject(jsonData);
            return ((JArray)json).Select(i => GetTask(i)).Where(i => i != null).ToList();
        }

        private ITask GetTask(JToken token)
        {
            var taskName = token.Values<JProperty>().SingleOrDefault(i => i.Name == "TaskName").ToString();
            if (string.IsNullOrWhiteSpace(taskName)) return null;
            var taskType = _taskMapper[taskName];
            if (taskType == null) return null;
            return GetTask(token, taskType);
        }

        private ITask GetTask(JToken token, Type taskType)
        {
            var instance = Activator.CreateInstance(taskType);
            var props = taskType.GetProperties();
            foreach(var prop in props)
            {
                var value = token.Values<JProperty>().SingleOrDefault(i => i.Name == prop.Name).Value.ToString();
                if (string.IsNullOrWhiteSpace(value)) continue;
                prop.SetValue(instance, Convert.ChangeType(value, prop.PropertyType));
            }
            return (ITask)instance;
        }
    }
}
