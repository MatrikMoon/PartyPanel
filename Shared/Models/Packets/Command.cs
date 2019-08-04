using System;

namespace PartyPanelShared.Models
{
    [Serializable]
    public class Command
    {
        public enum CommandType
        {
            Heartbeat,
            ReturnToMenu
        }

        public CommandType commandType;
    }
}
