using System;
using System.Collections.Generic;

namespace PartyPanelShared.Models
{
    [Serializable]
    public class Characteristic
    {
        public enum BeatmapDifficulty
        {
            Easy,
            Normal,
            Hard,
            Expert,
            ExpertPlus
        }

        public string SerializedName { get; set; }

        public BeatmapDifficulty[] difficulties { get; set; }
    }
}
