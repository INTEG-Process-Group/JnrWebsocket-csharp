using Integpg.JniorWebSocket;
using System;
using WebSocketExample.Actions;

namespace WebSocketExample
{
    public class UpdateProjectEngine
    {
        public event InformationEventHandler ProgressChanged;
        public event EventHandler UpdateComplete;


        public DateTime UpdateStartTime { get; private set; }
        public bool InProgress { get; private set; }
        public ActionResult UpdateResult { get; private set; }
        public ActionBase CurrentAction { get; set; }
        public Exception Error { get; private set; }
        public bool IsCancelled { get; private set; }

        private JniorWebSocket _jniorWebSocket;
        private UpdateProject _updateProject;



        public UpdateProjectEngine(JniorWebSocket jniorWebSocket, UpdateProject updateProject)
        {
            _jniorWebSocket = jniorWebSocket;
            _updateProject = updateProject;
        }



        public void Execute()
        {
            InProgress = true;
            UpdateStartTime = DateTime.Now;
            UpdateResult = ActionResult.NotNeeded;

            try
            {
                //foreach (var actionJson in _instructionsJson["Actions"])
                foreach (var actionBase in _updateProject.Steps)
                {
                    CurrentAction = actionBase;

                    //var actionName = (string)actionJson["Name"];
                    var newActionBase = ActionBaseFactory.CreateAction(actionBase.ActionJson);
                    newActionBase.UpdateInfo += ProgressChanged;
                    ProgressChanged?.Invoke(this, new InformationEventArgs("Start action '" + newActionBase.Name + "'"));

                    newActionBase.Execute(this, _jniorWebSocket);
                    if (newActionBase.ActionResult == ActionResult.Success)
                    {
                        UpdateResult = ActionResult.Success;
                    }
                    else if (newActionBase.ActionResult == ActionResult.Failed)
                    {
                        UpdateResult = ActionResult.Failed;
                        Error = newActionBase.Error;
                        break;
                    }
                    else if (newActionBase.ActionResult == ActionResult.Cancelled)
                    {
                        UpdateResult = ActionResult.Cancelled;
                        break;
                    }
                }

                //if (ActionResult.Failed != UpdateResult)
                //{
                //    UpdateResult = ActionResult.Success;
                //}
            }
            catch (Exception ex)
            {
                UpdateResult = ActionResult.Failed;
                Error = ex;
            }

            InProgress = false;
            UpdateComplete?.Invoke(this, EventArgs.Empty);
        }



        internal void Cancel()
        {
            IsCancelled = true;
            _jniorWebSocket.Close();
        }
    }
}
