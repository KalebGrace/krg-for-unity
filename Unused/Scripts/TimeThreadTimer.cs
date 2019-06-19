using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NS_RAIN
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
#endif

namespace KRG.Unused {

    //RAIN AI is no longer supported.

#if NS_RAIN
    ///Uses the GraphicsController's time thread instance for a RAIN AI timer.
    [RAINAction("Time Thread Timer")]
    public class TimeThreadTimer : RAINAction {

        public Expression Seconds = new Expression();

        private ActionResult m_result = ActionResult.NONE;

        public override ActionResult Execute(AI ai) {
            switch (m_result) {
                case ActionResult.NONE:
                    G.U.Assert(Seconds != null && Seconds.IsValid);
                    float sec = Seconds.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory);
                    GetGraphicsController(ai).timeThread.AddTrigger(sec, ExecuteComplete);
                    m_result = ActionResult.RUNNING;
                    return m_result;
                case ActionResult.RUNNING:
                    //do nothing
                    return m_result;
                case ActionResult.SUCCESS:
                    //clear this out for next run
                    m_result = ActionResult.NONE;
                    return ActionResult.SUCCESS;
                default:
                    G.U.Err("Unsupported ActionResult {0} for TimeThreadTimer {1}.", m_result, actionName);
                    return m_result;
            }
        }

        private static GraphicsController GetGraphicsController(AI ai) {
            string graphicsControllerName = "m_graphicsController";
            if (ai.WorkingMemory.ItemExists(graphicsControllerName)) {
                return ai.WorkingMemory.GetItem<GraphicsController>(graphicsControllerName);
            } else {
                var graphicsController = ai.Body.GetComponent<GraphicsController>();
                ai.WorkingMemory.SetItem(graphicsControllerName, graphicsController);
                ai.WorkingMemory.Commit();
                ai.WorkingMemory.Save();
                return graphicsController;
            }
        }

        private void ExecuteComplete(TimeTrigger tt) {
            m_result = ActionResult.SUCCESS;
        }
    }
#endif
}
