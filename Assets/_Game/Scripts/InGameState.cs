using System.Threading.Tasks;
using Tofunaut.Bootstrap;
using Tofunaut.TofuECS;
using UnityEngine;

namespace Tofunaut.TofuECS_Rogue
{
    public class InGameStateRequest
    {
        
    }
    
    public class InGameState : AppState<InGameStateRequest>
    {
        private Simulation _simulation;
        
        public override Task OnEnter(InGameStateRequest request)
        {
            return Task.CompletedTask;
        }
    }
}