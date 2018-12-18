using StarAI.PathFindingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.ExecutionCore
{
    public class CustomTask
    {
        public delegate void ObjectTask(object obj);
        public delegate void VoidTask();


        public ObjectTask objectTask;
        public object objectParameterDataArray;
        public VoidTask voidTask;

        public TaskMetaData taskMetaData;

        /// <summary>
        /// Create a custom task and calculate cost of the action automatically without having to pass cost to the meta data. Saves a lot of code space and memory.
        /// </summary>
        /// <param name="objTask"></param>
        /// <param name="arrayData"></param>
        /// <param name="TaskMetaData"></param>
        public CustomTask(ObjectTask objTask,object[] arrayData, TaskMetaData TaskMetaData)
        {
            objectTask = objTask;
            objectParameterDataArray = arrayData;
            this.taskMetaData = TaskMetaData;
            this.taskMetaData.calculateTaskCost((TileNode)arrayData[0]);
        }

        public CustomTask(VoidTask vTask, TaskMetaData TaskMetaData)
        {
            voidTask = vTask;
            this.taskMetaData = TaskMetaData;
        }

        public void runTask()
        {

            //Check Before running task if all prerequisites are working
            if (objectTask != null) objectTask.Invoke(objectParameterDataArray);

            if (voidTask != null) voidTask.Invoke();
        }

    }
}
